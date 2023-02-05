
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromHours(1);
});
builder.Services.AddMemoryCache();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();