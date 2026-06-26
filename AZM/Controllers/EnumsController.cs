using AZM.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AZM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumsController : ControllerBase
    {
        /// <summary>Returns all sport types with their integer values.</summary>
        [HttpGet("sport-types")]
        public IActionResult GetSportTypes()
            => Ok(Enum.GetValues<SportType>()
                .Select(e => new { value = (int)e, name = e.ToString() }));

        /// <summary>Returns all event statuses.</summary>
        [HttpGet("event-statuses")]
        public IActionResult GetEventStatuses()
            => Ok(Enum.GetValues<EventStatus>()
                .Select(e => new { value = (int)e, name = e.ToString() }));

        /// <summary>Returns all difficulty levels.</summary>
        [HttpGet("difficulty-levels")]
        public IActionResult GetDifficultyLevels()
            => Ok(Enum.GetValues<DifficultyLevel>()
                .Select(e => new { value = (int)e, name = e.ToString() }));

        /// <summary>Returns all genders.</summary>
        [HttpGet("genders")]
        public IActionResult GetGenders()
            => Ok(Enum.GetValues<Gender>()
                .Select(e => new { value = (int)e, name = e.ToString() }));

        /// <summary>All enums in one call — useful for app startup.</summary>
        [HttpGet]
        public IActionResult GetAll() => Ok(new
        {
            sportTypes = Enum.GetValues<SportType>().Select(e => new { value = (int)e, name = e.ToString() }),
            eventStatuses = Enum.GetValues<EventStatus>().Select(e => new { value = (int)e, name = e.ToString() }),
            difficultyLevels = Enum.GetValues<DifficultyLevel>().Select(e => new { value = (int)e, name = e.ToString() }),
            genders = Enum.GetValues<Gender>().Select(e => new { value = (int)e, name = e.ToString() }),
        });
    }
}

