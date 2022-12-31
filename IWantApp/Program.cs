using IWantApp.Domain.Users;
using IWantApp.Endpoints.Categories;
using IWantApp.Endpoints.Clients;
using IWantApp.Endpoints.Products;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

#pragma warning disable CS0618 // O tipo ou membro é obsoleto
builder.WebHost.UseSerilog((context, configuration) => {
    configuration
        .WriteTo.Console()
        .WriteTo.MSSqlServer(
            context.Configuration["ConnectionString:IWantDb"],
              sinkOptions: new MSSqlServerSinkOptions()
              {
                  AutoCreateSqlTable = true,
                  TableName = "LogAPI"
              });
});
#pragma warning restore CS0618 // O tipo ou membro é obsoleto

builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["ConnectionString:IWantDb"]);

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireLowercase = false;

}).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
      .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
      .RequireAuthenticatedUser()
      .Build();
    options.AddPolicy("EmployeePolicy", p =>
        p.RequireAuthenticatedUser().RequireClaim("EmployeeCode"));
    options.AddPolicy("Employee005Policy", p =>
        p.RequireAuthenticatedUser().RequireClaim("Employee005Policy", "005"));
    options.AddPolicy("CpfPolicy", p =>
        p.RequireAuthenticatedUser().RequireClaim("Cpf"));
});

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true, // Valida o tempo de expiração do token
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero, // Impede que exista tempo bonus apos expiração
        ValidIssuer = builder.Configuration["JwtBearerTokenSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtBearerTokenSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtBearerTokenSettings:SecretKey"]))
    };
});

builder.Services.AddScoped<QueryAllUsersWithClaimName>();
builder.Services.AddScoped<UserCreator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseAuthentication(); // 1
app.UseAuthorization(); // 2

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapMethods(CategoryPost.Template, CategoryPost.Methods, CategoryPost.Handle);
app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);
app.MapMethods(CategoryPut.Template, CategoryPut.Methods, CategoryPut.Handle);
app.MapMethods(EmployeesPost.Template, EmployeesPost.Methods, EmployeesPost.Handle);
app.MapMethods(EmployeesGetAll.Template, EmployeesGetAll.Methods, EmployeesGetAll.Handle);
app.MapMethods(TokenPost.Template, TokenPost.Methods, TokenPost.Handle);
app.MapMethods(ProductsPost.Template, ProductsPost.Methods, ProductsPost.Handle);
app.MapMethods(ProductsGetAll.Template, ProductsGetAll.Methods, ProductsGetAll.Handle);
app.MapMethods(ProductsGetShowCase.Template, ProductsGetShowCase.Methods, ProductsGetShowCase.Handle);
app.MapMethods(ClientPost.Template, ClientPost.Methods, ClientPost.Handle);

app.UseExceptionHandler("/error");
app.Map("error", (HttpContext http) =>
{
    var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;
    if(error != null)
    {
        if (error is SqlException)
            return Results.Problem(title: "DataBase out", statusCode: 500);
        if (error is BadHttpRequestException)
            return Results.Problem(title: "Error to convert data to other type. See all the information sent", statusCode: 500);
    }
    return Results.Problem(title: "An error ocurred", statusCode: 500);
});

app.Run();

