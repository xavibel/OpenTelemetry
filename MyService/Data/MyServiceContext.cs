using System;
using Microsoft.EntityFrameworkCore;
using MyService.Models;

namespace MyService.Data
{
    public class MyServiceContext : DbContext
    {
        public MyServiceContext (DbContextOptions<MyServiceContext> options)
            : base(options)
        {
        }

        public DbSet<Models.User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().Property(x => x.Id).HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<User>().HasData(
                new User() { Id = Guid.NewGuid(), MailAddress = "Mubeen@gmail.com", Name = "Mubeen", LastName = "Mubarak", BirthDate = new DateTime(1977, 2, 12)},
                new User() { Id = Guid.NewGuid(), MailAddress = "Tahir@gmail.com", Name = "Tahir", LastName = "Nahad", BirthDate = new DateTime(1977, 2, 12) },
                new User() { Id = Guid.NewGuid(), MailAddress = "Cheema@gmail.com", Name = "Cheema", LastName = "Celorio", BirthDate = new DateTime(1977, 2, 12) }
            );
        }
    }
}
