namespace AmazonScrapeSelenium.Models
{
    public class Product : BaseEntity
    {
        public string Image { get; set; }
        public string Name { get; set; }
        public string Asin { get; set; }
        public string Link { get; set; }
        public Category? Category { get; set; }
        public int CategoryId { get; set; }
    }
}
