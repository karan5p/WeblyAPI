using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.DTOs;
using API.Models.Entities;
using API.Models.Helpers;
using API.Models.Persistence;
using API.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }
        //POST /api/users
        //Add a new user to the users table. The name is required. The email should be valid and unique, otherwise the client should receive a 400 error.
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!user.Email.Contains("@") || !user.Email.Contains("."))
            {
                return BadRequest("Error 400: Email is not valid");
            }

            if (user.Name == null)
            {
                return BadRequest("Error 400: is required");
            }
            var userExists = _context.Users.Any(u => u.Email == user.Email);
            if (userExists)
            {
                return BadRequest("Error 400: email already exists");
            }
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
        //POST /api/users/{id}/images

        [HttpPost("{id}/image")]
        // Add one image to the given user images list. When adding the image, use the method ImageHelper.GetTags to generate the tags of the image and save these tags to the database.
        // The client should receive back a user json object that includes the last 10 images added by the user. If the user has less than 10 images then return them all
        public async Task<IActionResult> AddImage(string id, [FromBody] Image image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = _context.Users.Include(x => x.Images).FirstOrDefault(u => u.Id == new Guid(id));
            if (user == null)
            {
                return NotFound("Error 404: User not found");
            }

            var tags = ImageHelper.GetTags(image.Url);
            List<String> tagList = new List<String>();
            foreach (var tag in tags)
            {
                var tagged = new Tag { Text = tag };
                tagList.Add(tagged.Text);

            }
            //The client should receive back a user json object that includes the last 10 images added by the user. If the user has less than 10 images then return them all
            if (user.Images.Count < 10)
            {
                var imageToAdd = new Image
                {
                    Url = image.Url,
                    //    Tags = tagList
                };
                user.Images.Add(imageToAdd);
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            else
            {
                var imageToAdd = new Image
                {
                    Url = image.Url
                    //     Tags = tagList
                    // request.AddParameter("image_url", imageUrl);

                    //Tags = tagList.ImageHelper.GetTags(image.Url)
                    //save these tags to the database.

                };
                foreach (var i in ImageHelper.GetTags(image.Url))
                {
                    var tagged = new Tag { Text = i };
                    tagList.Add(tagged.Text);
                }
                user.Images.Add(imageToAdd);
                await _context.SaveChangesAsync();
                var lastTen = user.Images.OrderByDescending(i => i.Id).Take(10);
                var userDTO = new UserDTO
                {
                    Id = user.Id,
                    Username = user.Name,
                    Email = user.Email,
                    ImagesUrl = lastTen.Select(i => i.Url).ToList()
                };
                return Ok(userDTO);
            };
        }
        //GET /api/users/{id}
        [HttpGet("{id}")]
        //Return a user (given by id) object that includes the last 10 images added by the user. If the user has less than 10 images then return them all
        public async Task<IActionResult> GetUser(string id)
        {
            var user = _context.Users.Include(x => x.Images).FirstOrDefault(u => u.Id == new Guid(id));
            if (user == null)
            {
                return NotFound("Error 404: User not found");
            }
            if (user.Images.Count < 10)
            {
                var userDTO = new UserDTO
                {
                    Id = user.Id,
                    Username = user.Name,
                    Email = user.Email,
                    ImagesUrl = user.Images.Select(i => i.Url).ToList()
                };
                return Ok(userDTO);
            }
            else
            {
                var lastTen = user.Images.OrderByDescending(i => i.Id).Take(10);
                var userDTO = new UserDTO
                {
                    Id = user.Id,
                    Username = user.Name,
                    Email = user.Email,
                    ImagesUrl = lastTen.Select(i => i.Url).ToList()
                };
                return Ok(userDTO);
            };
        }

        //GET /api/users/{id}/images

        [HttpGet("{id}/images")]
        //Return all the images of a given user. The result should be paginated (10 images per page).
        //Paginated Response using ResponseHelper and PagedResponse, GetPagedResponse with Meta, Data, Links
        public async Task<IActionResult> GetUserImages(string id, [FromQuery] int page = 1)
        {
            var user = _context.Users.Include(x => x.Images).FirstOrDefault(u => u.Id == new Guid(id));
            if (user == null)
            {
                return NotFound("Error 404: User not found");
            }
            var images = user.Images.OrderByDescending(i => i.Id).Skip((page - 1) * 10).Take(10);
            var imagesDTO = new List<ImageDTO>();
            foreach (var image in images)
            {
                var imageDTO = new ImageDTO
                {
                    Id = image.Id,
                    Url = image.Url,
                    //Tags = image.Tags.Select(t => t.Text).ToList()
                };
                imagesDTO.Add(imageDTO);
            }
            var pagedResponse = ResponseHelper.GetPagedResponse("http://localhost:5000/api/users/{id}/images", imagesDTO, page, 10, user.Images.Count);
            return Ok(pagedResponse);
        }

        //DELETE /api/users{id}
        [HttpDelete("{id}")]
        //Remove the user (given by Id) and all of his images.
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == new Guid(id));
            if (user == null)
            {
                return NotFound("Error 404: User not found");
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

    }
}
