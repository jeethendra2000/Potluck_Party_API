using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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


        [HttpGet]
        public ActionResult<IEnumerable<RSVP_DTO>> GetRSVPs()
        {
            return Ok(RSVP_Store.RSVP_List);
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
                var RSVP = RSVP_Store.RSVP_List.FirstOrDefault(u => u.Email == email);

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

            RSVP_Store.RSVP_List.Add(rsvpDTO);

            return Ok(rsvpDTO);

        }


        [HttpDelete("email", Name = "DeleteRSVP")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteRSVP(string email)
        {
            var rsvp = RSVP_Store.RSVP_List.FirstOrDefault(u => u.Email == email);

            if (rsvp == null) { return NotFound(); }
               
            RSVP_Store.RSVP_List.Remove(rsvp);

            return NoContent();
        }



    }
}
