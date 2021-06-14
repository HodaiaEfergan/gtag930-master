#region

using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Web.Models;

#endregion

namespace Web.Data
{
    public class ApplicationDbContext :
        IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, ApplicationUserRole,
            IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>, IPersistedGrantDbContext
    {
        private readonly IOptions<OperationalStoreOptions> _operationalStoreOptions;

        public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options)
        {
            _operationalStoreOptions = operationalStoreOptions;
        }

        public DbSet<ApplicationUserUnitInfo> ApplicationUserUnitInfos { get; set; }
        public DbSet<TagScan> TagScans { get; set; }
        public DbSet<UnitInfo> UnitInfos { get; set; }
        public DbSet<UnitScan> UnitScans { get; set; }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

        Task<int> IPersistedGrantDbContext.SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigurePersistedGrantContext(_operationalStoreOptions.Value);

            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new {ur.UserId, ur.RoleId});

                userRole.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId).IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                userRole.HasOne(ur => ur.User).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.UserId).IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ApplicationUser>(builder =>
            {
                builder.HasMany(x => x.AssignedUnits).WithOne(x => x.ApplicationUser)
                    .HasForeignKey(x => x.ApplicationUserId).OnDelete(DeleteBehavior.Cascade);
                builder.HasMany(x => x.UserRoles).WithOne(x => x.User).HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                builder.HasMany(x => x.UnitScans).WithOne(x => x.Owner).HasForeignKey(x => x.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ApplicationRole>(builder =>
            {
                builder.HasMany(x => x.UserRoles).WithOne(x => x.Role).HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ApplicationUserUnitInfo>(builder =>
            {
                builder.HasKey(x => new {x.ApplicationUserId, x.UnitInfoId});
                builder.HasOne(x => x.ApplicationUser).WithMany(x => x.AssignedUnits)
                    .HasForeignKey(x => x.ApplicationUserId).OnDelete(DeleteBehavior.Cascade);
                builder.HasOne(x => x.UnitInfo).WithMany(x => x.AssignedUsers).HasForeignKey(x => x.UnitInfoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<TagScan>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).ValueGeneratedOnAdd();
            });

            builder.Entity<UnitInfo>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.HasMany(x => x.UnitScans).WithOne(x => x.UnitInfo).HasForeignKey(x => x.UnitInfoId)
                    .OnDelete(DeleteBehavior.Cascade);
                builder.HasMany(x => x.AssignedUsers).WithOne(x => x.UnitInfo).HasForeignKey(x => x.UnitInfoId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasData(new UnitInfo {Id = UnitInfo.DEFAULT_ID, Name = "Default Unit"});
            });

            builder.Entity<UnitScan>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).ValueGeneratedOnAdd();
                builder.HasOne(x => x.UnitInfo).WithMany(x => x.UnitScans).HasForeignKey(x => x.UnitInfoId)
                    .OnDelete(DeleteBehavior.Cascade);
                builder.HasMany(x => x.TagScans).WithOne(x => x.UnitScan).HasForeignKey(x => x.UnitScanId)
                    .OnDelete(DeleteBehavior.Cascade);
                builder.HasOne(x => x.Owner).WithMany(x => x.UnitScans).HasForeignKey(x => x.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();

            base.OnConfiguring(optionsBuilder);
        }
    }
}