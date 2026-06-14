using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyTCGBinder.Domain.Enums;

namespace MyTCGBinder.Domain.Entities;

[Table("user_cards")]
public class UserCard
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("tcg_card_id")]
    [MaxLength(50)]
    public string TcgCardId { get; set; } = string.Empty; // ex: "swsh4-25"

    [Required]
    [Column("name")]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Column("number")]
    [MaxLength(20)]
    public string Number { get; set; } = string.Empty; // ex: "25"

    [Required]
    [Column("set_id")]
    [MaxLength(20)]
    public string SetId { get; set; } = string.Empty; // ex: "swsh4"

    [Column("set_name")]
    [MaxLength(100)]
    public string SetName { get; set; } = string.Empty; // ex: "Vivid Voltage"

    [Column("rarity")]
    [MaxLength(50)]
    public string Rarity { get; set; } = string.Empty;

    [Column("image_url")]
    [MaxLength(255)]
    public string ImageUrl { get; set; } = string.Empty;

    [Column("image_url_large")]
    [MaxLength(255)]
    public string ImageUrlLarge { get; set; } = string.Empty;

    [Column("variant")]
    public CardVariant Variant { get; set; } = CardVariant.Normal;

    [Column("quantity")]
    public int Quantity { get; set; } = 1;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}