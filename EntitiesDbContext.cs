using GenericControllerLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GenericControllerLib
{
    /// <summary>
    ///     Controla el acceso a la BD cumpliendo el estandar de seguridad identity
    /// </summary>
    public class EntitiesDbContext : IdentityDbContext<User, Role, int>
    {
        public EntitiesDbContext(DbContextOptions<EntitiesDbContext> options) : base(options) { }

        /// <summary>
        ///     Cambia las opciones del acceso a la BD
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.EnableSensitiveDataLogging();
        }

        /// <summary>
        ///     Inserta datos a la hora de realizar el comando update-database
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var hasher = new PasswordHasher<User>();

            builder.Entity<Role>()
                .ToTable("T_ROLES")
                .HasData(
                    new Role { Id = 1, Name = "Superadmin", NormalizedName = "SUPERADMIN" },
                    new Role { Id = 2, Name = "Admin", NormalizedName = "ADMIN" },
                    new Role { Id = 3, Name = "Estandar", NormalizedName = "ESTANDAR" }
                );

            builder.Entity<User>()
                .ToTable("T_USUARIOS")
                .Ignore(i => i.Roles)
                .Ignore(i => i.TwoFactorEnabled)
                .HasData(
                    new User
                    {
                        Id = 1,
                        UserName = "superadmin",
                        NormalizedUserName = "SUPERADMIN",
                        PasswordHash = hasher.HashPassword(null, "1234"),
                        SecurityStamp = Guid.NewGuid().ToString()
                    },
                    new User
                    {
                        Id = 2,
                        UserName = "admin",
                        NormalizedUserName = "ADMIN",
                        PasswordHash = hasher.HashPassword(null, "1234"),
                        SecurityStamp = Guid.NewGuid().ToString()
                    },
                    new User
                    {
                        Id = 3,
                        UserName = "estandar",
                        NormalizedUserName = "ESTANDAR",
                        PasswordHash = hasher.HashPassword(null, "1234"),
                        SecurityStamp = Guid.NewGuid().ToString()
                    }
                );

            builder.Entity<IdentityUserRole<int>>()
                .ToTable("T_USUARIOS_X_ROLES")
                .HasData(
                    new IdentityUserRole<int> { UserId = 1, RoleId = 1 },
                    new IdentityUserRole<int> { UserId = 2, RoleId = 2 },
                    new IdentityUserRole<int> { UserId = 3, RoleId = 3 }
                );
        }
    }
}
