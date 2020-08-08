using Amazon;
using Amazon.Runtime;
using Amazon.DynamoDBv2;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2.Model;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;

namespace Kubika.Online
{
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