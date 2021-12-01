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
    public class ImagesController : ControllerBase
    {
        private readonly DataContext _context;
        public ImagesController(DataContext context)
        {
            _context = context;
        }
        //GET /api/images
        //Return all the images. The result should be paginated (10 images per page). The result should be ordered by posting date

        [HttpGet]
        //Return all the images. The result should be paginated (10 images per page).
        public async Task<IActionResult> GetImages([FromQuery] int page = 1)
        {
            var images = _context.Images.OrderByDescending(i => i.Id).Skip((page - 1) * 10).Take(10);
            var imagesDTO = new List<ImageDTO>();
            foreach (var image in images)
            {
                var imageDTO = new ImageDTO
                {
                    Id = image.Id,
                    Url = image.Url,
                    // Username = image.User.Username,
                    //Tags = image.Tags.Select(t => t.Text).ToList()
                };
                imagesDTO.Add(imageDTO);
            }
            var pagedResponse = ResponseHelper.GetPagedResponse("http://localhost:5000/api/images", imagesDTO, page, 10, _context.Images.Count());
            return Ok(pagedResponse);
        }
        //GET /api/images/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(string id)
        {
            var image = _context.Images.Include(i => i.User).FirstOrDefault(i => i.Id == new Guid(id));
            if (image == null)
            {
                return NotFound("Error 404: Image not found");
            }
            var imageDTO = new ImageDTO
            {
                Id = image.Id,
                Url = image.Url,
                // Username = image.User.Username,
                //Tags = image.Tags.Select(t => t.Text).ToList()
            };
            return Ok(imageDTO);
        }
        //GET /api/images/byTag?tag=cars
        [HttpGet("byTag")]
        //Return all the images that include the given tag. The result should be paginated (10 images per page). The result should be ordered by posting date. If the result is empty, return 404
        public async Task<IActionResult> GetImagesByTag([FromQuery] string tag, [FromQuery] int page = 1)
        {
            var images = _context.Images.Include(i => i.Tags).Where(i => i.Tags.Any(t => t.Text == tag)).OrderByDescending(i => i.Id).Skip((page - 1) * 10).Take(10);
            var imagesDTO = new List<ImageDTO>();
            foreach (var image in images)
            {
                var imageDTO = new ImageDTO
                {
                    Id = image.Id,
                    Url = image.Url,
                    // Username = image.User.Username,
                    //Tags = image.Tags.Select(t => t.Text).ToList()
                };
                imagesDTO.Add(imageDTO);
            }
            var pagedResponse = ResponseHelper.GetPagedResponse("http://localhost:5000/api/images/byTag?tag=" + tag, imagesDTO, page, 10, _context.Images.Count());
            return Ok(pagedResponse);
        }
        //GET /api/images/populartags
        [HttpGet("populartags")]
        //Return the top 5 popular tags from the database (the tags that repeated the most). The result should be ordered by popularity (the tags that are used the most should be at the top).
        public async Task<IActionResult> GetPopularTags()
        {
            var tags = _context.Tags.OrderByDescending(t => t.Images.Count).Take(5);
            var tagsDTO = new List<TagDTO>();
            foreach (var tag in tags)
            {
                var tagDTO = new TagDTO
                {
                    Text = tag.Text,
                    Count = tag.Images.Count
                };
                tagsDTO.Add(tagDTO);
            }
            return Ok(tagsDTO);
        }


    }
}