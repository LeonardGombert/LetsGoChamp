using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;

public class DynamoTest2 : MonoBehaviour
{
    public string identityPoolId;

    public string CognitoPoolRegion = RegionEndpoint.EUCentral1.SystemName;
    public string DynamoRegion = RegionEndpoint.EUCentral1.SystemName;

    private RegionEndpoint _CognitoPoolRegion { get { return RegionEndpoint.GetBySystemName(CognitoPoolRegion); } }
    private RegionEndpoint _DynamoRegion { get { return RegionEndpoint.GetBySystemName(DynamoRegion); } }

    private AWSCredentials _credentials;

    private static AmazonDynamoDBClient _ddbClient;

    public Text resultText;
    public TextAsset textAsset;
    private AWSCredentials credentials
    {
        get
        {
            if (_credentials == null)
                _credentials = new CognitoAWSCredentials(identityPoolId, _CognitoPoolRegion);
            return _credentials;
        }
    }
    protected IAmazonDynamoDB client
    {
        get
        {
            if (_ddbClient == null)
            {
                _ddbClient = new AmazonDynamoDBClient(credentials, _DynamoRegion);
            }
            _ddbClient.Config.Validate();
            _ddbClient.DescribeTableAsync("Bookshelf", new CreateTableRequest());

            return _ddbClient;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        Run();
    }

    private void Run()
    {
        Table table = Table.LoadTable(client, new TableConfig("Bookshelf"));

        string jsonText = textAsset.ToString();
        Document item = Document.FromJson(jsonText);

        table.PutItemAsync(item, (result) => { if (result.Exception == null) resultText.text += @"book saved"; });
    }
}
