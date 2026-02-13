using KidproblemService.Models;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using Amazon.S3;
using Microsoft.Extensions.Options;
using KidproblemService.Helpers;
using KidproblemService.Interfaces;

namespace KidproblemService.Services
{
    public class ScrapService: IScrapService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _s3BucketName;

        public ScrapService(IAmazonS3 s3Client, IOptions<AwsConfiguration> awsConfiguration)
        {
            _s3Client = s3Client;
            _s3BucketName = awsConfiguration.Value.S3BucketName!;
        }

        public Problem[] ProcessProblems(Problem[] problems)
        {
            for (int i = 0; i < problems.Length; i++)
            {
                problems[i].ProblemText = ProblemHelper.CleanProblemText(problems[i].ProblemText!);

                string prefix = string.IsNullOrEmpty(problems[i].ProblemTitle) ? $"{problems[i].ProblemTitle}_" : "";
                string downloadImagesLog = DownloadProblemImage(problems[i].ProblemText!, prefix);
                problems[i].ProblemText = ProblemHelper.BeautifyProblem(problems[i].ProblemText!);
                problems[i].ReturnResult = downloadImagesLog;
            }
            return problems;
        }

        public async Task<List<Problem>> GetProblemsAsync(ScrapDefinition definition)
        {
            var problems = new List<Problem>();
            string html = await GetHtmlTextAsync(definition.StartUrl!);
            List<string> problemHtmls;

            if (definition.UseSinglePattern ?? false)
            {
                problemHtmls = GetProblemHtmls(html, definition.RegexPattern!);
            }
            else
            {
                problemHtmls = GetProblemHtmls(html, definition.StartPattern!, definition.EndPattern!);
            }

            if (problemHtmls.Count == 0)
            {
                return problems;
            }

            for (int i = 0; i < problemHtmls.Count; i++)
            {
                problemHtmls[i] = ProblemHelper.CleanProblemText(problemHtmls[i]);
                string prefix = $"{definition.ProblemCategory}-{definition.ProblemYear}-{(i + 1).ToString().PadLeft(3, '0')}_";
                string downloadImagesLog = DownloadProblemImage(problemHtmls[i], prefix);
                problemHtmls[i] = ProblemHelper.BeautifyProblem(problemHtmls[i]);
                var problem = GetProblem(definition.ProblemCategory!, definition.ProblemYear!, problemHtmls[i], (i + 1).ToString());
                problem.ReturnResult = downloadImagesLog;
                problems.Add(problem);
            }
            return problems;
        }

