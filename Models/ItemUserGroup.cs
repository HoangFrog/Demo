using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace project.Models
{
    [Table("UsersGroups")]
    public class ItemUserGroup
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
    }
}
