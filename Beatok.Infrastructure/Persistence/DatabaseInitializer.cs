// using Beatok.Domain.Entities;
// using Microsoft.EntityFrameworkCore;
// using Beatok.Application.Interfaces;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace Beatok.Infrastructure.Persistence;
//
// public static class DatabaseInitializer
// {
//     public static async Task SeedAsync(IServiceProvider serviceProvider)
//     {
//         using var scope = serviceProvider.CreateScope();
//         var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//         var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
//
//         if (!await context.Users.AnyAsync(u => u.Role == UserRole.Administrator))
//         {
//             var admin = new User
//             {
//                 Name = "Administrator",
//                 Email = "admin@gmail.com",
//                 Role = UserRole.Administrator,
//                 PasswordHash = passwordHasher.GenerateHash("Admin123!")
//             };
//
//             await context.Users.AddAsync(admin);
//             await context.SaveChangesAsync();
//         }
//     }
// }