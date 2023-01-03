using RustServerStatus.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLazyCache();
builder.Services.AddTransient<IServerQueryService, ServerQueryService>();
builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");

app.Run();