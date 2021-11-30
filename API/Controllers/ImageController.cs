using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly DataContext _context;
        public ImageController(DataContext context)
        {
            _context = context;
        }
        //GET /api/images
        //return all the images. The result should be pagniated (10 images per page). The result should be ordered by posting date
        [HttpGet]
        public async Task<ActionResult> GetImages()
        {
            try
            {
                var images = await _context.Images.OrderByDescending(i => i.PostingDate).ToListAsync();
                return Ok(images);
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }
        //GET /api/images/{id}
        //Return all the details on an image (given by id). Return 404 if the id doesn't exist or return 400 if the id format is not correct
        [HttpGet("{id}")]
        public async Task<ActionResult> GetImage(int id)
        {
            try
            {
                var image = await _context.Images.FindAsync(id);
                if (image == null)
                {
                    return NotFound();
                }
                return Ok(image);
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }
        //GET /api/images/byTag?tag=cars
        //Return all the images that include the given tag . The result should be pagniated (10 images per page). The result should be ordered by posting date. If the result is empty, return 404

        //GET /api/images/populartags
        //Return the top 5 popular tags from the database (the tags that repeated the most). The result should be ordered by popularity (the tags that are used the most should be at the top)

    }
}