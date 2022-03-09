using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;

namespace AtlasTracker.Services;

public class BTTicketService : IBTTicketService
{
    public async Task AddNewTicketAsync(Ticket ticket)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateTicketAsync(Ticket ticket)
    {
        throw new NotImplementedException();
    }

    public async Task<Ticket> GetTicketByIdAsync(int ticketId)
    {
        throw new NotImplementedException();
    }

    public async Task ArchiveTicketAsync(Ticket ticket)
    {
        throw new NotImplementedException();
    }

    public async Task AssignTicketAsync(int ticketId, string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Ticket>> GetArchivedTicketsAsync(int companyId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
    {
        throw new NotImplementedException();
    }

    public async Task<BTUser> GetTicketDeveloperAsync(int ticketId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
    {
        throw new NotImplementedException();
    }

    public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
    {
        throw new NotImplementedException();
    }

    public async Task<int?> LookupTicketStatusIdAsync(string statusName)
    {
        throw new NotImplementedException();
    }

    public async Task<int?> LookupTicketTypeIdAsync(string typeName)
    {
        throw new NotImplementedException();
    }
}