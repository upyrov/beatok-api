using System.Security.Authentication;
using Beatok.Application.DTOs;
using Beatok.Application.DTOs.User;
using Beatok.Application.Exceptions;
using Beatok.Application.Interfaces;
using Beatok.Application.Interfaces.Repositories;
using Beatok.Application.Interfaces.Services;
using Beatok.Domain.Entities;
using FluentValidation;

namespace Beatok.Application.Services;

public class AuthService(IPasswordHasher passwordHasher,
    IUserRepository userRepository, IValidator<UserRegisterDto> validator,
    IJwtProvider jwtProvider): IAuthService
{
    public async Task RegisterAsync(UserRegisterDto dto)
    {
        var fluentValidationResult = await validator.ValidateAsync(dto);

        if (!fluentValidationResult.IsValid)
        {
            throw new ValidationException(fluentValidationResult.Errors);
        }
        
        if (await userRepository.ExistsByEmailAsync(dto.Email))
        {
            throw new EmailAlreadyExistsException("User with this email already exists");
        }
        
        var passwordHash = passwordHasher.GenerateHash(dto.Password);

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = passwordHash
        };
        
        await userRepository.AddAsync(user);
    }

    public async Task<AuthResult> LoginAsync(UserLoginDto dto)
    {
        var user = await userRepository.GetByEmailAsync(dto.Email);

        if (user == null)
        {
            throw new InvalidCredentialException("Invalid email or password");
        }
        
        var passwordVerified = passwordHasher.VerifyHash(dto.Password, user.PasswordHash!);

        if (!passwordVerified)
        {
            throw new InvalidCredentialException("Invalid email or password");
        }
        
        var jwtGenerateResult = jwtProvider.GenerateToken(user);

        return new AuthResult
        {
            Token = jwtGenerateResult.Token,
            Expires = jwtGenerateResult.Expires
        };
    }

    public async Task<AuthResult> LoginAnonymousAsync()
    {
        var userName = GenerateAnonymousName();

        var user = new User
        {
            Name = userName,
            IsAnonymous = true
        };
        
        await userRepository.AddAsync(user);
        
        var jwtGenerateResult = jwtProvider.GenerateToken(user, true);
        return new AuthResult
        {
            Token = jwtGenerateResult.Token,
            Expires = jwtGenerateResult.Expires
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