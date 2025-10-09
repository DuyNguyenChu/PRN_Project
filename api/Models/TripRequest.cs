using System;
using System.Collections.Generic;

namespace api.Models;

public partial class TripRequest
{
    public int Id { get; set; }

    public int RequesterId { get; set; }

    public string FromLocation { get; set; } = null!;

    public decimal? FromLatitude { get; set; }

    public decimal? FromLongtitude { get; set; }

    public string ToLocation { get; set; } = null!;

    public decimal? ToLatitude { get; set; }

    public decimal? ToLongtitude { get; set; }

    public string? Reason { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset? EndTime { get; set; }

    public int TripRequestStatusId { get; set; }

    public string Description { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User Requester { get; set; } = null!;

    public virtual TripRequestStatus TripRequestStatus { get; set; } = null!;

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
