using FluentValidation;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WalletSolution.Helpers;
using WalletSolution.Infrastructure.Persistence;

namespace WalletSolution.Application.Features.Users.Command
{
    public class LoginSummary
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }

    public class LoginUserCommand : IRequest<ApiResponse<LoginSummary>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email must nit be empty");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password must not be empty");
        }
    }

    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, ApiResponse<LoginSummary>>
    {
        private readonly WalletDbContext _walletDbContext;
        private readonly ILogger<LoginUserCommandHandler> _logger;
        private readonly IConfiguration _configuration;
        public LoginUserCommandHandler(WalletDbContext dbContext,
            ILogger<LoginUserCommandHandler> logger,
            IConfiguration configuration)
        {
            _walletDbContext = dbContext;
            _logger = logger;
            _configuration = configuration;
        }

        public Task<ApiResponse<LoginSummary>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var passwordHash = HasherHelper.Hasher(request.Password);

            if(!_walletDbContext.Users.Any(x => x.Email == request.Email && x.Password == passwordHash))
            {
                _logger.LogError("Username or Password is incorrect");
                return Task.FromResult(new ApiResponse<LoginSummary>(false, "Username or Password is incorrect", new LoginSummary()));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, request.Email)
                }),
                Expires = DateTime.Now.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = new Tokens { Token = tokenHandler.WriteToken(securityToken) };

            var loginSummary = new LoginSummary
            {
                Email = request.Email,
                Token = token.Token
            };

            return Task.FromResult(new ApiResponse<LoginSummary>(true, "Login successful", loginSummary));
        }
    }
}
