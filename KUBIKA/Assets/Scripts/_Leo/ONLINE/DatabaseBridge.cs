using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Kubika.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

namespace Kubika.Online
{
    public class DatabaseBridge : MonoBehaviour
    {
        IAmazonDynamoDB client;

        // Start is called before the first frame update
        void Start()
        {
            //requiered to communicate with DynamoDB database
            UnityInitializer.AttachToGameObject(gameObject);

            client = ClientFactory.ConfirmUserIdentity();
        }

        void UploadLevel(int kubiCode, string levelName, TextAsset levelFile)
        {
            // create a new upload request, input the relevant information
            var putItemRequest = new PutItemRequest
            {
                TableName = DynamoDBTableInfo.table_Name,
                Item = new Dictionary<string, AttributeValue>()
                {
                    { DynamoDBTableInfo.table_PPKey, new AttributeValue{ N = kubiCode.ToString()} },
                    { DynamoDBTableInfo.table_levelName, new AttributeValue { S = levelName } },
                    { DynamoDBTableInfo.table_Json, new AttributeValue { S = levelFile.ToString() }}
                }
            };

            // connect to dynamoDB database and pass the request information to upload the item
            client.PutItemAsync(putItemRequest, (result) =>
            {
                // if something goes wrong, debug the log message from AWS
                if(result.Exception != null)
                {
                    Debug.Log(result.Exception.Message);
                    return;
                }

                // else, the item has successfully been uploaded
                else Debug.Log(levelName + " has been uploaded successfully!");

            }, null);
        }

        void UpdateLevel()
        {

        }

        void DeleteLevel()
        {

        }

        void DownloadLevel(string kubiCode)
        {
            var getItemRequest = new GetItemRequest
            {
                TableName = DynamoDBTableInfo.table_Name,
                Key = new Dictionary<string, AttributeValue>()
                {
                    { DynamoDBTableInfo.table_PPKey, new AttributeValue{ N = kubiCode} }
                }
            };

            client.GetItemAsync(getItemRequest, (result)=>
            {
                if(result.Exception != null)
                {
                    Debug.Log(result.Exception.Message);
                    return;
                }

                else
                {
                    string levelName = "";
                    string jsonFile = "";

                    foreach (var keyValuePair in result.Response.Item)
                    {
                        if (keyValuePair.Key == DynamoDBTableInfo.table_levelName) levelName = keyValuePair.Value.S;
                        if (keyValuePair.Key == DynamoDBTableInfo.table_Json) jsonFile = keyValuePair.Value.S;
                    }

                    SaveAndLoad.instance.UserDownloadingLevel(levelName, jsonFile);
                }

            }, null);
        }

        private void ExportToJsonFile()
        {
            throw new NotImplementedException();
        }
    }
}