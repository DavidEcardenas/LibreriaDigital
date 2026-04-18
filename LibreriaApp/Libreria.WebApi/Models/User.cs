namespace Libreria.WebApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contra { get; set; } = string.Empty;

        
        public ICollection<Libro> Libros { get; set; } = new List<Libro>();
    }
}