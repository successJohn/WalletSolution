using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WalletSolution.Helpers;
using WalletSolution.Infrastructure.Persistence;

namespace WalletSolution.Application.Features.Wallet.Queries
{
    public class USDBalanceEnquiryQuery : IRequest<ApiResponse>
    {

    }

    public class USDBalanceEnquiryQueryHandler : IRequestHandler<USDBalanceEnquiryQuery, ApiResponse>
    {
        private readonly ILogger<USDBalanceEnquiryQueryHandler> _logger;
        private readonly WalletDbContext _walletDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public USDBalanceEnquiryQueryHandler(ILogger<USDBalanceEnquiryQueryHandler> logger,
            WalletDbContext walletDbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _walletDbContext = walletDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<ApiResponse> Handle(USDBalanceEnquiryQuery request, CancellationToken cancellationToken)
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

            return Task.FromResult(new ApiResponse(true, $"USD Account: {walletObject.USDWallet.AccountNumber} has a balance of: {walletObject.USDWallet.Balance} "));
        }
    }
}
