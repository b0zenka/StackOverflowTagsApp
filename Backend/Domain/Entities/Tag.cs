using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    [NotMapped]
    public double Percentage { get; set; }
}