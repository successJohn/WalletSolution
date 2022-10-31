using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WalletSolution.Application.Features.Users.Command;
using WalletSolution.Helpers;

namespace WalletSolution.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly ISender _sender;

        public UserController(ISender sender)
        {
            _sender = sender;
        }



        /// <summary>
        /// Signs up a new user
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [HttpPost("SignUp")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SignUp(SignUpUserCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var newUser = await _sender.Send(command);
            return Ok(newUser);
        }

        /// <summary>
        /// Logs in a signed up user
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [HttpPost("Login")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Login(LoginUserCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var loginResult = await _sender.Send(command);
            return Ok(loginResult);
        }

        /// <summary>
        /// Change the PIN of NGN wallet
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [HttpPut("changeNGNPin")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ChangeNGNPin(ChangeNGNWalletPinCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _sender.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Change the PIN of USD wallet
        /// </summary>
        /// <returns></returns>
        [Produces("application/json")]
        [HttpPut("changeUSDPin")]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ChangeUSDPin(ChangeUSDWalletPinCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _sender.Send(command);
            return Ok(result);
        }
    }
}
