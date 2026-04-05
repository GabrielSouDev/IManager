using IManager.Web.Domain.Consts;
using IManager.Web.Domain.Entities.Companies;
using IManager.Web.Domain.Entities.Payrolls;
using IManager.Web.Domain.Entities.TimeTrackings;
using IManager.Web.Domain.Entities.Users;
using IManager.Web.Presentation.Configurations;
using IManager.Web.Presentation.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IManager.Web.Data.Persistence;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    protected ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<JobTitle> JobTitles { get; set; }
    public DbSet<TimeEntry> TimeEntries { get; set; }
    public DbSet<TimeCheck> TimeChecks { get; set; }
    public DbSet<Payroll> Payrolls { get; set; }
    public DbSet<Payslip> Payslips { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var postgresSettings = configuration.GetSection("Postgres").Get<PostgresSettings>()
                ?? throw new Exception("Postgres config not found");

            optionsBuilder.UseNpgsql(postgresSettings.ConnectionString);
        }
    }

    #region OnModelCreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Chave primária explícita para UserProfile
        modelBuilder.Entity<UserProfile>()
            .HasKey(u => u.Id);

        // 1:1 User -> UserProfile
        modelBuilder.Entity<User>()
            .HasOne(u => u.UserProfile)
            .WithOne()
            .HasForeignKey<UserProfile>(p => p.Id)
            .IsRequired();

        // Company
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DocumentNumber).IsRequired().HasMaxLength(18);
            entity.Property(e => e.LegalName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TradeName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FoundedAt).IsRequired();

            entity.HasMany(e => e.Employees)
                  .WithOne(e => e.Company)
                  .HasForeignKey(e => e.CompanyId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Departments)
                  .WithOne(e => e.Company)
                  .HasForeignKey(e => e.CompanyId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Payrolls)
                  .WithOne(e => e.Company)
                  .HasForeignKey(e => e.CompanyId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Department
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

            entity.HasMany(e => e.JobTitles)
                  .WithOne(e => e.Department)
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // JobTitle
        modelBuilder.Entity<JobTitle>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DailyHours).IsRequired();

            entity.HasMany(e => e.Employees)
                  .WithOne(e => e.JobTitle)
                  .HasForeignKey(e => e.JobTitleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // UserProfile
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DocumentNumber).IsRequired().HasMaxLength(14);
            entity.Property(e => e.BirthDate).IsRequired();
            entity.Property(e => e.BaseSalary).HasColumnType("decimal(18,2)");

            entity.HasMany(e => e.TimeEntries)
                  .WithOne(e => e.Employee)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Payslips)
                  .WithOne(e => e.Employee)
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // TimeEntry
        modelBuilder.Entity<TimeEntry>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasMany(e => e.Checks)
                  .WithOne(e => e.TimeEntry)
                  .HasForeignKey(e => e.TimeEntryId)
                  .OnDelete(DeleteBehavior.Cascade); // checks dependem totalmente do TimeEntry
            
            entity.Ignore(te => te.HoursWorked);

            entity.Ignore(te => te.IsConsistent);
        });

        // TimeCheck
        modelBuilder.Entity<TimeCheck>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Timestamp).IsRequired();
        });

        // Payroll
        modelBuilder.Entity<Payroll>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PeriodStart).IsRequired();
            entity.Property(e => e.PeriodEnd).IsRequired();

            entity.HasMany(e => e.Payslips)
                  .WithOne(e => e.Payroll)
                  .HasForeignKey(e => e.PayrollId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Payslip
        modelBuilder.Entity<Payslip>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.GrossSalary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.OvertimeAdditionals).HasColumnType("decimal(18,2)");
            entity.Property(e => e.HazardPay).HasColumnType("decimal(18,2)");
            entity.Property(e => e.UnhealthyPay).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Commission).HasColumnType("decimal(18,2)");
            entity.Property(e => e.INSSDeduction).HasColumnType("decimal(18,2)");
            entity.Property(e => e.IRRFDeduction).HasColumnType("decimal(18,2)");
            entity.Property(e => e.OtherDeductions).HasColumnType("decimal(18,2)");

            // propriedades calculadas são ignoradas pelo EF
            entity.Ignore(e => e.TotalEarnings);
            entity.Ignore(e => e.TotalDeductions);
            entity.Ignore(e => e.NetSalary);
        });
    }
    #endregion

    #region SeedDataAsync
    public async Task SeedDataAsync(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, RegisterViewModel adminRequest)
    {
        var retries = 1;
        while (true)
        {
            try
            {
                await Database.MigrateAsync();
                break;
            }
            catch (Npgsql.NpgsqlException ex)
            {
                Console.WriteLine($"# {retries} - Tentativa de conexão ao Postgres falha!  Error: {ex.Message}");
                retries++;
                if (retries >= 10) throw;
                await Task.Delay(2000);
            }
        }

        foreach (var role in Role.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        if (userManager.Users.Any()) return;

        using var transaction = Database.BeginTransaction();
        try
        {
            await SeedStaffUserAsync(userManager, roleManager, adminRequest);

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    private async Task SeedStaffUserAsync(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, RegisterViewModel adminRequest)
    {
        var staffCompany = new Company
        {
            DocumentNumber = "00.000.000/0001-00",
            LegalName = "IManager",
            TradeName = "IManager",
            FoundedAt = new DateOnly(2000, 01, 01)
        };
        await Companies.AddAsync(staffCompany);
        await SaveChangesAsync();

        var staffDepartment = new Department
        {
            Name = "TI",
            CompanyId = staffCompany.Id
        };
        await Departments.AddAsync(staffDepartment);
        await SaveChangesAsync();

        var staffJobTitle = new JobTitle
        {
            Name = "Administrador",
            DepartmentId = staffDepartment.Id
        };
        await JobTitles.AddAsync(staffJobTitle);
        await SaveChangesAsync();

        var adminUser = new User { UserName = adminRequest.Email, Email = adminRequest.Email, EmailConfirmed = true };
        await userManager.CreateAsync(adminUser, adminRequest.Password);
        await userManager.AddToRoleAsync(adminUser, Role.Staff);

        var adminProfile = new UserProfile()
        {
            Id = adminUser.Id,
            FullName = "Staff User",
            DocumentNumber = "123.456.789-09",
            BirthDate = new DateOnly(2000, 01, 01),
            CompanyId = staffCompany.Id,
            JobTitleId = staffJobTitle.Id,
            Role = Role.Staff
        };

        await userManager.AddClaimsAsync(adminUser, new List<Claim>
            {
                new("FullName", adminProfile?.FullName ?? ""),
                new("CompanyId", staffCompany.Id.ToString() ?? "Null"),
                new("CompanyTradeName", staffCompany.TradeName.ToString() ?? "Null"),
                new("DepartmentId", staffDepartment.Id.ToString() ?? "Null"),
                new("Department", staffDepartment.Name.ToString() ?? "Null"),
                new("JobTitleId", staffJobTitle.Id.ToString() ?? "Null"),
                new("JobTitle", staffJobTitle.Name.ToString() ?? "Null")
            });

        await UserProfiles.AddAsync(adminProfile!);
        await SaveChangesAsync();
    }
    #endregion
}