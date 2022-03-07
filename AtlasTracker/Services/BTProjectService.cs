using AtlasTracker.Data;
using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtlasTracker.Services;

public class BTProjectService : IBTProjectService
{
    private readonly ApplicationDbContext _context;

    public BTProjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task AddNewProjectAsync(Project project)
    {
        try
        {
            _context.Add(project);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task<bool> AddProjectManagerAsync(string userId, int projectId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddUserToProjectAsync(string userId, int projectId)
    {
        throw new NotImplementedException();
    }

    public Task ArchiveProjectAsync(Project project)
    {
        throw new NotImplementedException();
    }

    public Task<List<Project>> GetAllProjectsByCompany(int companyId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
    {
        throw new NotImplementedException();
    }

    public Task<List<BTUser>> GetAllProjectMembersExceptPMAsync(int projectId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Project>> GetArchivedProjectsByCompany(int companyId)
    {
        throw new NotImplementedException();
    }

    public Task<List<BTUser>> GetDevelopersOnProjectAsync(int projectId)
    {
        throw new NotImplementedException();
    }

    public Task<BTUser> GetProjectManagerAsync(int projectId)
    {
        throw new NotImplementedException();
    }

    public Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role)
    {
        throw new NotImplementedException();
    }

    public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
    {
        try
        {
            var project = await _context.Projects
                .Include(p => p.Company)
                .Include(p => p.ProjectPriority)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

            return project!;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Task<List<BTUser>> GetSubmittersOnProjectAsync(int projectId)
    {
        throw new NotImplementedException();
    }

    public Task<List<BTUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Project>> GetUserProjectsAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsAssignedProjectManager(string userId, int projectId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsUserOnProject(string userId, int projectId)
    {
        throw new NotImplementedException();
    }

    public Task<int> LookupProjectPriorityId(string priorityName)
    {
        throw new NotImplementedException();
    }

    public Task RemoveProjectManagerAsync(int projectId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveUserFromProjectAsync(string userId, int projectId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateProjectAsync(Project project)
    {
        _context.Add(project);
        await _context.SaveChangesAsync();
    }

    public Task RestoreProjectAsync(Project project)
    {
        throw new NotImplementedException();
    }
}