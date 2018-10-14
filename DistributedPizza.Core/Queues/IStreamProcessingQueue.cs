using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Entities;

namespace DistributedPizza.Core.Queues
{
    public interface IStreamProcessingQueue
    {
        Task QueueOrder(Order order);
        void RetrieveOrders(int? messagesToRetreive = null, CancellationToken? token = null);
    }
}
