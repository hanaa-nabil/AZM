using AZM.Application.DTOs;
using AZM.Application.Users.Queries;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Handlers
{
    public class GetUserEventCountsBySportHandler : IRequestHandler<GetUserEventCountsBySportQuery, List<SportEventCountDto>>
    {
        private readonly IEventRepository _eventRepo;

        public GetUserEventCountsBySportHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<List<SportEventCountDto>> Handle(GetUserEventCountsBySportQuery request, CancellationToken ct)
        {
            var joined = await _eventRepo.GetUserJoinedEventsAsync(request.UserId, ct);

            // Only count events where this user's own participation is marked completed
            var completed = joined.Where(e =>
                e.Participants.Any(p =>
                    p.UserId == request.UserId &&
                    p.HasCompletedActivity));

            return completed
                .GroupBy(e => e.SportType)
                .Select(g => new SportEventCountDto
                {
                    SportType = g.Key.ToString(),
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .ToList();
        }
    }
}
