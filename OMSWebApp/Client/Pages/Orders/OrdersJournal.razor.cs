using OMSWebApp.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OMSWebApp.Client.Pages.Orders
{
    public partial class OrdersJournal
    {
        private List<Order> orders;

        protected override async Task OnInitializedAsync()
        {
            //#if DEBUG
            //        await Task.Delay(10000);
            //#endif

            orders = await client.GetFromJsonAsync<List<Order>>("api/orders");
        }
    }
}
