using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models;

public class Notification
{
    // Primary Key
    public int Id { get; set; }

    [DisplayName("Ticket")] public int TicketId { get; set; }

    [DisplayName("Title")] public string? Title { get; set; }

    [DisplayName("Message")] public string? Message { get; set; }

    [DataType(DataType.Date)] [DisplayName("Date")] public DateTimeOffset Created { get; set; }

    [Required] public string? RecipientId { get; set; }

    [Required] public string? SenderId { get; set; }

    [DisplayName("Has Been Viewed")] public bool Viewed { get; set; }

    [Required] public int NotificationTypeId { get; set; }
    
    
    // Navigation Properties
    public virtual NotificationType? NotificationType { get; set; }
    public virtual Ticket? Ticket { get; set; }
    public virtual BTUser? Recipient { get; set; }
    public virtual BTUser? Sender { get; set; }
    
    
    
}