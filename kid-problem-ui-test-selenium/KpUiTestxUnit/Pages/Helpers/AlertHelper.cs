using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace KpUiTestxUnit.Pages;

public static class AlertHelper
{

    public static void HandleAlert(WebDriverWait wait, bool confirm = true)
    {
        IAlert alert = wait.Until(d => d.SwitchTo().Alert());
        if (confirm)
        {
            alert.Accept();
        }
        else
        {
            alert.Dismiss();
        }
    }

}


