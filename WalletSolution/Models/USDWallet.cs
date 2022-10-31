using System.ComponentModel.DataAnnotations.Schema;

namespace WalletSolution.Models
{
    public class USDWallet
    {
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        public decimal Balance { get; set; }
        public string AccountNumber { get; set; }
        public DateTime DateOfCreation { get; set; }
        public string Pin { get; set; }
        public bool IsLocked { get; set; } = true;
    }
}
