﻿using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Kubika.Online.Tests
{
    public class AWSTest : MonoBehaviour
    {
        public int cubeId;
        public Text resultText;
        public string identityPoolId = "eu-central-1:f8990c10-6786-4c01-adc7-6614e9da700b";

        CognitoAWSCredentials credentials;
        IAmazonDynamoDB client;
        DynamoDBContext context;

        public string kubiCode;
        public string levelName;
        public TextAsset levelFile;

        public string receivedId;
        public string receivedName;
        public string receivedFile;

        // Start is called before the first frame update
        void Start()
        {
            /*UnityInitializer.AttachToGameObject(this.gameObject);
            AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;*/

            GetUserCredentials();
            //DescribeTable();
            SaveLevel();
            DownloadLevel();
        }

        private void GetUserCredentials()
        {
            credentials = new CognitoAWSCredentials(identityPoolId, RegionEndpoint.EUCentral1);
            client = new AmazonDynamoDBClient(credentials, RegionEndpoint.EUCentral1);
            context = new DynamoDBContext(client);

            Debug.Log("My Received credentials are : " + credentials.AccountId);
        }

        public void SaveLevel()
        {
            var request = new PutItemRequest
            {
                TableName = "KUBIKA_Testing",
                Item = new Dictionary<string, AttributeValue>()
            {
                { "kubikaID", new AttributeValue { N = kubiCode } },
                { "levelName", new AttributeValue { S = levelName } },
                { "levelFile", new AttributeValue { S = levelFile.ToString() } }
            }
            };

            client.PutItemAsync(request);/*client.PutItemAsync(request, (result) =>
            {
                if (result.Exception != null)
                {
                    resultText.text += result.Exception.Message;
                    Debug.Log(result.Exception);
                    return;
                }

                PutItemResponse description = result.Response;
                var metaData = description.Attributes.ToString();
                resultText.text += ("kubikaID: " + metaData + "\n");

            }, null);*/

            #region test shiz
            //////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Define item attributes
            /* Dictionary<string, AttributeValue> attributes = new Dictionary<string, AttributeValue>();

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
            #endregion
        }

        private void DownloadLevel()
        {
            Dictionary<string, AttributeValue> targetKeys = new Dictionary<string, AttributeValue>();

            var request = new GetItemRequest
            {
                TableName = "KUBIKA_Testing",
                Key = new Dictionary<string, AttributeValue>()
            {
                { "kubikaID", new AttributeValue { N = kubiCode } }
            },
            };

            var response = client.GetItemAsync(request);

            foreach (var keyValuePair in response.Result.Item)
            {
                if (keyValuePair.Key == "kubikaID") receivedId = "Worl" + keyValuePair.Value.N;
                if (keyValuePair.Key == "levelName") receivedName = keyValuePair.Value.S;
                if (keyValuePair.Key == "levelFile") receivedFile = keyValuePair.Value.S;
            }

            /*client.GetItemAsync(request, (result) =>
            {
            // use this to get error and warning debugs
            if (result.Exception != null)
                {
                    resultText.text += result.Exception.Message;
                    Debug.Log(result.Exception);
                    return;
                }

            //var is of type KeyValuePair
            foreach (var keyValuePair in result.Response.Item)
                {
                //if what you're reaading it the levelFile
                if (keyValuePair.Key == "kubikaID") receivedId = "Worl" + keyValuePair.Value.N;
                    if (keyValuePair.Key == "levelName") receivedName = keyValuePair.Value.S;
                    if (keyValuePair.Key == "levelFile") receivedFile = keyValuePair.Value.S;
                }

            }, null);*/
        }

        private void DescribeTable()
        {/*
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
            }, null);*/
        }
    }
}