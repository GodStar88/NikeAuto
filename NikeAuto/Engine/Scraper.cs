using NikeAuto.Define;
using NikeAuto.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace NikeAuto.Engine
{
    class Scraper
    {
        public Thread upcommingProductFetcher;
        private ChromeDriver driver;

        private void CreateDriver()
        {
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            driver = new ChromeDriver(driverService, options);
            driver.Manage().Cookies.DeleteAllCookies();
            //////driver.Navigate().GoToUrl("https://www.nike.com/launch/?s=upcoming");
            driver.Navigate().GoToUrl("https://www.nike.com/us/launch/?s=upcoming");
        }

        ////////public void FetchUpcommingProduct()
        ////////{
        ////////    try
        ////////    {
        ////////        //////Console.WriteLine("Creating Driver");
        ////////        CreateDriver();
        ////////        //////Console.WriteLine("Reaching here");
        ////////        driver.FindElementByXPath("//*[@id='root']/div/div/div[1]/div/div[3]/div[1]/header/div/div/div/div/div[2]/div/nav/ul/li[3]/a").Click();
        ////////        //////Console.WriteLine("After Click");
        ////////        new WebDriverWait(driver, new TimeSpan(0, 0, 5)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName("product-card")));
        ////////        //////Console.WriteLine("Presence");
        ////////        int productCount = driver.FindElementsByClassName("product-card").Count;
        ////////        for (int i = 0; i < productCount; i++)
        ////////        {
        ////////            new WebDriverWait(driver, new TimeSpan(0, 0, 5)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName("product-card")));
        ////////            var element = driver.FindElementsByClassName("product-card");
        ////////            if (i > 1)
        ////////            {
        ////////                Actions actions = new Actions(driver);
        ////////                actions.MoveToElement(element[i]);
        ////////                actions.Perform();
        ////////            }
        ////////            var prductButton = element[i].FindElement(By.ClassName("cta-container"));
        ////////            try
        ////////            {
        ////////                if (prductButton.FindElement(By.TagName("a")).Enabled)
        ////////                {
        ////////                    Product product = new Product();
        ////////                    var productNameElement = element[i].FindElement(By.ClassName("figcaption-content"));
        ////////                    string modelName = productNameElement.FindElement(By.TagName("h3")).Text;
        ////////                    string colorName = productNameElement.FindElement(By.TagName("h6")).Text;
        ////////                    product.Model = modelName.Remove(modelName.Length - 1);
        ////////                    string productColor = Regex.Replace(colorName, @"(/)", "");
        ////////                    product.Color = productColor.Remove(productColor.Length - 1);
        ////////                    product.ImageName = product.Model + "-" + product.Color + ".jpg";
        ////////                    string downloadURL = element[i].FindElement(By.TagName("img")).GetAttribute("src");
        ////////                    ImageDownloader _ImageDownloader = new ImageDownloader(downloadURL);
        ////////                    _ImageDownloader.SaveImage(@Program.basePath + "\\image\\" + product.ImageName, ImageFormat.Jpeg);

        ////////                    element[i].FindElement(By.TagName("a")).Click();
        ////////                    try
        ////////                    {
        ////////                        new WebDriverWait(driver, new TimeSpan(0, 0, 5)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName("product-info-container")));
        ////////                        var detailProductAside = driver.FindElementByClassName("product-info-container");
        ////////                        string dateTime = driver.FindElementByClassName("available-date-component").Text;
        ////////                        Actions actions = new Actions(driver);
        ////////                        actions.MoveToElement(detailProductAside);
        ////////                        actions.Perform();
        ////////                        var detailProductDiv = driver.FindElementByClassName("product-info");
        ////////                        var detailProductInfo = detailProductDiv.FindElements(By.XPath(".//*"));
        ////////                        product.Price = Convert.ToInt32(detailProductInfo[2].Text.Substring(1, detailProductInfo[2].Text.Length - 1));

        ////////                        string[] date = dateTime.Split(' ')[1].Split('/');
        ////////                        if (date[0].Length == 1)
        ////////                        {
        ////////                            date[0] = "0" + date[0];
        ////////                        }
        ////////                        if (date[1].Length == 1)
        ////////                        {
        ////////                            date[1] = "0" + date[1];
        ////////                        }
        ////////                        product.Date = DateTime.Now.Year.ToString() + "-" + date[0] + "-" + date[1];
        ////////                        string time = dateTime.Split(' ')[3];
        ////////                        if (Regex.Match(dateTime.Split(' ')[4], @"(.{2})\s*$").ToString() == "AM")
        ////////                        {
        ////////                            product.Time = time.Split(':')[0] + ":00:00";
        ////////                        }
        ////////                        else
        ////////                        {
        ////////                            product.Time = (Convert.ToInt32(time.Split(':')[0]) + 12).ToString() + ":00:00";
        ////////                        }
        ////////                        driver.Navigate().Back();
        ////////                        Console.WriteLine("Saving to db");
        ////////                        // Coloum imagename doesn't exist in the Product Table
        ////////                        // Where did you initialized the table?
        ////////                        /*public static void SaveProxy(Proxy proxy)
        ////////                        {
        ////////                            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
        ////////                            {
        ////////                                cnn.Execute("insert into Product (ip, port, userid, password) values (@Ip, @Port, @UserId, @Password)", proxy);
        ////////                            }
        ////////                        } In saveProxy you are inserting into Product table is it correct?*/
        ////////                        SqliteDataAccess.SaveProduct(product);
        ////////                    }
        ////////                    catch (Exception e)
        ////////                    {
        ////////                        Console.WriteLine("Product detail page working exception: " + e.Message);
        ////////                        driver.Navigate().Back();
        ////////                    }
        ////////                }
        ////////            }
        ////////            catch (Exception e)
        ////////            {
        ////////                Console.WriteLine("This product already released: " + e.Message);
        ////////            }
        ////////        }
        ////////    }
        ////////    catch (Exception e)
        ////////    {
        ////////        MessageBox.Show("Fetch the upcoming products exception: " + e.Message);
        ////////    }
        ////////}

        public void FetchUpcommingProduct()
        {
            try
            {
                CreateDriver();
                driver.FindElementByXPath("//*[@id='root']/div/div/div[1]/div/div[3]/div[1]/header/div/div/div/div/div[2]/div/nav/ul/li[3]/a").Click();
                new WebDriverWait(driver, new TimeSpan(0, 0, 10)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName("product-card")));
                int productCount = driver.FindElementsByClassName("product-card").Count;
                for (int i = 0; i < productCount; i++)
                {
                    driver.FindElementByXPath("//*[@id='root']/div/div/div[1]/div/div[3]/div[1]/header/div/div/div/div/div[2]/div/nav/ul/li[3]/a").Click();
                    new WebDriverWait(driver, new TimeSpan(0, 0, 10)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName("product-card")));
                    var element = driver.FindElementsByClassName("product-card");
                    ////////if (i > 1)
                    ////////{
                    ////////    Actions actions = new Actions(driver);
                    ////////    actions.MoveToElement(element[i]);
                    ////////    actions.Perform();
                    ////////}
                    var prductButton = element[i].FindElement(By.ClassName("cta-container"));
                    try
                    {
                        if (prductButton.FindElement(By.TagName("button")).Enabled)
                        {
                            Product product = new Product();
                            var productNameElement = element[i].FindElement(By.ClassName("figcaption-content"));
                            string modelName = productNameElement.FindElement(By.TagName("h3")).Text.Replace("'", "");
                            string colorName = productNameElement.FindElement(By.TagName("h6")).Text.Replace("'", "");
                            product.Model = modelName.Remove(modelName.Length - 1);
                            string productColor = Regex.Replace(colorName, @"(/)", "");
                            product.Color = productColor.Remove(productColor.Length - 1);
                            product.Image_Name = product.Model.Replace("'", "") + "-" + product.Color + ".jpg";
                            if(i > 1)
                            {
                                Actions actions1 = new Actions(driver);
                                actions1.MoveToElement(element[i]);
                                actions1.Perform();
                            }                           
                            string downloadURL = element[i].FindElement(By.TagName("img")).GetAttribute("src");
                            ImageDownloader _ImageDownloader = new ImageDownloader(downloadURL);
                            _ImageDownloader.SaveImage(@Program.basePath + "\\image\\" + product.Image_Name, ImageFormat.Jpeg);
                            element[i].FindElement(By.TagName("a")).Click();
                            try
                            {
                                new WebDriverWait(driver, new TimeSpan(0, 0, 10)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.ClassName("product-info-container")));
                                var detailProductAside = driver.FindElementByClassName("product-info-container");
                                Actions actions = new Actions(driver);
                                actions.MoveToElement(detailProductAside);
                                actions.Perform();
                                var detailProductDiv = driver.FindElementByClassName("product-info");
                                var detailProductInfo = detailProductDiv.FindElements(By.XPath(".//*"));

                                if (detailProductInfo[2].Text.ToString().Contains("$"))
                                {
                                    product.Price = Convert.ToInt32(detailProductInfo[2].Text.Substring(1, detailProductInfo[2].Text.Length - 1));
                                }

                                else if (detailProductInfo[3].Text.ToString().Contains("$"))
                                {
                                    product.Price = Convert.ToInt32(detailProductInfo[3].Text.Substring(1, detailProductInfo[3].Text.Length - 1));
                                }

                                else if (detailProductInfo[4].Text.ToString().Contains("$"))
                                {
                                    product.Price = Convert.ToInt32(detailProductInfo[4].Text.Substring(1, detailProductInfo[4].Text.Length - 1));
                                }

                                string dateTime = driver.FindElement(By.ClassName("available-date-component")).Text;
                                string[] date = dateTime.Split(' ')[1].Split('/');
                                if (date[0].Length == 1)
                                {
                                    date[0] = "0" + date[0];
                                }
                                if (date[1].Length == 1)
                                {
                                    date[1] = "0" + date[1];
                                }
                                product.Date = DateTime.Now.Year.ToString() + "-" + date[0] + "-" + date[1];
                                string time = dateTime.Split(' ')[3];
                                if (Regex.Match(dateTime.Split(' ')[4], @"(.{2})\s*$").ToString() == "AM")
                                {
                                    product.Time = time.Split(':')[0] + ":00:00";
                                }
                                else
                                {
                                    product.Time = (Convert.ToInt32(time.Split(':')[0]) + 12).ToString() + ":00:00";
                                }
                                new WebDriverWait(driver, new TimeSpan(0, 0, 10)).Until(SeleniumExtras.WaitHelpers.ExpectedConditions.PresenceOfAllElementsLocatedBy(By.TagName("meta")));
                                var SKU = driver.FindElementByName("branch:deeplink:styleColor").GetAttribute("content");
                                product.SKU = SKU;

                                driver.Navigate().Back();
                                ////////driver.Navigate().GoToUrl("https://www.nike.com/us/launch/?s=upcoming");
                                SqliteDataAccess.SaveProduct(product);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Product detail page working exception: " + e.Message);
                                driver.Navigate().Back();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("This product already released: " + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Fetch the upcoming products exception: " + e.Message);
            }
        }
    }
}