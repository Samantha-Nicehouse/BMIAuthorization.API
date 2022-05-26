using bmiWebAPI_3.Models;

namespace bmiWebAPI_3.Services; 

public interface ICosmosDbService
{
    Task<IEnumerable<ApplicationUser>> GetUsersAsync(string query);
    Task<ApplicationUser> GetUserAsync(string id);
    Task AddUserAsync(ApplicationUser user);
    Task UpdateUserAsync(string id, ApplicationUser user);
    Task DeleteUserAsync(string id);
}