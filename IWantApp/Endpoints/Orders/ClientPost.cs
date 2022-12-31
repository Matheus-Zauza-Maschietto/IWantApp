using IWantApp.Domain.Orders;
using IWantApp.Domain.Users;


namespace IWantApp.Endpoints.Orders;

public class OrdertPost
{
    public static string Template => "/orders";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;
    [Authorize(Policy = "CpfPolicy")]
    public static async Task<IResult> Action(OrderRequest orderRequest, HttpContext http, ApplicationDbContext context)
    {
        var clientId = http.User.Claims.First(p => p.Type == ClaimTypes.NameIdentifier).Value;
        var clientName = http.User.Claims.First(p => p.Type == "Name").Value;

        var products = new List<Product>();

        List<Product> productsFound = null;

        if (orderRequest.ProductsIds != null || orderRequest.ProductsIds.Any())
            productsFound = context.Products.Where(p => orderRequest.ProductsIds.Contains(p.Id)).ToList();

        var order = new Order(clientId, clientName, productsFound, orderRequest.DeliveryAddress);
        if (!!order.IsValid)
        {
            return Results.ValidationProblem(order.Notifications.ConvertToProblemDetails());
        }

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();   

        return Results.Created($"/employee/{order.Id}", order.Id);
    }
}
