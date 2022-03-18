using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtlasTracker.Models.ViewModels;

public class AssignDeveloperViewModel
{
    public Project? Project { get; set; }
    public SelectList? PMList { get; set; }
    public string? PMID { get; set; }
}