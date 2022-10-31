using MediatR;
using System.Security.Claims;
using WalletSolution.Helpers;
using WalletSolution.Infrastructure.Persistence;

namespace WalletSolution.Application.Features.Transactions.Queries
{
    public class TransactionSummary
    {
        public DateTime DateOfTransaction { get; set; }
        public int UserId { get; set; }
        public string TransactionType { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public string AccountType { get; set; }
        public decimal Amount { get; set; }
    }
    public class TransactionHistoryQuery : IRequest<ApiResponse<List<IEnumerable<TransactionSummary>>>>
    {

    }

    public class TransactionHistoryQueryHandler : IRequestHandler<TransactionHistoryQuery, ApiResponse<List<IEnumerable<TransactionSummary>>>>
    {
        private readonly ILogger<TransactionHistoryQueryHandler> _logger;
        private readonly WalletDbContext _walletDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransactionHistoryQueryHandler(ILogger<TransactionHistoryQueryHandler> logger,
            WalletDbContext walletDbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _walletDbContext = walletDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<ApiResponse<List<IEnumerable<TransactionSummary>>>> Handle(TransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            var currentUser = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email);
            if (currentUser is null)
            {
                return Task.FromResult(new ApiResponse<List<IEnumerable<TransactionSummary>>>(false, "Unauthorized. Please login to continue"));
            }

            var transactions = _walletDbContext.Users.Where(x => x.Email == currentUser.Value).Select(t => t.Transactions.Select(t => new TransactionSummary
            {
                AccountType = t.AccountType,
                DateOfTransaction = t.DateOfTransaction,
                Amount = t.Amount,
                CurrentBalance = t.CurrentBalance,
                PreviousBalance = t.PreviousBalance,
                TransactionType = t.TransactionType,
                UserId = t.UserId
            })).ToList();

            return Task.FromResult(new ApiResponse<List<IEnumerable<TransactionSummary>>>(true, "Transaction history Retrieved", transactions));
        }
    }
}
