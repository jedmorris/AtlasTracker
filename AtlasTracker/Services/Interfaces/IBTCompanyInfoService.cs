using AtlasTracker.Models;

namespace AtlasTracker.Services.Interfaces;

public interface IBTCompanyInfoService
{
    public Task<Company> GetCompanyInfoByIdAsync(int? companyId);
    
         
    
    
    
}
