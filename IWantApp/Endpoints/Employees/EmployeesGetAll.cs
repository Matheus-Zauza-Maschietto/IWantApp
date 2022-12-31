namespace IWantApp.Endpoints.Employees;

public class EmployeesGetAll
{
    public static string Template => "/employee";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;
    public static async Task<IResult> Action(int? page, int? rows, QueryAllUsersWithClaimName query)
    {
        if (page == null || rows == null || page <= 0 || rows <= 0 || rows > 10)
        {
            return Results.BadRequest("Parametros incorretos, page e rows devem ser maiores que 0 e rows deve ter um valor maximo de 10.");
        }
        var result = await query.Execute(page.Value, rows.Value);
        return Results.Ok(result);
    }
}
