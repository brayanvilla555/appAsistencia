using AspNetCoreHero.ToastNotification;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using proyectoAsistencia.Data;
using proyectoAsistencia.Service;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext") ?? throw new InvalidOperationException("Connection string 'AppDbContext' not found.")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSingleton<EventoPonenteService>();
// Add services to the container.
builder.Services.AddControllersWithViews();


//configuracion de las alertas
//https://codewithmukesh.com/blog/toast-notifications-in-aspnet-core/
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 3;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
});


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "UserLoginCookie";
        options.LoginPath = "/Home/Index";
        options.AccessDeniedPath = "/Login/AccessDenied";
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdministradorPolicy", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("EstudiantePolicy", policy => policy.RequireRole("Estudiante"));
    options.AddPolicy("DocentePolicy", policy => policy.RequireRole("Docente"));
});
builder.Services.AddSession();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    var context = service.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    BDInicializar.Inicializar(context);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();




app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); 


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
