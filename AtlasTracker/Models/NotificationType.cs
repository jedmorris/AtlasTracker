using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models;

public class NotificationType
{
    // Primary Key
    public int Id { get; set; }

    [Required] [DisplayName("NotificationType")] public string? Name { get; set; }
    
    
    
    
    
    
}