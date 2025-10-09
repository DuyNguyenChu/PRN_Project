using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace api.Models;

public partial class PrnprojectContext : DbContext
{
    public PrnprojectContext()
    {
    }

    public PrnprojectContext(DbContextOptions<PrnprojectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Action> Actions { get; set; }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<DriverStatus> DriverStatuses { get; set; }

    public virtual DbSet<DriverViolation> DriverViolations { get; set; }

    public virtual DbSet<ExpenseType> ExpenseTypes { get; set; }

    public virtual DbSet<FileUpload> FileUploads { get; set; }

    public virtual DbSet<FolderUpload> FolderUploads { get; set; }

    public virtual DbSet<FuelLog> FuelLogs { get; set; }

    public virtual DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

    public virtual DbSet<MaintenanceRecordDetail> MaintenanceRecordDetails { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    public virtual DbSet<TripExpense> TripExpenses { get; set; }

    public virtual DbSet<TripRequest> TripRequests { get; set; }

    public virtual DbSet<TripRequestStatus> TripRequestStatuses { get; set; }

    public virtual DbSet<TripStatus> TripStatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserStatus> UserStatuses { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<VehicleAccident> VehicleAccidents { get; set; }

    public virtual DbSet<VehicleAssignment> VehicleAssignments { get; set; }

    public virtual DbSet<VehicleBranch> VehicleBranches { get; set; }

    public virtual DbSet<VehicleInspection> VehicleInspections { get; set; }

    public virtual DbSet<VehicleInsurance> VehicleInsurances { get; set; }

    public virtual DbSet<VehicleModel> VehicleModels { get; set; }

    public virtual DbSet<VehicleRegistration> VehicleRegistrations { get; set; }

    public virtual DbSet<VehicleStatus> VehicleStatuses { get; set; }

    public virtual DbSet<VehicleType> VehicleTypes { get; set; }

    public virtual DbSet<ViolationType> ViolationTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        var configuration = builder.Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    }
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=DUNGHOA;Database=PRNProject;User Id=sa;Password=123;Encrypt=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Action>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Action__3214EC0715F3A2FD");

            entity.ToTable("Action");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Driver__3214EC071998B55B");

            entity.ToTable("Driver");

            entity.Property(e => e.BankBranch).HasMaxLength(500);
            entity.Property(e => e.BankNumber).HasMaxLength(50);
            entity.Property(e => e.BaseSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EmergencyContactName).HasMaxLength(500);
            entity.Property(e => e.EmergencyContactPhone).HasMaxLength(10);
            entity.Property(e => e.LicenseClass).HasMaxLength(50);
            entity.Property(e => e.LicenseNumber).HasMaxLength(50);
            entity.Property(e => e.SocialInsuranceNumber).HasMaxLength(50);

            entity.HasOne(d => d.DriverStatus).WithMany(p => p.Drivers)
                .HasForeignKey(d => d.DriverStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Driver_DriverStatus");
        });

        modelBuilder.Entity<DriverStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DriverSt__3214EC07390B912E");

            entity.ToTable("DriverStatus");

            entity.Property(e => e.Color).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<DriverViolation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DriverVi__3214EC0753CED5C3");

            entity.ToTable("DriverViolation");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.PenaltyAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ReportedBy).HasMaxLength(255);
            entity.Property(e => e.ViolationLocation).HasMaxLength(500);

