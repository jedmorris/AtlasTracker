using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;

namespace AtlasTracker.Services;

public class BTCompanyInfoService : IBTCompanyInfoService
{
    public Task<Company> GetCompanyInfoByIdAsync(int? companyId)
    {
        throw new NotImplementedException();
    }
}