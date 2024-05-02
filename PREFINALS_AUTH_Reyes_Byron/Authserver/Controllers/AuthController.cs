using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using PREFINALS_AUTH_Reyes_Byron.Authserver.Models;
using PREFINALS_AUTH_Reyes_Byron.Authserver.Services;
using PREFINALS_AUTH_Reyes_Byron.Models;
using Newtonsoft.Json;

namespace PREFINALS_AUTH_Reyes_Byron.Authserver.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private static User user = new User();
        private static readonly Random _random = new Random();

        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMyName()
        {
            return Ok(_userService.GetMyName());
        }

        [HttpPost("register")]
        public ActionResult<User> Register(UserDto request)
        {
            if (request.Password != request.ConfirmPassword)
            {
                return BadRequest("Passwords do not match.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.Course = request.Course;
            user.Section = request.Section;

            return Ok(user);
        }


        [HttpPost("login")]
        public ActionResult<string> Login(UserDto request)
        {
            if (user.Username != request.Username)
            {
                return BadRequest("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Wrong password.");
            }

            string token = CreateToken(user);

            return Ok(token);
        }

        [HttpGet("user-info-string"), Authorize]
        public ActionResult<string> GetUserInfoString(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name cannot be empty.");
            }

            var username = user.Username;

            if (string.IsNullOrEmpty(username))
            {
                return NotFound("No user has been registered yet.");
            }

            var section = User.FindFirst(ClaimTypes.UserData)?.Value;
            var course = User.FindFirst(ClaimTypes.GivenName)?.Value;
            var funFacts = GenerateFunFacts();

            var userInfo = new
            {
                RegisteredUsername = username,
                Section = section,
                Course = course,
                FunFacts = funFacts
            };

            // Serialize the UserInfo object to JSON string
            string userInfoString = JsonConvert.SerializeObject(userInfo);

            return Ok(userInfoString);
        }



        // GET api/auth/about/me
        [HttpGet("about/me")]
        public ActionResult<string> GetAboutMe()
        {
            List<string> aboutMe = new List<string>
            {
                "The API creator loves hiking in their free time.",
                "They have a pet cat named Whiskers.",
                "They enjoy experimenting with new programming languages.",
                "The API creator is an avid reader and loves science fiction novels.",
                "They are passionate about promoting diversity and inclusion in tech.",
                "They once traveled to Japan and fell in love with the culture.",
                "The API creator is a fan of puzzle games like Sudoku and Crosswords.",
                "They enjoy cooking and experimenting with different cuisines.",
                "They are a big fan of open-source software and contribute to several projects.",
                "The API creator believes in lifelong learning and is currently studying machine learning."
            };

            int index = _random.Next(aboutMe.Count);
            return Ok(new { AboutMe = aboutMe[index] });
        }

        // GET api/auth/about
        [HttpGet("about")]
        public ActionResult<string> About(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name cannot be empty.");
            }

            string ownerName = "Byron Reyes";

            if (string.IsNullOrEmpty(user.Username))
            {
                return NotFound("No user has been registered yet.");
            }

            List<string> funFacts = new List<string>
    {
        "The API creator loves hiking in their free time.",
        "They have a pet cat named Whiskers.",
        "They enjoy experimenting with new programming languages.",
        "The API creator is an avid reader and loves science fiction novels.",
        "They are passionate about promoting diversity and inclusion in tech.",
        "They once traveled to Japan and fell in love with the culture.",
        "The API creator is a fan of puzzle games like Sudoku and Crosswords.",
        "They enjoy cooking and experimenting with different cuisines.",
        "They are a big fan of open-source software and contribute to several projects.",
        "The API creator believes in lifelong learning and is currently studying machine learning."
    };

            string randomFunFact = funFacts[_random.Next(funFacts.Count)];
            string message = $"Hi {name}, I am {ownerName}. Did you know {randomFunFact}";

            return Ok(new { RegisteredUsername = user.Username, Message = message });
        }






        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.UserData, user.Section),
                new Claim(ClaimTypes.GivenName, user.Course),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Role, "User")
            };

            // Funfacts
            var funFacts = GenerateFunFacts();
            var randomFunFact = funFacts[new Random().Next(funFacts.Count)];

            claims.Add(new Claim("FunFact", randomFunFact));

            var keyBytes = new byte[64];
            new RNGCryptoServiceProvider().GetBytes(keyBytes);
            var key = new SymmetricSecurityKey(keyBytes);

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private List<string> GenerateFunFacts()
        {
            return new List<string>
            {
                "The API creator loves hiking in their free time.",
                "They have a pet cat named Whiskers.",
                "They enjoy experimenting with new programming languages.",
                "The API creator is an avid reader and loves science fiction novels.",
                "They are passionate about promoting diversity and inclusion in tech.",
                "They once traveled to Japan and fell in love with the culture.",
                "The API creator is a fan of puzzle games like Sudoku and Crosswords.",
                "They enjoy cooking and experimenting with different cuisines.",
                "They are a big fan of open-source software and contribute to several projects.",
                "The API creator believes in lifelong learning and is currently studying machine learning."
            };
        }
    }
}
