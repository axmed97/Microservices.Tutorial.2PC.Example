var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/ready", () =>
{
    Console.WriteLine("Payment Service Ready");
    return true;
});


app.MapGet("/commit", () =>
{
    Console.WriteLine("Payment Service Commited");
    return true;
});

app.MapGet("/rollback", () =>
{
    Console.WriteLine("Payment Service Rollback");
});


app.Run();
