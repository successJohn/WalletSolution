using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WalletSolution.Enums;
using WalletSolution.Helpers;
using WalletSolution.Infrastructure.Persistence;

namespace WalletSolution.Application.Features.Wallet.Command
{
    public class WithdrawUSDCommand : IRequest<ApiResponse>
    {
        public decimal Amount { get; set; }
        public string Pin { get; set; }
    }

    public class WithdrawUSDCommandHandler : IRequestHandler<WithdrawUSDCommand, ApiResponse>
    {
        private readonly ILogger<WithdrawUSDCommandHandler> _logger;
        private readonly WalletDbContext _walletDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WithdrawUSDCommandHandler(ILogger<WithdrawUSDCommandHandler> logger,
            WalletDbContext walletDbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _walletDbContext = walletDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<ApiResponse> Handle(WithdrawUSDCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email);
            if (currentUser is null)
            {
                return Task.FromResult(new ApiResponse(false, "Unauthorized. Please login to continue"));
            }

            var walletObject = _walletDbContext.Users.Include(n => n.USDWallet).FirstOrDefault(x => x.Email == currentUser.Value);
            if (walletObject.USDWallet.IsLocked is true)
            {
                return Task.FromResult(new ApiResponse(false, "Please change your default pin to proceed"));
            }

            var pinHash = HasherHelper.Hasher(request.Pin);
            if (pinHash != walletObject.USDWallet.Pin)
            {
                return Task.FromResult(new ApiResponse(false, "You have entered a wrong pin"));
            }

            if (request.Amount > walletObject.USDWallet.Balance)
            {
                return Task.FromResult(new ApiResponse(false, "Your balance is insufficient for this withdrawal"));
            }

            if (request.Amount <= 0)
            {
                return Task.FromResult(new ApiResponse(false, "You have entered an invalid amount"));
            }

            var newBal = walletObject.USDWallet.Balance -= request.Amount;
            var transaction = new Models.Transaction
            {
                AccountType = AccountType.USD.ToString(),
                CurrentBalance = newBal,
                PreviousBalance = walletObject.USDWallet.Balance + request.Amount,
                DateOfTransaction = DateTime.Now,
                TransactionType = TransactionType.DEBIT.ToString(),
                UserId = walletObject.Id,
                Amount = request.Amount,
            };

            _walletDbContext.Transactions.Add(transaction);
            _walletDbContext.Update(walletObject);
            _walletDbContext.SaveChanges();
            return Task.FromResult(new ApiResponse(true, $"USD Account: {walletObject.USDWallet.AccountNumber} has been debited with: {request.Amount} " +
                $"new account balance is {newBal}"));
        }
    }
}
