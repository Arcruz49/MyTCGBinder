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
    public string TcgCardId { get; set; } = string.Empty;

    [Column("variant")]
    public CardVariant Variant { get; set; } = CardVariant.Normal;

    [Column("quantity")]
    public int Quantity { get; set; } = 1;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public TCGCard TCGCard { get; set; } = null!;
}