﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaApi.Data;
using CinemaApi.Models;
using AuthenticationPlugin;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private CinemaDbContext _dbContext;
        private IConfiguration _configuration;
        private readonly AuthService _auth;

        public UserController(CinemaDbContext dbContext, IConfiguration configuration)
        {
            _configuration = configuration;
            _auth = new AuthService(_configuration);
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult Register([FromBody] User user)
        {
            var userWithSameEmail = _dbContext.Users.Where(u => u.Email == user.Email).SingleOrDefault();
            if (userWithSameEmail!=null)
            {
                return BadRequest("User whith same Email already exists");
            }

            var userObj = new User()
            {
                Name = user.Name,
                Email = user.Email,
                Password = SecurePasswordHasherHelper.Hash(user.Password) ,
                Role = "Users"
            };
            _dbContext.Users.Add(userObj);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost]
        public IActionResult Login([FromBody] User user)
        {
            var userEmail = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email);
            if (userEmail == null)
            {
                return NotFound();
            }
            if (!SecurePasswordHasherHelper.Verify(user.Password, userEmail.Password))
            {
                return Unauthorized();
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, userEmail.Role)
            };
            var token = _auth.GenerateAccessToken(claims);
            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                expires_in = token.ExpiresIn,
                token_type = token.TokenType,
                creation_Time = token.ValidFrom,
                expiration_Time = token.ValidTo,
                user_id=userEmail.Id
            });


        }
    }
}
