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
        public Dropdown downloadLevelDropdown;

        public List<string> uploadDropdownOptions = new List<string>();
        List<string> downloadDropdownOptions = new List<string>();

        public List<UnityEngine.Object> assets = new List<UnityEngine.Object>();

        // Start is called before the first frame update
        void Start()
        {
            //requiered to communicate with DynamoDB database
            UnityInitializer.AttachToGameObject(gameObject);

            //create a new client to make function calls
            client = ClientFactory.ConfirmUserIdentity();

            OnLoadScene();
        }

        void OnLoadScene()
        {
            PopulateDropdownList();
            RequestIDs();
        }

        // get information concerning the last id that was uploaded to the server. This happens in the KUBIKA_information table
        private void RequestIDs()
        {
            var request = new GetItemRequest
            {
                ConsistentRead = true,
                TableName = DynamoDBTableInfo.infoTable_Name,
                Key = new Dictionary<string, AttributeValue>() 
                {
                    { DynamoDBTableInfo.infoTable_PPKey, new AttributeValue{ N = DynamoDBTableInfo.infoTable_key}  }
                }
            };

            client.GetItemAsync(request, (result) =>
            {
                if (result.Exception != null)
                {
                    Debug.Log(result.Exception.Message);
                    return;
                }

                else
                {
                    string jsonFile = "";

                    foreach (var keyValuePair in result.Response.Item)
                    {
                        if (keyValuePair.Key == DynamoDBTableInfo.infoTable_info) jsonFile = keyValuePair.Value.S;
                    }

                    // create a DynamoDBInfo file from the json information stored in the Table
                    DynamoDBInfo info = JsonUtility.FromJson<DynamoDBInfo>(jsonFile);

                    //info.lastIdUsed;
                }

            }, null);
        }

        // call this when dropdown buttons are on screen to populate them
        private void PopulateDropdownList()
        {
            uploadDropdownOptions = UserLevelFiles.GetUserLevelNames();
            uploadLevelDropdown.ClearOptions();
            uploadLevelDropdown.AddOptions(uploadDropdownOptions);
        }

        // when the player wants to upload his level directly from the editor
        public void UploadLevelFromEditor()
        {
            string json = SaveAndLoad.instance.SaveToPublish();


        }

        // when the player is uploading a level from the levelsList on the community page
        // called by Upload Button onClick Event
        public void UploadLevelFromList()
        {
            // translate the name of the level in the dropdown list to a levelFile
            string json = GetLevelFromUserFolder(uploadLevelDropdown.captionText.text);

            LevelEditorData level = new LevelEditorData();
            level = JsonUtility.FromJson<LevelEditorData>(json);

            string levelName = level.levelName;
            string kubicode = GenerateKubiCode();

            // create a new upload request, input the relevant information
            var putItemRequest = new PutItemRequest
            {
                TableName = DynamoDBTableInfo.table_Name,
                Item = new Dictionary<string, AttributeValue>()
                {
                    { DynamoDBTableInfo.table_PPKey, new AttributeValue{ N =  kubicode } },
                    { DynamoDBTableInfo.table_level, new AttributeValue { S = levelName } },
                    { DynamoDBTableInfo.table_Json, new AttributeValue { S = json } }
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
                else Debug.Log(DynamoDBTableInfo.table_retrievedLevel + " has been uploaded successfully!");

            }, null);

            DynamoDBTableInfo.CleanInfo();
        }

        private string GenerateKubiCode()
        {
            return "";
        }

        // load one of the levels from the Biome 1 folder
        string GetLevelFromUserFolder(string targetLevel)
        {
            Debug.Log(targetLevel);

            string folder = Application.persistentDataPath + "/UserLevels";
            string fileName = targetLevel + ".json";

            string path = Path.Combine(folder, fileName);

            if (File.Exists(path))
            {
                string target = File.ReadAllText(path);

                Debug.Log("Foud target, returning to upload routine");
                return target;
            }

            else
            {
                Debug.Log("Couldn't find that level");
                return null;
            }
        }

        void UpdateLevel()
        {

        }

        void DeleteLevel()
        {

        }

        // called by Download Button onCLick Event
        public void DownloadLevel()
        {
            string kubiCode = downloadLevelDropdown.captionText.text;

            var getItemRequest = new GetItemRequest
            {
                ConsistentRead = true,
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
                    foreach (var keyValuePair in result.Response.Item)
                    {
                        if (keyValuePair.Key == DynamoDBTableInfo.table_level) DynamoDBTableInfo.table_retrievedLevel = keyValuePair.Value.S;
                        if (keyValuePair.Key == DynamoDBTableInfo.table_Json) DynamoDBTableInfo.table_retrievedJson = keyValuePair.Value.S;
                    }

                    Debug.Log("Downloading " + DynamoDBTableInfo.table_retrievedLevel);

                    // call the save and load instance to convert the received information into a useable JSON file
                    SaveAndLoad.instance.UserDownloadingLevel(DynamoDBTableInfo.table_retrievedLevel, DynamoDBTableInfo.table_retrievedJson);
                }

            }, null);

            DynamoDBTableInfo.CleanInfo();
        }
    }
}