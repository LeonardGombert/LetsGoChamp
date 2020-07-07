using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
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
        //DescribeTable();
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
        // Define item attributes
        Dictionary<string, AttributeValue> attributes = new Dictionary<string, AttributeValue>();
        /*
        // Author is hash-key
        attributes["kubikaID"] = new AttributeValue { N = "1"};
        // Title is range-key
        attributes["Title"] = new AttributeValue { S = "The Adventures of Tom Sawyer" };
        // Other attributes
        attributes["Year"] = new AttributeValue { N = "1876" };
        attributes["Setting"] = new AttributeValue { S = "Missouri" };
        attributes["Pages"] = new AttributeValue { N = "275" };
        attributes["Genres"] = new AttributeValue
        {
            SS = new List<string> { "Satire", "Folk", "Children's Novel" }
        };*/

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
        var request = new PutItemRequest
        {
            TableName = @"KUBIKA_Testing",
            Item = attributes
        };

        var _operationConfig = new DynamoDBOperationConfig
        {
            OverrideTableName = @"KUBIKA_Testing"
        };

        //string item = Document.FromJson(myLevelBruther.ToString());
        attributes["kubikaID"] = new AttributeValue { N = "01" };
        //attributes["levelName"] = new AttributeValue { S = myLevelBruther.ToString() };

        client.PutItemAsync("KUBIKA_Testing", attributes, (result) =>
        {
            if (result.Exception != null)
            {
                resultText.text += result.Exception.Message;
                Debug.Log(result.Exception);
                return;
            }

            var response = result.Response;
            PutItemResponse description = new PutItemResponse();
            var metaData = response.ResponseMetadata.Metadata.ToString();
            resultText.text += ("Name: " + metaData + "\n");
        }, null);
        //context.SaveAsync(request, _operationConfig, null);

        //////////////////////////////////////////////////////////////////////////////////////////////////////////
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
