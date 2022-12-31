using System.Drawing;

namespace IWantApp.Endpoints.Products;

public record class ProductRequest(string Name, Guid CategoryId, string Description, bool HasStock, bool Active, decimal Price);

