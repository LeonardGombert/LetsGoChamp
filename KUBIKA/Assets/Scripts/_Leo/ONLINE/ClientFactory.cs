using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon;

namespace Kubika.Online
{
    public struct AmazonCognito
    {
        public static string identityPoolId = "eu-central-1:f8990c10-6786-4c01-adc7-6614e9da700b";
        public static RegionEndpoint region = RegionEndpoint.EUCentral1;
    }

    public class ClientFactory
    {
        public static AmazonDynamoDBClient GetUserIdentity()
        {
            var credentials = new CognitoAWSCredentials(AmazonCognito.identityPoolId, AmazonCognito.region);
            var client = new AmazonDynamoDBClient(credentials, AmazonCognito.region);

            return client;
        }
    }
}