using FluentValidation;
using MediatR;
using WalletSolution.Helpers;
using WalletSolution.Infrastructure.Persistence;
using WalletSolution.Models;

namespace WalletSolution.Application.Features.Users.Command
{
    public class SignUpUserCommand : IRequest<ApiResponse>
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class SignUpUserCommandValidator : AbstractValidator<SignUpUserCommand>
    {
        public SignUpUserCommandValidator()
        {
            RuleFor(x => x.Email).EmailAddress().WithMessage("Please provide a valid email address");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Please provide a valid first name");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Please provide a valid last name");
            RuleFor(x => x.Password).MinimumLength(8).NotEmpty().WithMessage("Password length should not be less than 8");
        }
    }

    public class SignUpUserCommandHandler : IRequestHandler<SignUpUserCommand, ApiResponse>
    {
        private readonly WalletDbContext _dbContext;
        private readonly ILogger<SignUpUserCommandHandler> _logger;
        private const string DefaultPin = "0000";
        public SignUpUserCommandHandler(WalletDbContext dbContext,
            ILogger<SignUpUserCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ApiResponse> Handle(SignUpUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = _dbContext.Users.FirstOrDefault(x => x.Email == request.Email);
            if (existingUser != null) return new ApiResponse(false, "This email has been registered already, please log into your account");

            var validatePassword = Passwordvalidator(request.Password, request.ConfirmPassword);
            if (validatePassword == false) return new ApiResponse(false, "Password and confirm password does not match");

            _logger.LogInformation("--- Creating new User ---");

            var user = UserObject(request);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"--- Creating USD Wallet for User: {user.FirstName} {user.LastName} ---");

            var usdWallet = new USDWallet
            {
                AccountNumber = Guid.NewGuid().ToString("N").ToUpper(),
                Balance = 0,
                DateOfCreation = DateTime.Now,
                UserId = user.Id,
                Pin = HasherHelper.Hasher(DefaultPin),
            };
            await _dbContext.USDWallets.AddAsync(usdWallet);


            _logger.LogInformation($"--- Creating NGN wallet for User: {user.FirstName} {user.LastName} ---");

            var ngnWallet = new NGNWallet
            {
                AccountNumber = Guid.NewGuid().ToString("N").ToUpper(),
                Balance = 0,
                DateOfCreation = DateTime.Now,
                UserId = user.Id,
                Pin = HasherHelper.Hasher(DefaultPin),
            };
            await _dbContext.NGNWallets.AddAsync(ngnWallet);

            await _dbContext.SaveChangesAsync();

            return new ApiResponse(true, $"User has been created successfully Your USD account number is {usdWallet.AccountNumber}," +
                $" your NGN account number is {ngnWallet.AccountNumber} and your default pin for both wallets is {DefaultPin} " +
                $"You have to change your pin before any transaction can be done.");
        }

        private User UserObject(SignUpUserCommand request)
        {
            var newUser = new User
            {
                Email = request.Email,
                FirstName = request.FirstName.ToUpper(),
                LastName = request.LastName.ToUpper(),
                Password = HasherHelper.Hasher(request.Password),
                DateOfCreation = DateTime.Now
            };
            return newUser;
        }
        private bool Passwordvalidator(string password, string confirmPassword)
        {
            if (password != confirmPassword) return false;
            return true;
        }
    }

}