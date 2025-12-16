using FINTCS.Data;
using FINTCS.Repositories;
using FINTCS.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// MVC enable
builder.Services.AddControllersWithViews();

// Swagger Enable
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repository DI
builder.Services.AddScoped(typeof(ISocietyService<>), typeof(SocietyService<>));
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IAccountService, AccountService>();

// DB enable
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Enable Swagger Only in Development OR Always
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FINTCS API V1");
    c.RoutePrefix = "swagger"; // URL ? /swagger
});

app.UseHttpsRedirection();

// Static Files
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(@"D:\MemberPhotos"),
    RequestPath = "/MemberPhotos"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(@"D:\MemberSignatures"),
    RequestPath = "/MemberSignatures"
});

app.UseRouting();

// MVC Routes
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
