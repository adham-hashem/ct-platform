using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Domain.Entities;
using Domain.Enums;
using System.Text.Json;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<LessonAccessCode> LessonAccessCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Lesson configuration
            builder.Entity<Lesson>()
                .HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review configuration
            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(r => r.Course)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Review>()
                .HasIndex(r => r.UserId);

            builder.Entity<Review>()
                .HasIndex(r => r.CourseId);

            // Comment configuration
            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.Lesson)
                .WithMany(l => l.Comments)
                .HasForeignKey(c => c.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasIndex(c => c.UserId);

            builder.Entity<Comment>()
                .HasIndex(c => c.LessonId);

            // Question configuration
            builder.Entity<Question>()
                .HasOne(q => q.User)
                .WithMany(u => u.Questions)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Question>()
                .HasOne(q => q.Lesson)
                .WithMany(l => l.Questions)
                .HasForeignKey(q => q.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Question>()
                .HasIndex(q => q.UserId);

            builder.Entity<Question>()
                .HasIndex(q => q.LessonId);

            // Subscription configuration
            builder.Entity<Subscription>()
                .HasOne(s => s.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Subscription>()
                .Property(s => s.SubscribedMonths)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                    v => JsonSerializer.Deserialize<List<int>>(v, new JsonSerializerOptions()) ?? new List<int>(),
                    new ValueComparer<List<int>>(
                        (a, b) => (a == null && b == null) || (a != null && b != null && a.SequenceEqual(b)),
                        list => list != null ? list.Aggregate(0, (hash, i) => HashCode.Combine(hash, i)) : 0,
                        list => list != null ? list.ToList() : new List<int>()
                    ));

            builder.Entity<Subscription>()
                .Property(s => s.AccessedLessons)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                    v => JsonSerializer.Deserialize<List<Guid>>(v, new JsonSerializerOptions()) ?? new List<Guid>(),
                    new ValueComparer<List<Guid>>(
                        (a, b) => (a == null && b == null) || (a != null && b != null && a.SequenceEqual(b)),
                        list => list != null ? list.Aggregate(0, (hash, i) => HashCode.Combine(hash, i)) : 0,
                        list => list != null ? list.ToList() : new List<Guid>()
                    ));

            builder.Entity<Subscription>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Subscription>()
                .HasIndex(s => new { s.UserId, s.Grade });

            // Payment configuration
            builder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Payment>()
                .HasIndex(p => p.UserId);

            // Notification configuration
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Notification>()
                .HasIndex(n => n.UserId);

            // LessonAccessCode configuration
            builder.Entity<LessonAccessCode>()
                .HasOne(lac => lac.Lesson)
                .WithMany()
                .HasForeignKey(lac => lac.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<LessonAccessCode>()
                .HasOne(lac => lac.User)
                .WithMany()
                .HasForeignKey(lac => lac.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<LessonAccessCode>()
                .HasIndex(lac => lac.Code)
                .IsUnique();

            builder.Entity<LessonAccessCode>()
                .HasIndex(lac => lac.LessonId);
        }
    }
}