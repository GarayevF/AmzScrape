namespace AmazonScrapeSelenium.Models
{
    public class Commenter : BaseEntity
    {
        public string Profilelink { get; set; }
        public string FullName { get; set; }
        public List<Product>? Products { get; set; }
    }
}
