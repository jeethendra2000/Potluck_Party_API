using System.ComponentModel.DataAnnotations;

namespace Potluck_Party_API.Models
{
    public class RSVP
    {
        [Key]
        public string Email { get; set; }
        public string Name { get; set; }
        public string Food { get; set; }
        public int Quantity { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
