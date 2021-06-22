using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using API.Models;
using API.Services;
using API.Types;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Repositories
{
  public class UserRepository : IUserRepository
  {
    public readonly AppDbContext _context;
    private readonly IMapper _mapper;
    public UserRepository(
      AppDbContext context,
      IMapper mapper
    )
    {
      _mapper = mapper;
      _context = context;
    }

    public void DeactivateUser(AppUser user)
    {
      user.DeletedAt = DateTime.Now;
      user.isDeactivated = true;
    }

    public async Task<UserDTO> GetUserAsync(string username)
    {
      return await _context.Users.Where(user => user.UserName == username)
                                 .ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
                                 .SingleOrDefaultAsync();
    }

    public async Task<Pagination<UserDTO>> GetUsersAsync(PaginationParams paginationParams,
                                                         Dictionary<string, string> filter,
                                                         string sort)
    {
      var serializer = JsonConvert.SerializeObject(filter);
      var _filter = JsonConvert.DeserializeObject<UserFilterModel>(serializer);

      var stat = _context.Users.Where(u => u.Fullname.Contains(_filter.fullname) || u.Email.Contains(_filter.fullname))
                                .ProjectTo<UserDTO>(_mapper.ConfigurationProvider);

      switch (sort)
      {
        case "created_at":
          stat = stat.OrderBy(s => s.CreatedAt);
          break;
        case "-created_at":
          stat = stat.OrderByDescending(s => s.CreatedAt);
          break;
      }
      var query = stat.AsQueryable();
      return await PaginationService.GetPagination<UserDTO>(query, paginationParams.pageNumber, paginationParams.pageSize);

    }

    public void RetrieveUser(AppUser user)
    {
      user.isDeactivated = false;
      user.DeletedAt = null;
    }

    public async Task<AppUser> UpdateUserAsync(AppUser user)
    {
      var _user = await _context.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).FirstOrDefaultAsync(x => x.Email == user.Email);
      if (_user != null)
      {
        _user.Nickname = user.Nickname;
        _user.Fullname = user.Fullname;
        _user.Position = user.Position;
      }

      return _user;
    }

  }
}