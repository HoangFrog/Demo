using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace project.Models
{
    [Table("Groups")]
    public class ItemGroup
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Arrange { get; set; }
    }
}
