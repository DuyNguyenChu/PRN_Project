using System;
using System.Collections.Generic;

namespace api.Models;

public partial class FileUpload : EntityBase<int>
{

    public int FolderUploadId { get; set; }

    public string FileName { get; set; } = null!;

    public long FileSize { get; set; }

    public string FileType { get; set; } = null!;

    public string FileKey { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public DateTimeOffset? LastModifiedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual FolderUpload FolderUpload { get; set; } = null!;
}
