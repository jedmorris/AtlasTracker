using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models;

public class TicketComment
{
    // Primary Key
    public int Id { get; set; }

    [Required] [DisplayName("Member Comment")] public string? Comment { get; set; }

    [DataType(DataType.Date)] [DisplayName("Date")] public DateTimeOffset Created { get; set; }

    public int TicketId { get; set; }

    [Required] public string? UserId { get; set; }


    // Navigation Properties
    [DisplayName("Ticket")] public virtual Ticket? Ticket { get; set; }
    [DisplayName("Team Member")] public virtual BTUser? User { get; set; }
}