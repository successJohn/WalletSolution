using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WalletSolution.Application.Features.Wallet.Command;
using WalletSolution.Application.Features.Wallet.Queries;
using WalletSolution.Helpers;

namespace WalletSolution.Controllers
{
    [ApiController]
    [Route("wallet")]
    public class WalletController : ControllerBase
    {
        private readonly ISender _sender;

        public WalletController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Funds a USD wallet
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [Authorize]
        [HttpPut("fundUSDWallet")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> FundUSDWallet(FundUSDWalletCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var fundResult = await _sender.Send(command);
            return fundResult.Status ? Ok(fundResult) : Unauthorized(fundResult);
        }


        /// <summary>
        /// Funds a NGN wallet
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [Authorize]
        [HttpPut("fundNGNWallet")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> FundNGNWallet(FundNGNWalletCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var fundResult = await _sender.Send(command);
            return fundResult.Status ? Ok(fundResult) : Unauthorized(fundResult);
        }

        /// <summary>
        /// Withdraw from a NGN wallet
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [Authorize]
        [HttpPut("withdrawNGNWallet")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> WithdrawNGNWallet(WithdrawNGNCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var fundResult = await _sender.Send(command);
            return fundResult.Status ? Ok(fundResult) : Unauthorized(fundResult);
        }

        /// <summary>
        /// Withdraw from a USD wallet
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [Authorize]
        [HttpPut("withdrawUSDWallet")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> WithdrawUSDWallet(WithdrawUSDCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var fundResult = await _sender.Send(command);
            return fundResult.Status ? Ok(fundResult) : Unauthorized(fundResult);
        }

        /// <summary>
        /// Gets the account balance of a NGN wallet
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [Authorize]
        [HttpGet("balanceNGNWallet")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> BalanceNGNWallet()
        {
            var query = new NGNBalanceEnquiryQuery();
            var fundResult = await _sender.Send(query);
            return fundResult.Status ? Ok(fundResult) : Unauthorized(fundResult);
        }

        /// <summary>
        /// Gets the account balance of a USD wallet
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [Authorize]
        [HttpGet("balanceUSDWallet")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> BalanceUSDWallet()
        {
            var query = new USDBalanceEnquiryQuery();
            var fundResult = await _sender.Send(query);
            return fundResult.Status ? Ok(fundResult) : Unauthorized(fundResult);
        }
    }
}
