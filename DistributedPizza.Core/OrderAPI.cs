using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DistributedPizza.Core.Data.Models;
using RestSharp;

namespace DistributedPizza.Core
{
    public static class OrderAPI
    {

        public static void UpdateOrder(OrderDTO order)
        {
            var client = new RestClient("http://localhost/distributedpizza.web.api/");
            var request = new RestRequest("/api/orders/update", Method.POST);

            request.RequestFormat = DataFormat.Json;
            request.AddBody(order);

            var response = client.ExecuteAsync(request);
        }

        public static async Task<RestResponse> ExecuteAsync(this RestClient client, RestRequest request)
        {
            TaskCompletionSource<IRestResponse> taskCompletion = new TaskCompletionSource<IRestResponse>();
            RestRequestAsyncHandle handle = client.ExecuteAsync(request, r => taskCompletion.SetResult(r));
            return (RestResponse)(await taskCompletion.Task);
        }
    }
}
