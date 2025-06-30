using System.ComponentModel.DataAnnotations;

namespace FilmsDB.Domain.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public List<Film> Films { get; set; } = new();
}