using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmsDB.Domain.Models;

public class Film
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// ID на кинопоиске
    /// </summary>
    public int UId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public double? Rating { get; set; }

    public int Year { get; set; }

    public string PosterUrl { get; set; }

    public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; }
}