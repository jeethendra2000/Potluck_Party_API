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

        // Check if it reaches the maximum servings quantity when i add/update the quantity
        private bool IsMaximumServingsQuantityExceeds(string food, int quantity)
        {
            var servingsLimit = Convert.ToInt32(Environment.GetEnvironmentVariable("MAXIMUM_SERVINGS_PER_DISH"));
            var servingQuantity = _db.Dish.AsNoTracking().Where(d => d.Food == food).Sum(d => d.Quantity);


            return servingQuantity + quantity > servingsLimit;
        }

        // Check User Limit
        private bool IsMaximumAttendeesReached()
        {
            var userLimit = Convert.ToInt32(Environment.GetEnvironmentVariable("MAXIMUM_ATTENDEES"));
            var userCount = _db.User.AsNoTracking().Count();

            return userCount >= userLimit;
        }

        // Check User is already present or not
        private bool IsUserExist(string email) => _db.User.AsNoTracking().Any(u => u.Email == email);


        // Creation of RSVP and Dish Details 
        [HttpPost(Name = "CreateRSVP")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<RSVP_Create_DTO> CreateRSVP([FromBody] RSVP_Create_DTO rsvpDTO)
        {
            if (rsvpDTO == null)
                return BadRequest(rsvpDTO);

            // Existing User
            if (IsUserExist(rsvpDTO.Email))
            {
                ModelState.AddModelError("Error Message", "Email already Exists!");
                return BadRequest(ModelState);
            }

            // Attendance Limit
            if (IsMaximumAttendeesReached())
            {
                ModelState.AddModelError("Message", "Sorry, we've reached the maximum number of attendees. We hope you can celebrate with us next time!");
                return BadRequest(ModelState);
            }

            // Dish Quantity Management & Dish Duplication Prevention
            if (IsMaximumServingsQuantityExceeds(rsvpDTO.Food, rsvpDTO.Quantity))
            {
                ModelState.AddModelError("Message", $"We appreciate your enthusiasm, but we've already reached the maximum quantity for '{rsvpDTO.Food}’. Please choose another dish.");
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

        // Deletion of RSVP and Dish Details 
        [HttpDelete("email", Name = "DeleteRSVP")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteRSVP(string email)
        {
            // Get the related record 
            var user = _db.User.FirstOrDefault(u => u.Email == email);

            if (user == null)
                return NotFound();

            // Update the Data
            _db.User.Remove(user);
            _db.SaveChanges();

            return NoContent();
        }

        // Updating Dish Details 
        [HttpPut(Name = "UpdateRSVP")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateRSVP([FromBody] RSVP_Update_DTO rsvpUpdateDTO)
        {
            if (rsvpUpdateDTO == null)
                return BadRequest();

            // If User record is not present
            if (!IsUserExist(rsvpUpdateDTO.Email))
                return NotFound($"User with {rsvpUpdateDTO.Email} Not Found!");

            // Retrieve the Dish record
            Dish? dish = _db.Dish.FirstOrDefault(d => d.Email == rsvpUpdateDTO.Email);

            if (dish == null)
                return NotFound($"Dish with {rsvpUpdateDTO.Food} for {rsvpUpdateDTO.Email} Not Found!");

            // Dish Quantity Management & Dish Duplication Prevention
            if (IsMaximumServingsQuantityExceeds(rsvpUpdateDTO.Food, rsvpUpdateDTO.Quantity - dish.Quantity))
            {
                ModelState.AddModelError("Message", $"We appreciate your enthusiasm, but we've already reached the maximum quantity for '{rsvpUpdateDTO.Food}’. Please choose another dish.");
                return BadRequest(ModelState);
            }

            // Update the Data
            dish.Email = rsvpUpdateDTO.Email;
            dish.Food = rsvpUpdateDTO.Food;
            dish.Quantity = rsvpUpdateDTO.Quantity;

            _db.Dish.Update(dish);
            _db.SaveChanges();

            return NoContent();
        }

    }
}
