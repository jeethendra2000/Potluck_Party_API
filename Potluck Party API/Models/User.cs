using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Potluck_Party_API.Models
{
    public class User
    {
        [Key]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime CreatedDate { get; set; }

        public Dish Dish { get; set; }

    }
}
