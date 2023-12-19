using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace project.Models
{
    [Table("MenusGroups")]
    public class ItemMenuGroup
    {
        [Key]
        public int Id { get; set; }
        public int MenuId { get; set; }
        public int GroupId { get; set; }
    }
}
