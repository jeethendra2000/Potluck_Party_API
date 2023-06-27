using System.ComponentModel.DataAnnotations;

namespace Potluck_Party_API.Models.Dto
{
    public class RSVP_Create_DTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? Food { get; set; }
        public int Quantity { get; set; }
    }
}
