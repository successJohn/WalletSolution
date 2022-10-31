using System.ComponentModel.DataAnnotations.Schema;

namespace WalletSolution.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime DateOfTransaction { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        public string TransactionType { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public string AccountType { get; set; }
        public decimal Amount { get; set; }
    }
}
