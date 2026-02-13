using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using KidproblemService.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace Migration
{
    internal class MigrationHelper
    {
        private const string SourceResourceRepo = "D:\\Temp\\kp_assets";
        private const string DesitnationResourceRepo = "D:\\Temp\\s3_kp_assets";

        public static async Task<int> PrepareDynamoDbTables(string? prefix)
        {
            var result = await DynamoDbHelper.CreateTables(prefix??string.Empty);
            return result;
        }

        public static async Task<long> MigrateProblemsAsync(bool development, string? prefix = "dev_")
        {
            DynamoDBContextConfig? config = null;
            if (development)
            {
                config = new()
                {
                    TableNamePrefix = prefix
                };
            }

            Console.WriteLine("Migrate Problems from MySQL to DynamoDb.");
            long total = 0;
            int chunkSize = 200;
            int offset = 0; // the offset will be 3291
            int cnt;
            do
            {
                var result = MySqlDbHelper.GetProblems(offset, chunkSize);
                cnt = result.Count;
                Console.WriteLine($"Exported {cnt} records from MySQL.");
                Console.WriteLine(ProcessImages(result));

                if (result.Count > 0)
                {
                    var count = await DynamoDbHelper.BatchWriteAsync(result, config);
                    Console.WriteLine($"Imported {count} records to DynamoDB.");
                    total += count;
                }

                offset += chunkSize;
            } while (cnt > 0);

            return total;
        }

        private static string ProcessImages(List<Problem> problems)
        {
            StringBuilder result = new();
            
            foreach(var problem in problems)
            {
                List<string> filenames = new List<string>();
                string regMatchImgTag = "<img src=\"(.*?)\".*?\\/>";
                string regMatchFileName = @"(.+?\/)*(.*)";
                string problemText = problem.ProblemText!;
                var matches = Regex.Matches(problemText, regMatchImgTag, RegexOptions.Multiline);
                foreach (Match match in matches)
                {
                    string filename = match.Groups[1].Value;
                    filenames.Add(filename);
                }
                int counter = 1;
                foreach (var filename in filenames)
                {
                    string assetName = Regex.Match(filename, regMatchFileName).Groups[2].Value;
                    string localSourceFileName = Path.Combine(SourceResourceRepo, assetName);
                    if (!File.Exists(localSourceFileName))
                    {
                        // this asset might have been "cleaned" and renamed
                        int pos = assetName.IndexOf("_");
                        if (pos >= 0)
                        {
                            var cleanedAssetName = assetName[(pos + 1)..];
                            localSourceFileName = Path.Combine(SourceResourceRepo, cleanedAssetName);
                            //result.AppendLine($"{problem.ProblemTitle}: Cannot find {assetName}. Replaced with {cleanedAssetName}.");
                        }
                    }

                    if (File.Exists(localSourceFileName))
                    {
                        var ext = Path.GetExtension(assetName);
                        string newAssetName = $"{problem.ProblemTitle}_{counter.ToString().PadLeft(2,'0')}.{ext}";
                        string localDestinationFileName = Path.Combine(DesitnationResourceRepo, newAssetName);
                        counter++;
                        problem.ProblemText!.Replace(assetName, newAssetName);
                        File.Copy(localSourceFileName, localDestinationFileName,true);
                        //result.AppendLine($"{problem.ProblemTitle}: Copied {assetName} to {newAssetName}.");
                    }  else
                    {
                        result.AppendLine($"{problem.ProblemTitle}: Cannot find {localSourceFileName}.");
                    }                   
                }
            }

            return result.ToString();
        }

        
        // exam_hdr_key = 564
        public static async Task<long> MigrateExamRunsAsync(bool development, string? prefix = "dev_")
        {
            Console.WriteLine("Migrate Exams from MySQL to DynamoDb.");
            long total = 0;
            int chunkSize = 200;
            int offset = 0;
            int cnt;

            List<ExamDefinition> examDefinitions = new();
            List<ExamRun> examRuns = new();
            List<ExamSummary> examSummaries = new();
            List<ProblemSummary> problemSummaries = new();

            do
            {
                var result = MySqlDbHelper.GetExams(offset, chunkSize);
                cnt = result.Count;
                Console.WriteLine($"Exported {cnt} records from MySQL.");

                foreach (var exam in result)
                {
                    ExamDefinition? def = examDefinitions.FirstOrDefault(d => d.ExamTitle == exam.ExamTitle);
                    if (def == null)
                    {
                        def = ConvertToExamDefinition(exam);
                        examDefinitions.Add(def);
                    }
                    def.ExamDetails!.Add(ConvertToExamDetail(exam));

                    ExamRun? run = examRuns.FirstOrDefault(d => d.ExamTitle == exam.ExamTitle);
                    if (run == null)
                    {
                        run = ConvertToExamRun(exam);
                        examRuns.Add(run);
                    }
                    run.ExamRunDetails!.Add(ConvertToExamRunDetail(exam));
                    run.TotalCount++;
                    if (exam.IsCorrect ?? false) run.CorrectCount++;
                    if (exam.IsGuess ?? false) run.GuessCount++;
                    if ((exam.IsCorrect ?? false) && (exam.IsGuess ?? false)) run.GuessCorrectCount++;

                    ExamSummary? esum = examSummaries.FirstOrDefault(d => d.ProblemCategory == exam.ProblemCategory && d.AnswerBy == exam.AnswerBy);
                    if (esum == null)
                    {
                        esum = ConvertToExamSummary(exam);
                        examSummaries.Add(esum);
                    }
                    esum.TotalCount++;
                    esum.TotalDuration += exam.Duration;
                    if (exam.IsCorrect ?? false) esum.CorrectCount++;
                    if (exam.IsGuess ?? false) esum.GuessCount++;
                    if ((exam.IsCorrect ?? false) && (exam.IsGuess ?? false)) esum.GuessCorrectCount++;

                    ProblemSummary? psum = problemSummaries.FirstOrDefault(d => d.ProblemTitle == exam.ProblemTitle && d.AnswerBy == exam.AnswerBy);
                    if (psum == null)
                    {
                        psum = ConvertToProblemSummary(exam);
                        problemSummaries.Add(psum);
                    }
                    psum.TotalCount++;
                    psum.TotalDuration += exam.Duration;
                    if (exam.IsCorrect ?? false) psum.CorrectCount++;
                    if (exam.IsGuess ?? false) psum.GuessCount++;
                    if ((exam.IsCorrect ?? false) && (exam.IsGuess ?? false)) psum.GuessCorrectCount++;
                    if (psum.TotalCount > 0)
                    {
                        psum.TrueCorrectRate = (double) (psum.CorrectCount.GetValueOrDefault() - psum.GuessCorrectCount.GetValueOrDefault()) / psum.TotalCount.GetValueOrDefault();
                    }
                }

                offset += chunkSize;
            } while (cnt > 0);

            Console.WriteLine($"Exported {examDefinitions.Count} Exam Definitions from MySQL.");
            Console.WriteLine($"Exported {examRuns.Count} Exam Runs from MySQL.");
            Console.WriteLine($"Exported {examSummaries.Count} Exam Summaries from MySQL.");
            Console.WriteLine($"Exported {problemSummaries.Count} Problem Summaries from MySQL.");
            
            DynamoDBContextConfig? config = null;
            if (development)
            {
                config = new()
                {
                    TableNamePrefix = prefix
                };
            } else
            {
                // kp_exam_assignments and kp_exam_runs uses guid as Id
                // Thus, duplicate records will be created
                // My solution is deleting and re-creating these 2 tables.
                await DynamoDbHelper.PrepareTablesForProduction();
            }

            if (examDefinitions.Count > 0)
            {
                var count = await DynamoDbHelper.BatchWriteAsync(examDefinitions, config);
                Console.WriteLine($"Imported {count} Exam Definitions to DynamoDB.");
                total += count;
            }

            if (examRuns.Count > 0)
            {
                // generate assignment records
                List<Assignment> assignments = new();
                foreach(var run in examRuns)
                {
                    var assignment = new Assignment()
                    {
                        Id = Guid.NewGuid().ToString(),
                        FamilyId = run.FamilyId,
                        CreateTime = run.CreateTime,
                        ExamCategory = run.ExamCategory,
                        ExamTitle = run.ExamTitle,
                        IsComplete = true,
                        ExamRunIds = new() { run.Id! }
                    };
                    assignments.Add(assignment);
                    run.AssignmentId = assignment.Id;
                }

                var count = await DynamoDbHelper.BatchWriteAsync(examRuns, config);
                Console.WriteLine($"Imported {count} Exam Runs to DynamoDB.");
                total += count;

                count = await DynamoDbHelper.BatchWriteAsync(assignments, config);
                Console.WriteLine($"Imported {count} Assignments to DynamoDB.");
                total += count;
            }

            if (examSummaries.Count > 0)
            {
                var count = await DynamoDbHelper.BatchWriteAsync(examSummaries, config);
                Console.WriteLine($"Imported {count} Exam Summaries to DynamoDB.");
                total += count;
            }

            if (problemSummaries.Count > 0)
            {
                var count = await DynamoDbHelper.BatchWriteAsync(problemSummaries, config);
                Console.WriteLine($"Imported {count} Problem Summaries to DynamoDB.");
                total += count;
            }

            return total;
        }

        private static ExamDefinition ConvertToExamDefinition(KpExam exam)
        {
            return new ExamDefinition()
            {
                ExamCategory = exam.ProblemCategory,
                ExamTitle = exam.ExamTitle,
                ExamYear = exam.ProblemYear,
                ExamType = "H",
                Active = false,
                Memo = "",
                ExamDetails = new()
            };
        }

        private static ExamDetail ConvertToExamDetail(KpExam exam)
        {
            return new ExamDetail()
            {
                ProblemTitle = exam.ProblemTitle,
                ProblemAnswer = exam.ProblemAnswer,
                AnswerOptions = exam.AnswerOptions
            };
        }

        private static ExamRun ConvertToExamRun(KpExam exam)
        {
            return new ExamRun()
            {
                Id = Guid.NewGuid().ToString(),
                ExamCategory = exam.ProblemCategory,
                CreateTime = exam.CreateTime,
                ExamTitle = exam.ExamTitle,
                AnswerBy = exam.AnswerBy,
                StartTime = exam.StartTime,
                CompleteTime = exam.CompleteTime,
                FamilyId = exam.FamilyId,
                TotalCount = 0,
                CorrectCount = 0,
                GuessCount = 0,
                GuessCorrectCount = 0,
                TotalDuration = exam.TotalDuration,
                ExamRunDetails = new()
            };
        }

        private static ExamRunDetail ConvertToExamRunDetail(KpExam exam)
        {
            return new ExamRunDetail()
            {
                Id = Guid.NewGuid().ToString(),
                ProblemTitle = exam.ProblemTitle,
                UserAnswer = exam.Answer,
                IsCorrect = exam.IsCorrect,
                IsGuess = exam.IsGuess,
                Duration = exam.Duration
            };
        }

        private static ExamSummary ConvertToExamSummary(KpExam exam)
        {
            return new ExamSummary()
            {
                ProblemCategory = exam.ProblemCategory,
                AnswerBy = exam.AnswerBy,
                FamilyId=exam.FamilyId,
                TotalCount = 0,
                CorrectCount = 0,
                GuessCount = 0,
                GuessCorrectCount = 0,
                TotalDuration = 0
            };
        }

        private static ProblemSummary ConvertToProblemSummary(KpExam exam)
        {
            return new ProblemSummary()
            {
                ProblemTitle = exam.ProblemTitle,
                ProblemCategory = exam.ProblemCategory,
                AnswerBy = exam.AnswerBy,
                FamilyId = exam.FamilyId,
                TotalCount = 0,
                CorrectCount = 0,
                GuessCount = 0,
                GuessCorrectCount = 0,
                TrueCorrectRate = 0,
                TotalDuration = 0
            };
        }

    }

}
