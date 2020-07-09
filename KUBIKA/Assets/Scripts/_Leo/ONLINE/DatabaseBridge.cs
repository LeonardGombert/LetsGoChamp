using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Kubika.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Kubika.Online
{
    // functions are called from inside the COmmunity Levels scene
    public class DatabaseBridge : MonoBehaviour
    {
        IAmazonDynamoDB client;

        public Dropdown uploadLevelDropdown;
        public Dropdown downloadLevel;

        // Start is called before the first frame update
        void Start()
        {
            //requiered to communicate with DynamoDB database
            UnityInitializer.AttachToGameObject(gameObject);

            //create a new client to make function calls
            client = ClientFactory.ConfirmUserIdentity();
        }

        // load one of the levels from the Biome 1 folder
        TextAsset GetLevelFromUserFolder(string targetLevel)
        {
            string folder = Application.dataPath + "/Resources/MainLevels/01_Plains";
            string fileName = targetLevel + ".json";

            string path = Path.Combine(folder, fileName);

            if (File.Exists(path))
            {
                TextAsset target = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                Debug.Log("Foud a level, returning to upload routine");
                return target;
            }

            else
            {
                Debug.Log("Couldn't find that leve");
                return null;
            }
        }

        // when the player wants to upload his level directly from the editor
        public void UploadLevelFromEditor()
        {

        }

        // when the player is uploading a level from the levelsList on the community page
        // called by Upload Button onClick Event
        public void UploadLevelFromList()
        {
            // translate the name of the level in the dropdown list to a levelFile
            TextAsset levelFile = GetLevelFromUserFolder(uploadLevelDropdown.captionText.text);

            LevelEditorData level = JsonUtility.FromJson<LevelEditorData>(levelFile.ToString());

            string kubiCode = level.Kubicode;
            string levelName = level.levelName;

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
                if (result.Exception != null)
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

        // called by Download Button onCLick Event
        public void DownloadLevel(string kubiCode)
        {
            var getItemRequest = new GetItemRequest
            {
                TableName = DynamoDBTableInfo.table_Name,
                Key = new Dictionary<string, AttributeValue>()
                {
                    { DynamoDBTableInfo.table_PPKey, new AttributeValue{ N = kubiCode} }
                }
            };

            client.GetItemAsync(getItemRequest, (result) =>
            {
                if (result.Exception != null)
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

                    // call the save and load instance to convert the received information into a useable JSON file
                    SaveAndLoad.instance.UserDownloadingLevel(levelName, jsonFile);
                }

            }, null);
        }
    }
}