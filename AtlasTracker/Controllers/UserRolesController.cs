using AtlasTracker.Models;
using AtlasTracker.Models.ViewModels;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AtlasTracker.Controllers;

[Authorize(Roles = "Admin")]
public class UserRolesController : Controller
{
    private readonly IBTRolesService _rolesService;
    private readonly IBTCompanyInfoService _companyInfoService;

    public UserRolesController(IBTRolesService rolesService, IBTCompanyInfoService companyInfoService)
    {
        _rolesService = rolesService;
        _companyInfoService = companyInfoService;
    }

    public IActionResult Index()
    {
        return View();
    }

    // GET
    [HttpGet]
    public async Task<IActionResult> ManageUserRoles()
    {
        
        return View();
    }

    
    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
    {
        return View();
    }













}