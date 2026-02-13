using KidproblemService.Models;
using MySql.Data.MySqlClient;
using System;

namespace Migration
{
    internal class MySqlDbHelper
    {
        // update the connectionString first, this server has been decommissioned. The credential shown here is harmless.
        private const string connectionString = "server=ec2-54-190-58-22.us-west-2.compute.amazonaws.com;user=remote;database=KIDPROBLEMS;port=3306;password=ekfX4hIJZATdchQRTKT2";

        public static List<Problem> GetProblems(int offset = 0, int size = 100)
        {
            List<Problem> result = new();
            using MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                connection.Open();

                string sql = $"SELECT * FROM kp_problem LIMIT {offset}, {size}";
                MySqlCommand command = new MySqlCommand(sql, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Problem problem = new();
                    problem.ProblemCategory = reader["PROBLEM_CATEGORY"].ToString();
                    problem.ProblemYear = reader["PROBLEM_YEAR"].ToString();
                    problem.ProblemTitle = reader["PROBLEM_TITLE"].ToString();
                    problem.ProblemNumber = reader["PROBLEM_NUMBER"].ToString();
                    problem.ProblemText = reader["PROBLEM_TEXT"].ToString();
                    problem.ProblemAnswer = reader["PROBLEM_ANSWER"].ToString();
                    problem.SolutionText = reader["SOLUTION_TEXT"].ToString();
                    problem.AnswerOptions = reader["ANSWER_OPTIONS"].ToString();
                    problem.IsStaging = reader["IS_STAGING"].ToString() == "Y";
                    problem.ProblemTags = (reader["PROBLEM_TAGS"].ToString() ?? string.Empty).Split(',');
                    result.Add(problem);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            connection.Close();
            return result;
        }

        public static List<KpExam> GetExams(int offset = 0, int size = 100)
        {
            List<KpExam> result = new();
            using MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                connection.Open();

                string sql = $"SELECT P.PROBLEM_TITLE,P.PROBLEM_CATEGORY,P.PROBLEM_YEAR,P.PROBLEM_ANSWER,P.ANSWER_OPTIONS,D.ANSWER,D.IS_CORRECT,D.IS_GUESS,D.ANSWER_BY,D.ANSWER_TIME,D.DURATION,D.FAMILY_ID,CONCAT(H.EXAM_TITLE,' ',H.EXAM_HDR_KEY) EXAM_TITLE,H.CREATE_TIME,H.START_TIME,H.COMPLETE_TIME,H.DURATION TOTAL_DURATION FROM kp_exam_dtl D,kp_problem P,kp_exam_hdr H WHERE D.PROBLEM_KEY=P.PROBLEM_KEY AND D.EXAM_HDR_KEY=H.EXAM_HDR_KEY LIMIT {offset}, {size}";
                MySqlCommand command = new MySqlCommand(sql, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    KpExam exam = new();
                    exam.ProblemTitle = reader["PROBLEM_TITLE"].ToString();
                    exam.ProblemCategory = reader["PROBLEM_CATEGORY"].ToString();
                    exam.ProblemYear = reader["PROBLEM_YEAR"].ToString();
                    exam.ProblemAnswer = reader["PROBLEM_ANSWER"].ToString();
                    exam.AnswerOptions = reader["ANSWER_OPTIONS"].ToString();
                    exam.Answer = reader["ANSWER"].ToString();
                    exam.IsCorrect = reader["IS_CORRECT"].ToString() == "Y";
                    exam.IsGuess = reader["IS_GUESS"].ToString() == "Y";
                    exam.AnswerBy = GetUserFullname(reader["ANSWER_BY"].ToString());
                    exam.AnswerTime = ConvertStringToDate(reader["ANSWER_TIME"].ToString());
                    exam.Duration = Convert.ToDouble(reader["DURATION"].ToString());
                    exam.FamilyId = GetFamilyIdName(reader["FAMILY_ID"].ToString());
                    exam.ExamTitle = reader["EXAM_TITLE"].ToString();
                    exam.CreateTime = ConvertStringToDate(reader["CREATE_TIME"].ToString());
                    exam.StartTime = ConvertStringToDate(reader["START_TIME"].ToString());
                    exam.CompleteTime = ConvertStringToDate(reader["COMPLETE_TIME"].ToString());
                    exam.TotalDuration = Convert.ToDouble(reader["TOTAL_DURATION"].ToString());

                    result.Add(exam);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            connection.Close();
            return result;
        }

        private static string GetUserFullname(string? userId)
        {
            switch (userId)
            {
                case "1":
                    return "dev";
                case "2":
                    return "chongtiangao";
                case "3":
                    return "tang";
                case "4":
                    return "yinkaigao";
                case "5":
                    return "demoparent";
                case "6":
                    return "demokid";
                case "7":
                    return "testuser";
                default:
                    return "unknown";
            }
        }

        private static string GetFamilyIdName(string? familyId)
        {
            switch (familyId)
            {
                case "1":
                default:
                    return "MainFamilyGroup";
                case "2":
                    return "DemoFamilyGroup";
            }
        }

        private static DateTime? ConvertStringToDate(string? dateTimeString)
        {
            if (dateTimeString == null)
            {
                return null;
            }

            if (DateTime.TryParse(dateTimeString, out DateTime result))
            {
                DateTime utcDateTime = DateTime.SpecifyKind(result, DateTimeKind.Utc);
                return utcDateTime;
            }

            return null;
        }

    }

}
