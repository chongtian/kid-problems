using System.Text.RegularExpressions;

namespace KidproblemService.Helpers
{
    public class ProblemHelper
    {
        public const string AssetsName = "assets";
        public const string DefaultProblemAnswerOptions = "A,B,C,D,E";
        public const string ProblemCategoryAIME = "AIME";

        public const string RegexGetProblem = ">Problem \\d{1,2}.*?(<p>.*?)<span class=\"mw-headline\"";
        public const string RegexProblemEndPattern = "<a.+?>Solution<\\/a>";
        public const string RegexProblemPattern = "(<h2><span class=\"mw-headline\".*?)<p><a href=";
        public const string RegexProblemPatternAlt = "(<span class=\"mw-headline\".*?)<h2>";
        public const string RegexProblemStartPattern = "<h2>.+?Problem\\s\\d{1,}.+?<\\/h2>";

        public static string CleanProblemText(string problemText)
        {
            string pattern, substitution;
            Regex regex;
            RegexOptions options = RegexOptions.Multiline;

            // replace \[ ... \] with $ ... $
            pattern = @"(alt=\"")(\\\[)(.+?)(\\\])(\"")";
            substitution = @"$1$$$3$$$5";
            regex = new Regex(pattern, options);
            string result = regex.Replace(problemText, substitution);

            // if the img tag has math latex as its alt attribute
            // use latex to replace the img tag
            pattern = @"\<img.+?src=\""(.+?)\"".+?alt=\""(\$.+?\$)\"".+?\>";
            substitution = @" $2 ";
            regex = new Regex(pattern, options);
            result = regex.Replace(result, substitution);

            // beautify some latex
            result = result.Replace("\\frac", "\\dfrac");
            return result;
        }

        public static string BeautifyProblem(string problemText)
        {
            string pattern = @"(<img src="")(\/\/.*?\/.{1}\/.{1}\/.{1}\/)";
            var replaced = Regex.Replace(problemText, pattern, "$1" + AssetsName + "/");

            pattern = @"<a.*?>.*?</a>";
            replaced = Regex.Replace(replaced, pattern, "");

            //pattern = @"<h2.*?>.*?</h2>";
            //replaced = Regex.Replace(replaced, pattern, "");

            pattern = @"(<h2>|<h3>)";
            replaced = Regex.Replace(replaced, pattern, "");

            //replaced = replaced.Replace(@"class=""latexcenter""", @"class=""latex""");
            //replaced = replaced.Replace("<h2>","<h3>").Replace("</h2>","</h3>");
            //replaced = replaced.Replace("<center>", "");
            return replaced;
        }

        public static string GetCategoryFromProblemTitle(string problemTitle)
        {
            if(string.IsNullOrEmpty(problemTitle))
            {
                return string.Empty;
            }

            int p = problemTitle.IndexOf('-');
            if (p < 0)
            {
                return string.Empty;
            }
            else
            {
                return problemTitle[..p];
            }
        }
    }
}
