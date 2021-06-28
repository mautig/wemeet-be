using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Models;
using API.Types;

namespace API.Interfaces
{
  public interface IUserRepository
  {
    Task<AppUser> FindById(int id);
    Task<UserWithTeamUsersDTO> GetUserAsync(int id);
    Task<UserDTO> FindByEmail(string email);
    Task<AppUser> UpdateUserAsync(AppUser user, int id);
    Task<Pagination<UserWithTeamDTO>> GetUsersAsync(Query<UserFilterModel> query);
    void DeactivateUser(AppUser user);
    void RetrieveUser(AppUser user);
  }
}