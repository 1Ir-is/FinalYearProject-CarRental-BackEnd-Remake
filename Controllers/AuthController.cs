﻿using CarRental_BE.Models.Auth;
using CarRental_BE.Repositories.User;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CarRental_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginVM model)
        {
            var user = await _userRepository.Login(model);

            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            // Here you can generate and return a JWT token for authentication.

            return Ok("Login successful!");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            var success = await _userRepository.Register(model);

            if (!success)
            {
                return Conflict("User with this email already exists");
            }

            return Ok("Registration successful!");
        }


        // Since logout generally involves removing the token stored client-side,
        // it's not typically handled on the server side in token-based authentication.
        // However, if you're storing any session data server-side, you could clear that session data upon logout.
    }
}
