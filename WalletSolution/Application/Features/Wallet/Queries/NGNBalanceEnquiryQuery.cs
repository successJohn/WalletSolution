using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WalletSolution.Helpers;
using WalletSolution.Infrastructure.Persistence;

namespace WalletSolution.Application.Features.Wallet.Queries
{
    public class NGNBalanceEnquiryQuery : IRequest<ApiResponse>
    {

    }

    public class NGNBalanceEnquiryQueryHandler : IRequestHandler<NGNBalanceEnquiryQuery, ApiResponse>
    {
        private readonly ILogger<NGNBalanceEnquiryQueryHandler> _logger;
        private readonly WalletDbContext _walletDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NGNBalanceEnquiryQueryHandler(ILogger<NGNBalanceEnquiryQueryHandler> logger,
            WalletDbContext walletDbContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _walletDbContext = walletDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<ApiResponse> Handle(NGNBalanceEnquiryQuery request, CancellationToken cancellationToken)
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

            return Task.FromResult(new ApiResponse(true, $"NGN Account: {walletObject.NGNWallet.AccountNumber} has a balance of: {walletObject.NGNWallet.Balance} "));
        }
    }
}
