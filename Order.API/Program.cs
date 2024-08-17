var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/ready", () =>
{
    Console.WriteLine("Order Service Ready");
    return true;
});


app.MapGet("/commit", () =>
{
    Console.WriteLine("Order Service Commited");
    return true;
});

app.MapGet("/rollback", () =>
{
    Console.WriteLine("Order Service Rollback");
});


app.Run();
