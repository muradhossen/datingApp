using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _dbContext;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext dataContext,ITokenService tokenService)
        {
            _dbContext = dataContext;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.userName)) return BadRequest("User name is taken.");
            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                UserName = registerDto.userName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.password)),
                PasswordSalt = hmac.Key
            };
            _dbContext.users.Add(user);
            await _dbContext.SaveChangesAsync();
            return new UserDto
            {
                userName=user.UserName,
                token=_tokenService.CreateToken(user)
            };
        }
        private async Task<bool> UserExists(string userName)
        {
            return await _dbContext.users.AnyAsync(x => x.UserName == userName.ToLower());
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _dbContext.users.SingleOrDefaultAsync(x => x.UserName == loginDto.userName);
            if (user == null) return Unauthorized("Unauthorized user");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }
            return new UserDto
            {
                userName = user.UserName,
                token = _tokenService.CreateToken(user)
            };
        }
    }
}
