using Amazon;
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
using Kubika.Game;

namespace Kubika.Online.Test
{
    // functions are called from inside the COmmunity Levels scene
    public class DatabaseBridge : MonoBehaviour
    {
        private static DatabaseBridge _instance;
        public static DatabaseBridge instance { get { return _instance; } }

        IAmazonDynamoDB client;

        public Dropdown uploadLevelDropdown;
        public Dropdown downloadLevelDropdown;

        public List<string> uploadDropdownOptions = new List<string>();
        List<string> downloadDropdownOptions = new List<string>();

        public List<Object> assets = new List<Object>();

        public int numberOfLevelsToGet;
        public Text levelname, kubiCode;

        public DynamoReceivedInfo info;

        public GameObject listPrefab;
        public Transform listTransform;

        void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            //requiered to communicate with DynamoDB database
            //UnityInitializer.AttachToGameObject(gameObject);

            //create a new client to make function calls
            client = ClientFactory.ConfirmUserIdentity();

            //StartCoroutine(GetRandomLevels());
        }

        #region // LIST RANDOM LEVELS
        public void WESH()
        {
            StartCoroutine(LevelsManager.instance.PlayCommunityLevel(DatabaseInfo.userContent_retrievedLevel));
        }

        IEnumerator GetRandomLevels()
        {
            DynamoDBInfo ids = RequestIDs();
            DynamoReceivedInfo receivedInfo = new DynamoReceivedInfo();

            while (ids.listOfIndexes.Count == 0) yield return null;

            List<int> idsToGet = SelectRandomLevels(ids, numberOfLevelsToGet);

            for (int i = 0; i < idsToGet.Count; i++)
            {
                receivedInfo.ClearNames();
                receivedInfo = GetLevelById(idsToGet[i]);

                while (receivedInfo.levelName == "" && receivedInfo.kubicode == "") yield return null;

                Debug.Log("I'm saving kubicode : " + receivedInfo.kubicode);

                GameObject newListObj = Instantiate(listPrefab, listTransform);
                kubiCode = newListObj.transform.GetChild(1).GetComponent<Text>();
                levelname = newListObj.transform.GetChild(0).GetComponent<Text>();

                kubiCode.text = receivedInfo.kubicode;
                levelname.text = receivedInfo.levelName;

                Button button = newListObj.GetComponentInChildren<Button>();
                button.onClick.AddListener(() => DownloadLevel(receivedInfo.kubicode));
            }

            yield return null;
        }

        DynamoReceivedInfo GetLevelById(int id)
        {
            var getLevelRequest = new GetItemRequest
            {
                ConsistentRead = true,
                TableName = DatabaseInfo.userContent_tableName,
                Key = new Dictionary<string, AttributeValue>()
                {
                    { DatabaseInfo.userContent_pKey, new AttributeValue { S = id.ToString() } }
                },
            };

            var response = client.GetItemAsync(getLevelRequest);

            foreach (var keyValuePair in response.Result.Item)
            {
                if (keyValuePair.Key == DatabaseInfo.userContent_levelName) info.levelName = keyValuePair.Value.S;
                if (keyValuePair.Key == DatabaseInfo.userContent_pKey) info.kubicode = keyValuePair.Value.S;
            }

            return info;

            /*client.GetItemAsync(getLevelRequest, (result) =>
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
                        if (keyValuePair.Key == DatabaseInfo.userContent_levelName) info.levelName = keyValuePair.Value.S;
                        if (keyValuePair.Key == DatabaseInfo.userContent_pKey) info.kubicode = keyValuePair.Value.S;
                    }
                    Debug.Log("Retrived name is " + info.levelName);
                }
            }, null);*/

        }

        List<int> SelectRandomLevels(DynamoDBInfo ids, int amountOfLevels)
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
        #endregion

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
            string json = SaveAndLoad.instance.GetLevelFile();

