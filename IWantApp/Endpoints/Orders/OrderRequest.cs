namespace IWantApp.Endpoints.Orders;

public record class OrderRequest(List<Guid> ProductsIds, string DeliveryAddress);
