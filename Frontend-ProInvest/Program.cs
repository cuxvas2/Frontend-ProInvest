using Frontend_ProInvest.Services.Backend;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
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
app.UseSession();
app.UseRouting();
app.UseAuthorization();


app.MapControllerRoute(
    name: "Admin",
    pattern: "admin",
    defaults: new { controller = "Admin", action = "InicioSesion" });
app.MapControllerRoute(
    name: "AdministrarBancos",
    pattern: "admin/bancos",
    defaults: new { controller = "Admin", action = "AdministrarBancos" });
app.MapControllerRoute(
    name: "Menu",
    pattern: "admin/menu",
    defaults: new { controller = "Menu", action = "Index" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Use(async (context, next) =>
{
    if (!context.Request.Path.Value.StartsWith("/admin"))
    {
        var cookieOptions = new CookieOptions
        {
            Path = "/admin",
        };
        context.Response.Cookies.Delete("tokenAdministrador", cookieOptions);
    }
    await next.Invoke();
});
app.Run();
