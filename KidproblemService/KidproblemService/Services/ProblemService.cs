using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.S3;
using Amazon.S3.Model;
using KidproblemService.Interfaces;
using KidproblemService.Models;
using Microsoft.Extensions.Options;
using System;
using System.Data.SqlTypes;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KidproblemService.Services
{
    public class ProblemService : IProblemService
    {
        private readonly IDynamoDBContext _context;
        private readonly IAmazonS3 _s3Client;
        private readonly string _s3BucketName;
        private readonly IScrapService _scrapService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<ProblemService> _logger;
        public const string PaginationTokenNonRecordReturned = "{}";

        public ProblemService(IDynamoDBContext context,
        IAmazonS3 s3Client,
        IOptions<AwsConfiguration> awsConfiguration,
        IScrapService scrapService,
        ICacheService cacheService,
        ILogger<ProblemService> logger
        )
        {
            _context = context;
            _s3Client = s3Client;
            _s3BucketName = awsConfiguration.Value.S3BucketName!;
            _scrapService = scrapService;
            _cacheService = cacheService;
            _logger = logger;
        }

        /// <summary>
        /// Get single problem by problem title. 
        /// If there are multiple problems with the given problem title (which should not happen),
        /// it returns the first problem.
        /// If there is no problem having the given problem title,
        /// it return null.
        /// </summary>
        /// <param name="problemTitle"></param>
        /// <returns>A Problem with all Attributes populated</returns>
        public async Task<Problem?> GetProblemAsync(string problemTitle)
        {
            Problem? entity = _cacheService.Get<Problem>(problemTitle);
            if (entity != null)
            {
                return entity;
            }

            QueryOperationConfig queryConfig = new QueryOperationConfig
            {
                KeyExpression = new()
                {
                    ExpressionAttributeValues = new() {
                        { ":problem_title", problemTitle.ToUpper() },
                    },
                    ExpressionStatement = "problem_title = :problem_title"
                }
            };

            var entities = await _context.FromQueryAsync<Problem>(queryConfig).GetRemainingAsync();
            entity = entities.FirstOrDefault();

            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _s3BucketName);
            if (entity != null && bucketExists)
            {
                await ReplaceImageWithBase64String(entity);
                _cacheService.Set(entity);
            }

            return entity;
        }

        /// <summary>
        /// Get image from S3 Bucket, and generate base64 string from the image
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task ReplaceImageWithBase64String(Problem entity)
        {
            Dictionary<string, string> images = new Dictionary<string, string>();
            string regMatchImgTag = "<img.+?\\/>";
            string regMatchSrcTag = "src.*?=['\"](.+?)['\"]";
            string regMatchAlt = "alt.*?=['\"]\\[asy\\](.+?)\\[\\/asy\\]['\"]";
            string problemText = entity.ProblemText!;
            var matches = Regex.Matches(problemText, regMatchImgTag, RegexOptions.Multiline);
            foreach (Match match in matches)
            {
                string image = match.Groups[0].Value;
                var matchSrc = Regex.Match(image, regMatchSrcTag, RegexOptions.Multiline);
                if (matchSrc.Success)
                {
                    var filename = matchSrc.Groups[1].Value;
                    images.Add(filename, "");

                    var matchAlt = Regex.Match(image, regMatchAlt, RegexOptions.Multiline);
                    if (matchAlt.Success)
                    {
                        images[filename] = matchAlt.Groups[1].Value.Trim();
                    }
                }
            }

            foreach (var img in images)
            {
                string assetName = img.Key.Split('/').Last();
                var ext = Path.GetExtension(assetName);
                if (!string.IsNullOrEmpty(ext))
                {
                    ext = ext.ToLower()[1..]; // remove the period
                    string image_ext = "";
                    switch (ext)
                    {
                        case "png":
                            image_ext = "png";
                            break;
                        case "jpg":
                            image_ext = "jpeg";
                            break;
                        case "gif":
                            image_ext = "gif";
                            break;
                        case "svg":
                            image_ext = "svg";
                            break;
                        default:
                            continue;
                    }

                    try
                    {
                        _logger.LogInformation($"Retrieving asset {assetName} from S3.");
                        var s3Object = await _s3Client.GetObjectAsync(_s3BucketName, assetName);
                        if (s3Object.ResponseStream != null)
                        {
                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                s3Object.ResponseStream.CopyTo(memoryStream);
                                string base64ImageRepresentation = Convert.ToBase64String(memoryStream.ToArray());
                                problemText = problemText.Replace(img.Key, $"data:image/{image_ext};base64," + base64ImageRepresentation);
                            }
                        }
                    }
                    catch (AmazonS3Exception ex)
                    {
                        _logger.LogInformation($"Asset {img.Key} does not exist in S3.");
                        if (!string.IsNullOrEmpty(img.Value))
                        {
                            var asyPng = await ProcessAsymptoteCode(assetName, img.Value);
                            if (!string.IsNullOrEmpty(asyPng))
                            {
                                problemText = problemText.Replace(img.Key, asyPng);
                                continue;
                            }
                        }

                        entity.ReturnResult += $"Failed to load resource {assetName} from S3: {ex.Message}";
                    }
                }
            }
            entity.ProblemTextBase64 = problemText;
        }

        /// <summary>
        /// Query problems
        /// </summary>
        /// <param name="keyword">In the format of "category-year-number". category is required. year and number are optional.</param>
        /// <param name="staging"></param>
        /// <param name="usePagination"></param>
        /// <param name="pageSize"></param>
        /// <param name="paginationToken">A Json string indicate where to start the next Query</param>
        /// <returns>A Tuple. Item1 is a list of Problems; Item2 is the pagination token (if pagination is used). 
        /// The returned Problem only has these attributes populated: problem_title, category, year, staging, answer, answer_options</returns>
        public async Task<Tuple<List<Problem>, string?>> QueryProblemsAsync(string keyword, bool staging, bool usePagination = false, int? pageSize = 25, string? paginationToken = null)
        {
            List<Problem> result = new();

            // parse keyword
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new Tuple<List<Problem>, string?>(result, null);
            }
            var filters = keyword.Replace("  ", " ").Replace(" ", "-").Replace("--", "-").Split("-");
            // the first filter is category
            string category = filters[0];
            // the second filter is year
            string? year = filters.Length > 1 ? filters[1] : null;
            // the third filter is number
            string? number = filters.Length > 2 ? filters[2] : null;
            // other filters are not implemented

            var problemTable = _context.GetTargetTable<Problem>();
            QueryOperationConfig queryConfig = new QueryOperationConfig
            {
                IndexName = "category-year-index",
                Select = SelectValues.SpecificAttributes,
                AttributesToGet = new() { "problem_title", "category", "year", "staging", "answer", "answer_options" },
            };
            queryConfig.Filter.AddCondition("category", QueryOperator.Equal, category);
            queryConfig.Filter.AddCondition("staging", QueryOperator.Equal, staging);
            if (!string.IsNullOrEmpty(year))
            {
                queryConfig.Filter.AddCondition("year", QueryOperator.BeginsWith, year.ToUpper());
            }
            // number is currently not supported by this index, as it does not project the attribute 'number'
            // When number is available, I query with problemTitle which is built from category, year, and number
            if (!string.IsNullOrEmpty(number))
            {
                string problemTitle = $"{category}-{year}-{number}";
                queryConfig.Filter.AddCondition("problem_title", ScanOperator.Contains, problemTitle.ToUpper());
            }

            if (usePagination)
            {
                queryConfig.Limit = pageSize.GetValueOrDefault();
                queryConfig.PaginationToken = paginationToken;
            }

            var query = problemTable.Query(queryConfig);
            List<Document> entities;
            if (usePagination)
            {
                entities = await query.GetNextSetAsync();
                paginationToken = query.PaginationToken;

                // Due to an issue from AWS SDK,
                // GetNextSetAsync() could return an empty collection 
                // with a valid pagination token.
                // if this happens, the service keep querying until the returned collection is not empty
                // or, the pagination token indicates there is no record
                while (entities.Count == 0)
                {
                    entities = await query.GetNextSetAsync();
                    paginationToken = query.PaginationToken;
                    if (string.IsNullOrEmpty(paginationToken) || paginationToken.Trim() == PaginationTokenNonRecordReturned)
                    {
                        break;
                    }
                }

            }
            else
            {
                entities = await query.GetRemainingAsync();
            }
            result.AddRange(_context.FromDocuments<Problem>(entities));
            result = result.OrderBy(r => r.ProblemTitle).ToList();

            return new Tuple<List<Problem>, string?>(result, paginationToken);

        }

        /// <summary>
        /// Insert Problem as is.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Problem> CreateAsync(Problem entity)
        {
            entity.Action = Models.Action.Create;
            if (await Validate(entity))
            {
                await _context.SaveAsync(entity);
            }
            return entity;
        }

        /// <summary>
        /// Update problem. If the problem text has changed, it processes img tags.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="existing"></param>
        /// <returns></returns>
        public async Task<Problem> UpdateAsync(Problem entity, Problem existing)
        {
            entity.Action = Models.Action.Update;
            if (await Validate(entity))
            {
                // Problem should not be moved back to Staging
                entity.IsStaging = existing.IsStaging && entity.IsStaging;

                // process image tags
                if (existing.ProblemText != entity.ProblemText)
                {
                    var results = _scrapService.ProcessProblemImage(entity.ProblemText!, entity.ProblemTitle!);
                    entity.ProblemText = results.Item1;
                    entity.ReturnResult = results.Item2;
                }

                await _context.SaveAsync(entity);
                _cacheService.Unset<Problem>(entity.ProblemTitle!);
            }
            return entity;
        }

        /// <summary>
        /// Delete the Problem. 
        /// Warning: this method does not check if the Problem is used by any other entities.
        /// ToDo: delete assets with Problem
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Problem> DeleteAsync(Problem entity)
        {
            entity.Action = Models.Action.Delete;
            await _context.DeleteAsync(entity);
            _cacheService.Unset<Problem>(entity.ProblemTitle!);

            // TODO: delete assets
            return entity;
        }

        /// <summary>
        /// Scrap Problems from external websites
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        public async Task<List<Problem>> ScrapAsync(ScrapDefinition definition)
        {
            List<Problem> problems = await _scrapService.GetProblemsAsync(definition);

            foreach (var problem in problems)
            {
                if (problem.ProblemTitle == null || problem.ProblemCategory == null || problem.ProblemYear == null)
                {
                    continue;
                }

                var existing = await GetProblemAsync(problem.ProblemTitle);
                if (existing == null)
                {
                    await CreateAsync(problem);
                }
                else
                {
                    await UpdateAsync(problem, existing);
                }
            }
            return problems;
        }

        /// <summary>
        /// Bulk create Problems and process img tags
        /// </summary>
        /// <param name="problems"></param>
        /// <returns></returns>
        public async Task<Problem[]> ScrapAsync(Problem[] problems)
        {
            var results = _scrapService.ProcessProblems(problems);
            foreach (var problem in results)
            {
                if (problem.ProblemTitle == null || problem.ProblemCategory == null || problem.ProblemYear == null)
                {
                    continue;
                }

                var existing = await GetProblemAsync(problem.ProblemTitle);
                if (existing == null)
                {
                    await CreateAsync(problem);
                }
                else
                {
                    await UpdateAsync(problem, existing);
                }

            }
            return results;
        }

        private async Task<bool> Validate(Problem entity)
        {
            if (entity.ProblemTitle == null || entity.ProblemCategory == null || entity.ProblemYear == null)
            {
                entity.ReturnResult = "Problem Title, Problem Category or Problem Year cannot be null.";
                entity.IsSuccessful = false;
                return false;
            }

            if (entity.Action == Models.Action.Create)
            {
                var existing = await GetProblemAsync(entity.ProblemTitle);
                if (existing != null)
                {
                    entity.ReturnResult = "Found duplicate Problem Title.";
                    entity.IsSuccessful = false;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The method ProcessAsymptoteCode() cleans up Asymptote code first, 
        /// and then call http://asymptote.ualberta.ca:10007 to convert the code to a png file.
        /// If the api call is successful, it uploads the png file to S3, 
        /// and return base64 string of the png file.
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="asyCode"></param>
        /// <returns></returns>
        private async Task<string?> ProcessAsymptoteCode(string assetName, string asyCode)
        {
            if (!string.IsNullOrEmpty(asyCode))
            {
                //clean up
                asyCode = System.Net.WebUtility.HtmlDecode(asyCode);
                asyCode = Regex.Replace(asyCode, @"\s+", " ");
            }
            _logger.LogDebug($"Cleaned code: {asyCode}");

            // The endpoint with the format parameter set to png
            string url = "http://asymptote.ualberta.ca:10007?f=png";
            byte[] codeBytes = Encoding.UTF8.GetBytes(asyCode);
            using var content = new ByteArrayContent(codeBytes);
            // Force the header to be exactly "text/plain"
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");

            _logger.LogInformation("Call asymptote API ...");
            using var client = new HttpClient();
            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                string base64ImageRepresentation = Convert.ToBase64String(imageBytes);
                if (imageBytes.Length == 0 || string.IsNullOrEmpty(base64ImageRepresentation))
                {
                    _logger.LogInformation("Failed to receive image from asymptote API");
                    return null;
                }
                _logger.LogInformation("Successful received image from asymptote API");
                
                await _s3Client.UploadObjectFromStreamAsync(_s3BucketName, assetName, new MemoryStream(imageBytes), null);
                _logger.LogInformation($"Successful uploaded image {assetName} to S3.");
                return "data:image/png;base64," + base64ImageRepresentation;
            }
            else
            {
                _logger.LogInformation($"Failed to convert Asymptote code to image. Status code: {response.StatusCode}");
            }

            return null;
        }
    }
}
