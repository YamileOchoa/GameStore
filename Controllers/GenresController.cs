using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaVideojuegosApi.Models;

namespace TiendaVideojuegosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly TiendaContext _context;

    public GenresController(TiendaContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Genre>>> GetAll()
    {
        var list = await _context.Genres.AsNoTracking().OrderBy(g => g.Name).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Genre>> GetById(int id)
    {
        var item = await _context.Genres.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    // Ruta anidada: /api/genres/{id}/games
    [HttpGet("{id:int}/games")]
    public async Task<ActionResult<IEnumerable<Game>>> GetGames(int id)
    {
        if (!await _context.Genres.AnyAsync(g => g.Id == id)) return NotFound();

        var games = await _context.Games.AsNoTracking()
            .Include(g => g.Developer)
            .Where(g => g.GenreId == id)
            .OrderBy(g => g.Title)
            .ToListAsync();

        return Ok(games);
    }

    [HttpPost]
    public async Task<ActionResult<Genre>> Post(Genre model)
    {
        if (await _context.Genres.AnyAsync(g => g.Name.ToLower() == model.Name.ToLower()))
            return Conflict(new { mensaje = "Ya existe un género con ese nombre." });

        _context.Genres.Add(model);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, Genre model)
    {
        if (id != model.Id)
            return BadRequest(new { mensaje = "El id de la ruta no coincide con el del cuerpo." });

        var existing = await _context.Genres.FindAsync(id);
        if (existing == null) return NotFound();

        if (await _context.Genres.AnyAsync(g => g.Id != id && g.Name.ToLower() == model.Name.ToLower()))
            return Conflict(new { mensaje = "Ya existe otro género con ese nombre." });

        existing.Name = model.Name;
        existing.Description = model.Description;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _context.Genres.FindAsync(id);
        if (existing == null) return NotFound();

        if (await _context.Games.AnyAsync(g => g.GenreId == id))
            return Conflict(new { mensaje = "No se puede eliminar: el género tiene juegos." });

        _context.Genres.Remove(existing);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}