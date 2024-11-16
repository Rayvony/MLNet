using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using MLNetProyecto.Entidades.EF;
using MLNetProyecto.Logica;
using MLNetProyecto.Web.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MLNetProyectoDbContextConnection") ?? throw new InvalidOperationException("Connection string 'MLNetProyectoDbContextConnection' not found.");

builder.Services.AddDbContext<MlnetProyectoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MLNetProyectoDbContextConnection")));

builder.Services.AddDbContext<MLNetProyectoDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<MLNetProyectoDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<MLContext>();
builder.Services.AddScoped<MlnetProyectoContext>();
builder.Services.AddScoped<MLNetProyectoDbContext>();
builder.Services.AddScoped<IMLNetLogica, MLNetLogica>();
builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
});
    

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
