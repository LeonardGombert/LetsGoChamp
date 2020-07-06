﻿using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamoDBTest : MonoBehaviour
{
    public string identityPoolId;
    DynamoDBContext context;

    public string CognitoPoolRegion = RegionEndpoint.EUCentral1.SystemName;
    public string DynamoRegion = RegionEndpoint.EUCentral1.SystemName;

    private RegionEndpoint _CognitoPoolRegion { get { return RegionEndpoint.GetBySystemName(CognitoPoolRegion); } }
    private RegionEndpoint _DynamoRegion { get { return RegionEndpoint.GetBySystemName(DynamoRegion); } }

    private AWSCredentials _credentials;

    private static AmazonDynamoDBClient _ddbClient;

    private AWSCredentials credentials
    {
        get
        {
            if (_credentials == null)
                _credentials = new CognitoAWSCredentials(identityPoolId, _CognitoPoolRegion);
            return _credentials;
        }
    }

    protected AmazonDynamoDBClient client
    {
        get
        {
            if (_ddbClient == null)
            {
                _ddbClient = new AmazonDynamoDBClient(credentials, _DynamoRegion);
            }

            return _ddbClient;
        }
    }

    public Text resultText;
    public int bookID;

    // Start is called before the first frame update
    void Start()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
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
        context.SaveAsync(myBook, (result) => { if (result.Exception == null) resultText.text += @"book saved"; });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) PerformCreateOperation();
    }
}