        public Tuple<string, string> ProcessProblemImage(string problemText, string problemTitle)
        {
            Dictionary<string, string> replaceImgSrc = new Dictionary<string, string>();

            string pattern = @"<img src=\""(https{0,1}:\/\/.*?)\""";
            foreach (Match m in Regex.Matches(problemText, pattern))
            {
                string imgSrc = m.Groups[1].Value;
                if (replaceImgSrc.ContainsKey(imgSrc))
                {
                    continue;
                }

                string fileName = $"{problemTitle}_{imgSrc.Substring(imgSrc.LastIndexOf("/") + 1)}";
                replaceImgSrc.Add(imgSrc, fileName);
            }

            string message = BatchDownloadImages(replaceImgSrc).Result;

            foreach (var kvp in replaceImgSrc)
            {
                problemText = problemText.Replace(kvp.Key, ProblemHelper.AssetsName + "/" + kvp.Value);
            }

            return new Tuple<string, string>(problemText, message);
        }

        private Problem GetProblem(string problemCategory, string problemYear, string problemText, string problemNumber, string problemAnswer = "")
        {
            var problem = new Problem();
            problem.ProblemCategory = problemCategory;
            problem.ProblemYear = problemYear;
            problem.ProblemNumber = problemNumber;
            problem.ProblemTitle = $"{problemCategory}-{problemYear}-{problemNumber.PadLeft(3, '0')}";
            problem.ProblemText = problemText;
            problem.ProblemAnswer = problemAnswer;
            problem.ProblemTags = new string[] { };
            problem.IsStaging = true;
            problem.AnswerOptions = problemCategory == ProblemHelper.ProblemCategoryAIME ? "" : ProblemHelper.DefaultProblemAnswerOptions;
            problem.SolutionText = "";
            return problem;
        }

        private async Task<string> GetHtmlTextAsync(string url)
        {
            using (HttpClient client = new())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream receiveStream = await response.Content.ReadAsStreamAsync())
                    {
                        StreamReader readStream;
                        readStream = new StreamReader(receiveStream);
                        string data = readStream.ReadToEnd();
                        readStream.Close();
                        return data;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private List<string> GetProblemHtmls(string html, string userpattern)
        {
            List<string> problems = new List<string>();
            string pattern;
            //string pattern = @"(<h2><span class=""mw-headline"".*?)<p><a href=";
            //string pattern = @"(<span class=""mw-headline"".*?)<h2>"; //only for AMC8 2000 

            if (!string.IsNullOrWhiteSpace(userpattern))
            {
                pattern = userpattern;
            }
            else
            {
                // pattern = Resource.regexGetProblem;
                pattern = ProblemHelper.RegexProblemPattern;
            }

            RegexOptions options = RegexOptions.Singleline;
            var matches = Regex.Matches(html, pattern, options);
            if (matches.Count == 0)
            {
                pattern = ProblemHelper.RegexProblemPatternAlt;
                matches = Regex.Matches(html, pattern, options);
            }

            foreach (Match m in matches)
            {
                GroupCollection groups = m.Groups;
                problems.Add(groups[1].Value);
            }

            return problems;
        }

        private List<string> GetProblemHtmls(string html, string startPattern, string endPattern)
        {
            List<string> problems = new List<string>();

            if (string.IsNullOrWhiteSpace(startPattern))
            {
                startPattern = ProblemHelper.RegexProblemStartPattern;
            }
            if (string.IsNullOrWhiteSpace(endPattern))
            {
                endPattern = ProblemHelper.RegexProblemEndPattern;
            }

            RegexOptions options = RegexOptions.Multiline;
            var startMateches = Regex.Matches(html, startPattern, options);
            var endMateches = Regex.Matches(html, endPattern, options);

            // startPattern and endPattern shall return the same number of match. 
            // if the counts are different or zero, this means the patterns are not fit
            // return an empty collection to remind the caller to change a different set of patterns
            if (startMateches.Count != endMateches.Count || startMateches.Count == 0 || endMateches.Count == 0)
            {
                return problems;
            }

            for (int i = 0; i < startMateches.Count; i++)
            {
                var startMatch = startMateches[i];
                int startPos = startMatch.Index;
                var endmatch = endMateches[i];
                int endPos = endmatch.Index;
                problems.Add(html.Substring(startPos, endPos - startPos));
            }
            return problems;
        }

        private string DownloadProblemImage(string problem, string prefix = "")
        {
            Dictionary<string, string> replaceImgSrc = new Dictionary<string, string>();
            string pattern = @"<img src=""\/\/(.*?)""";
            foreach (Match m in Regex.Matches(problem, pattern))
            {
                string imgSrc = "http://" + m.Groups[1].Value;
                string fileName = prefix + imgSrc.Substring(imgSrc.LastIndexOf("/") + 1);
                if (replaceImgSrc.ContainsKey(imgSrc))
                {
                    continue;
                }
                replaceImgSrc.Add(imgSrc, fileName);
            }
            string message = BatchDownloadImages(replaceImgSrc).Result;
            return message;
        }

        private async Task<string> BatchDownloadImages(Dictionary<string, string> imgSources)
        {
            if (imgSources.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder messages = new StringBuilder();
            using (HttpClient client = new HttpClient())
            {
                for (int i = 0; i < imgSources.Count; i++)
                {
                    string fileName = imgSources.Values.ElementAt(i);
                    string externalLink = imgSources.Keys.ElementAt(i);
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(externalLink);
                        response.EnsureSuccessStatusCode();

                        using (Stream imageStream = await response.Content.ReadAsStreamAsync())
                            await _s3Client.UploadObjectFromStreamAsync(_s3BucketName, fileName, imageStream, null);
                        var fileInfo = await _s3Client.GetObjectMetadataAsync(_s3BucketName, fileName);
                        if (fileInfo.ContentLength > 0)
                        {
                            messages.AppendLine($"Downloaded {externalLink} to {fileName}. ");
                        }
                        else
                        {
                            // Download failed
                            messages.AppendLine($"[ERROR] Failed to download {externalLink} due to unknown reason.");
                            imgSources[imgSources.Keys.ElementAt(i)] = imgSources.Keys.ElementAt(i);
                        }

                    }
                    catch (Exception ex)
                    {
                        messages.AppendLine($"[ERROR] Failed to download {externalLink} due to error: {ex.Message}");
                        // Since download fails, it will not update the img src.
                        imgSources[imgSources.Keys.ElementAt(i)] = imgSources.Keys.ElementAt(i);
                    }
                }
            }

            return messages.ToString();
        }

    }
}
