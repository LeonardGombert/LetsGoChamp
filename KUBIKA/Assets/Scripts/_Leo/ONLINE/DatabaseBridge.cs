using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Collections;
using System.Collections.Generic;
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
            var request = new PutItemRequest
            {
                TableName = DynamoDBTableInfo.table_Name,
                Item = new Dictionary<string, AttributeValue>()
                {
                    { DynamoDBTableInfo.table_PPKey, new AttributeValue{ N = kubiCode.ToString()} },
                    { DynamoDBTableInfo.table_levelName, new AttributeValue { S = levelName } },
                    { DynamoDBTableInfo.table_Json, new AttributeValue { S = levelFile.ToString() }}
                }
            };
        }

        void UpdateLevel()
        {

        }

        void DeleteLevel()
        {

        }

        void DownloadLevel()
        {

        }
    }
}