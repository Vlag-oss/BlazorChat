using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using BlazorChat.Server.Models;
using BlazorChat.Shared.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.IdentityModel.Tokens;
using User = BlazorChat.Server.Models.User;

namespace BlazorChat.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly BlazorChatContext _context;
    private readonly IConfiguration _configuration;

    public UserController(ILogger<UserController> logger, BlazorChatContext context, IConfiguration configuration)
    {
        _logger = logger;
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("loginuser")]
    public async Task<ActionResult<User>> LoginUser(User user)
    {
        user.Password = Utility.Encrypt(user.Password!);
        var loggedInUser = await _context.Users
            .Where(u => u.EmailAddress == user.EmailAddress && u.Password == user.Password)
            .FirstOrDefaultAsync();

        if (loggedInUser is not null)
        {
            var claimEmail = new Claim(ClaimTypes.Email, loggedInUser.EmailAddress!);
            var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, loggedInUser.UserId.ToString());

            var claimIdentity = new ClaimsIdentity(new[] { claimEmail, claimNameIdentifier }, "serverAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimIdentity);

            await HttpContext.SignInAsync(claimsPrincipal, GetAuthenticationProperties());
        }

        return await Task.FromResult(loggedInUser) ?? throw new InvalidOperationException();
    }

    [HttpGet("getcurrentuser")]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        var currentUser = new User();

        if (User.Identity is { IsAuthenticated: true })
        {
            currentUser = await _context.Users.Where(u => u.EmailAddress == User.FindFirstValue(ClaimTypes.Email))
                .FirstOrDefaultAsync();

            if (currentUser is null)
            {
                currentUser = new User
                {
                    UserId = _context.Users.Max(user => user.UserId) + 1,
                    EmailAddress = User.FindFirstValue(ClaimTypes.Email),
                    Password = Utility.Encrypt(currentUser!.EmailAddress!),
                    Source = "EXTL"
                };

                _context.Users.Add(currentUser);
                await _context.SaveChangesAsync();
            }
        }

        return await Task.FromResult(currentUser);
    }

    [HttpGet("logoutuser")]
    public async Task<ActionResult<string>> LogoutUser()
    {
        await HttpContext.SignOutAsync();
        return "Success";
    }

    [HttpGet("GoogleSignIn")]
    public async Task GoogleSignIn()
    {
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, GetAuthenticationProperties());
    }

    [HttpPost("createaccount")]
    public async Task<ActionResult> CreateAccount(User user)
    {
        var emailAddressExists = _context.Users.FirstOrDefault(u => u.EmailAddress == user.EmailAddress);

        if (emailAddressExists == null)
        {
            user.Password = Utility.Encrypt(user.Password!);
            user.Source = "APPL";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        return Ok();
    }

    private static AuthenticationProperties GetAuthenticationProperties()
    {
        return new AuthenticationProperties
        {
            ExpiresUtc = DateTime.Now.AddMinutes(10),
            RedirectUri = "/profile"
        };
    }

    [HttpGet("notauthorized")]
    public IActionResult NotAuthorized()
    {
        return Unauthorized();
    }

    private string GenerateJwtToken(User user)
    {
        var secretKey = _configuration["JWTSettings:SecretKey"];
        var key = Encoding.ASCII.GetBytes(secretKey);

        var claimEmail = new Claim(ClaimTypes.Email, user.EmailAddress!);
        var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString());

        var claimsIdentity = new ClaimsIdentity(new[] { claimEmail, claimNameIdentifier }, "serverAuth");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    [HttpPost("authenticatejwt")]
    public async Task<ActionResult<AuthenticationResponse>> AuthenticateJWT(AuthenticationRequest authenticationRequest)
    {
        var token = string.Empty;

        authenticationRequest.Password = Utility.Encrypt(authenticationRequest.Password);
        var loggedInUser = await _context.Users
            .Where(u => u.EmailAddress == authenticationRequest.EmailAddress && u.Password == authenticationRequest.Password)
            .FirstOrDefaultAsync();

        if (loggedInUser != null)
        {
            token = GenerateJwtToken(loggedInUser);
        }
        return await Task.FromResult(new AuthenticationResponse() { Token = token });
    }

    [HttpPost("getuserbyjwt")]
    public async Task<ActionResult<User>> GetUserByJWT([FromBody]string jwtToken)
    {
        try
        {
            var secretKey = _configuration["JWTSettings:SecretKey"];
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            
            var principle = tokenHandler.ValidateToken(jwtToken, tokenValidationParameters, out var securityToken);
            var jwtSecurityToken = (JwtSecurityToken)securityToken;

            if (JwtIsValid())
            {
                var userId = principle.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _context.Users.Where(u => u.UserId == Convert.ToInt64(userId)).FirstOrDefaultAsync();
                return result;
            }

            bool JwtIsValid() => jwtSecurityToken != null &&
                                 jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                     StringComparison.InvariantCultureIgnoreCase);
        }
        catch (Exception e)
        {
            throw e;
        }

        return null;
    }
}