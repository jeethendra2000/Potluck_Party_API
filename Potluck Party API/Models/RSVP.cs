namespace Potluck_Party_API.Models
{
    public class RSVP
    {   

        public string Name { get; set; }
        public string Email { get; set; }
        public string Food { get; set; }
        public int Quantity { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
