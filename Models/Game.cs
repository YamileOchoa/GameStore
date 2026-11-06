using System.ComponentModel.DataAnnotations;

namespace TiendaVideojuegosApi.Models;

public partial class Game
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(120)]
    public string Title { get; set; } = null!;

    [Range(0, 500, ErrorMessage = "El precio debe estar entre 0 y 500")]
    public decimal Price { get; set; }

    public DateOnly ReleaseDate { get; set; }

    [Required]
    [RegularExpression("^[ETM]$", ErrorMessage = "La clasificación debe ser E, T o M")]
    public string AgeRating { get; set; } = null!;

    [Range(0, 100000, ErrorMessage = "El stock debe estar entre 0 y 100000")]
    public int KeyStock { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Indique una desarrolladora válida")]
    public int DeveloperId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Indique un género válido")]
    public int GenreId { get; set; }

    // Propiedades de navegación nulas para evitar exigencias al hacer POST
    public virtual Developer? Developer { get; set; }
    public virtual Genre? Genre { get; set; }
}