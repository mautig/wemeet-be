using System;
using System.IO;
using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Interfaces;
using API.Types;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  public class AuthController : BaseApiController
  {
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IWebHostEnvironment _hostEnvironment;

    public AuthController(
      UserManager<AppUser> userManager,
      SignInManager<AppUser> signInManager,
      RoleManager<AppRole> roleManager,
      ITokenService tokenService,
      IMapper mapper,
      IWebHostEnvironment hostEnvironment
    )
    {
      _tokenService = tokenService;
      _mapper = mapper;
      _userManager = userManager;
      _signInManager = signInManager;
      _roleManager = roleManager;
      _hostEnvironment = hostEnvironment;
    }

    private async Task<bool> CheckUserExist(string username)
    {
      return await _userManager.Users.AnyAsync(user => user.UserName == username);
    }

    [HttpPost("register")]
    public async Task<ActionResult<Response<AuthDTO>>> Register([FromForm] RegisterDTO registerDTO)
    {
      if (await CheckUserExist(registerDTO.Username))
      {
        return BadRequest("User already taken");
      }

      var user = _mapper.Map<AppUser>(registerDTO);

      user.Avatar = await SaveImage(registerDTO.AvatarFile);

      user.UserName = registerDTO.Username.ToLower();

      var createStatus = await _userManager.CreateAsync(user, registerDTO.Password);

      if (!createStatus.Succeeded)
      {
        return BadRequest(createStatus.Errors);
      }

      var addRoleStatus = await _userManager.AddToRoleAsync(user, "Staff");

      if (!addRoleStatus.Succeeded)
      {
        return BadRequest(addRoleStatus.Errors);
      }

      var authDTO = new AuthDTO
      {
        token = await _tokenService.CreateToken(user),
        User = _mapper.Map<UserDTO>(user),
        Role = await _userManager.GetRolesAsync(user)
      };

      return new Response<AuthDTO>
      {
        status = 200,
        success = true,
        Data = authDTO
      };

    }

    [HttpPost("login")]
    public async Task<ActionResult<Response<AuthDTO>>> Login(LoginDTO loginDTO)
    {
      var user = await _userManager.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.Username.ToLower());

      if (user == null)
      {
        return Unauthorized("Invalid User");
      }

      var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);

      if (!result.Succeeded)
      {
        return Unauthorized();
      }

      var authDTO = new AuthDTO
      {
        User = _mapper.Map<UserDTO>(user),
        token = await _tokenService.CreateToken(user),
        Role = await _userManager.GetRolesAsync(user)
      };

      return new Response<AuthDTO>
      {
        Data = authDTO,
        success = true,
        status = 200
      };
    }

    private async Task<string> SaveImage(IFormFile imageFile)
    {
      string imageName = DateTime.Now.ToString("yyyymmssfff") + Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

      var uploadPath = Path.Combine(_hostEnvironment.ContentRootPath, "Uploads", "Avatars", imageName);

      using (var fileStream = new FileStream(uploadPath, FileMode.Create))
      {
        await imageFile.CopyToAsync(fileStream);
      }

      return imageName;
    }
  }
}