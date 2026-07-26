using AZM.Application.DTOs.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Queries
{
    public record GetMyAchievementsQuery(Guid UserId) : IRequest<List<AchievementDto>>;
}
