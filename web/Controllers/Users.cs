﻿﻿using System;
using System.IO;
 using System.Linq;
 using System.Threading.Tasks;
using core.Interfaces;
using core.Models;
using core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace web.Controllers
{
    [Route("/api/[controller]")]
    public class Users : Controller
    {
        private UserService UserService { get; }
        private UserImageService UserImageService { get; }
        private IHostingEnvironment Environment { get; }
        
        public Users(IUserServiceFactory userServiceFactory, 
                     IUserImageServiceFactory userImageServiceFactory,
                     IHostingEnvironment environment)
        {
            UserService = userServiceFactory.GetUserService();
            UserImageService = userImageServiceFactory.GetUserImageService();
            Environment = environment;
        }
        
        // GET: /api/users
        // get all users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await UserService.GetAll();
                return Ok(users.OrderByDescending(x => x.CreateDateTime).Select(x => new
                {
                    firstName = x.RowKey,
                    lastName = x.PartitionKey,
                    email = x.Email,
                    age = x.Age,
                    image = x.Image
                }));
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
        [HttpPost("/api/users")]
        public async Task<IActionResult> AddUser(string firstName, string lastName, int age, string email, IFormFile image)
        {
            try
            {
                // check if user already exists
                var prevuser = await UserService.GetSingle(lastName, firstName);
                if (prevuser != null)
                {
                    // return error
                    return Ok(new
                    {
                        code = 1,
                        error = "User already exits."
                    });
                }
                
                // check if valid image
                var imageType = Path.GetExtension(image.FileName).ToLower();

				// allow only jpg images for now
                if (image.Length <= 0 ||  (imageType != ".jpg" && imageType != ".jpeg"))
				{
					return Ok(new
					{
						code = 1,
						error = "Invalid Image. Please select a valid .jpg file."
					});
                }

                // read image content
                byte[] imageBytes;
                using (var fs = image.OpenReadStream())
                {
                    using(var ms = new MemoryStream())
                    {
                        await fs.CopyToAsync(ms);
                        imageBytes = ms.ToArray();
                    }
                }

				// create unique name for image to prevent overwrites
				var imageId = $"{Guid.NewGuid()}";

                // save image
                var imageUrl = await UserImageService.AddImage(imageId, imageBytes);

                // create user entity
                var user = new User
                {
                    PartitionKey = lastName,
                    RowKey = firstName,
                    Age = age,
                    Email = email,
                    Image = imageUrl,
                    CreateDateTime = DateTime.Now
                };
                
                // save user to db
                await UserService.Add(user);
                
                // return success
                return Ok(new
                {
                    code = 0
                });

            }
            catch (Exception)
            {
                // retunr error
                return Ok(new
                {
                    code = 1,
                    error = "Error while saving user."
                });
            }
            
        }
        
    }
}