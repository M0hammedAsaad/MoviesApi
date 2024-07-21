using Microsoft.AspNetCore.Mvc;
using MoviesApi.Models;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetGenres()
        {
            List<Genre> genre = _context.Genres.OrderBy(g => g.Name).ToList();
            return Ok(genre);
        }

        [HttpPost]
        public IActionResult CreateGenre(Genre genre)
        {
            _context.Genres.Add(genre);
            _context.SaveChanges();
            return Ok(genre);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateGenre(int id, Genre genre)
        {
            var oldGenre = _context.Genres.FirstOrDefault(g => g.Id == id);
            if (oldGenre == null)
                return NotFound($"No genre was found with Id:{id}");
            oldGenre.Name = genre.Name;
            _context.SaveChanges();
            return Ok(oldGenre);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteGenre(int id)
        {
            Genre deletedGenre = _context.Genres.Find(id);
            if (deletedGenre == null)
                return NotFound($"No genre was found with Id:{id}");

            _context.Genres.Remove(deletedGenre);
            _context.SaveChanges();

            return Ok(deletedGenre);
        }
    }
}
