using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project.Models
{
    // class nay se tac dong den table nao
    [Table("Users")]
    public class ItemUser
    {
        // dinh nghia key
        [Key]
        public int Id { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public String Password { get; set; }

    }
}
