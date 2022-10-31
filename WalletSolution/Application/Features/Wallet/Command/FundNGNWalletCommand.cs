using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WalletSolution.Enums;
using WalletSolution.Helpers;
using WalletSolution.Infrastructure.Persistence;

namespace WalletSolution.Application.Features.Wallet.Command
{
    public class FundNGNWalletCommand : IRequest<ApiResponse>
    {
        public decimal Amount { get; set; }
        public string Pin { get; set; }
    }

    public class FundNGNWalletCommandHandler : IRequestHandler<FundNGNWalletCommand, ApiResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly WalletDbContext _walletDbContext;
        private readonly ILogger<FundNGNWalletCommandHandler> _logger;
        public FundNGNWalletCommandHandler(IHttpContextAccessor httpContextAccessor,
            WalletDbContext walletDbContext, ILogger<FundNGNWalletCommandHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _walletDbContext = walletDbContext;
            _logger = logger;
        }

        public Task<ApiResponse> Handle(FundNGNWalletCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email);
            if (currentUser is null)
            {
                return Task.FromResult(new ApiResponse(false, "Unauthorized. Please login to continue"));
            }

            var walletObject = _walletDbContext.Users.Include(n => n.NGNWallet).FirstOrDefault(x => x.Email == currentUser.Value);
            if (walletObject.NGNWallet.IsLocked is true)
            {
                return Task.FromResult(new ApiResponse(false, "Please change your default pin to proceed"));
            }
            var pinHash = HasherHelper.Hasher(request.Pin);
            if (pinHash != walletObject.NGNWallet.Pin)
            {
                return Task.FromResult(new ApiResponse(false, "You have entered a wrong pin"));
            }

            var newBal = walletObject.NGNWallet.Balance += request.Amount;
            var transaction = new Models.Transaction
            {
                AccountType = AccountType.NGN.ToString(),
                CurrentBalance = newBal,
                PreviousBalance = walletObject.NGNWallet.Balance - request.Amount,
                DateOfTransaction = DateTime.Now,
                TransactionType = TransactionType.CREDIT.ToString(),
                UserId = walletObject.Id,
                Amount = request.Amount,
            };


            _walletDbContext.Transactions.Add(transaction);
            _walletDbContext.Update(walletObject);
            _walletDbContext.SaveChanges();
            return Task.FromResult(new ApiResponse(true, $"NGN wallet has been successfully funded with {request.Amount} and new account balance is {newBal}"));
        }
    }
}
