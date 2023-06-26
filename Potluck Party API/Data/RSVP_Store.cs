using Potluck_Party_API.Models.Dto;

namespace Potluck_Party_API.Data
{
    public static class RSVP_Store
    {
        public static List<RSVP_DTO> RSVP_List = new() {
            new RSVP_DTO { Name = "SRS", Email = "srs@gmail.com", Food = "Parimala Prasada", Quantity = 3 },
            new RSVP_DTO { Name = "Jeethendra", Email = "jsr@gmail.com", Food = "Khunafa", Quantity = 4 }

        };

    }
}
