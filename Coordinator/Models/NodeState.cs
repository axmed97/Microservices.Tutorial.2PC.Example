using Coordinator.Enums;

namespace Coordinator.Models
{
    public record NodeState(Guid TransactionId)
    {
        public Guid Id { get; set; }
        /// <summary>
        /// First State ready or not
        /// </summary>
        public ReadyType IsReady { get; set; }
        /// <summary>
        /// Second State ready or not
        /// </summary>
        public TransactionState TransactionState { get; set; }
        public Node Node { get; set; }
    }
}
