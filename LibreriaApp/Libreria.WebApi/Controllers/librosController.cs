using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Libreria.WebApi.Models;

namespace Libreria.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LibrosController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Libro>>> GetLibros()
        {
            return await _context.Libros
                .Include(l => l.User)
                .ToListAsync();
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Libro>> GetLibro(int id)
        {
            var libro = await _context.Libros
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (libro == null)
            {
                return NotFound();
            }

            return libro;
        }

        
        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Libro>>> GetLibrosByUsuario(int usuarioId)
        {
            var user = await _context.Users.FindAsync(usuarioId);

            if (user == null)
            {
                return NotFound($"Usuario con ID {usuarioId} no encontrado");
            }

            var libros = await _context.Libros
                .Where(l => l.UsuarioId == usuarioId)
                .ToListAsync();

            return libros;
        }

        
        [HttpPost]
        public async Task<ActionResult<Libro>> PostLibro(Libro libro)
        {
            
            var user = await _context.Users.FindAsync(libro.UsuarioId);
            if (user == null)
            {
                return BadRequest($"El usuario con ID {libro.UsuarioId} no existe");
            }

           
            if (libro.Calificacion < 1 || libro.Calificacion > 5)
            {
                return BadRequest("La calificación debe ser entre 1 y 5 estrellas");
            }

            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLibro), new { id = libro.Id }, libro);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLibro(int id, Libro libro)
        {
            if (id != libro.Id)
            {
                return BadRequest("El ID no coincide");
            }

            var existingLibro = await _context.Libros.FindAsync(id);
            if (existingLibro == null)
            {
                return NotFound();
            }

            
            var user = await _context.Users.FindAsync(libro.UsuarioId);
            if (user == null)
            {
                return BadRequest($"El usuario con ID {libro.UsuarioId} no existe");
            }

           
            if (libro.Calificacion < 1 || libro.Calificacion > 5)
            {
                return BadRequest("La calificación debe ser entre 1 y 5 estrellas");
            }

           
            existingLibro.Titulo = libro.Titulo;
            existingLibro.Autor = libro.Autor;
            existingLibro.AnioPublicacion = libro.AnioPublicacion;
            existingLibro.RutaPortada = libro.RutaPortada;
            existingLibro.Calificacion = libro.Calificacion;
            existingLibro.Resena = libro.Resena;
            existingLibro.UsuarioId = libro.UsuarioId;

            _context.Entry(existingLibro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LibroExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        
        [HttpPatch("{id}/calificar")]
        public async Task<IActionResult> CalificarLibro(int id, [FromBody] CalificacionRequest request)
        {
            var libro = await _context.Libros.FindAsync(id);

            if (libro == null)
            {
                return NotFound($"Libro con ID {id} no encontrado");
            }

            
            if (request.Calificacion < 1 || request.Calificacion > 5)
            {
                return BadRequest("La calificación debe ser entre 1 y 5 estrellas");
            }

            libro.Calificacion = request.Calificacion;

            
            if (!string.IsNullOrEmpty(request.Resena))
            {
                libro.Resena = request.Resena;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mensaje = "Libro calificado exitosamente",
                LibroId = libro.Id,
                Titulo = libro.Titulo,
                NuevaCalificacion = libro.Calificacion,
                Resena = libro.Resena
            });
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLibro(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                return NotFound();
            }

            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LibroExists(int id)
        {
            return _context.Libros.Any(e => e.Id == id);
        }
    }

    
    public class CalificacionRequest
    {
        public int Calificacion { get; set; }  
        public string? Resena { get; set; }     
    }
}