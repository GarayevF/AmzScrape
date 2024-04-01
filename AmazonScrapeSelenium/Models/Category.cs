
namespace AmazonScrapeSelenium.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public bool IsVisited { get; set; }
    }
}
