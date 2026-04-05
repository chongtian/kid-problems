using KpUiTestxUnit.Utilties;
using OpenQA.Selenium;

namespace KpUiTestxUnit.Pages;

public sealed class MenuBarComponentPage : BasePage
{

    private const int MAX_ATTEMPTS = 3;
    public MenuBarComponentPage(IWebDriver driver) : base(driver)
    { }

    public void ClickExamRunsBrowseAll()
    {
        var lv1MenuId = "menuExamRuns";
        var lv2MenuId = "menuBrowseAllExam";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    public void ClickExamRunsBrowseLatest()
    {
        var lv1MenuId = "menuExamRuns";
        var lv2MenuId = "menuBrowseLatestExam";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    public void ClickExamRunsExamSummary()
    {
        var lv1MenuId = "menuExamRuns";
        var lv2MenuId = "menuBrowseExamSummary";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    public void ClickExamRunsProblemSummary()
    {
        var lv1MenuId = "menuExamRuns";
        var lv2MenuId = "menuBrowseProblemSummary";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    public void ClickBrowseAllAssignments()
    {
        var lv1MenuId = "menuAssignments";
        var lv2MenuId = "menuBrowseAllAssignment";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    public void ClickBrowseLatestAssignments()
    {
        var lv1MenuId = "menuAssignments";
        var lv2MenuId = "menuBrowseLatestAssignment";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    public void ClickCreateExamDefinition()
    {
        var lv1MenuId = "menuExamDefinitions";
        var lv2MenuId = "menuCreateExamDefinitions";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    public void ClickBrowseActiveExamDefinition()
    {
        var lv1MenuId = "menuExamDefinitions";
        var lv2MenuId = "menuBrowseActiveExamDefinitions";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    public void ClickBrowseAllExamDefinition()
    {
        var lv1MenuId = "menuExamDefinitions";
        var lv2MenuId = "menuBrowseAllExamDefinitions";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    public void ClickBrowseProblems()
    {
        var lv1MenuId = "menuProblems";
        var lv2MenuId = "menuBrowseProblems";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    public void ClickCreateProblem()
    {
        var lv1MenuId = "menuProblems";
        var lv2MenuId = "menuCreateProblem";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    public void ClickBrowseStagingProblems()
    {
        var lv1MenuId = "menuProblems";
        var lv2MenuId = "menuBrowseStagingProblems";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    public void ClickCrawlProblems()
    {
        var lv1MenuId = "menuProblems";
        var lv2MenuId = "menuCrawlProblems";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    public void ClickUploadAnswers()
    {
        var lv1MenuId = "menuProblems";
        var lv2MenuId = "menuUploadAnswers";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    public void ClickBulkUpdate()
    {
        var lv1MenuId = "menuProblems";
        var lv2MenuId = "menuBulkUpdate";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    public void ClickBulkCreate()
    {
        var lv1MenuId = "menuProblems";
        var lv2MenuId = "menuBulkCreate";
        ClickSubmenu(lv1MenuId, lv2MenuId);
    }

    private void ClickSubmenu(string lv1MenuId, string lv2MenuId)
    {
        if (!IsNotLoading())
        {
            throw new ElementNotInteractableException("page is loading and the element is covered by overlay.");
        }

        var lv1MenuItem = _wait.Until(d => d.FindElement(By.CssSelector($"kp-menu button[data-testid=\"{lv1MenuId}\"]")));
        if (lv1MenuItem == null)
        {
            throw new NoSuchElementException($"Cannot find element {lv1MenuId}");
        }
        if (!lv1MenuItem!.Enabled || !lv1MenuItem.Displayed)
        {
            throw new ElementNotInteractableException($"Cannot interact with element {lv1MenuId}");
        }
        lv1MenuItem.Click();

        _wait.Until(d => d.FindElements(By.CssSelector(".cdk-overlay-pane")).Count > 0);
        int attempt = 0;
        int status = 0;
        while (attempt < MAX_ATTEMPTS)
        {
            attempt++;
            var lv2MenuItem = _wait.Until(d => d.FindElement(By.CssSelector($"a[data-testid=\"{lv2MenuId}\"]")));
            if (lv2MenuItem == null)
            {
                status = 1;
                continue;
            }
            if (!lv2MenuItem!.Enabled || !lv2MenuItem.Displayed)
            {
                status = 2;
                continue;
            }
            try
            {
                lv2MenuItem.Click();
                status = 0;
                break;
            }
            catch (ElementClickInterceptedException)
            {
                status = 2;
                continue;
            }

        }

        if (status == 1)
        {
            throw new NoSuchElementException($"Cannot find element {lv2MenuId} after {attempt} attempts.");
        }

        if (status == 2)
        {
            throw new ElementNotInteractableException($"Cannot interact with element {lv2MenuId} after {attempt} attempts.");
        }

        if (status == 3)
        {
            throw new ElementClickInterceptedException($"Cannot click element {lv2MenuId} after {attempt} attempts.");
        }

    }

}
