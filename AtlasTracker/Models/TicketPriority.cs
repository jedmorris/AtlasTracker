using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AtlasTracker.Models;

public class TicketPriority
{
    public int Id { get; set; }

    [Required] [DisplayName("Type Name")] public string? Name { get; set; }
}