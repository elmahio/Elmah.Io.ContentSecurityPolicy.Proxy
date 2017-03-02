using System;
using System.IO;
using Elmah.Io.Client;
using Elmah.Io.Client.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Elmah.Io.ContentSecurityPolicy.Proxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables().Build();

            var apiKey = config["ApiKey"];
            var logId = new Guid(config["LogId"]);

            var client = ElmahioAPI.Create(apiKey);

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .ConfigureLogging(l => l.AddConsole(config.GetSection("Logging")))
                .ConfigureServices(s => s.AddRouting())
                .Configure(app =>
                {
                    app.UseRouter(r =>
                        r.MapPost("reportOnly", async (request, response, routeData) =>
                        {
                            using (var streamReader = new StreamReader(request.Body))
                            {
                                var body = streamReader.ReadToEnd();
                                await client.Messages.CreateAndNotifyAsync(
                                    logId, new CreateMessage
                                    {
                                        Title = "Content-Security-Policy report",
                                        Severity = Severity.Information.ToString(),
                                        Detail = body,
                                        Hostname = request.Headers["Referer"],
                                    });
                            }
                        }));
                })
                .Build();

            host.Run();
        }
    }
}