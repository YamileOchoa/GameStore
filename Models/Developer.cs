using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TiendaVideojuegosApi.Models;

public partial class Developer
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "El país es obligatorio")]
    [StringLength(60)]
    public string Country { get; set; } = null!;

    [Range(1970, 2026, ErrorMessage = "El año debe estar entre 1970 y 2026")]
    public int FoundedYear { get; set; }

    [JsonIgnore]
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}