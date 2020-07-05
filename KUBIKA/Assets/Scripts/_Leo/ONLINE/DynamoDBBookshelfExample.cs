using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamoDBBookshelfExample : MonoBehaviour
{
    public string identityPoolID = "eu-central-1:05b3f8f2-0056-45f1-b1a8-73c6ddce974b";

    public AmazonDynamoDBClient client; //The class provides methods for creating, describing, updating, and deleting tables
    public DynamoDBContext Context; //Context adds a further layer of abstraction over the client and enables you to use additional functionality like the Persistance Model.

    public int bookID;
    public Text resultText;

    private void Awake()
    {
        UnityInitializer.AttachToGameObject(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        var credentials = new CognitoAWSCredentials(identityPoolID, RegionEndpoint.EUCentral1); // set the credentials for the desired pool
        client = new AmazonDynamoDBClient(credentials, RegionEndpoint.EUCentral1);
        Context = new DynamoDBContext(client);

        PerformCreateOperation();
        resultText.text += ("\n*** Retrieving table information ***\n");
        var request = new DescribeTableRequest
        {
            TableName = @"ProductCatalog"
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


    [DynamoDBTable("ProductCatalog")]
    public class Book
    {
        [DynamoDBHashKey] // Hash key.
        public int Id { get; set; }
        [DynamoDBProperty]
        public string Title { get; set; }
        [DynamoDBProperty]
        public string ISBN { get; set; }
        [DynamoDBProperty("Authors")] // Multi-valued (set type) attribute.
        public List<string> BookAuthors { get; set; }
    }

    private void PerformCreateOperation()
    {
        Book myBook = new Book
        {
            Id = ++bookID,
            Title = "object persistence-AWS SDK for.NET SDK-Book 1001",
            ISBN = "111-1111111001",
            BookAuthors = new List<string> { "Author 1", "Author 2" },
        };
        // Save the book.
        Context.SaveAsync(myBook, (result) => {
            if (result.Exception == null)
                resultText.text += @"book saved";
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            PerformCreateOperation();
    }
}
