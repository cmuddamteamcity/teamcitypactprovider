using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace PactProviderTests.ProviderStates
{
    public class ProviderStateMiddleware
    {
        private readonly RequestDelegate _next;
 
        private readonly IDictionary<string, Action> _providerStates;
        public ProviderStateMiddleware(RequestDelegate next)
        {
            _next = next;

            _providerStates = new Dictionary<string, Action>
            {
                { "There is a new user", CreateUser }
            };
        }

        public async Task Invoke(HttpContext context)
        {
            if(context.Request.Path.Value == "/provider-states")
            {
                ProviderStatesRequest(context);
                await context.Response.WriteAsync(string.Empty);
            }
            else
            {
                await _next(context);
            }
        }

        private void ProviderStatesRequest(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if(context.Request.Method.ToUpper() == HttpMethod.Post.ToString().ToUpper() && context.Request.Body != null)
            {
                string jsonRequestBody = string.Empty;
                    using(var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                    {
                    jsonRequestBody = reader.ReadToEnd();
                    }
                    var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                if(providerState != null && string.IsNullOrEmpty(providerState.State) && _providerStates.ContainsKey(providerState.State))
                {
                    _providerStates[providerState.State].Invoke();
                }
            }
        }

        private void CreateUser()
        {

        }
    }
}