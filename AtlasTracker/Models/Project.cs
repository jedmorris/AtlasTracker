using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtlasTracker.Models;

public class Project
{
    public int Id { get; set; }

    public int CompanyId { get; set; }
    
    [Required] [StringLength(240, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)] [DisplayName("Project Name")] public string? Name { get; set; }

    [DisplayName("Project Description")] public string? Description { get; set; }

    [Required] [DisplayName("Created Date")] [DataType(DataType.Date)] public DateTimeOffset CreatedDate { get; set; }

    [DisplayName("Project Start Date")] [DataType(DataType.Date)] public DateTimeOffset StartDate { get; set; }
    
    [DisplayName("Project End Date")] [DataType(DataType.Date)] public DateTimeOffset EndDate { get; set; }

    public int ProjectPriorityId { get; set; }

    [NotMapped] [DataType(DataType.Upload)] public IFormFile? ImageFormFile { get; set; }

    [DisplayName("File Name")] public string? ImageFileName { get; set; }

    public byte[]? ImageFileData { get; set; }

    [DisplayName("File Extension")] public string? ImageContentType { get; set; }

    public bool Archived { get; set; }

    // Navigation Properties
    public virtual Company? Company { get; set; }
    public virtual ProjectPriority? ProjectPriority { get; set; }
    public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
    public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
}