using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syncfusion.Blazor;

namespace OMSWebApp.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //Register Syncfusion license 
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzEwODgzQDMxMzgyZTMyMmUzMGFBc05Mc0N4VVdFeGdqQkRGNWVNaUk1dEphTTJ4U1YyRFc1ZXJYVCtiSEU9;MzEwODg0QDMxMzgyZTMyMmUzMEo3QXR0ZlVPeTJSWFE0WHVyOGxoWmRUdjZybWxaK0ZtdjJ0a0xBSzkzbWs9;MzEwODg1QDMxMzgyZTMyMmUzMEhTam0wellqVXhBVjdZeUJpbm5DcjRmbUduUGZ6L2g1QWVrYU9GUnNNc2c9;MzEwODg2QDMxMzgyZTMyMmUzMGg5N0ZaZytKY2ZFR1VXQWFKRFFRejhoZTkzTElFZTVUTTI0U3VLUFF4dEE9;MzEwODg3QDMxMzgyZTMyMmUzMFFxOUYyVGFabFY0TG1WOUN3L0JSbW9JYVpEZWFka3gvZnpocUFaS0h1Ulk9;MzEwODg4QDMxMzgyZTMyMmUzMFpWS2xCM2JXNHh0WjRyUXhINEtVLzNYQ0J5ODJEL2Z3VUhQQ3UxbkFxd1k9;MzEwODg5QDMxMzgyZTMyMmUzMEZFU05TSStMQllKN1phZjFkc0NzQ3E4cmZOaVp1ajlzUjd4dHYvVE1qTFE9;MzEwODkwQDMxMzgyZTMyMmUzMFgrNXVjdmxlaVcwQkpJdmtkQUhrZnZva0o1VzBZc1lWY0RKNlJJTlpQekE9;MzEwODkxQDMxMzgyZTMyMmUzMEpxaDd6M3N1VFpKRE9qbm5SZHd1czZXa3FDTTFDUFlBeEVVZXdwTEFDRFU9;NT8mJyc2IWhia31hfWN9Z2doYmF8YGJ8ampqanNiYmlmamlmanMDHmggNzIwMDwmPScTKjI7PDx9MDw+;MzEwODkyQDMxMzgyZTMyMmUzMFpjdld0MVFZcEo2NnJ5N01QSUY4UkFuTnVPZXN6cmhxcExpUld4QUpBbEU9");

            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSyncfusionBlazor();

            await builder.Build().RunAsync();
        }
    }
}
