using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnAccount.Areas.Identity.Data;
using OnAccount.Models;

namespace OnAccount.Areas.Identity.Data;
public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }

        
    //private DbSet<AIModel> AIModel { get; set; }
    //private DbSet<PostModel> PostModel { get; set; }
    //private DbSet<VoteModel> VoteModels { get; set; }
    //private DbSet<FavoritesModel> FavoritesModel { get; set; }
    //public DbSet<OnAccount.Models.AppUserModel> AppUserModel { get; set; } = default!;

    //the next section overrides the default db naming // migrate and update database afterwords
    protected override void OnModelCreating(ModelBuilder builder)
    {
        
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("Identity");
        builder.Entity<AppUser>(entity => { entity.ToTable(name: "Users"); });
        builder.Entity<IdentityRole>(entity => { entity.ToTable(name: "Roles"); });
        builder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable(name: "UserRoles"); });
        builder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable(name: "UserClaims"); });
        builder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable(name: "UserLogins"); });
        builder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable(name: "RoleClaims"); });
        builder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable(name: "UserTokens"); });
        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
    }
    
}

internal class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(u => u.ScreenName).HasMaxLength(256);
        builder.Property(u => u.FirstName).HasMaxLength(256);
        builder.Property(u => u.LastName).HasMaxLength(256);
        builder.Property(u => u.Address).HasMaxLength(256);
        builder.Property(u => u.City).HasMaxLength(256);
        builder.Property(u => u.State).HasMaxLength(256);
        builder.Property(u => u.Zip).HasMaxLength(256);
        builder.Property(u => u.DateofBirth).HasMaxLength(256);
        builder.Property(u => u.PhoneNumber).HasMaxLength(256);
        builder.Property(u => u.UserRole).HasMaxLength(256);
        builder.Property(u => u.ActiveStatus);
        builder.Property(u => u.AcctReinstatementDate);
        builder.Property(u => u.AcctSuspensionDate);
        builder.Property(u => u.LastPasswordChangedDate);
        builder.Property(u => u.PasswordResetDays);
    }
}

