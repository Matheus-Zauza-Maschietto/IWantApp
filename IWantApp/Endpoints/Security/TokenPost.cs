namespace IWantApp.Endpoints.Employees;

public class TokenPost
{
    public static string Template => "/token";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;
    [AllowAnonymous]
    public static IResult Action(LoginRequest loginRequest,  UserManager<IdentityUser> userManager, IConfiguration configuration, ILogger<TokenPost> log, IWebHostEnvironment environment)
    {
        log.LogInformation("Getting token");
        log.LogWarning("warning");
        log.LogError("error");

        var user = userManager.FindByEmailAsync(loginRequest.Email).Result;
        if(user == null)
            Results.BadRequest();
        
        if(!userManager.CheckPasswordAsync(user, loginRequest.Password).Result)
            Results.BadRequest();

        var claims = userManager.GetClaimsAsync(user).Result;
        var Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, loginRequest.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            });
        foreach (var item in claims)
        {
            Subject.AddClaim(item);
        }
        
        var key = Encoding.ASCII.GetBytes(configuration["JwtBearerTokenSettings:SecretKey"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = Subject,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Audience = configuration["JwtBearerTokenSettings:Audience"],
            Issuer = configuration["JwtBearerTokenSettings:Issuer"],
            Expires = environment.IsDevelopment() || environment.IsStaging() ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMinutes(2)
            //Expires = DateTime.UtcNow.AddMinutes(2) // Define que o tempo de expiração do token será de 30 segundos
        };

        var TokenHandler = new JwtSecurityTokenHandler();
        var token = TokenHandler.CreateToken(tokenDescriptor);
        return Results.Ok(new
        {
            token = TokenHandler.WriteToken(token)
        });
      
    }
}
