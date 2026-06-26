using AZM.Application.Auth.DTOs.Participants;
using AZM.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Queries
{    public record GetEventParticipantsQuery(Guid EventId) : IRequest<Result<ParticipantListDto>>;

   

}
