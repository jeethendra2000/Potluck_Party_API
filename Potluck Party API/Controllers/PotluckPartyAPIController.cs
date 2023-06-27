using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Potluck_Party_API.Data;
using Potluck_Party_API.Models;
using Potluck_Party_API.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Potluck_Party_API.Controllers
{
    [Route("api/RSVP")]
    [ApiController]
    public class PotluckPartyAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public PotluckPartyAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost(Name = "CreateRSVP")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<RSVP_Create_DTO> CreateRSVP([FromBody] RSVP_Create_DTO rsvpDTO)
        {
            if (rsvpDTO == null)
                return BadRequest(rsvpDTO);

            // Check user is already present
            bool isUserExists = _db.User.AsNoTracking().Any(u => u.Email == rsvpDTO.Email);

            var userCount = _db.User.AsNoTracking().Count();

            var userLimit = Convert.ToInt32(Environment.GetEnvironmentVariable("MAXIMUM_ATTENDEES"));

            if (isUserExists)
            {
                ModelState.AddModelError("Error Message", "Email already Exists!");
                return BadRequest(ModelState);
            }


            if (userCount >= userLimit)
            {
                ModelState.AddModelError("Message", "Sorry, we've reached the maximum number of attendees. We hope you can celebrate with us next time!");
                return BadRequest(ModelState);
            }

            // Mapping User & Dish data
            User user = new()
            {
                Name = rsvpDTO.Name,
                Email = rsvpDTO.Email
            };

            Dish dish = new()
            {
                Email = rsvpDTO.Email,
                Food = rsvpDTO.Food,
                Quantity = rsvpDTO.Quantity,
            };

            // Save changes to databse
            _db.User.Add(user);
            _db.Dish.Add(dish);
            _db.SaveChanges();

            return CreatedAtAction(nameof(CreateRSVP), new { email = rsvpDTO.Email }, rsvpDTO);

        }


        [HttpDelete("email", Name = "DeleteRSVP")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteRSVP(string email)
        {
            // Get the related record 
            var user = _db.User.FirstOrDefault(u => u.Email == email);

            if (user == null)
                return NotFound();

            _db.User.Remove(user);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut(Name = "UpdateRSVP")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateRSVP([FromBody] RSVP_Update_DTO rsvpUpdateDTO)
        {
            if (rsvpUpdateDTO == null)
                return BadRequest();

            // Retrieve the record
            User? userRecord = _db.User.FirstOrDefault(u => u.Email == rsvpUpdateDTO.Email);


            // If User record is not present
            if (userRecord == null)
                return NotFound($"User with {rsvpUpdateDTO.Email} Not Found!");

            // Update the data
            Dish dish = new()
            {
                Email = rsvpUpdateDTO.Email,
                Food = rsvpUpdateDTO.Food,
                Quantity = rsvpUpdateDTO.Quantity,
            };


            _db.Dish.Update(dish);
            _db.SaveChanges();

            return NoContent();
        }

    }
}
