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
using System.Security.Policy;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.Net.NetworkInformation;

namespace BotRadar_v2._0
{
    public class Bing_ResultsCrawler
    {
        public string searchString;
        public string searchUrlFilter;
        string baseFolder = "./output/";
        string outputFolder = string.Empty;
        string outputFile = "search_results.csv";
        string SE_GOOGLE = "Google";
        bool listenBrowser = true;

        public string searchUrl = string.Empty;
        public string searchStartUrl = "https://www.bing.com/search?q=";
        public string searchEndUrl = "";//"&sca_esv=be445f0cc062ab15&sxsrf=ADLYWIJbX2cqq4wyqVTSiBmRitn5TpXtfw:1715016452232&ei=BBM5ZpnmDYu3vr0Px4ez0Ag&start=100&sa=N&ved=2ahUKEwiZ-KyTxvmFAxWLm68BHcfDDIoQ8tMDegQIChAE&cshid=1715016540079012&biw=1366&bih=633&dpr=1";
        IWebDriver browserDriver = null;

         HashSet<string> allUrlHashmap = new HashSet<string>();
        //Hashtable allUrlHashmap = new Hashtable();
        List<string> allUrlLinks = new List<string>();

        public Bing_ResultsCrawler(string searchStr, string urlFilter)
        {
            if (!Directory.Exists(baseFolder))
            {
                Directory.CreateDirectory(baseFolder);
            }
            outputFolder = baseFolder + DateTime.Now.Ticks.ToString();
            Directory.CreateDirectory(outputFolder);

            searchUrlFilter = urlFilter;   
            searchString = searchStr;
            searchUrl = searchStartUrl + searchString + searchEndUrl;

            // Initialize WebDriver (replace with path to your ChromeDriver)
            browserDriver = new ChromeDriver();
        }
        public void PullSearchResults(String fName)
        {

            OpenSearchPage();
            GotoEndOfPage();
            ExtractResultUrls();
            while(IsNextSearchResultPageAvailable())
            {
                new WebDriverWait(browserDriver, TimeSpan.FromSeconds(1));
                MoveToNextSearchResultPage();
                GotoEndOfPage();
                ExtractResultUrls();
            }    
            
            // Extract and store search results
          
            // Close browser
            // driver.Quit();

            // Write results to CSV file
           WriteResultsToCsv(allUrlLinks, fName);
            allUrlLinks.Clear();
            Console.WriteLine($"Search results saved to: {outputFile}");
        }

        public void PullSearchResultsInExistingBrowser(String fName)
        {

            GotoEndOfPage();
            ExtractResultUrls();
            while (IsNextSearchResultPageAvailable())
            {
                new WebDriverWait(browserDriver, TimeSpan.FromSeconds(1));
                MoveToNextSearchResultPage();
                GotoEndOfPage();
                ExtractResultUrls();
            }

            // Extract and store search results

            // Close browser
            // driver.Quit();

            // Write results to CSV file
            WriteResultsToCsv(allUrlLinks, fName);

            Console.WriteLine($"Search results saved to: {outputFile}");
        }
        private void OpenSearchPage()
        {
            browserDriver.Navigate().GoToUrl(searchUrl);
        }


        private void GotoEndOfPage()
        {
            bool CanScrollMore = true;
            long scrollHeight = 0;

            do
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)browserDriver;
                var newScrollHeight = (long)js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight); return document.body.scrollHeight;");

