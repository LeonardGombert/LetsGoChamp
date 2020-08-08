using Amazon;
using Amazon.Runtime;
using Amazon.DynamoDBv2;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2.Model;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;

namespace Kubika.Online
{
    public struct AmazonCognito
    {
        public static string identityPoolId = "eu-central-1:f8990c10-6786-4c01-adc7-6614e9da700b";
        public static string appClientId = "9db7pkdnppvrlui6sackifg6d";
        public static string userPoolId = "eu-central-1_UXwaae0M4";
        public static string userPoolName = "UXwaae0M4";
        public static string authenticatedUserId = "12345";
        public static RegionEndpoint cognitoIdentityRegion = RegionEndpoint.EUCentral1;
    }

    public class ClientFactory
    {
        public static AmazonDynamoDBClient GetUserIdentity()
        {
            var credentials = new CognitoAWSCredentials(AmazonCognito.identityPoolId, AmazonCognito.cognitoIdentityRegion);
            var client = new AmazonDynamoDBClient(credentials, AmazonCognito.cognitoIdentityRegion);

            return client;
        }

        private static AmazonCognitoIdentityProviderClient _cognitoIdentityProvider = null;
        public static AmazonCognitoIdentityProviderClient CognitoIdentityProvider 
        {
            get {
                if (_cognitoIdentityProvider == null)
                {
                    var config = new AmazonCognitoIdentityProviderConfig();
                    config.RegionEndpoint = AmazonCognito.cognitoIdentityRegion;
                    _cognitoIdentityProvider = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), config);
                }
                return _cognitoIdentityProvider;
            }
        }
    }
}