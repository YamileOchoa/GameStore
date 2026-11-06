using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaVideojuegosApi.Models;

namespace TiendaVideojuegosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DevelopersController : ControllerBase
{
    private readonly TiendaContext _context;

    public DevelopersController(TiendaContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Developer>>> GetAll()
    {
        var list = await _context.Developers.AsNoTracking().OrderBy(d => d.Name).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Developer>> GetById(int id)
    {
        var item = await _context.Developers.FindAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    // Ruta anidada: /api/developers/{id}/games
    [HttpGet("{id:int}/games")]
    public async Task<ActionResult<IEnumerable<Game>>> GetGames(int id)
    {
        if (!await _context.Developers.AnyAsync(d => d.Id == id)) return NotFound();

        var games = await _context.Games.AsNoTracking()
            .Include(g => g.Genre)
            .Where(g => g.DeveloperId == id)
            .OrderBy(g => g.Title)
            .ToListAsync();

        return Ok(games);
    }

    [HttpPost]
    public async Task<ActionResult<Developer>> Post(Developer model)
    {
        if (await _context.Developers.AnyAsync(d => d.Name.ToLower() == model.Name.ToLower()))
            return Conflict(new { mensaje = "Ya existe una desarrolladora con ese nombre." });

        _context.Developers.Add(model);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, Developer model)
    {
        if (id != model.Id)
            return BadRequest(new { mensaje = "El id de la ruta no coincide con el del cuerpo." });

        var existing = await _context.Developers.FindAsync(id);
        if (existing == null) return NotFound();

        if (await _context.Developers.AnyAsync(d => d.Id != id && d.Name.ToLower() == model.Name.ToLower()))
            return Conflict(new { mensaje = "Ya existe otra desarrolladora con ese nombre." });

        existing.Name = model.Name;
        existing.Country = model.Country;
        existing.FoundedYear = model.FoundedYear;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _context.Developers.FindAsync(id);
        if (existing == null) return NotFound();

        if (await _context.Games.AnyAsync(g => g.DeveloperId == id))
            return Conflict(new { mensaje = "No se puede eliminar: la desarrolladora tiene juegos." });

        _context.Developers.Remove(existing);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}