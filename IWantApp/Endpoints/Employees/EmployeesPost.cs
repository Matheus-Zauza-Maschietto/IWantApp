using IWantApp.Domain.Users;
using IWantApp.Endpoints.Clients;

namespace IWantApp.Endpoints.Employees;

public class EmployeesPost
{
    public static string Template => "/employee";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;
    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(EmployeeRequest employeeRequest,  UserCreator userCreator, HttpContext http)
    {
        var userId = http.User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;
        var userClaims = new List<Claim>
        {
            new Claim("EmployeeCode",  employeeRequest.employeeCode),
            new Claim("Name", employeeRequest.Name),
            new Claim("CreatedBy",  userId),
        };
        (IdentityResult identity, string userId) result = await userCreator.Create(employeeRequest.Email, employeeRequest.Password, userClaims);

        if (!result.identity.Succeeded)
            return Results.ValidationProblem(result.identity.Errors.ConvertToProblemDetails());

        return Results.Created($"/employee/{result.userId}", result.userId);
    }
}
