namespace IWantApp.Endpoints.Products;

public class ProductsPost
{
    public static string Template => "/products";
    public static string[] Methods => new string[]
    {
       HttpMethod.Post.ToString()
    };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(ProductRequest productRequest, HttpContext http, ApplicationDbContext context)
    {
        var UserId = http.User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;
        var category = await context.Categories.FirstOrDefaultAsync(p => p.Id == productRequest.CategoryId);
        var product = new Product(productRequest.Name, category, productRequest.Description, productRequest.HasStock, UserId, productRequest.Price);
        if (!product.IsValid)
        {
            return Results.ValidationProblem(product.Notifications.ConvertToProblemDetails());
        }
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        return Results.Created($"/products/{product.Id}", product.Id);
    }
}
