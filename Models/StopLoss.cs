using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockAPI.Models;

[Table("stop_losses")]
public class StopLoss
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public required int UserId { get; set; }

    [Column("transaction_id")]
    public required int TransactionId { get; set; }

    [Column("stock_id")]
    public required int StockId { get; set; }

    [Column("is_active")]
    public required bool IsActive { get; set; }

    [Column("price")]
    public required decimal Price { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [ForeignKey("TransactionId")]
    public StockTransaction? StockTransaction { get; set; }

    [ForeignKey("StockId")]
    public Stock? Stock { get; set; }
}
