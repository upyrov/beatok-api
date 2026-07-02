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
}