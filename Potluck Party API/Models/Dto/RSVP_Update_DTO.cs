using System.ComponentModel.DataAnnotations;

namespace Potluck_Party_API.Models.Dto
{
    public class RSVP_Update_DTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Food { get; set; }
        public int Quantity { get; set; }
    }
}
