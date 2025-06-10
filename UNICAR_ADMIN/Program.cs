using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UNICAR_ADMIN.Data;
using UNICAR_ADMIN.Models.Renta;
using UNICAR_ADMIN.Servicios.LocalImage_Services;
using UNICAR_ADMIN.Servicios.Proveedores_Services;
using UNICAR_ADMIN.Servicios.Vehiculos_Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//contexto para el
builder.Services.AddDbContext<RentaDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
//     .AddRoles<IdentityRole>()                         // <— esto es clave
//    .AddEntityFrameworkStores<ApplicationDbContext>(); ;
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(opts => {
        opts.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();            // <-- monta las páginas Razor de Identity

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();  
builder.Services.AddScoped<IRepositorio_Vehiculo, Repositorio_Vehiculo>();
builder.Services.AddScoped<IRepositorio_Proveedores, Repositorio_Proveedores>();
builder.Services.AddScoped<ILocalImageService, LocalImageService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
// 1) Primero autenticación
app.UseAuthentication();
// 2) Luego autorización
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