            StartCoroutine(UploadLevel(json));
        }
        #endregion

        #region // UPLOAD USER GENERATED CONTENT LEVELS
        // when the player is uploading a level from the levelsList on the community page
        // called by Upload Button onClick Event
        public void UploadLevelFromList()
        {
            string levelToGet = uploadLevelDropdown.captionText.text;
            // translate the name of the level in the dropdown list to a levelFile
            string json = GetLevelFromUserFolder(levelToGet);

            StartCoroutine(UploadLevel(json));
        }

        IEnumerator UploadLevel(string jsonFile)
        {
            DevEditorData level = new DevEditorData();
            level = JsonUtility.FromJson<DevEditorData>(jsonFile);

            DynamoDBInfo requested = RequestIDs();

            while (requested.listOfIndexes.Count == 0 || requested.lastIdUsed == 0) yield return null;

            string levelName = level.levelName;
            string kubicode = GenerateKubiCode(requested);

            // create a new upload request, input the relevant information
            var putItemRequest = new PutItemRequest
            {
                TableName = DatabaseInfo.userContent_tableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                    { DatabaseInfo.userContent_pKey, new AttributeValue{ S =  kubicode } },
                    { DatabaseInfo.userContent_levelName, new AttributeValue { S = levelName } },
                    { DatabaseInfo.userContent_jsonFile, new AttributeValue { S = jsonFile } }
                }
            };

            var response = client.PutItemAsync(putItemRequest);

            // if something goes wrong, debug the log message from AWS
            if (response.Exception != null)
            {
                Debug.Log(response.Exception.Message);
            }

            // else, the item has successfully been uploaded
            else
            {
                Debug.Log(levelName + " has been uploaded successfully!");
                UserLevelFiles.AddToUploads(uploadLevelDropdown.captionText.text);
            }
            /*
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
                else
                {
                    Debug.Log(levelName + " has been uploaded successfully!");
                    UserLevelFiles.AddToUploads(uploadLevelDropdown.captionText.text);
                }

            }, null);*/

            yield return null;
        }
        
        #endregion

        #region // UPLOAD GAME LEVELS
        public void UploadBaseLevels()
        {
            StartCoroutine(UploadAllGameLevel());
        }

        IEnumerator UploadAllGameLevel()
        {
            foreach (Object level in assets)
            {
                DevEditorData levelFile = JsonUtility.FromJson<DevEditorData>(level.ToString());

                var uploadRequest = new PutItemRequest
                {
                    TableName = DatabaseInfo.levels_tableName,
                    Item = new Dictionary<string, AttributeValue>()
                    {
                        { DatabaseInfo.levels_pKey + ".MyManBro", new AttributeValue{ S = levelFile.Kubicode} },
                        { DatabaseInfo.levels_levelName, new AttributeValue{ S = levelFile.levelName } },
                        { DatabaseInfo.levels_jsonFile, new AttributeValue{ S = level.ToString() } },
                    }
                };

                var response = client.PutItemAsync(uploadRequest);

                // if something goes wrong, debug the log message from AWS
                if (response.Exception != null) Debug.Log(response.Exception.Message);

                // else, the item has successfully been uploaded
                else Debug.Log(levelFile.levelName + " has been uploaded successfully!");

                /*client.PutItemAsync(uploadRequest, (result) =>
                {
                    // if something goes wrong, debug the log message from AWS
                    if (result.Exception != null)
                    {
                        Debug.Log(result.Exception.Message);
                        return;
                    }

                    // else, the item has successfully been uploaded
                    else Debug.Log(levelFile.levelName + " has been uploaded successfully!");

                }, null);*/

                DatabaseInfo.CleanInfo();

                yield return new WaitForSeconds(1.5f);
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
        #endregion

        #region // GET SET INFO TABLE IDs
        // get information concerning the last id that was uploaded to the server. This happens in the KUBIKA_information table
        private DynamoDBInfo RequestIDs()
        {
            DynamoDBInfo info = new DynamoDBInfo();

            var getIdsRequest = new GetItemRequest
            {
                ConsistentRead = true,
                TableName = DatabaseInfo.info_tableName,
                Key = new Dictionary<string, AttributeValue>()
                {
                    { DatabaseInfo.info_pKey, new AttributeValue{ N = DatabaseInfo.info_key} }
                }
            };

            var response = client.GetItemAsync(getIdsRequest);

            string jsonFile = "";

            foreach (var keyValuePair in response.Result.Item)
            {
                if (keyValuePair.Key == DatabaseInfo.info_jsonFile) jsonFile = keyValuePair.Value.S;
            }

            if (jsonFile == "" || jsonFile == null) CreateIDsFile();

            // create a DynamoDBInfo file from the json information stored in the Table
            DynamoDBInfo copy = JsonUtility.FromJson<DynamoDBInfo>(jsonFile);

            info.backupListOfIndexes = copy.backupListOfIndexes;
            info.listOfIndexes = copy.listOfIndexes;
            info.lastIdUsed = copy.listOfIndexes[copy.listOfIndexes.Count - 1];

            /*client.GetItemAsync(getIdsRequest, (result) =>
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
                        if (keyValuePair.Key == DatabaseInfo.info_jsonFile) jsonFile = keyValuePair.Value.S;
                    }

                    if (jsonFile == "" || jsonFile == null) CreateIDsFile();

                    // create a DynamoDBInfo file from the json information stored in the Table
                    DynamoDBInfo copy = JsonUtility.FromJson<DynamoDBInfo>(jsonFile);

                    info.backupListOfIndexes = copy.backupListOfIndexes;
                    info.listOfIndexes = copy.listOfIndexes;
                    info.lastIdUsed = copy.listOfIndexes[copy.listOfIndexes.Count - 1];

                    Debug.Log("Last used is " + info.lastIdUsed);
                }

            }, null);*/

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
                TableName = DatabaseInfo.info_tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { DatabaseInfo.info_pKey, new AttributeValue { N = DatabaseInfo.info_key} },
                    { DatabaseInfo.info_jsonFile, new AttributeValue{ S = json} }
                }
            };

            var response = client.PutItemAsync(request);
            
            if(response.Exception == null) Debug.Log("Info file has been uploaded successfully!");
            /*
            client.PutItemAsync(request, (result) =>
            {
                if (result.Exception != null)
                {
                    Debug.Log(result.Exception.Message);
                    return;
                }
                // else, the item has successfully been uploaded
                else Debug.Log("Info file has been uploaded successfully!");

            }, null);*/
        }

        private void UpdateInfoFile(DynamoDBInfo file)
        {
            string json = JsonUtility.ToJson(file);

            var updateRequest = new UpdateItemRequest
            {
                TableName = DatabaseInfo.info_tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    { DatabaseInfo.info_pKey, new AttributeValue { N = DatabaseInfo.info_key } }
                },

                AttributeUpdates = new Dictionary<string, AttributeValueUpdate>
                {
                    {
                        DatabaseInfo.info_jsonFile, new AttributeValueUpdate
                        {
                            Action = AttributeAction.PUT,
                            Value = new AttributeValue {S = json }
                        }
                    }
                }
            };

            var response = client.UpdateItemAsync(updateRequest);

            if (response.Exception == null) Debug.Log("The item has been updated");

            /*client.UpdateItemAsync(updateRequest, (result) =>
            {
                if (result.Exception != null)
                {
                    Debug.Log(result.Exception.Message);
                    return;
                }

                else Debug.Log("The item has been updated");
            });*/
        }

        #endregion

        #region // GET, UPDATE, DELETE USER GENERATED CONTENT
        void UpdateLevel()
        {

        }

        void DeleteLevel()
        {
            string levelToGet = uploadLevelDropdown.captionText.text;
            UserLevelFiles.RemoveFromUploads(levelToGet);
        }

        // called by Download Button onCLick Event
        public void DownloadLevel(string kubicode)
        {
            Debug.Log("I'm downloading level with kubicode : " + kubicode);

            var getItemRequest = new GetItemRequest
            {
                TableName = DatabaseInfo.userContent_tableName,
                Key = new Dictionary<string, AttributeValue>()
                {
                    { DatabaseInfo.userContent_pKey, new AttributeValue{ S = kubicode} }
                },
                ConsistentRead = true
            };

            var response = client.GetItemAsync(getItemRequest);

            foreach (var keyValuePair in response.Result.Item)
            {
                if (keyValuePair.Key == DatabaseInfo.userContent_levelName) DatabaseInfo.userContent_retrievedLevel = keyValuePair.Value.S;
                if (keyValuePair.Key == DatabaseInfo.userContent_jsonFile) DatabaseInfo.userContent_retrievedJson = keyValuePair.Value.S;
            }

            Debug.Log("Downloading " + DatabaseInfo.userContent_retrievedLevel);

            // call the save and load instance to convert the received information into a useable JSON file
            SaveAndLoad.instance.UserDownloadingLevel(DatabaseInfo.userContent_retrievedLevel, DatabaseInfo.userContent_retrievedJson);

            /*
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
                        if (keyValuePair.Key == DatabaseInfo.userContent_levelName) DatabaseInfo.userContent_retrievedLevel = keyValuePair.Value.S;
                        if (keyValuePair.Key == DatabaseInfo.userContent_jsonFile) DatabaseInfo.userContent_retrievedJson = keyValuePair.Value.S;
                    }

                    Debug.Log("Downloading " + DatabaseInfo.userContent_retrievedLevel);

                    // call the save and load instance to convert the received information into a useable JSON file
                    SaveAndLoad.instance.UserDownloadingLevel(DatabaseInfo.userContent_retrievedLevel, DatabaseInfo.userContent_retrievedJson);
                }

            }, null);*/

            DatabaseInfo.CleanInfo();
        }
        #endregion

    } //class
} //namespace