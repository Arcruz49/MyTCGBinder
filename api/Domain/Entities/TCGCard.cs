using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyTCGBinder.Domain.Entities;

[Table("tcg_cards")]
public class TCGCard
{
    [Key]
    [Column("id")]
    [MaxLength(50)]
    public string Id { get; set; } = string.Empty; // ex: "swsh4-25"

    [Required]
    [Column("name")]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("number")]
    [MaxLength(20)]
    public string Number { get; set; } = string.Empty;

    [Required]
    [Column("set_id")]
    [MaxLength(20)]
    public string SetId { get; set; } = string.Empty;

    [Required]
    [Column("set_name")]
    [MaxLength(100)]
    public string SetName { get; set; } = string.Empty;

    [Column("series")]
    [MaxLength(100)]
    public string Series { get; set; } = string.Empty;

    [Column("rarity")]
    [MaxLength(50)]
    public string Rarity { get; set; } = string.Empty;

    [Column("image_small")]
    [MaxLength(255)]
    public string ImageSmall { get; set; } = string.Empty;

    [Column("image_large")]
    [MaxLength(255)]
    public string ImageLarge { get; set; } = string.Empty;

    [Column("synced_at")]
    public DateTime SyncedAt { get; set; }

    public ICollection<UserCard> UserCards { get; set; } = [];
}