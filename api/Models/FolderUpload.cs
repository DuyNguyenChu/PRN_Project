using System;
using System.Collections.Generic;

namespace api.Models;

public partial class FolderUpload
{
    public int Id { get; set; }

    public string FolderName { get; set; } = null!;

    public string FolderPath { get; set; } = null!;

    public int? ParentId { get; set; }

    public string TreeIds { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<FileUpload> FileUploads { get; set; } = new List<FileUpload>();
}
