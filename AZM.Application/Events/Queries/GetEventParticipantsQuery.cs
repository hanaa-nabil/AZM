using AZM.Application.Common;
using AZM.Application.DTOs.Participants;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Queries
{    public record GetEventParticipantsQuery(Guid EventId) : IRequest<Result<ParticipantListDto>>;

   

}
