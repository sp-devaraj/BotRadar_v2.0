using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using static System.Net.WebRequestMethods;

namespace BotRadar_v2._0
{
    public class BotCrawler
    {
        static string searchTerm;
        static string searchEngine;
        static string outputFile = "search_results.csv";
        static string SE_GOOGLE = "Google";
        static bool listenBrowser = true;
        public static void PullSearchResults(string searchString)
        {

            // Initialize WebDriver (replace with path to your ChromeDriver)
            var driver = new ChromeDriver();
            //String url = "https://www.google.com/search?q=" + searchString + "&sca_esv=b20e215ce61cde95&source=hp&ei=CxA5ZunjDYjm2roPltSZuAw&iflsig=AL9hbdgAAAAAZjkeGxydtnuaXHIZLqTIJJxSrEQBPBTZ&ved=0ahUKEwiplb2ow_mFAxUIs1YBHRZqBscQ4dUDCA0&uact=5&oq=site%3Aconceptvines.com&gs_lp=Egdnd3Mtd2l6IhVzaXRlOmNvbmNlcHR2aW5lcy5jb21IwylQAFiLJ3AAeACQAQGYAf0BoAG4DqoBBjE3LjIuMrgBA8gBAPgBAZgCBKAC0ATCAhEQABiABBiRAhixAxiDARiKBcICFxAuGIAEGJECGLEDGNEDGIMBGMcBGIoFwgIOEAAYgAQYsQMYgwEYigXCAg4QLhiABBixAxiDARiKBcICERAuGIAEGLEDGNEDGIMBGMcBwgILEAAYgAQYsQMYgwHCAg4QABiABBiRAhixAxiKBcICERAuGIAEGJECGNEDGMcBGIoFwgILEC4YgAQYsQMYgwHCAggQABiABBixA8ICBRAuGIAEwgIFEAAYgATCAgoQABiABBhDGIoFwgILEC4YgAQY0QMYxwHCAgsQABiABBiRAhiKBcICBBAAGAOYAwCSBwUxLjIuMaAHykE&sclient=gws-wiz&zx=1715015684252&no_sw_cr=1";
            String url = "https://www.google.com/search?q=" + searchString + "&sca_esv=be445f0cc062ab15&sxsrf=ADLYWIJbX2cqq4wyqVTSiBmRitn5TpXtfw:1715016452232&ei=BBM5ZpnmDYu3vr0Px4ez0Ag&start=100&sa=N&ved=2ahUKEwiZ-KyTxvmFAxWLm68BHcfDDIoQ8tMDegQIChAE&cshid=1715016540079012&biw=1366&bih=633&dpr=1";
            driver.Navigate().GoToUrl(url );

            

            // Extract and store search results
            List<string> allLinks = ExtractSearchResults(driver);

            // Close browser
           // driver.Quit();

            // Write results to CSV file
            WriteResultsToCsv(allLinks);

            Console.WriteLine($"Search results saved to: {outputFile}");
        }

        private static string GetSearchEngineUrl(String searchEngine)
        {
            return searchEngine == SE_GOOGLE ? "https://www.google.com" : "https://www.bing.com";
        }

        private static List<string> ExtractSearchResults(IWebDriver driver)
        {
            var allLinks = new List<string>();
            try
            {
              //  var nextButton = driver.FindElement(By.XPath("//span[text()='More Results']")); // Common next button selector

                while (listenBrowser == true)
                {
                    // Extract links from current page
                    var links = driver.FindElements(By.TagName("a"));
                    foreach (var link in links)
                    {
                        allLinks.Add(link.GetAttribute("href"));
                    }

                    // Click next button and wait for page to load
                  //  nextButton.Click();
                    new WebDriverWait(driver, TimeSpan.FromSeconds(1)).Until(d => d.Title != "");
                    
                    // Check for next button again
                    //nextButton = driver.FindElement(By.XPath("//span[text()='More Results']"));
                }
            }
            catch { }

            return allLinks;
        }

        private static void WriteResultsToCsv(List<string> allLinks)
        {
            using (var writer = new StreamWriter(outputFile))
            {
                writer.WriteLine("Link");
                foreach (var link in allLinks)
                {
                    writer.WriteLine(link);
                }
            }
        }
    }
}
