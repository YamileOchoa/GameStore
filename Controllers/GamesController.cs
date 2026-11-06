using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaVideojuegosApi.Models;

namespace TiendaVideojuegosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly TiendaContext _context;

    public GamesController(TiendaContext context) => _context = context;

    // Filtros por query string: ?genre=RPG&ageRating=M&minPrice=10&maxPrice=50&title=witcher
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Game>>> GetAll(
        [FromQuery] string? genre,
        [FromQuery] string? ageRating,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? title)
    {
        var query = _context.Games.AsNoTracking()
            .Include(g => g.Developer)
            .Include(g => g.Genre)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(genre))
            query = query.Where(g => g.Genre!.Name.ToLower() == genre.ToLower());

        if (!string.IsNullOrWhiteSpace(ageRating))
            query = query.Where(g => g.AgeRating == ageRating.ToUpper());

        if (minPrice.HasValue)
            query = query.Where(g => g.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(g => g.Price <= maxPrice.Value);

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(g => g.Title.ToLower().Contains(title.ToLower()));

        return Ok(await query.OrderBy(g => g.Title).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Game>> GetById(int id)
    {
        var item = await _context.Games.AsNoTracking()
            .Include(g => g.Developer)
            .Include(g => g.Genre)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Game>> Post(Game model)
    {
        if (!await _context.Developers.AnyAsync(d => d.Id == model.DeveloperId))
            return BadRequest(new { mensaje = "La desarrolladora indicada no existe." });

        if (!await _context.Genres.AnyAsync(g => g.Id == model.GenreId))
            return BadRequest(new { mensaje = "El género indicado no existe." });

        if (await _context.Games.AnyAsync(g => g.DeveloperId == model.DeveloperId && g.Title.ToLower() == model.Title.ToLower()))
            return Conflict(new { mensaje = "Esa desarrolladora ya tiene un juego con ese título." });

        _context.Games.Add(model);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, Game model)
    {
        if (id != model.Id)
            return BadRequest(new { mensaje = "El id de la ruta no coincide con el del cuerpo." });

        var existing = await _context.Games.FindAsync(id);
        if (existing == null) return NotFound();

        if (!await _context.Developers.AnyAsync(d => d.Id == model.DeveloperId))
            return BadRequest(new { mensaje = "La desarrolladora indicada no existe." });

        if (!await _context.Genres.AnyAsync(g => g.Id == model.GenreId))
            return BadRequest(new { mensaje = "El género indicado no existe." });

        if (await _context.Games.AnyAsync(g => g.Id != id && g.DeveloperId == model.DeveloperId && g.Title.ToLower() == model.Title.ToLower()))
            return Conflict(new { mensaje = "Esa desarrolladora ya tiene un juego con ese título." });

        existing.Title = model.Title;
        existing.Price = model.Price;
        existing.ReleaseDate = model.ReleaseDate;
        existing.AgeRating = model.AgeRating;
        existing.KeyStock = model.KeyStock;
        existing.DeveloperId = model.DeveloperId;
        existing.GenreId = model.GenreId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _context.Games.FindAsync(id);
        if (existing == null) return NotFound();

        _context.Games.Remove(existing);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}