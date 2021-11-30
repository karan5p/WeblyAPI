using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;
using API.Models.Helpers;
using API.Models.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        public UserController(DataContext context)
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
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
        //POST /api/users/{id}/images

        [HttpPost("{id}/image")]
        // Add one image to the given user images list. When adding the image, use the method ImageHelper.GetTags to generate the tags of the image and save these tags to the database.
        // The client should receive back a user json object that includes the last 10 images added by the user. If the user has less than 10 images then return them all
        public async Task<IActionResult> AddImage(string id, string image)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users.FindAsync(new Guid(id));
            if (user == null)
            {
                return NotFound();
            }
            var newImage = new Image()
            {
                Id = Guid.NewGuid(),
                Url = image,
                User = user,
                PostingDate = DateTime.Now,
            };
            var tags = new List<Tag>();
            var imageTags = ImageHelper.GetTags(image);
            foreach (var tag in imageTags)
            {
                tags.Add(new Tag()
                {
                    Id = Guid.NewGuid(),
                    Text = tag,
                    Images = new List<Image>() { newImage }

                });
            }
            newImage.Tags = tags;
            _context.Images.Add(newImage);
            await _context.SaveChangesAsync();
            return Ok(newImage);


        }






        //GET /api/users/{id}
        //Return a user (given by id) object that includes the last 10 images added by the user. If the user has less than 10 images then return them all.

        //GET /api/users/{id}/images
        //Return all the images of a given user. The result should be pagniated (10 images per page). The result should be ordered by posting date.

        //DELETE /api/users/{id}
        //Remove the user (given by Id) and all of his images.

        //Error
        //In case of any error, return an object error to the user. You should use return BadRequest(error).

    }
}