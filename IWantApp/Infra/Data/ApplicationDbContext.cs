using IWantApp.Domain.Orders;

namespace IWantApp.Infra.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Ignore<Notification>();
        builder.Entity<Product>().Property(p => p.Description).HasMaxLength(255).IsRequired();
        builder.Entity<Product>().Property(p => p.Name).IsRequired();
        builder.Entity<Category>().Property(p => p.Name).IsRequired();
        builder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(10, 2)").IsRequired();
        builder.Entity<Order>().Property(p => p.ClientId).IsRequired();
        builder.Entity<Order>().Property(p => p.DeliveryAddress).IsRequired();

        builder.Entity<Order>().HasMany(p => p.Products).WithMany(p => p.Orders).UsingEntity(p => p.ToTable("OrderProducts"));
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
    {
        configuration.Properties<string>().HaveMaxLength(100);

    }
}
