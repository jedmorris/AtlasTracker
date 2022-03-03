using AtlasTracker.Data;
using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtlasTracker.Services;

public class BTCompanyInfoService : IBTCompanyInfoService
{
    private readonly ApplicationDbContext _context;

    public BTCompanyInfoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
    {
        Company? company = new();

        try
        {
            if (companyId != null)
            {
                company = await _context.Companies
                    .Include(c => c.Members)
                    .Include(c => c.Projects)
                    .Include(c => c.Invites)
                    .FirstOrDefaultAsync(c => c.Id == companyId);
            }
            return company!;
        }
        catch (Exception e)
        {
            throw;
        }



    }
}