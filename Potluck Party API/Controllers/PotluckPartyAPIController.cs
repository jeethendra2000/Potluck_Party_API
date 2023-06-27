using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public ActionResult<IEnumerable<RSVP_DTO>> GetRSVPs()
        {
            return Ok(_db.RSVP.ToList());
        }

        [HttpGet("email", Name = "CreateRSVP")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<RSVP_DTO?> GetRSVP(string email)
        {

            string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";
            if (Regex.IsMatch(email, regex, RegexOptions.IgnoreCase))
            {
                var RSVP = _db.RSVP.FirstOrDefault(u => u.Email == email);

                if (RSVP == null) { return NotFound(); }

                return Ok(RSVP);
            }
            else { return BadRequest(); }

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<RSVP_DTO> CreateRSVP([FromBody] RSVP_DTO rsvpDTO)
        {
            // Check if email is already present
            //var isAlreadyPresent = RSVP_Store.RSVP_List.FirstOrDefault(u => u.Email.ToLower() == rsvpDTO.Email.ToLower());
            //if (isAlreadyPresent != null)
            //{
            //    ModelState.AddModelError("CustomeError", "Email already Exists!");
            //    return BadRequest(ModelState);
            //}

            if (rsvpDTO == null)
                return BadRequest(rsvpDTO);

            string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";
            if (Regex.IsMatch(rsvpDTO.Email, regex, RegexOptions.IgnoreCase))
                return StatusCode(StatusCodes.Status500InternalServerError);

            RSVP rsvp = new()
            {
                Email = rsvpDTO.Email,
                Name = rsvpDTO.Name,
                Food = rsvpDTO.Food,
                Quantity = rsvpDTO.Quantity,
                CreatedDate = DateTime.Now
            };

            _db.RSVP.Add(rsvp);
            _db.SaveChanges();

            //RSVP_Store.RSVP_List.Add(rsvpDTO);

            return Ok(rsvpDTO);

        }


        [HttpDelete("email", Name = "DeleteRSVP")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteRSVP(string email)
        {
            var rsvp = _db.RSVP.FirstOrDefault(u => u.Email == email);

            if (rsvp == null) { return NotFound(); }
               
            _db.RSVP.Remove(rsvp);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut(Name = "UpdateRSVP")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateRSVP( [FromBody]RSVP_DTO rsvpDTO)
        {
            if(rsvpDTO == null)
            {
                return BadRequest();
            }
        
            RSVP rsvp = new()
            {   Name = rsvpDTO.Name,
                Email = rsvpDTO.Email,
                Food = rsvpDTO.Food,
                Quantity = rsvpDTO.Quantity,
            };

            _db.RSVP.Update(rsvp);
            _db.SaveChanges(); 
            return NoContent();

        }

        //[HttpPatch("email", Name = "UpdateDishDetails")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //public ActionResult<RSVP_DTO> UpdateRSVP(string email, JsonPatchDocument<RSVP_DTO> patchRSVP_DTO)
        //{
        //    var rsvp = RSVP_Store.RSVP_List.FirstOrDefault(u => u.Email == email);

        //    if (rsvp == null) { return NotFound(); }

        //    patchRSVP_DTO.ApplyTo(rsvp, ModelState);

        //    if(!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    return Ok(rsvp);
             
        //}




    }
}
