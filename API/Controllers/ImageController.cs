using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;
using API.Models.Helpers;
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
        //Response
        //{ "meta": { "totalPages": 20, "totalImages": 200, }, "data": [ { "id": "233-9A2-…", "url": "url", "username": "name" } ], "links": { "first": "url", "prev": "url", "next": "url", "last": "url",1" } }
        //use try catch to handle the error


        //GET /api/images/{id}
        //Return all the details on an image (given by id). Return 404 if the id doesn't exist or return 400 if the id format is not correct
        //Response
        //{ "id": "233-9A2-…", "url": "url", "user-name": "name", "user-id": "123-33A-…", "tags": ["bird", "nature"] }


        //GET /api/images/byTag?tag=cars
        //Return all the images that include the given tag . The result should be paginated (10 images per page). The result should be ordered by posting date. If the result is empty, return 404
        //Response
        //{ "meta": { "totalPages": 20, "totalImages": 200, }, "data": [ { "id": "233-9A2-…", "url": "url", "username": "name" } ], "links": { "first": "url", "prev": "url", "next": "url", "last": "url",1" } }


        //GET /api/images/populartags
        //Return the top 5 popular tags from the database (the tags that repeated the most). The result should be ordered by popularity (the tags that are used the most should be at the top)
        //Response
        //[ { "tag": "car", "count": "298" }, { "tag": "nature", "count": "243" }, }


    }
}