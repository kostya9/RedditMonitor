using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using KPI.RedditMonitor.Api.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KPI.RedditMonitor.Api.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IAmazonCognitoIdentityProvider _cognito;
        private readonly IOptions<CognitoOptions> _options;
        private readonly ILogger<UsersController> _log;

        public UsersController(IAmazonCognitoIdentityProvider cognito, IOptions<CognitoOptions> options, ILogger<UsersController> log)
        {
            _cognito = cognito;
            _options = options;
            _log = log;
        }

        [HttpPost("signin")]
        public async Task<ActionResult<LoginResponse>> Signin(LoginRequest request)
        {
            var auth = new AdminInitiateAuthRequest
            {
                ClientId = _options.Value.ClientId,
                UserPoolId = _options.Value.UserPoolId,
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
            };

            auth.AuthParameters.Add("USERNAME", request.Username);
            auth.AuthParameters.Add("PASSWORD", request.Password);

            try
            {
                var authResponse = await _cognito.AdminInitiateAuthAsync(auth);

                if (authResponse.AuthenticationResult == null)
                {
                    _log.LogError("Got response @{response}, error", authResponse);
                    return BadRequest();
                }

                return new LoginResponse
                {
                    Token = authResponse.AuthenticationResult.AccessToken,
                    ExpiresAt = DateTime.UtcNow.AddSeconds(authResponse.AuthenticationResult.ExpiresIn)
                };
            }
            catch(UserNotFoundException e)
            {
                _log.LogInformation(e, $"User ${request.Username} does not exist");
                return NotFound();
            }
            catch(UserNotConfirmedException e)
            {
                _log.LogInformation(e, $"User #{request.Username} is not confirmed yet");
                return BadRequest("Please confirm your account (or contact us at webmaster@reddit.knine.xyz)");
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(SignupRequest request)
        {
            if (!request.Password.Equals(request.ConfirmPassword))
                return BadRequest("Password and confirmation should be equal");

            var auth = new SignUpRequest
            {
                ClientId = _options.Value.ClientId,
                Password = request.Password,
                Username = request.Username
            };

            var emailAttribute = new AttributeType
            {
                Name = "email",
                Value = request.Email
            };
            auth.UserAttributes.Add(emailAttribute);

            var authResponse = await _cognito.SignUpAsync(auth);
            
            if(authResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                _log.LogError("Got response @{response}, error", authResponse);
                return BadRequest();
            }

            return Ok();

        }
    }

    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class SignupRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}
