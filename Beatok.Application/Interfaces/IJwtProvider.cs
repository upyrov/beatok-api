using Beatok.Application.DTOs;
using Beatok.Domain.Entities;

namespace Beatok.Application.Interfaces;

public interface IJwtProvider
{
    JwtGenerateResult GenerateToken(User user);
}