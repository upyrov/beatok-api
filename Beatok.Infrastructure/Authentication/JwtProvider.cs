using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Beatok.Application.DTOs;
using Beatok.Application.Interfaces;
using Beatok.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Beatok.Infrastructure.Authentication;

public class JwtProvider(IOptions<JwtOptions> options): IJwtProvider
{
    private readonly JwtOptions _options = options.Value;
    
    public JwtGenerateResult GenerateToken(User user)
    {
        Claim[] claims = [new(ClaimTypes.NameIdentifier, user.Id.ToString())];
        
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)), 
            SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            issuer: _options.Issuer,
            audience: _options.Audience,
            expires: DateTime.UtcNow.AddHours(_options.ExpiresHours)
        );
        
        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return new JwtGenerateResult
        {
            Token = tokenValue,
            Expires = token.ValidTo
        };
    }
}