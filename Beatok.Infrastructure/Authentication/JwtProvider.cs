using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Beatok.Application.Interfaces;
using Beatok.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Beatok.Infrastructure.Authentication;

public class JwtProvider(IOptions<JwtOptions> options): IJwtProvider
{
    private readonly JwtOptions _options = options.Value;
    
    public string GenerateToken(User user, bool isAnonymous = false)
    {
        Claim[] claims = [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new("is_anonymous", isAnonymous.ToString().ToLower())];
        
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)), 
            SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            issuer: _options.Issuer,
            audience: _options.Audience,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiresMinutes)
        );
        
        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenValue;
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
    
    public string ComputeHash(string input)
    {
        using var sha256 = SHA256.Create();
        return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }
}