            entity.HasOne(d => d.Driver).WithMany(p => p.DriverViolations)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DriverViolation_Driver");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.DriverViolations)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DriverViolation_Vehicle");

            entity.HasOne(d => d.ViolationType).WithMany(p => p.DriverViolations)
                .HasForeignKey(d => d.ViolationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DriverViolation_ViolationType");
        });

        modelBuilder.Entity<ExpenseType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExpenseT__3214EC076CC86863");

            entity.ToTable("ExpenseType");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<FileUpload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FileUplo__3214EC077376952D");

            entity.ToTable("FileUpload");

            entity.Property(e => e.FileKey).HasMaxLength(1000);
            entity.Property(e => e.FileName).HasMaxLength(500);
            entity.Property(e => e.FileType).HasMaxLength(50);

            entity.HasOne(d => d.FolderUpload).WithMany(p => p.FileUploads)
                .HasForeignKey(d => d.FolderUploadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FileUpload_FolderUpload");
        });

        modelBuilder.Entity<FolderUpload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FolderUp__3214EC0791B54597");

            entity.ToTable("FolderUpload");

            entity.Property(e => e.FolderName).HasMaxLength(500);
            entity.Property(e => e.FolderPath).HasMaxLength(500);
            entity.Property(e => e.TreeIds).HasMaxLength(255);
        });

        modelBuilder.Entity<FuelLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FuelLog__3214EC0734357AD1");

            entity.ToTable("FuelLog");

            entity.Property(e => e.FuelType).HasMaxLength(50);
            entity.Property(e => e.GasStation).HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RejectReason).HasMaxLength(500);
            entity.Property(e => e.TotalCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Driver).WithMany(p => p.FuelLogs)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelLog_Driver");

            entity.HasOne(d => d.Trip).WithMany(p => p.FuelLogs)
                .HasForeignKey(d => d.TripId)
                .HasConstraintName("FK_FuelLog_Trip");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.FuelLogs)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FuelLog_Vehicle");
        });

        modelBuilder.Entity<MaintenanceRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Maintena__3214EC0720B546DB");

            entity.ToTable("MaintenanceRecord");

            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.RejectReason).HasMaxLength(500);
            entity.Property(e => e.ServiceCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ServiceProvider).HasMaxLength(255);
            entity.Property(e => e.ServiceType).HasMaxLength(100);

            entity.HasOne(d => d.Driver).WithMany(p => p.MaintenanceRecords)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaintenanceRecord_Driver");

            entity.HasOne(d => d.Trip).WithMany(p => p.MaintenanceRecords)
                .HasForeignKey(d => d.TripId)
                .HasConstraintName("FK_MaintenanceRecord_Trip");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.MaintenanceRecords)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaintenanceRecord_Vehicle");
        });

        modelBuilder.Entity<MaintenanceRecordDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Maintena__3214EC074A4F0161");

            entity.ToTable("MaintenanceRecordDetail");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Record).WithMany(p => p.MaintenanceRecordDetails)
                .HasForeignKey(d => d.RecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MaintenanceRecordDetail_MaintenanceRecord");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Menu__3214EC0771B50846");

            entity.ToTable("Menu");

            entity.Property(e => e.Icon)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.MenuType).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.TreeIds).HasMaxLength(500);
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permissi__3214EC07DEFF236F");

            entity.ToTable("Permission");

            entity.HasOne(d => d.Action).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.ActionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Permission_Action");

            entity.HasOne(d => d.Menu).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Permission_Menu");

            entity.HasOne(d => d.Role).WithMany(p => p.Permissions)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Permission_Role");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC07441D33C8");

            entity.ToTable("Role");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Trip__3214EC07C246072D");

            entity.ToTable("Trip");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DriverSalary).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FromLatitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.FromLocation).HasMaxLength(500);
            entity.Property(e => e.FromLongtitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Revenue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ToLatitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.ToLocation).HasMaxLength(500);
            entity.Property(e => e.ToLongtitude).HasColumnType("decimal(18, 6)");

            entity.HasOne(d => d.Driver).WithMany(p => p.Trips)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Trip_Driver");

            entity.HasOne(d => d.TripRequest).WithMany(p => p.Trips)
                .HasForeignKey(d => d.TripRequestId)
                .HasConstraintName("FK_Trip_TripRequest");

            entity.HasOne(d => d.TripStatus).WithMany(p => p.Trips)
                .HasForeignKey(d => d.TripStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Trip_TripStatus");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Trips)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Trip_Vehicle");
        });

        modelBuilder.Entity<TripExpense>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripExpe__3214EC0779E438C0");

            entity.ToTable("TripExpense");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.RejectReason).HasMaxLength(500);

            entity.HasOne(d => d.Driver).WithMany(p => p.TripExpenses)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripExpense_Driver");

            entity.HasOne(d => d.ExpenseType).WithMany(p => p.TripExpenses)
                .HasForeignKey(d => d.ExpenseTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripExpense_ExpenseType");

            entity.HasOne(d => d.Trip).WithMany(p => p.TripExpenses)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripExpense_Trip");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.TripExpenses)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripExpense_Vehicle");
        });

        modelBuilder.Entity<TripRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripRequ__3214EC07DC68B909");

            entity.ToTable("TripRequest");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.FromLatitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.FromLocation).HasMaxLength(500);
            entity.Property(e => e.FromLongtitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.ToLatitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.ToLocation).HasMaxLength(500);
            entity.Property(e => e.ToLongtitude).HasColumnType("decimal(18, 6)");

            entity.HasOne(d => d.Requester).WithMany(p => p.TripRequests)
                .HasForeignKey(d => d.RequesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripRequest_User");

            entity.HasOne(d => d.TripRequestStatus).WithMany(p => p.TripRequests)
                .HasForeignKey(d => d.TripRequestStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripRequest_TripRequestStatus");
        });

        modelBuilder.Entity<TripRequestStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripRequ__3214EC074AD4B5D1");

            entity.ToTable("TripRequestStatus");

            entity.Property(e => e.Color).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<TripStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripStat__3214EC0786184D13");

            entity.ToTable("TripStatus");

            entity.Property(e => e.Color).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC0767CF4375");

            entity.ToTable("User");

            entity.Property(e => e.Email)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FirstName).HasMaxLength(255);
            entity.Property(e => e.LastName).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(1000);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Driver).WithMany(p => p.Users)
                .HasForeignKey(d => d.DriverId)
                .HasConstraintName("FK_User_Driver");

            entity.HasOne(d => d.UserStatus).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_UserStatus");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC07773266BD");

            entity.ToTable("UserRole");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_Role");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_User");
        });

        modelBuilder.Entity<UserStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserStat__3214EC07CBBD4CEC");

            entity.ToTable("UserStatus");

            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vehicle__3214EC07AA808553");

            entity.ToTable("Vehicle");

            entity.Property(e => e.Color).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EngineNumber).HasMaxLength(50);
            entity.Property(e => e.IdentificationNumber).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.RegistrationNumber).HasMaxLength(50);

            entity.HasOne(d => d.CurrentDriver).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.CurrentDriverId)
                .HasConstraintName("FK_Vehicle_Driver");

            entity.HasOne(d => d.VehicleBranch).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.VehicleBranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_VehicleBranch");

            entity.HasOne(d => d.VehicleModel).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.VehicleModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_VehicleModel");

            entity.HasOne(d => d.VehicleStatus).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.VehicleStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_VehicleStatus");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.VehicleTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicle_VehicleType");
        });

        modelBuilder.Entity<VehicleAccident>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleA__3214EC0774B1CF49");

            entity.ToTable("VehicleAccident");

            entity.Property(e => e.DamageCost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.RejectReason).HasMaxLength(500);

            entity.HasOne(d => d.Driver).WithMany(p => p.VehicleAccidents)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VehicleAccident_Driver");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.VehicleAccidents)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VehicleAccident_Vehicle");
        });

        modelBuilder.Entity<VehicleAssignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleA__3214EC076E277C2A");

            entity.ToTable("VehicleAssignment");

            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(d => d.Driver).WithMany(p => p.VehicleAssignments)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VehicleAssignment_Driver");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.VehicleAssignments)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VehicleAssignment_Vehicle");
        });

        modelBuilder.Entity<VehicleBranch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleB__3214EC0782DE7C26");

            entity.ToTable("VehicleBranch");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<VehicleInspection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleI__3214EC07C23592AC");

            entity.ToTable("VehicleInspection");

            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Result).HasMaxLength(500);

            entity.HasOne(d => d.Inspector).WithMany(p => p.VehicleInspections)
                .HasForeignKey(d => d.InspectorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VehicleInspection_User");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.VehicleInspections)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VehicleInspection_Vehicle");
        });

        modelBuilder.Entity<VehicleInsurance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleI__3214EC07A0DEF2AC");

            entity.ToTable("VehicleInsurance");

            entity.Property(e => e.InsuranceProvider).HasMaxLength(255);
            entity.Property(e => e.PolicyNumber).HasMaxLength(50);
            entity.Property(e => e.Premium).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.VehicleInsurances)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VehicleInsurance_Vehicle");
        });

        modelBuilder.Entity<VehicleModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleM__3214EC07551B122E");

            entity.ToTable("VehicleModel");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<VehicleRegistration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleR__3214EC07D83559DA");

            entity.ToTable("VehicleRegistration");

            entity.Property(e => e.RegistrationNumber).HasMaxLength(50);

            entity.HasOne(d => d.Vehicle).WithMany(p => p.VehicleRegistrations)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VehicleRegistration_Vehicle");
        });

        modelBuilder.Entity<VehicleStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleS__3214EC075E5EB876");

            entity.ToTable("VehicleStatus");

            entity.Property(e => e.Color).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VehicleT__3214EC078961417E");

            entity.ToTable("VehicleType");

            entity.Property(e => e.Color).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<ViolationType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Violatio__3214EC078B380144");

            entity.ToTable("ViolationType");

            entity.Property(e => e.Color).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
