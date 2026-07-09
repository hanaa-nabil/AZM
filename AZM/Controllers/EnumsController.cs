using AZM.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AZM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumsController : ControllerBase
    {
        [HttpGet("sport-types")]
        public IActionResult GetSportTypes() => Ok(ToDto<SportType>());

        [HttpGet("difficulty-levels")]
        public IActionResult GetDifficultyLevels() => Ok(ToDto<DifficultyLevel>());

        [HttpGet("paces")]
        public IActionResult GetPaces() => Ok(ToDto<Pace>());

        [HttpGet("participant-statuses")]
        public IActionResult GetParticipantStatuses() => Ok(ToDto<ParticipantStatus>());

        [HttpGet("genders")]
        public IActionResult GetGenders() => Ok(ToDto<Gender>());

        [HttpGet("event-statuses")]
        public IActionResult GetEventStatuses() => Ok(ToDto<EventStatus>());

        /// <summary>
        /// Convenience endpoint: returns all enums in one call, useful for a
        /// single "app config" fetch on client startup.
        /// </summary>
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            return Ok(new
            {
                sportTypes = ToDto<SportType>(),
                difficultyLevels = ToDto<DifficultyLevel>(),
                paces = ToDto<Pace>(),
                participantStatuses = ToDto<ParticipantStatus>(),
                genders = ToDto<Gender>(),
                eventStatuses = ToDto<EventStatus>()
            });
        }

        private static List<EnumDto> ToDto<TEnum>() where TEnum : struct, Enum
        {
            return Enum.GetValues<TEnum>()
                .Select(e => new EnumDto(Convert.ToInt32(e), e.ToString()))
                .ToList();
        }
    }

    public record EnumDto(int Value, string Name);
}

