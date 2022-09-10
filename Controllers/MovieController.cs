using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaApi.Data;
using CinemaApi.Models;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace CinemaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private CinemaDbContext _dbContext;

        public MovieController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //[Authorize]
        [HttpGet("[action]")]
        public IActionResult AllMovies()
        {
            var movies = from movie in _dbContext.Movies
                         select new
                         {
                             Id = movie.Id,
                             Name = movie.Name,
                             Duratinon = movie.Duration,
                             Language = movie.Language,
                             Rating = movie.Rating,
                             Genre = movie.Genre,
                             ImageUrl = movie.ImageUrl
                         }; 
            return Ok(movies);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromForm] Movie movieObj)
        {
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");
            if (movieObj.Image !=null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movieObj.Image.CopyTo(fileStream);
            }

            movieObj.ImageUrl = filePath.Remove(0,7);
            _dbContext.Movies.Add(movieObj);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }
        [Authorize(Roles ="Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm] Movie movieObj)
        {
            var movie = _dbContext.Movies.Find(id);
            if (movie == null)
                return NotFound("No record found against this Id");
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");
            if (movieObj.Image != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movieObj.Image.CopyTo(fileStream);
                movie.ImageUrl = filePath.Remove(0, 7);

            }
            movie.Name = movieObj.Name;
            movie.Language = movieObj.Language;
            movie.Description = movieObj.Description;
            movie.Duration = movieObj.Duration;
            movie.Genre = movieObj.Genre;
            movie.PlayingDate = movieObj.PlayingDate;
            movie.PlayingTime = movieObj.PlayingTime;
            movie.TrailorUrl = movieObj.TrailorUrl;
            movie.TicketPrice = movieObj.TicketPrice;
            movie.Rating = movieObj.Rating;
            _dbContext.SaveChanges();
            return Ok("Record updated succesfully");
        }
        [Authorize (Roles ="Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var movie = _dbContext.Movies.Find(id);
            if (movie == null)
                return NotFound("No record found against this Id");
            _dbContext.Movies.Remove(movie);
            _dbContext.SaveChanges();
            return Ok("Record deleted succesfully");
        }

    }
}
