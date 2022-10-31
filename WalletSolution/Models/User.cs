namespace WalletSolution.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public DateTime DateOfCreation { get; set; }
        public USDWallet USDWallet { get; set; }
        public NGNWallet NGNWallet { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}
