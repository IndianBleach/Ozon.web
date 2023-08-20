using Grpc.Net.Client;

namespace Ozon.Api.GrpcServiceProviders
{
    public class AuthorizatonServiceProvider
    {
        private string url = "https://localhost:5001";

        public AuthorizatonServiceProvider()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");

            channel.ope

            var client = new Greet.GreeterClient(channel);
        }


        // open
        // request
        // response
        // close
    }
}
