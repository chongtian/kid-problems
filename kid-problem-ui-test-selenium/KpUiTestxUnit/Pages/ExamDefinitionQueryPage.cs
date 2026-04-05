using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class ExamDefinitionQueryPage : BasePage
{
    public readonly static string PageUrlAll = $"{Constants.BASE_URL}/examdefs/all";
    public readonly static string PageUrlActive = $"{Constants.BASE_URL}/examdefs";
    public ExamDefinitionQueryPage(IWebDriver driver) : base(driver)
    { }



}
