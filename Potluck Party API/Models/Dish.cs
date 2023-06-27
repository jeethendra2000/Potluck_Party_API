using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Potluck_Party_API.Models
{
    public class Dish
    {
        [Key]
        [ForeignKey("User")] // Foreign key to User table
        public string Email { get; set; }

        public string Food { get; set; }
        public int Quantity { get; set; }

        public User User { get; set; }

    }
}
