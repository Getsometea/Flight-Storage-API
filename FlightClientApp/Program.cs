var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("flightsApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7270/");
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Flights}/{action=Search}/{id?}");

app.Run();
