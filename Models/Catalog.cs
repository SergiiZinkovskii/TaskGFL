namespace TestGFL.Models
{
    public class Catalog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public List<Catalog> Children { get; set; }
    }
}
