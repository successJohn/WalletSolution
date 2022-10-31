using Ardalis.GuardClauses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WalletSolution.Helpers;
using WalletSolution.Infrastructure.Persistence;

namespace WalletSolution.Application.Features.Users.Command
{
    public class ChangeUSDWalletPinCommand : IRequest<ApiResponse>
    {
        public string OldPin { get; set; }
        public string NewPin { get; set; }
        public string ConfirmNewPin { get; set; }
    }

    public class ChangeUSDPinCommandHandler : IRequestHandler<ChangeUSDWalletPinCommand, ApiResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly WalletDbContext _walletDbContext;
        private readonly ILogger<ChangeUSDPinCommandHandler> _logger;

        public ChangeUSDPinCommandHandler(IHttpContextAccessor httpContextAccessor,
            WalletDbContext walletDbContext,
            ILogger<ChangeUSDPinCommandHandler> logger)
        {
            _httpContextAccessor = Guard.Against.Null(httpContextAccessor, nameof(httpContextAccessor));
            _walletDbContext = walletDbContext;
            _logger = logger;
        }

        public Task<ApiResponse> Handle(ChangeUSDWalletPinCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email);
            if (currentUser is null)
            {
                return Task.FromResult(new ApiResponse(false, "Unauthorized. Please login to continue"));
            }
            var pinHash = HasherHelper.Hasher(request.OldPin);
            var walletObject = _walletDbContext.Users.Include(u => u.USDWallet).FirstOrDefault(x => x.Email == currentUser.Value);
            if (pinHash != walletObject.USDWallet.Pin)
            {
                return Task.FromResult(new ApiResponse(false, "Old pin is incorrect"));
            }
            if (request.NewPin != request.ConfirmNewPin)
            {
                return Task.FromResult(new ApiResponse(false, "Newpin and Oldpin does not match!!"));
            }

            var newPinHash = HasherHelper.Hasher(request.NewPin);

            walletObject.USDWallet.Pin = newPinHash;
            walletObject.USDWallet.IsLocked = false;

            _walletDbContext.Update(walletObject);
            _walletDbContext.SaveChanges();

            return Task.FromResult(new ApiResponse(true, "New pin has been set!"));
        }
    }
}
