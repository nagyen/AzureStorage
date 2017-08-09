using System;
using System.Threading.Tasks;
using core.Interfaces;
using core.Models;
using core.Services;
using Microsoft.AspNetCore.Mvc;

namespace web.Controllers
{
    [Route("/api/[controller]")]
    public class Users : Controller
    {
        private UserService UserService { get; }
        
        public Users(IUserServiceFactory userServiceFactory)
        {
            UserService = userServiceFactory.GetUserService();
        }
        
        // GET: /api/users
        // get all users
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await UserService.GetAll());
            }
            catch (Exception)
            {
                return Ok(new
                {
                    code = 1,
                    errors = "Error while getting users list."
                });
            }
        }
        
        // POST: /api/users
        // add new user
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            try
            {
                await UserService.Add(user);

                return Ok(new
                {
                    code = 0
                });
            }
            catch (Exception)
            {
                return Ok(new
                {
                    code = 1,
                    errors = "Error while adding new user."
                });
            }
        }
    }
}