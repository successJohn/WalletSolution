using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WalletSolution.Enums;
using WalletSolution.Helpers;
using WalletSolution.Infrastructure.Persistence;

namespace WalletSolution.Application.Features.Wallet.Command
{
    public class FundUSDWalletCommand : IRequest<ApiResponse>
    {
        public decimal Amount { get; set; }
        public string Pin { get; set; }
    }

    public class FundUSDWalletCommandHandler : IRequestHandler<FundUSDWalletCommand, ApiResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly WalletDbContext _walletDbContext;
        private readonly ILogger<FundUSDWalletCommandHandler> _logger;
        public FundUSDWalletCommandHandler(IHttpContextAccessor httpContextAccessor,
            WalletDbContext walletDbContext, ILogger<FundUSDWalletCommandHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _walletDbContext = walletDbContext;
            _logger = logger;
        }

        public Task<ApiResponse> Handle(FundUSDWalletCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email);
            if (currentUser is null)
            {
                return Task.FromResult(new ApiResponse(false, "Unauthorized. Please login to continue"));
            }

            var walletObject = _walletDbContext.Users.Include(u => u.USDWallet).FirstOrDefault(x => x.Email == currentUser.Value);
            if (walletObject.USDWallet.IsLocked is true)
            {
                return Task.FromResult(new ApiResponse(false, "Please change your default pin to proceed"));
            }

            var pinHash = HasherHelper.Hasher(request.Pin);
            if(pinHash != walletObject.USDWallet.Pin)
            {
                return Task.FromResult(new ApiResponse(false, "You have entered a wrong pin"));
            }


            var newBal = walletObject.USDWallet.Balance += request.Amount;
            var transaction = new Models.Transaction
            {
                AccountType = AccountType.USD.ToString(),
                CurrentBalance = newBal,
                PreviousBalance = walletObject.USDWallet.Balance - request.Amount,
                DateOfTransaction = DateTime.Now,
                TransactionType = TransactionType.CREDIT.ToString(),
                UserId = walletObject.Id,
                Amount = request.Amount,
            };


            _walletDbContext.Transactions.Add(transaction);
            _walletDbContext.Update(walletObject);
            _walletDbContext.SaveChanges();
            return Task.FromResult(new ApiResponse(true, $"USD wallet has been successfully funded with {request.Amount} and new account balance is {newBal}"));
        }
    }
}
