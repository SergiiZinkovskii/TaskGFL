using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestGFL.Models
{
    public class Catalog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
       
        public string Name { get; set; }

       
        public int? ParentId { get; set; }
    
        public List<Catalog> Children { get; set; }
    }
}
