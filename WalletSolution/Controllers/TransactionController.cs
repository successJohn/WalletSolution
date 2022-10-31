using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WalletSolution.Application.Features.Transactions.Queries;
using WalletSolution.Helpers;

namespace WalletSolution.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionController : ControllerBase
    {
        private readonly ISender _sender;

        public TransactionController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Gets the transaction history of a user
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> BalanceUSDWallet()
        {
            var query = new TransactionHistoryQuery();
            var transactionResult = await _sender.Send(query);
            return transactionResult.Succeeded ? Ok(transactionResult) : Unauthorized(transactionResult);
        }
    }
}
