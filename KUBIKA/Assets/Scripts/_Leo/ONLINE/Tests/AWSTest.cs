using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AWSTest : MonoBehaviour
{
    public int cubeId;
    public Text resultText;
    public string identityPoolId = "eu-central-1:f8990c10-6786-4c01-adc7-6614e9da700b";

    CognitoAWSCredentials credentials;
    IAmazonDynamoDB client;
    DynamoDBContext context;
    IDynamoDBContext _context;
    DynamoDBOperationConfig _operationConfig;

    public TextAsset myLevelBruther;

    // Start is called before the first frame update
    void Start()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;

        GetUserCredentials();
        DescribeTable(); 
        SaveLevel();
    }

    private void GetUserCredentials()
    {
        credentials = new CognitoAWSCredentials(identityPoolId, RegionEndpoint.EUCentral1);
        client = new AmazonDynamoDBClient(credentials, RegionEndpoint.EUCentral1);
        context = new DynamoDBContext(client);

        Debug.Log("My Received credentials are : " + credentials.AccountId);
    }

    private void CreateAnObject()
    {
        context.SaveAsync<TextAsset>(myLevelBruther, (result) =>
        {
            if (result.Exception == null)
                resultText.text += @"book saved";
        });
    }

    public void SaveLevel()
    {
        _operationConfig = new DynamoDBOperationConfig
        {
            OverrideTableName = "KUBIKA_Testing"
        };

        context.SaveAsync(Document.FromJson(myLevelBruther.ToString()), _operationConfig, null);
    }

    private void DescribeTable()
    {
        resultText.text += ("\n*** Retrieving table information ***\n");
        var request = new DescribeTableRequest
        {
            TableName = @"KUBIKA_Testing"
        };
        client.DescribeTableAsync(request, (result) =>
        {
            if (result.Exception != null)
            {
                resultText.text += result.Exception.Message;
                Debug.Log(result.Exception);
                return;
            }
            var response = result.Response;
            TableDescription description = response.Table;
            resultText.text += ("Name: " + description.TableName + "\n");
            resultText.text += ("# of items: " + description.ItemCount + "\n");
            resultText.text += ("Provision Throughput (reads/sec): " +
            description.ProvisionedThroughput.ReadCapacityUnits + "\n");
            resultText.text += ("Provision Throughput (reads/sec): " +
            description.ProvisionedThroughput.WriteCapacityUnits + "\n");
        }, null);
    }
}
