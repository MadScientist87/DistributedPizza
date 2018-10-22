using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Entities;
using Microsoft.Extensions.Logging;

namespace DistributedPizza.Core.Queues
{
    public interface IStreamProcessingQueue
    {
        void QueueOrder(Order order);
        void RetrieveOrders(ILogger _logger,int? messagesToRetreive = null, CancellationToken? token = null);
    }
}
