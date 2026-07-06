using Beatok.Application.DTOs;
using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(User user, bool isAnonymous = false);
    string GenerateRefreshToken();
    string ComputeHash(string input);
}