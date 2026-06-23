using AZM.Domain.Entities;

namespace AZM.Domain.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user, IList<string> roles);
    }
}