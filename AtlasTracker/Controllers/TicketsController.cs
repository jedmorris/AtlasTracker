// #nullable disable
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

namespace AtlasTracker.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTProjectService _projectService;
        private readonly IBTLookupService _lookupService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTFileService _fileService;
        private readonly IBTTicketService _ticketService;
        private readonly IBTTicketHistoryService _ticketHistoryService;

        public TicketsController(ApplicationDbContext context,
            UserManager<BTUser> userManager,
            IBTLookupService lookupService,
            IBTProjectService projectService,
            IBTFileService fileService,
            IBTCompanyInfoService companyInfoService,
            IBTTicketService ticketService,
            IBTTicketHistoryService ticketHistoryService)
        {
            _context = context;
            _userManager = userManager;
            _lookupService = lookupService;
            _projectService = projectService;
            _fileService = fileService;
            _companyInfoService = companyInfoService;
            _ticketService = ticketService;
            _ticketHistoryService = ticketHistoryService;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: My Tickets
        public async Task<IActionResult> MyTickets()
        {
            // string userId = _userManager.GetUserId(User);
            // int companyId = User.Identity.GetCompanyId();
            BTUser btUser = await _userManager.GetUserAsync(User);

            List<Ticket> tickets = await _ticketService.GetTicketsByUserIdAsync(btUser.Id, btUser.CompanyId);

            return View(tickets);
        }

        // GET: All Tickets
        public async Task<IActionResult> AllTickets()
        {
            int companyId = User.Identity.GetCompanyId();

            List<Ticket> tickets = await _ticketService.GetAllTicketsByCompanyAsync(companyId);

            if (User.IsInRole(nameof(BTRole.Developer)) || User.IsInRole(nameof(BTRole.Submitter)))
            {
                return View(tickets.Where(t => t.Archived == false));
            }
            else
            {
                return View(tickets);
            }
        }

        // GET: Archived Ticket
        public async Task<IActionResult> ArchivedTickets()
        {
            int companyId = User.Identity.GetCompanyId();

            List<Ticket> tickets = await _ticketService.GetArchivedTicketsAsync(companyId);
            
            return View(tickets);
        }

        // GET: Unassigned Tickets
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> UnassignedTickets()
        {
            int companyId = User.Identity!.GetCompanyId();
            string btUserId = _userManager.GetUserId(User);

            List<Ticket> tickets = await _ticketService.GetUnassignedTicketsAsync(companyId);

            if (User.IsInRole(nameof(BTRole.Admin)))
            {
                return View(tickets);
            }
            else
            {
                List<Ticket> pmTickets = new();

                foreach (Ticket ticket in tickets)
                {
                    if (await _projectService.IsAssignedProjectManagerAsync(btUserId, ticket.ProjectId))
                    {
                        pmTickets.Add(ticket);
                    }
                }

                return View(pmTickets);
            }
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpGet]
        public async Task<IActionResult> AssignDeveloper(int id)
        {
            return View();
        }


        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDeveloper(AssignDeveloperViewModel model)
        {
            if (model.DeveloperId != null)
            {
                BTUser btUser = await _userManager.GetUserAsync(User);

                try
                {
                    await _ticketService.AssignTicketAsync(model.Ticket.Id, model.DeveloperId);
                }
                catch (Exception e)
                {
                    throw;
                }

                //New Ticket
                Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(model.Ticket.Id);
                await _ticketHistoryService.AddHistoryAsync(oldTicket, newTicket, btUser.Id);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketComment([Bind("Id, TicketId, Comment")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ticketComment.UserId = _userManager.GetUserId(User);
                    ticketComment.Created = DateTime.UtcNow;

                    await _ticketService.AddTicketCommentAsync(ticketComment);

                    //Add history
                    await _ticketHistoryService.AddHistoryAsync(ticketComment.TicketId, nameof(TicketComment),
                        ticketComment.UserId);
                }
                catch (Exception e)
                {
                    throw;
                }
            }

            return RedirectToAction("Details", new {id = ticketComment.TicketId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTicketAttachment(
            [Bind("Id, FormFile, Description, TicketId")]
            TicketAttachment ticketAttachment)
        {
            string statusMessage;
            ModelState.Remove("UserId");

            if (ModelState.IsValid && ticketAttachment.ImageFormFile != null)
            {
                try
                {
                    ticketAttachment.ImageFileData =
                        await _fileService.ConvertFileToByteArrayAsync(ticketAttachment.ImageFormFile);
                    ticketAttachment.ImageFileName = ticketAttachment.ImageFormFile.FileName;
                    ticketAttachment.ImageContentType = ticketAttachment.ImageFormFile.ContentType;

                    ticketAttachment.Created = DateTime.UtcNow;
                    ticketAttachment.UserId = _userManager.GetUserId(User);

                    await _ticketService.AddTicketAttachmentAsync(ticketAttachment);

                    //Add history
                    await _ticketHistoryService.AddHistoryAsync(ticketAttachment.TicketId, nameof(ticketAttachment),
                        ticketAttachment.UserId);
                }
                catch (Exception e)
                {
                    throw;
                }

                statusMessage = "Success: New attachment added to Ticket."
            }
        }

        public async Task<IActionResult> ShowFile(int id)
        {
            return View();
        }


        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public async Task<IActionResult> Create()
        {
            BTUser btUser = await _userManager.GetUserAsync(User);


            if (User.IsInRole(nameof(BTRole.Admin)))
            {
                ViewData["ProjectId"] =
                    new SelectList(await _projectService.GetAllProjectsByCompanyAsync(btUser.CompanyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] =
                    new SelectList(await _projectService.GetUserProjectsAsync(btUser.Id), "Id", "Name");
            }

            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");

            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId")] Ticket ticket)
        {
            BTUser btUser = await _userManager.GetUserAsync(User);
            ModelState.Remove("OwnerUserId");

            if (ModelState.IsValid)
            {
                try
                {
                    ticket.Created = DateTime.UtcNow;
                    ticket.OwnerUserId = btUser.Id;

                    ticket.TicketStatusId = (await _ticketService.LookupTicketStatusIdAsync(nameof(BTTicketStatus.New)))
                        .Value;

                    await _ticketService.AddNewTicketAsync(ticket);

                    // Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAysnc(ticket.id);
                    // await _ticketHistoryService.AddHistoryAsync(null!, newTicket, btUser.Id);
                }
                catch (Exception e)
                {
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            if (User.IsInRole(nameof(BTRole.Admin)))
            {
                ViewData["ProjectId"] =
                    new SelectList(await _projectService.GetAllProjectsByCompanyAsync(btUser.CompanyId), "Id", "Name");
            }
            else
            {
                ViewData["ProjectId"] =
                    new SelectList(await _projectService.GetUserProjectsAsync(btUser.Id), "Id", "Name");
            }

            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetTicketPrioritiesAsync(), "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetTicketTypesAsync(), "Id", "Name");
            
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);

            if (ticket == null)
            {
                return NotFound();
            }


            ViewData["TicketPriorityId"] = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(await _lookupService.GetProjectPrioritiesAsync(), "Id", "Name", ticket.TicketTypeId);
            
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind(
                "Id,Title,Description,Created,Updated,Archived,ArchivedByProject,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")]
            Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                BTUser btUser = await _userManager.GetUserAsync(User);
                // Ticket oldTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.Id);

                try
                {
                    // ticket.Created = _postgresDateService.FormatDate(ticket.Created.DateTime);
                    ticket.Updated = DateTime.UtcNow;
                    await _ticketService.UpdateTicketAsync(ticket);

                    //TODO: Send notification

                    //Notify PM & Admin

                    //Notify Developer
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // TODO: Add Ticket History
                // Ticket newTicket = await _ticketService.GetTicketAsNoTrackingAsync(ticket.id);
                // await _ticketHistoryService.AddHistoryAsync(oldTicket, newTicket, btUser.Id);

                return RedirectToAction(nameof(AllTickets));
            }

            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);

            return View(ticket);
        }

        // GET: Tickets/Archive/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Ticket ticket = await _ticketService.GetTicketByIdAsync(id.Value);
            
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Archive/5
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            Ticket ticket = await _ticketService.GetTicketByIdAsync(id);
            ticket.Archived = true;
            await _ticketService.UpdateTicketAsync(ticket);


            return RedirectToAction(nameof(AllTickets));
        }

        //GET:Tickets/Restore/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Restore(int? id)
        {
            return View();
        }

        //POST: Tickets/Restore/5
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            return View();
        }

        public async Task<bool> TicketExists(int id)
        {
            int companyId = User.Identity.GetCompanyId();

            return (await _ticketService.GetAllTicketsByCompanyAsync(companyId)).Any(t => t.Id == id);
        }
    }
}