                if (newScrollHeight == scrollHeight)
                {
                    CanScrollMore = false;
                    break;
                }
                else
                {
                    scrollHeight = newScrollHeight;
                    Thread.Sleep(1000);
                }
            } while (CanScrollMore);
        }
        private bool IsNextSearchResultPageAvailable()
        {
            bool isNextExist = false;
            string tname = string.Empty;
            string tlink = string.Empty;
            IWebElement nextButton = null;

            try
            {
                // var nextButton = browserDriver.FindElement(By.XPath("//a//h3//div//span[text()='More Results']")); // Common next button selector
                var links = browserDriver.FindElements(By.TagName("a"));
                foreach (var link in links)
                {
                    tlink = link.GetAttribute("href");
                    tname = link.GetAttribute("title");
                    if (tlink != null && tlink.IndexOf("/search?q=") >= 0 && tname.IndexOf("Next page") >= 0)
                    {
                        nextButton = link;
                        break;
                    }

                }
            }
            catch { }

            if (nextButton != null)
            {
                isNextExist = true;
            }
            return isNextExist;
        }

        private void MoveToNextSearchResultPage()
        {

            // var nextButton = browserDriver.FindElement(By.XPath("//a//h3//div//span[text()='More Results']")); // Common next button selector
            var links = browserDriver.FindElements(By.TagName("a"));
            string tlink = string.Empty;
            string tname = string.Empty;
            IWebElement nextButton = null;
            bool isNextButtonPresent = false;
            foreach(var link in  links)
            {
                tlink = link.GetAttribute("href");
                tname = link.GetAttribute("title");
                if (tlink != null && tlink.IndexOf("/search?q=") >= 0 && tname.IndexOf("Next page") >= 0)
                {
                    nextButton = link;
                    isNextButtonPresent = true;
                    break;
                }

            }    

            //var nextButton = browserDriver.FindElement(By.TagName("a"))
            if (nextButton != null && isNextButtonPresent)
            {
                // nextButton.Click();
                browserDriver.Navigate().GoToUrl(tlink);
                new WebDriverWait(browserDriver, TimeSpan.FromSeconds(1)).Until(d => d.Title != "");

            }
        }

        private void ExtractResultUrls()
        {
            // Extract links from current page
            string tLink = string.Empty;
            var links = browserDriver.FindElements(By.TagName("a"));
            foreach (var link in links)
            {
                try
                {

                    tLink = link.GetAttribute("href");
                if (tLink != null ) tLink = tLink.Trim().ToLower();
                if (tLink != null && tLink.IndexOf(searchUrlFilter) > 0 && tLink.IndexOf("bing.com") == -1 && !IsLinkAlreadyPresent(tLink))
                    {
                        allUrlHashmap.Add(tLink);
                        allUrlLinks.Add(tLink);
                    }
                }
                catch { }
            }

        }

        private List<string> ExtractSearchResults(IWebDriver driver)
        {
            var allHashmap = new HashSet<string>();
            var allLinks = new List<string>();
            String tLink;
            try
            {
                var nextButton = driver.FindElement(By.XPath("//span[text()='More Results']")); // Common next button selector

                while (listenBrowser == true)
                {
                    // Extract links from current page
                    var links = driver.FindElements(By.TagName("a"));
                    foreach (var link in links)
                    {
                        tLink = link.GetAttribute("href");

                        try
                        {
                            if (tLink != null && tLink.IndexOf("google.com") <= 0)
                            {
                                if (!IsLinkAlreadyPresent(tLink))
                                {
                                    allHashmap.Add(tLink);
                                    allLinks.Add(tLink);
                                }
                            }
                        }
                        catch { }
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

        private bool IsLinkAlreadyPresent(string link)
        {
            bool isPresent = false;
            foreach (string str in allUrlLinks)
            {
                if (str.Equals(link))
                { isPresent = true; break; }
            }
            return isPresent;

        }

        private void WriteResultsToCsv(List<string> allLinks, string fileName)
        {
            string fname; StreamWriter writer;
            if (string.IsNullOrEmpty(fileName))
            {

                fname = searchUrlFilter.ToString() + "_" + DateTime.Now.Ticks.ToString() + "_" + outputFile;
            }
            else
            {
                fname = fileName;
            }
           
            writer = new StreamWriter(fname, true);
            using (writer)
            {
             //   writer.WriteLine("Link");
                foreach (var link in allLinks)
                {
                    writer.WriteLine(link);
                }
                writer.Flush();
                writer.Close();

            }

        }

    }
}
