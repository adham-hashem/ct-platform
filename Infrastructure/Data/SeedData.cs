using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            // Seed roles
            if (!await roleManager.RoleExistsAsync("Teacher"))
            {
                await roleManager.CreateAsync(new IdentityRole { Name = "Teacher", NormalizedName = "TEACHER", Id = "1" });
            }
            if (!await roleManager.RoleExistsAsync("Student"))
            {
                await roleManager.CreateAsync(new IdentityRole { Name = "Student", NormalizedName = "STUDENT", Id = "2" });
            }

            // Seed teacher
            var teacherId = "teacher-owner-id";
            var teacher = await userManager.FindByIdAsync(teacherId);
            if (teacher == null)
            {
                teacher = new ApplicationUser
                {
                    Id = teacherId,
                    UserName = "teacher@platform.com",
                    Email = "teacher@platform.com",
                    NormalizedUserName = "TEACHER@PLATFORM.COM",
                    NormalizedEmail = "TEACHER@PLATFORM.COM",
                    EmailConfirmed = true,
                    FirstName = "Platform",
                    LastName = "Owner",
                    RegistrationDate = new DateTime(2025, 6, 28, 0, 0, 0, DateTimeKind.Utc),
                    IsOnline = false,
                    VideoCompletionRate = 0.0,
                    Grade = EducationalLevel.FirstYear
                };
                var password = Environment.GetEnvironmentVariable("TeacherPassword") ?? "TeacherPassword123!";
                var result = await userManager.CreateAsync(teacher, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(teacher, "Teacher");
                }
                else
                {
                    throw new Exception($"Failed to create teacher: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
