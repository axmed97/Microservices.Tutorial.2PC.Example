using Coordinator.Enums;
using Coordinator.Models;
using Coordinator.Models.Context;
using Coordinator.Services.Abstract;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Coordinator.Services.Concrete
{
    public class TransactionService(TwoPhaseCommitContext _context,
        IHttpClientFactory _httpClientFactory) : ITransactionService
    {
        //private readonly TwoPhaseCommitContext _context;

        //public TransactionService(TwoPhaseCommitContext context)
        //{
        //    _context = context;
        //}

        HttpClient _orderHttpClient = _httpClientFactory.CreateClient("OrderApi");
        HttpClient _stockHttpClient = _httpClientFactory.CreateClient("StockApi");
        HttpClient _paymentHttpClient = _httpClientFactory.CreateClient("PaymentApi");

        public async Task<bool> CheckReadyServicesAsync(Guid transactionId)
              => (await _context.NodeStates
                .Where(x => x.TransactionId == transactionId)
                .ToListAsync()).TrueForAll(x => x.IsReady == ReadyType.Ready);

        public async Task<bool> CheckTransactionStateServicesAsync(Guid transactionId)
            => (await _context.NodeStates.Where(x => x.TransactionId == transactionId).ToListAsync())
            .TrueForAll(x => x.TransactionState == TransactionState.Done);

        public async Task CommitAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                .Include(x => x.Node).Where(x => x.TransactionId == transactionId).ToListAsync();

            foreach (var transactionNode in transactionNodes)
            {
                try
                {
                    var response = await (transactionNode.Node.Name switch
                    {
                        "Order.API" => _orderHttpClient.GetAsync("ready"),
                        "Stock.API" => _stockHttpClient.GetAsync("ready"),
                        "Payment.API" => _paymentHttpClient.GetAsync("ready")
                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());

                    transactionNode.TransactionState = result ? TransactionState.Done : TransactionState.Abort;
                }
                catch
                {
                    transactionNode.TransactionState = TransactionState.Abort;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Guid> CreateTransactionAsync()
        {
            Guid transactionId = Guid.NewGuid();

            var nodes = await _context.Node.ToListAsync();

            nodes.ForEach(node =>
            {
                node.NodeStates = new List<NodeState>()
                {
                    new(transactionId)
                    {
                        IsReady = Enums.ReadyType.Pending,
                        TransactionState = Enums.TransactionState.Pending
                    }
                };
            });

            await _context.SaveChangesAsync();
            return transactionId;
        }

        public async Task PrepeareServicesAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
               .Include(x => x.Node)
               .Where(x => x.TransactionId == transactionId)
               .ToListAsync();

            foreach (var node in transactionNodes)
            {
                try
                {
                    var response = await (node.Node.Name switch
                    {
                        "Order.API" => _orderHttpClient.GetAsync("ready"),
                        "Stock.API" => _stockHttpClient.GetAsync("ready"),
                        "Payment.API" => _paymentHttpClient.GetAsync("ready")
                    });

                    var result = bool.Parse(await response.Content.ReadAsStringAsync());

                    node.IsReady = result ? Enums.ReadyType.Ready : Enums.ReadyType.Unready;

                }
                catch (Exception)
                {
                    node.IsReady = Enums.ReadyType.Unready;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task RollbackAsync(Guid transactionId)
        {
            var transactionNodes = await _context.NodeStates
                .Include(x => x.Node)
                .Where(x => x.TransactionId == transactionId).ToListAsync();

            foreach(var transactionNode in transactionNodes)
            {
                try
                {
                    if(transactionNode.TransactionState == TransactionState.Done)
                        _ = await (transactionNode.Node.Name switch
                        {
                            "Order.API" => _orderHttpClient.GetAsync("ready"),
                            "Stock.API" => _stockHttpClient.GetAsync("ready"),
                            "Payment.API" => _paymentHttpClient.GetAsync("ready")
                        });

                    transactionNode.TransactionState = TransactionState.Abort;
                }
                catch
                {
                    transactionNode.TransactionState = TransactionState.Abort;
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
