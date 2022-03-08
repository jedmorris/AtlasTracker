#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AtlasTracker.Data;
using AtlasTracker.Extensions;
using AtlasTracker.Models;
using AtlasTracker.Models.Enums;
using AtlasTracker.Models.ViewModels;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Index = System.Index;

namespace AtlasTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _projectService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTRolesService _rolesService;
        private readonly IBTLookupService _lookupService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTFileService _fileService;


        public ProjectsController(ApplicationDbContext context, IBTProjectService projectService,
            UserManager<BTUser> userManager, IBTRolesService rolesService, IBTLookupService lookupService,
            IBTCompanyInfoService companyInfoService, IBTFileService fileService)
        {
            _context = context;
            _projectService = projectService;
            _userManager = userManager;
            _rolesService = rolesService;
            _lookupService = lookupService;
            _companyInfoService = companyInfoService;
            _fileService = fileService;
        }

        // MyProjects
        public async Task<IActionResult> MyProjects()
        {
            string userId = _userManager.GetUserId(User);
            List<Project> projects = await _projectService.GetUserProjectsAsync(userId);

            return View(projects);
        }

        // All Projects
        public async Task<IActionResult> AllProjects()
        {
            List<Project> projects;
            int companyId = User.Identity.GetCompanyId();
            if (User.IsInRole(nameof(BTRole.Admin)) || User.IsInRole(nameof(BTRole.ProjectManager)))
            {
                projects = await _companyInfoService.GetAllProjectsAsync(companyId);
            }
            else
            {
                projects = await _projectService.GetAllProjectsByCompany(companyId);
            }

            return View(projects);
        }

        // ArchivedProjects
        public async Task<IActionResult> ArchivedProjects()
        {
        }


        // UnassignedProjects
        public async Task<IActionResult> UnassignedProjects()
        {
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Projects.Include(p => p.Company).Include(p => p.ProjectPriority);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // string userId = _userManager.GetUserId(User);
            // BTUser btUser = _context.Users.Find(userId);
            int companyId = User.Identity.GetCompanyId();

            Project project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity.GetCompanyId();

            AddProjectWithPMViewModel model = new();

            model.PMList =
                new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRole.ProjectManager), companyId), "Id",
                    "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");

            return View(model);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
        {
            int companyId = User.Identity.GetCompanyId();
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Project?.ImageFormFile != null)
                    {
                        model.Project.ImageFileData =
                            await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFormFile);
                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.ImageContentType = model.Project.ImageFormFile.ContentType;
                    }

                    model.Project.CompanyId = companyId;
                    model.Project.CreatedDate = DateTime.UtcNow;

                    model.Project.StartDate = DateTime.SpecifyKind(model.Project.StartDate.DateTime, DateTimeKind.Utc);
                    model.Project.EndDate = DateTime.SpecifyKind(model.Project.EndDate.DateTime, DateTimeKind.Utc);

                    await _projectService.AddNewProjectAsync(model.Project);
                    
                    // Add PM if one was chosen
                    if (!string.IsNullOrEmpty(model.PMID))
                    {
                        await _projectService.AddProjectManagerAsync(model.PMID, model.Project.Id);
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    throw;
                }
            }

            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(BTRole.ProjectManager), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View(model.Project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] =
                new SelectList(_context.ProjectPriorities, "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind(
                "Id,CompanyId,Name,Description,CreatedDate,StartDate,EndDate,ProjectPriorityId,ImageFileName,ImageFileData,ImageContentType,Archived")]
            Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _projectService.UpdateProjectAsync(project);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] =
                new SelectList(_context.ProjectPriorities, "Id", "Name", project.ProjectPriorityId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Company)
                .Include(p => p.ProjectPriority)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}