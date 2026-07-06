using AZM.Domain.Entities;

namespace AZM.Domain.Interfaces
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAtUtc) GenerateJwtToken(User user, IList<string> roles);
    }
}