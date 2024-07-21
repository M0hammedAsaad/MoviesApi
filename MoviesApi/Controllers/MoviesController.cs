using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dtos;
using MoviesApi.Models;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private List<string> _allowedExtention = new List<string> { ".jpg", ".png" };
        private long _maxSize = 1100000;
        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllMovies()
        {
            var movies = _context.Movies.Include(m => m.Genre).OrderByDescending(m => m.Rate).ToList();
            return Ok(movies);
        }

        [HttpGet("{id}")]
        public IActionResult GetByID(int id)
        {
            var movie = _context.Movies.Include(m => m.Genre).FirstOrDefault(m => m.Id == id);
            if (movie == null)
                return NotFound("Movie is not found");
            return Ok(movie);
        }

        [HttpPost]
        public IActionResult CreateMovie([FromForm] MovieDto dto)
        {
            if (dto.Poster == null)
                return BadRequest("Poster is required");

            if (!_allowedExtention.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Only .png or .jpg are allowed");

            else if (dto.Poster.Length > _maxSize)
                return BadRequest("Max size allowed for poster is 1MB");

            var isValidGenre = _context.Genres.Any(g => g.Id == dto.GenreId);
            if (!isValidGenre)
                return BadRequest("ID is Invalid");

            using var stream = new MemoryStream();
            dto.Poster.CopyTo(stream);

            Movie movie = new Movie()
            {
                GenreId = dto.GenreId,
                Title = dto.Title,
                Rate = dto.Rate,
                Poster = stream.ToArray(),
                StoreLine = dto.StoreLine,
                Year = dto.Year
            };

            _context.Movies.Add(movie);
            _context.SaveChanges();

            return Ok(movie);
        }

        [HttpPut("{id}")]
        public IActionResult EditMovie(int id, [FromForm] MovieDto dto)
        {
            var movie = _context.Movies.FirstOrDefault(g => g.Id == id);
            if (movie == null)
                return NotFound($"Movie not found with id:{id}");

            var isValidGenre = _context.Genres.Any(g => g.Id == dto.GenreId);
            if (!isValidGenre)
                return BadRequest("ID is Invalid");

            if (dto.Poster != null)
            {
                if (!_allowedExtention.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Only .png or .jpg are allowed");

                else if (dto.Poster.Length > _maxSize)
                    return BadRequest("Max size allowed for poster is 1MB");

                using var stream = new MemoryStream();
                dto.Poster.CopyTo(stream);
                movie.Poster = stream.ToArray();
            }

            movie.GenreId = dto.GenreId;
            movie.Title = dto.Title;
            movie.Rate = dto.Rate;
            movie.StoreLine = dto.StoreLine;
            movie.Year = dto.Year;

            _context.SaveChanges();
            return Ok(movie);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMovie(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if (movie == null)
                return NotFound($"Movie you want delete is not found with ID:{id}");

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            return Ok(movie);
        }
    }
}
