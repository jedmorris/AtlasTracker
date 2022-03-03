using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AtlasTracker.Models;

public class BTUser : IdentityUser
{
    [Required]
    [StringLength(25, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
    [DisplayName("First Name")]
    public string? FirstName { get; set; }

    [Required]
    [StringLength(25, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
    [DisplayName("Last Name")]
    public string? LastName { get; set; }

    [NotMapped]
    [DisplayName("Full Name")]
    public string? FullName
    {
        get { return $"{FirstName} {LastName}"; }
    }

    // Image handling
    [NotMapped]
    [DataType(DataType.Upload)]
    public IFormFile? AvatarFormFil { get; set; }

    [DisplayName("Avatar")] 
    public string? AvatarName { get; set; }

    public byte[]? AvatarData { get; set; }

    [Display(Name = "File Extension")] 
    public string? AvatarContentType { get; set; }

    public int CompanyId { get; set; }


    // Navigation Properties
    public virtual Company? Company { get; set; }
    public virtual ICollection<Project>? Projects { get; set; }
}