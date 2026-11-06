using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TiendaVideojuegosApi.Models;

public partial class Genre
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(200)]
    public string? Description { get; set; }

    [JsonIgnore]
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}