namespace IWantApp.Endpoints.Products;

public record class ProductResponse(string Name, String CategoryName, string Description, bool HasStock, bool Active, decimal Price);
