﻿using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Kubika.Saving;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

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

        public List<Object> assets = new List<Object>();

        public int numberOfLevelsToGet;
        public Text levelname1, levelname2, levelname3, kubiCode1, kubiCode2, kubiCode3;
        
        public DynamoDBInfo ids;
        public DynamoReceivedInfo info;
        // Start is called before the first frame update
        void Start()
        {
            //requiered to communicate with DynamoDB database
            UnityInitializer.AttachToGameObject(gameObject);

            //create a new client to make function calls
            client = ClientFactory.ConfirmUserIdentity();

            //OnLoadScene();

            StartCoroutine(GetRandomLevels());
        }

        public IEnumerator GetRandomLevels()
        {
            ids = RequestIDs();

            yield return new WaitForSeconds(2f);

            Debug.Log("Selecting Ids");

            List<int> idsToGet = SelectRandomLevels(ids, numberOfLevelsToGet);

            yield return new WaitForSeconds(2f);

            for (int i = 0; i < idsToGet.Count; i++)
            {
                DynamoReceivedInfo receivedInfo = GetLevelById(idsToGet[i]);

                yield return new WaitForSeconds(2f);

                if (i == 0) levelname1.text = receivedInfo.levelName;
                if (i == 0) kubiCode1.text = receivedInfo.kubicode;

                if (i == 1) levelname2.text = receivedInfo.levelName;
                if (i == 1) kubiCode2.text = receivedInfo.kubicode;

                if (i == 2) levelname3.text = receivedInfo.levelName;
                if (i == 2) kubiCode3.text = receivedInfo.kubicode;

                yield return new WaitForSeconds(2f);
            }

            yield return null;
        }

        private DynamoReceivedInfo GetLevelById(int id)
        {
            var getLevelRequest = new GetItemRequest
            {
                ConsistentRead = true,
                TableName = DynamoDBTableInfo.testingTable_tableName,
                Key = new Dictionary<string, AttributeValue>()
                {
                    { DynamoDBTableInfo.testingTable_pKey, new AttributeValue{ S = id.ToString()} }
                }
            };

            client.GetItemAsync(getLevelRequest, (result) =>
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
                        if (keyValuePair.Key == DynamoDBTableInfo.testingTable_levelName) info.levelName = keyValuePair.Value.S;
                        if (keyValuePair.Key == DynamoDBTableInfo.testingTable_pKey) info.kubicode = keyValuePair.Value.S;
                    }
                }
            }, null);

            Debug.Log("Retrived name is " + info.levelName);
            Debug.Log("Retrived kubicode is " + info.kubicode);

            return info;
        }

        public List<int> SelectRandomLevels(DynamoDBInfo ids, int amountOfLevels)
        {
            List<int> randomLevels = new List<int>();

            for (int i = 0; i < amountOfLevels; i++)
            {
                int randomLevelToGet = ids.listOfIndexes[Random.Range(0, ids.listOfIndexes.Count)];
                ids.listOfIndexes.Remove(randomLevelToGet);

                randomLevels.Add(randomLevelToGet);
            }

            return randomLevels;
        }


        #region // BASIC SETUP METHODS
        void OnLoadScene()
        {
            PopulateDropdownList();
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
        #endregion

        #region // UPLOAD USER GENERATED CONTENT LEVELS
        // when the player is uploading a level from the levelsList on the community page
        // called by Upload Button onClick Event
        public void UploadLevelFromList()
        {
            StartCoroutine(UploadLevel());
        }

        IEnumerator UploadLevel()
        {
            // translate the name of the level in the dropdown list to a levelFile
            string json = GetLevelFromUserFolder(uploadLevelDropdown.captionText.text);

            LevelEditorData level = new LevelEditorData();
            level = JsonUtility.FromJson<LevelEditorData>(json);

            DynamoDBInfo requested = RequestIDs();

            yield return new WaitForSeconds(.5f);

            string levelName = level.levelName;
            string kubicode = GenerateKubiCode(requested);

            yield return new WaitForSeconds(.5f);

            // create a new upload request, input the relevant information
            var putItemRequest = new PutItemRequest
            {
                TableName = DynamoDBTableInfo.testingTable_tableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                    { DynamoDBTableInfo.testingTable_pKey, new AttributeValue{ S =  kubicode } },
                    { DynamoDBTableInfo.testingTable_levelName, new AttributeValue { S = levelName } },
                    { DynamoDBTableInfo.testingTable_jsonFile, new AttributeValue { S = json } }
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

            yield return null;
        }
        #endregion

        #region // ADDITIONAL UPLOAD METHODS
        public void UploadBaseLevels()
        {
            StartCoroutine(UploadAllGameLevel());
        }

        IEnumerator UploadAllGameLevel()
        {
            foreach (Object level in assets)
            {
                LevelEditorData levelFile = JsonUtility.FromJson<LevelEditorData>(level.ToString());

                var uploadRequest = new PutItemRequest
                {
                    TableName = DynamoDBTableInfo.levelsTable_tableName,
                    Item = new Dictionary<string, AttributeValue>()
                    {
                        { DynamoDBTableInfo.levelsTable_pKey, new AttributeValue{ S = levelFile.Kubicode } },
                        { DynamoDBTableInfo.levelsTable_levelName, new AttributeValue{ S = levelFile.levelName } },
                        { DynamoDBTableInfo.levelsTable_jsonFile, new AttributeValue{ S = level.ToString() } },
                    }
                };

                client.PutItemAsync(uploadRequest, (result) =>
                {
                    // if something goes wrong, debug the log message from AWS
                    if (result.Exception != null)
                    {
                        Debug.Log(result.Exception.Message);
                        return;
                    }

                    // else, the item has successfully been uploaded
                    else Debug.Log(levelFile.levelName + " has been uploaded successfully!");

                }, null);

                DynamoDBTableInfo.CleanInfo();
            }

            yield return null;
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

        // use to generate a new code based on the last existing one
        private string GenerateKubiCode(DynamoDBInfo requested)
        {
            int index = ++requested.lastIdUsed;

            requested.listOfIndexes.Add(index);
            requested.lastIdUsed = index;

            Debug.Log("new is " + requested.lastIdUsed);

            UpdateInfoFile(requested);

            return index.ToString();
        }

        public void ClientHandler(PutItemRequest putRequest = null, GetItemRequest getRequest = null)
        {
            if (putRequest != null)
            {
                client.PutItemAsync(putRequest, (result) =>
                {
                    // if something goes wrong, debug the log message from AWS
                    if (result.Exception != null)
                    {
                        Debug.Log(result.Exception.Message);
                        return;
                    }

                    // else, the item has successfully been uploaded
                    else Debug.Log(DynamoDBTableInfo.testingTable_retrievedLevel + " has been uploaded successfully!");

                }, null);

                DynamoDBTableInfo.CleanInfo();
            }

            if (getRequest != null)
            {
                DynamoDBInfo info = new DynamoDBInfo();

                client.GetItemAsync(getRequest, (result) =>
                {
                    if (result.Exception != null)
                    {
                        Debug.Log(result.Exception.Message);
                        return;
                    }

                    else
                    {
                        Debug.Log("Extracting an info file");

                        string jsonFile = "";

                        foreach (var keyValuePair in result.Response.Item)
                        {
                            if (keyValuePair.Key == DynamoDBTableInfo.infoTable_jsonFile) jsonFile = keyValuePair.Value.S;
                        }

                        if (jsonFile == "" || jsonFile == null) CreateIDsFile();

                        // create a DynamoDBInfo file from the json information stored in the Table
                        DynamoDBInfo copy = JsonUtility.FromJson<DynamoDBInfo>(jsonFile);

                        info.backupListOfIndexes = copy.backupListOfIndexes;
                        info.listOfIndexes = copy.listOfIndexes;
                        info.lastIdUsed = copy.listOfIndexes[copy.listOfIndexes.Count - 1];

                        Debug.Log("Last used is " + info.lastIdUsed);
                    }

                }, null);
            }
        }
        #endregion

        #region // GET SET INFO TABLE IDs
        // get information concerning the last id that was uploaded to the server. This happens in the KUBIKA_information table
        private DynamoDBInfo RequestIDs()
        {
            DynamoDBInfo info = new DynamoDBInfo();

            var getIdsRequest = new GetItemRequest
            {
                ConsistentRead = true,
                TableName = DynamoDBTableInfo.infoTable_tableName,
                Key = new Dictionary<string, AttributeValue>()
                {
                    { DynamoDBTableInfo.infoTable_pKey, new AttributeValue{ N = DynamoDBTableInfo.infoTable_key} }
                }
            };

            client.GetItemAsync(getIdsRequest, (result) =>
            {
                if (result.Exception != null)
                {
                    Debug.Log(result.Exception.Message);
                    return;
                }

                else
                {
                    Debug.Log("Extracting an info file");

                    string jsonFile = "";

                    foreach (var keyValuePair in result.Response.Item)
                    {
                        if (keyValuePair.Key == DynamoDBTableInfo.infoTable_jsonFile) jsonFile = keyValuePair.Value.S;
                    }

                    if (jsonFile == "" || jsonFile == null) CreateIDsFile();

                    // create a DynamoDBInfo file from the json information stored in the Table
                    DynamoDBInfo copy = JsonUtility.FromJson<DynamoDBInfo>(jsonFile);

                    info.backupListOfIndexes = copy.backupListOfIndexes;
                    info.listOfIndexes = copy.listOfIndexes;
                    info.lastIdUsed = copy.listOfIndexes[copy.listOfIndexes.Count - 1];

                    Debug.Log("Last used is " + info.lastIdUsed);
                }

            }, null);

            return info;
        }

        private void CreateIDsFile()
        {
            Debug.Log("No file was found, creating a new info file");

            DynamoDBInfo info = new DynamoDBInfo();
            info.listOfIndexes = new List<int>();
            info.backupListOfIndexes = new List<int>();

            info.lastIdUsed = 000000000000000000; // 18 zeros
            info.listOfIndexes.Add(info.lastIdUsed);
            info.backupListOfIndexes = info.listOfIndexes;

            string json = JsonUtility.ToJson(info);

            var request = new PutItemRequest
            {
                TableName = DynamoDBTableInfo.infoTable_tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { DynamoDBTableInfo.infoTable_pKey, new AttributeValue { N = DynamoDBTableInfo.infoTable_key} },
                    { DynamoDBTableInfo.infoTable_jsonFile, new AttributeValue{ S = json} }
                }
            };

            client.PutItemAsync(request, (result) =>
            {
                if (result.Exception != null)
                {
                    Debug.Log(result.Exception.Message);
                    return;
                }
                // else, the item has successfully been uploaded
                else Debug.Log("Info file has been uploaded successfully!");

            }, null);
        }

        private void UpdateInfoFile(DynamoDBInfo file)
        {
            string json = JsonUtility.ToJson(file);

            var UpdateRequest = new UpdateItemRequest
            {
                TableName = DynamoDBTableInfo.infoTable_tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { DynamoDBTableInfo.infoTable_pKey, new AttributeValue { N = DynamoDBTableInfo.infoTable_key } }
                },

                AttributeUpdates = new Dictionary<string, AttributeValueUpdate>
                {
                    {
                        DynamoDBTableInfo.infoTable_jsonFile, new AttributeValueUpdate
                        {
                            Action = AttributeAction.PUT,
                            Value = new AttributeValue {S = json }
                        }
                    }
                }
            };

            client.UpdateItemAsync(UpdateRequest, (result) =>
            {
                if (result.Exception != null)
                {
                    Debug.Log(result.Exception.Message);
                    return;
                }

                else Debug.Log("The item has been updated");
            });
        }

        #endregion

        #region // GET, UPDATE, DELETE USER GENERATED CONTENT
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
                TableName = DynamoDBTableInfo.testingTable_tableName,
                Key = new Dictionary<string, AttributeValue>()
                {
                    { DynamoDBTableInfo.testingTable_pKey, new AttributeValue{ S = kubiCode} }
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
                        if (keyValuePair.Key == DynamoDBTableInfo.testingTable_levelName) DynamoDBTableInfo.testingTable_retrievedLevel = keyValuePair.Value.S;
                        if (keyValuePair.Key == DynamoDBTableInfo.testingTable_jsonFile) DynamoDBTableInfo.testingTable_retrievedJson = keyValuePair.Value.S;
                    }

                    Debug.Log("Downloading " + DynamoDBTableInfo.testingTable_retrievedLevel);

                    // call the save and load instance to convert the received information into a useable JSON file
                    SaveAndLoad.instance.UserDownloadingLevel(DynamoDBTableInfo.testingTable_retrievedLevel, DynamoDBTableInfo.testingTable_retrievedJson);
                }

            }, null);

            DynamoDBTableInfo.CleanInfo();
        }
        #endregion

    } //class
} //namespace