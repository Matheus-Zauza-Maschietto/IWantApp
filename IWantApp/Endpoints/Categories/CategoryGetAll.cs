namespace IWantApp.Endpoints.Categories;

public class CategoryGetAll
{
    public static string Template => "/Categories";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;
    [Authorize(Policy = "EmployeePolicy")]
    public static IResult Action(ApplicationDbContext context)
    {
        var categories = context.Categories.ToList();
        var response = categories.Select(p => new CategoryResponse(p.Id, p.Name, p.Active ));
        return Results.Ok(response);
    }
}
