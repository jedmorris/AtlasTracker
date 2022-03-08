using AtlasTracker.Extensions;
using AtlasTracker.Models;
using AtlasTracker.Models.ViewModels;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        // 1. Add an instance of the ViewModel as a list (model)
        List<ManageUserRolesViewModel> model = new ();
        
        
        // 2. Get CompanyId
        int companyId = User.Identity!.GetCompanyId();

        // Get all company users
        List<BTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);
        // 3. Loop over the users to populate the ViewModel
        //      - instantiate ViewModel
        //      - use _rolesService
        //      - create multi-selects
        foreach (BTUser user in users)
        {
            ManageUserRolesViewModel viewModel = new();
            viewModel.BtUser = user;
            IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
            viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name","Name", selected);

            model.Add(viewModel);
        }
           // 4. Return model to the View 
        return View(model);
    }

    
    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
    {
        return View();
    }













}