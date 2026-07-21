using System.Security.Authentication;
using Beatok.Application.DTOs;
using Beatok.Application.DTOs.User;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;

namespace Beatok.Application.Services;

public class AuthService(IPasswordHasher passwordHasher,
    IValidator<UserSignupDto> validator,
    IJwtProvider jwtProvider, IUnitOfWork unitOfWork): IAuthService
{
    public async Task SignUpAsync(UserSignupDto dto)
    {
        var fluentValidationResult = await validator.ValidateAsync(dto);

        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }
        
        if (await unitOfWork.Users.ExistsByEmailAsync(dto.Email))
        {
            throw new EmailAlreadyExistsException("User with this email already exists");
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
        
        await unitOfWork.Users.AddAsync(user);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<AuthResultDto> SignInAsync(UserSigninDto dto)
    {
        var user = await unitOfWork.Users.GetByEmailAsync(dto.Email);

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

        await unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
        await unitOfWork.SaveChangesAsync();
        
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
        
        await unitOfWork.Users.AddAsync(user);
        
        var accessToken = jwtProvider.GenerateToken(user, true);
        var refreshToken = jwtProvider.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            TokenHash = jwtProvider.ComputeHash(refreshToken),
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddYears(1)
        };
        
        await unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
        await unitOfWork.SaveChangesAsync();
        
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
        var refreshTokenEntity = await unitOfWork.RefreshTokens.GetByHashAsync(tokenHash);

        if (refreshTokenEntity == null || refreshTokenEntity.Expires < DateTime.UtcNow)
        {
            throw new TokenExpiredException("The refresh token has expired");
        }
        
        unitOfWork.RefreshTokens.Delete(refreshTokenEntity);

        string accessToken = jwtProvider.GenerateToken(refreshTokenEntity.User!);
        string newRefreshToken = jwtProvider.GenerateRefreshToken();

        RefreshToken newRefreshTokenEntity = new RefreshToken
        {
            TokenHash = jwtProvider.ComputeHash(newRefreshToken),
            Expires = refreshTokenEntity.User!.IsAnonymous ? 
                DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddDays(30), 
            UserId = refreshTokenEntity.UserId
        };
        
        await unitOfWork.RefreshTokens.AddAsync(newRefreshTokenEntity);
        await unitOfWork.SaveChangesAsync();

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