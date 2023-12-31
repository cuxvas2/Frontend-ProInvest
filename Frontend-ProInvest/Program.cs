using Frontend_ProInvest.Services.Backend;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Soporte para consultar el API
builder.Services.AddHttpClient();
builder.Services.AddScoped<IUsuarios, Usuarios>();
builder.Services.AddScoped<IAdministrador, Administrador>();

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
app.MapControllerRoute(
    name: "Admin",
    pattern: "admin",
    defaults: new { controller = "InicioSesion", action = "Index" });

app.Run();
