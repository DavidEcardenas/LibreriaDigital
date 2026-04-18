using Libreria.WebApi.Models;

public class Libro
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Autor { get; set; } = string.Empty;
    public int AnioPublicacion { get; set; }
    public string? RutaPortada { get; set; }
    public int Calificacion { get; set; }
    public string? Resena { get; set; }
    public int UsuarioId { get; set; }

    
    public User? User { get; set; }
}