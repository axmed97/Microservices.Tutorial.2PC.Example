using Coordinator.Models.Context;
using Coordinator.Services.Abstract;
using Coordinator.Services.Concrete;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TwoPhaseCommitContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddHttpClient("OrderApi", client => client.BaseAddress = new Uri("https://localhost:7278/"));
builder.Services.AddHttpClient("StockApi", client => client.BaseAddress = new Uri("https://localhost:7022/"));
builder.Services.AddHttpClient("PaymentApi", client => client.BaseAddress = new Uri("https://localhost:7008/"));

builder.Services.AddSingleton<ITransactionService, TransactionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/create-order-transaction", async (ITransactionService _transactionService) =>
{
    // Phase 1 - Prepare
    var transactionId = await _transactionService.CreateTransactionAsync();

    await _transactionService.PrepeareServicesAsync(transactionId);

    bool transactionState = await _transactionService.CheckReadyServicesAsync(transactionId);

    if (transactionState)
    {
        // Phase 2 - Commit
        await _transactionService.CommitAsync(transactionId);
        transactionState =  await _transactionService.CheckTransactionStateServicesAsync(transactionId);
    }

    if (!transactionState)
        await _transactionService.RollbackAsync(transactionId);

});

app.Run();
