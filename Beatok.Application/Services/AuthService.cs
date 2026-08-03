using System.Security.Authentication;
using Beatok.Application.DTOs;
using Beatok.Application.DTOs.User;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Beatok.Application.Services;

public class AuthService(IPasswordHasher passwordHasher,
    IValidator<UserSignupDto> validator,
    IJwtProvider jwtProvider, IApplicationDbContext context): IAuthService
{
    public async Task SignUpAsync(UserSignupDto dto, Guid? userId)
    {
        var fluentValidationResult = await validator.ValidateAsync(dto);

        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }
        
        if (await context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            throw new EmailAlreadyExistsException("User with this email already exists");
        }
        
        User? existingUser = await context.Users.FindAsync(userId);
        if (existingUser != null && existingUser.IsAnonymous)
        {
            await ConvertAnonymousUserAsync(dto, existingUser);
            return;
        }
        
        var passwordHash = passwordHasher.GenerateHash(dto.Password);

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = passwordHash,
            LastActiveAt = null,
            Role = UserRole.Player
        };
        
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    private async Task ConvertAnonymousUserAsync(UserSignupDto dto, User user)
    {
        user.Name = dto.Name;
        user.Email = dto.Email;
        user.LastActiveAt = null;
        user.PasswordHash = passwordHasher.GenerateHash(dto.Password);
        user.Role = UserRole.Player;
        user.IsAnonymous = false;
        await context.SaveChangesAsync();
    }

    public async Task<AuthResultDto> SignInAsync(UserSigninDto dto)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
        {
            throw new InvalidCredentialException("Invalid email or password");
        }
        
        var passwordVerified = passwordHasher.VerifyHash(dto.Password, user.PasswordHash!);

        if (!passwordVerified)
        {
            throw new InvalidCredentialException("Invalid email or password");
        }
        
        var accessToken = jwtProvider.GenerateToken(user);
        var refreshToken = jwtProvider.GenerateRefreshToken();
        
        var refreshTokenEntity = new RefreshToken
        {
            TokenHash = jwtProvider.ComputeHash(refreshToken),
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(30)
        };

        await context.RefreshTokens.AddAsync(refreshTokenEntity);
        await context.SaveChangesAsync();
        
        return new AuthResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expires = refreshTokenEntity.Expires
        };
    }

    public async Task<AuthResultDto> AuthenticateExternalUserAsync(ExternalUserInfo userInfo, Guid? userIdClaim)
    {
        if (!userInfo.EmailVerified)
        {
            throw new BadRequestException("Email not verified");
        }
        
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == userInfo.Email);
        
        if (userIdClaim.HasValue)
        {
            var currentUser = await context.Users.FindAsync(userIdClaim);
            if (currentUser != null && currentUser.IsAnonymous)
            {
                if (user != null)
                {
                    throw new BadRequestException("User with this email already exists");
                }
                currentUser.IsAnonymous = false;
                currentUser.Name = userInfo.Name;
                currentUser.Email = userInfo.Email;
                currentUser.LastActiveAt = null;
                user = currentUser;
            }
        }
        if (user == null)
        {
            user = new User
            {
                Name = userInfo.Name,
                Email = userInfo.Email,
                LastActiveAt = null,
                Role = UserRole.Player,
                IsAnonymous = false
            };
            await context.Users.AddAsync(user);
        }
        
        var accessToken = jwtProvider.GenerateToken(user);
        var refreshToken = jwtProvider.GenerateRefreshToken();
        
        var refreshTokenEntity = new RefreshToken
        {
            TokenHash = jwtProvider.ComputeHash(refreshToken),
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(30)
        };

        await context.RefreshTokens.AddAsync(refreshTokenEntity);
        await context.SaveChangesAsync();
        
        return new AuthResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expires = refreshTokenEntity.Expires
        };
    }
    
    public async Task<AuthResultDto> SignInAnonymousAsync()
    {
        var userName = GenerateAnonymousName();

        var user = new User
        {
            Name = userName,
            IsAnonymous = true,
            LastActiveAt = DateTime.UtcNow,
            Role = UserRole.Player
        };
        
        await context.Users.AddAsync(user);
        
        var accessToken = jwtProvider.GenerateToken(user, true);
        var refreshToken = jwtProvider.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            TokenHash = jwtProvider.ComputeHash(refreshToken),
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddYears(1)
        };
        
        await context.RefreshTokens.AddAsync(refreshTokenEntity);
        await context.SaveChangesAsync();
        
        return new AuthResultDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expires = refreshTokenEntity.Expires
        };
    }

    public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
    {
        var tokenHash = jwtProvider.ComputeHash(refreshToken);
        var refreshTokenEntity = await context.RefreshTokens
            .Where(r => r.TokenHash == tokenHash)
            .Include(r => r.User)
            .FirstOrDefaultAsync();

        if (refreshTokenEntity == null || refreshTokenEntity.Expires < DateTime.UtcNow)
        {
            throw new TokenExpiredException("The refresh token has expired");
        }

        context.RefreshTokens.Remove(refreshTokenEntity);
        await context.SaveChangesAsync();
        
        string accessToken = jwtProvider.GenerateToken(refreshTokenEntity.User!);
        string newRefreshToken = jwtProvider.GenerateRefreshToken();

        RefreshToken newRefreshTokenEntity = new RefreshToken
        {
            TokenHash = jwtProvider.ComputeHash(newRefreshToken),
            Expires = refreshTokenEntity.User!.IsAnonymous ? 
                DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddDays(30), 
            UserId = refreshTokenEntity.UserId
        };
        
        await context.RefreshTokens.AddAsync(newRefreshTokenEntity);
        await context.SaveChangesAsync();

        return new AuthResultDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            Expires = newRefreshTokenEntity.Expires
        };
    }

    private string GenerateAnonymousName()
    {
        string[] adjectives = 
        [
            "Swift", "Silent", "Clever", "Brave", "Bright", "Calm", "Fierce", 
            "Quick", "Wise", "Bold", "Kind", "Lucky", "Wild", "Sharp", "Active"
        ];

        string[] animals = 
        [
            "Fox", "Owl", "Wolf", "Bear", "Cat", "Hawk", "Deer", 
            "Lion", "Lynx", "Falcon", "Eagle", "Tiger", "Panda", "Badger"
        ];
        
        var adjective = adjectives[Random.Shared.Next(adjectives.Length)];
        var animal = animals[Random.Shared.Next(animals.Length)];
        var number = Random.Shared.Next(1000, 10000); 

        return $"{adjective}{animal}{number}";
    }
}