using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;

namespace Kubika.Online
{
    public class ClientFactory
    {
        public static IAmazonDynamoDB ConfirmUserIdentity()
        {
            //AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;

            var credentials = new CognitoAWSCredentials(DatabaseInfo.identityPoolId, DatabaseInfo.region);
            var client = new AmazonDynamoDBClient(credentials, DatabaseInfo.region);

            return client;
        }
    }
}
