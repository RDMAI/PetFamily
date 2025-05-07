using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects;
using System.Text.Json;

namespace PetFamily.Accounts.Infrastructure.Identity;

public class AccountDBContext : IdentityDbContext<User, Role, Guid>
{
    private readonly string _connectionString;

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<AdminAccount> AdminAccounts => Set<AdminAccount>();
    public DbSet<VolunteerAccount> VolunteerAccounts => Set<VolunteerAccount>();
    public DbSet<ParticipantAccount> ParticipantAccounts => Set<ParticipantAccount>();
    public DbSet<RefreshSession> RefreshSessions => Set<RefreshSession>();

    public AccountDBContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(DBConstants.SCHEMA);

        // Identity configs
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");
        builder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");
        
        // User configs
        builder.Entity<User>().ToTable("users");

        builder.Entity<User>()
            .Property(u => u.FirstName)
            .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
            .HasColumnName("first_name");

        builder.Entity<User>()
            .Property(u => u.LastName)
            .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
            .HasColumnName("last_name");

        builder.Entity<User>()
            .Property(u => u.FatherName)
            .HasMaxLength(Constants.MAX_LOW_TEXT_LENGTH)
            .HasColumnName("father_name");

        builder.Entity<User>()
            .Property(d => d.SocialNetworks)
            .HasConversion(
                reqToDB => Serialize(reqToDB),
                jsonFromDB => Deserialize<SocialNetwork>(jsonFromDB),
                GetValueComparer<SocialNetwork>())
            .HasColumnType("jsonb")
            .HasColumnName("social_networks");

        // Role configs
        builder.Entity<Role>().ToTable("roles");

        // Permission configs
        builder.Entity<Permission>().ToTable("permissions");

        builder.Entity<Permission>()
            .HasIndex(p => p.Code)
            .IsUnique();

        // RolePermission configs
        builder.Entity<RolePermission>().ToTable("role_permissions");

        builder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        builder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        builder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany()
            .HasForeignKey(rp => rp.PermissionId);

        // AdminAccount configurations
        builder.Entity<AdminAccount>()
            .ToTable("admin_accounts")
            .HasKey(a => a.Id);

        builder.Entity<AdminAccount>()
            .HasIndex(a => a.UserId)
            .IsUnique();

        builder.Entity<AdminAccount>()
            .HasOne(a => a.User)
            .WithOne()
            .HasForeignKey<AdminAccount>(a => a.UserId);

        // VolunteerAccount configurations
        builder.Entity<VolunteerAccount>()
            .ToTable("volunteer_accounts")
            .HasKey(v => v.Id);

        builder.Entity<VolunteerAccount>()
            .HasOne(v => v.User)
            .WithOne()
            .HasForeignKey<VolunteerAccount>(v => v.UserId);

        builder.Entity<VolunteerAccount>()
            .Property(v => v.Requisites)
            .HasConversion(
                reqToDB => Serialize(reqToDB),
                jsonFromDB => Deserialize<Requisites>(jsonFromDB),
                GetValueComparer<Requisites>())
            .HasColumnType("jsonb")
            .HasColumnName("requisites");

        // ParticipantAccount configurations
        builder.Entity<ParticipantAccount>()
            .ToTable("participant_accounts")
            .HasKey(p => p.Id);

        builder.Entity<ParticipantAccount>()
            .HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<ParticipantAccount>(p => p.UserId);

        // RefreshSession configurations
        builder.Entity<RefreshSession>()
            .ToTable("refresh_sessions")
            .HasKey(r => r.Id);

        builder.Entity<RefreshSession>()
            .HasIndex(r => r.RefreshToken)
            .IsUnique();

        builder.Entity<RefreshSession>()
            .HasOne(r => r.User)
            .WithOne()
            .HasForeignKey<RefreshSession>(r => r.UserId);

        builder.Entity<RefreshSession>()
            .Property(d => d.CreatedAt)
            .HasConversion(
                src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
            ).IsRequired();

        builder.Entity<RefreshSession>()
            .Property(d => d.ExpiresAt)
            .HasConversion(
                src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc)
            ).IsRequired();
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });

    private static string Serialize<T>(List<T> list)
    {
        return JsonSerializer.Serialize(list, JsonSerializerOptions.Default);
    }

    private static List<T> Deserialize<T>(string json)
    {
        if (json == "{}") return [];  // default empty value

        var deserialized = JsonSerializer.Deserialize<List<T>>(json, JsonSerializerOptions.Default);
        if (deserialized == null) return [];  // if value somehow is null

        return deserialized;
    }

    private static ValueComparer<List<T>> GetValueComparer<T>()
    {
        return new ValueComparer<List<T>>(
            (vl1, vl2) => vl1!.SequenceEqual(vl2!),
            vl => vl.Aggregate(0, (s, f) => HashCode.Combine(s, f!.GetHashCode())),
            vl => vl.ToList());
    }
}
