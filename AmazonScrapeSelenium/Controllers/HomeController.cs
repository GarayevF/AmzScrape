using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using AmazonScrapeSelenium.Models;
using AmazonScrapeSelenium.DAL;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace AmazonScrapeSelenium.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetCategoryLinks")]
        public async Task<IActionResult> GetCategories()
        {
            //var driver = new ChromeDriver();
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("no-sandbox");

            ChromeDriver driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(60));

            string amazonnewreleaseurl = "https://www.amazon.com/gp/new-releases/?ref_=nav_em_cs_newreleases_0_1_1_3";

            driver.Url = amazonnewreleaseurl;
            Thread.Sleep(5000);
            driver.Navigate().Refresh();

            IList<IWebElement> elements = driver.FindElements(By.CssSelector("._p13n-zg-nav-tree-all_style_zg-browse-item__1rdKf a"));


            List<Category> categories = new List<Category>();
            List<Category> categories2 = new List<Category>();

            foreach (IWebElement item in elements)
            {
                categories.Add(new Category
                {
                    Name = item.Text,
                    Link = item.GetAttribute("href")
                });
                categories2.Add(new Category
                {
                    Name = item.Text,
                    Link = item.GetAttribute("href")
                });
            }

            //driver.Url = "https://www.amazon.com/gp/new-releases/amazon-renewed/24430048011/ref=zg_bsnr_nav_amazon-renewed_1";
            //Thread.Sleep(1000);
            //driver.Navigate().Refresh();

            //IWebElement categoryelement1 = driver.FindElement(By.CssSelector("._p13n-zg-nav-tree-all_style_zg-selected__1SfhQ"));

            //IWebElement parent1 = categoryelement1.FindElement(By.XPath(".."));

            //if (IsElementPresent(driver, By.CssSelector("div._p13n-zg-nav-tree-all_style_zg-browse-group__88fbz"), parent1))
            //{
            //    Console.WriteLine("a");
            //}
            //else
            //{
            //    Console.WriteLine("a");

            //}

            while (categories2.Count > 0)
            {
                Category firstElement = categories2[0];

                driver.Url = firstElement.Link;

                Thread.Sleep(1000);
                driver.Navigate().Refresh();

                IWebElement categoryelement = driver.FindElement(By.CssSelector("._p13n-zg-nav-tree-all_style_zg-selected__1SfhQ"));

                IWebElement parent = categoryelement.FindElement(By.XPath(".."));

                if (IsElementPresent(driver, By.CssSelector("div._p13n-zg-nav-tree-all_style_zg-browse-group__88fbz"), parent))
                {
                    IWebElement nextSibling = parent.FindElement(By.XPath("..")).FindElement(By.CssSelector("div._p13n-zg-nav-tree-all_style_zg-browse-group__88fbz"));

                    IList<IWebElement> innerElements = nextSibling.FindElements(By.CssSelector("._p13n-zg-nav-tree-all_style_zg-browse-item__1rdKf a"));

                    List<Category> test = innerElements.Select(item =>
                    {
                        return new Category { Name = item.Text, Link = item.GetAttribute("href") };
                    }).ToList();


                    categories2.AddRange(innerElements.Select(item =>
                    {
                        return new Category { Name = item.Text, Link = item.GetAttribute("href") };
                    }).Where(p => !categories.Any(p1 => p1.Name == p.Name)));

                    categories.AddRange(innerElements.Select(item =>
                    {
                        return new Category { Name = item.Text, Link = item.GetAttribute("href") };
                    }).Where(p => !categories.Any(p1 => p1.Name == p.Name)));

                    var text = JsonConvert.SerializeObject(categories2.Select(a => a.Name));

                    
                }

                categories2.RemoveAt(0);
            }

            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();



            //((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight - 150)");

            return Ok(categories);
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var driver = new ChromeDriver();
            Thread.Sleep(5000);
            driver.Navigate().Refresh();
            driver.Manage().Window.Maximize();

            List<Category> categories = await _context.Categories.Where(c => c.IsVisited == false).ToListAsync();
            

            foreach (Category category in categories)
            {
                
                List<Product> products = new List<Product>();
                driver.Url = category.Link;
                Thread.Sleep(1000);
                driver.Navigate().Refresh();

                if (!IsElementPresent(driver, By.CssSelector("#endOfList")))
                {
                    category.IsVisited = true;
                    await _context.SaveChangesAsync();
                    continue;
                }

                var lastHeight = ((IJavaScriptExecutor)driver).ExecuteScript("return document.body.scrollHeight");

                List<IWebElement> pages = driver.FindElements(By.CssSelector("li.a-normal")).ToList();

                #region FirstPage

                while (true)
                {

                    if (!IsElementPresent(driver, By.CssSelector("#endOfList"))) break;

                    ((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector(\"#endOfList\").scrollIntoView()");

                    Thread.Sleep(2000);

                    var newHeight = ((IJavaScriptExecutor)driver).ExecuteScript("return document.body.scrollHeight");

                    if ((int)(long)newHeight == (int)(long)lastHeight) break;

                    lastHeight = newHeight;
                }

                IList<IWebElement> elements = driver.FindElements(By.CssSelector("._cDEzb_grid-column_2hIsc"));

                foreach (IWebElement element in elements)
                {
                    try
                    {
                        Product tempproduct = new Product
                        {
                            Asin = element.FindElement(By.CssSelector(".p13n-grid-content ._cDEzb_iveVideoWrapper_JJ34T")).GetAttribute("data-asin"),
                            Link = element.FindElement(By.CssSelector(".p13n-grid-content ._cDEzb_iveVideoWrapper_JJ34T .zg-grid-general-faceout .p13n-sc-uncoverable-faceout a")).GetAttribute("href"),
                            Name = element.FindElements(By.CssSelector(".p13n-grid-content ._cDEzb_iveVideoWrapper_JJ34T .zg-grid-general-faceout .p13n-sc-uncoverable-faceout a"))[1].FindElement(By.CssSelector("span div")).Text,
                            Image = element.FindElements(By.CssSelector(".p13n-grid-content ._cDEzb_iveVideoWrapper_JJ34T .zg-grid-general-faceout .p13n-sc-uncoverable-faceout a"))[0].FindElement(By.CssSelector("div img")).GetAttribute("src"),
                            Category = category,
                            CategoryId = category.Id,
                        };

                        bool isexist = await _context.Products.AnyAsync(a => a.Asin == tempproduct.Asin);

                        if (!isexist)
                        {
                            products.Add(tempproduct);
                        }
                        
                    }
                    catch (Exception)
                    {

                    }

                }

                #endregion

                #region OtherPages

                if(pages != null && pages.Count > 0 )
                {
                    foreach (IWebElement page in pages)
                    {
                        driver.Url = page.FindElement(By.CssSelector("a")).GetAttribute("href");
                        Thread.Sleep(1000);
                        driver.Navigate().Refresh();

                        while (true)
                        {
                            if (!IsElementPresent(driver, By.CssSelector("#endOfList"))) break;

                            ((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector(\"#endOfList\").scrollIntoView()");

                            Thread.Sleep(2000);

                            var newHeight = ((IJavaScriptExecutor)driver).ExecuteScript("return document.body.scrollHeight");

                            if ((int)(long)newHeight == (int)(long)lastHeight) break;

                            lastHeight = newHeight;
                        }

                        IList<IWebElement> elements2 = driver.FindElements(By.CssSelector("._cDEzb_grid-column_2hIsc"));

                        foreach (IWebElement element in elements2)
                        {
                            try
                            {
                                products.Add(new Product
                                {
                                    Asin = element.FindElement(By.CssSelector(".p13n-grid-content ._cDEzb_iveVideoWrapper_JJ34T")).GetAttribute("data-asin"),
                                    Link = element.FindElement(By.CssSelector(".p13n-grid-content ._cDEzb_iveVideoWrapper_JJ34T .zg-grid-general-faceout .p13n-sc-uncoverable-faceout a")).GetAttribute("href"),
                                    Name = element.FindElements(By.CssSelector(".p13n-grid-content ._cDEzb_iveVideoWrapper_JJ34T .zg-grid-general-faceout .p13n-sc-uncoverable-faceout a"))[1].FindElement(By.CssSelector("span div")).Text,
                                    Image = element.FindElements(By.CssSelector(".p13n-grid-content ._cDEzb_iveVideoWrapper_JJ34T .zg-grid-general-faceout .p13n-sc-uncoverable-faceout a"))[0].FindElement(By.CssSelector("div img")).GetAttribute("src"),
                                    Category = category,
                                    CategoryId = category.Id,
                                });
                            }
                            catch (Exception)
                            {

                            }

                        }
                    }
                }

                #endregion

                category.IsVisited = true;
                await _context.Products.AddRangeAsync(products);
                await _context.SaveChangesAsync();

            }

            

            return Ok();
        }


        [HttpGet("GetProductCount")]
        public async Task<IActionResult> GetProductCount()
        {
            IEnumerable<Product> products = await _context.Products.ToListAsync();

            return Ok(products.Count());
        }


        [HttpGet("GetUnvisitedCategoryCount")]
        public async Task<IActionResult> GetUnvisitedCategoryCount()
        {
            IEnumerable<Category> categories = await _context.Categories.Where(a => a.IsVisited == false).ToListAsync();

            return Ok(categories.Count());
        }

        [HttpGet("GetVisitedCategoryCount")]
        public async Task<IActionResult> GetCategoryCount()
        {
            IEnumerable<Category> categories = await _context.Categories.Where(a => a.IsVisited == true).ToListAsync();

            return Ok(categories.Count());
        }





        private bool IsElementPresent(ChromeDriver driver, By by, IWebElement? element=null)
        {
            if(element != null)
            {
                try
                {
                    element.FindElement(By.XPath("..")).FindElement(by);
                    return true;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    driver.FindElement(by);
                    return true;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            }
        }
    }
}
