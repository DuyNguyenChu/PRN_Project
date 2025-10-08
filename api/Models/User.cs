using System;
using System.Collections.Generic;

namespace api.Models;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int? Gender { get; set; }

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTimeOffset? BirthDay { get; set; }

    public string? PhoneNumber { get; set; }

    public int UserStatusId { get; set; }

    public int? DriverId { get; set; }

    public int? AvatarId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Driver? Driver { get; set; }

    public virtual ICollection<TripRequest> TripRequests { get; set; } = new List<TripRequest>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual UserStatus UserStatus { get; set; } = null!;

    public virtual ICollection<VehicleInspection> VehicleInspections { get; set; } = new List<VehicleInspection>();
}
