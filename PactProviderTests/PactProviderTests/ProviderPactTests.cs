using PactNet.Verifier;
using Woolworths.Pact.Provider;
using Xunit.Abstractions;

namespace PactProviderTests.ProviderStates
{
    public class ProviderPactTests : ProviderPactTestsBase
    {
        private readonly PactVerifier _verifier;
        private readonly string _pactBrokerUri;
        private readonly string _pactBrokerToken;
        private readonly string _version;
        private readonly string _branch;
        private readonly string _pact_url;
        private readonly PactVerifierOptions _options;

        public ProviderPactTests(ITestOutputHelper output)
        {
            _options = new PactVerifierOptions
            {
                ProviderName = ProviderPactTestsConstants.ProviderName,
                ConsumerName = ProviderPactTestsConstants.ConsumerName,
                PactPath = Path.Combine(@"./Pacts", "UserConsumer-UserProvider.json"),
                ProviderUri = "http://localhost:4000"
            };

            _verifier = new PactVerifier(new PactVerifierConfig
            {
                Outputters = new[]
                {
                    new XUnitOutput(output)
                }
            });

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PACTBROKER_PACT_URI")))
            {
                // check if triggered by pact webhook for contract content changed even, need to run verification only for changed contract
                // ensure the required env variables for pactUri and pactConsumer are passed in parameters of the webhook
                _pactBrokerUri = Environment.GetEnvironmentVariable("PACTBROKER_PACT_URI");
                _pactBrokerToken = Environment.GetEnvironmentVariable("PACTBROKER_PACT_TOKEN");
                _version = Environment.GetEnvironmentVariable("PROVIDER_VERSION");
                _branch = Environment.GetEnvironmentVariable("BRANCH");
                _pact_url = Environment.GetEnvironmentVariable("PACT_URL");
            }
        }

        [Fact]
        public async Task EnsureCreateUser()
        {
            // Act & Assert

            if (_pactBrokerUri == null)
            {
                Console.WriteLine("**** _pactBrokerUri value is null ****");
                    _verifier
                    .ServiceProvider(_options.ProviderName, new Uri(_options.ProviderUri))
                    .WithFileSource(new FileInfo(_options.PactPath))
                    .WithProviderStateUrl(new Uri(_options.ProviderUri + "/provider-states"))
                    .Verify();
            }
          /*  Console.WriteLine("_pact_url value is ---> " + _pact_url);
            if (_pact_url == null)
            {
                Console.WriteLine("**** _pact_url value is null ****");
                    _verifier
                    .ServiceProvider(_options.ProviderName, new Uri(_options.ProviderUri))
                    .WithPactBrokerSource(new Uri(_pactBrokerUri), (options) =>
                    {
                        options.ConsumerVersionSelectors(
                            new ConsumerVersionSelector { MainBranch = true },
                            new ConsumerVersionSelector { MatchingBranch = true }
                        )
                        .ProviderBranch(_branch)
                        .PublishResults(_version, (results) =>
                        {
                            results.ProviderBranch(_branch);
                        }).TokenAuthentication(_pactBrokerToken);
                    })
                    .WithProviderStateUrl(new Uri(_options.ProviderUri + "/provider-states"))
                    .Verify();
            }
            else
            {
                Console.WriteLine("#### _pact_url value is not null ####");
                    _verifier
                    .ServiceProvider(_options.ProviderName, new Uri(_options.ProviderUri))
                    .WithUriSource(new Uri(_pact_url), (options) =>
                    {
                        options.PublishResults(_version, results =>
                        {
                            results.ProviderBranch(_branch);
                        }).TokenAuthentication(_pactBrokerToken);
                    })
                    .WithProviderStateUrl(new Uri(_options.ProviderUri + "/provider-states"))
                    .Verify();
            } */
        }
    }
}