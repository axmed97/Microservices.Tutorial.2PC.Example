var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/ready", () =>
{
    Console.WriteLine("Stock Service Ready");
    return true;
});


app.MapGet("/commit", () =>
{
    Console.WriteLine("Stock Service Commited");
    return true;
});

app.MapGet("/rollback", () =>
{
    Console.WriteLine("Stock Service Rollback");
});


app.Run();
