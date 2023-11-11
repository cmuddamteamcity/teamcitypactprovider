using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using PactProviderTests.ProviderStates;
using System.Reflection;
using System.Text.Json.Serialization;
using WebApi.Controllers;
using WebApi.Helpers;
using WebApi.Services;

namespace PactProviderTests
{
    public class ProviderPactTestsBase
    {
        static ProviderPactTestsBase()
        {
            var builder = WebApplication.CreateBuilder(new string[] {"",""});

            // add services to DI container
            {
                var services = builder.Services;
                var env = builder.Environment;

                services.AddDbContext<DataContext>();
                services.AddCors();
                services.AddControllers().AddJsonOptions(x =>
                {
                    // serialize enums as strings in api responses (e.g. Role)
                    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

                    // ignore omitted parameters on models to enable optional params (e.g. User update)
                    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });
                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

                var controllerAssembly = Assembly.GetAssembly(typeof(UsersController));
                services.AddMvc().AddApplicationPart(controllerAssembly).AddControllersAsServices();

                services.Configure<KestrelServerOptions>(options =>
                {
                    options.AllowSynchronousIO = true;
                });

                // configure DI for application services
                 services.AddScoped<IUserService, UserService>();
            }

            var app = builder.Build();

            // configure HTTP request pipeline
            {
                // global cors policy
                app.UseCors(x => x
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

                // global error handler
                app.UseMiddleware<ErrorHandlerMiddleware>();
                app.UseMiddleware<ProviderStateMiddleware>();
                app.MapControllers();
            }

            app.RunAsync("http://localhost:4000");
        }
    }
}
