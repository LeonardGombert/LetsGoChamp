using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Kubika.Saving;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using Kubika.Game;
using Random = UnityEngine.Random;

namespace Kubika.Online
{
    public class DatabaseBridge : MonoBehaviour
    {
        private static DatabaseBridge _instance;
        public static DatabaseBridge instance { get { return _instance; } }

        AmazonDynamoDBClient client;

        public GameObject levelListPrefab;
        public Transform listTransform;

        public TextAsset levelToUpload;
        public Button uploadLevels;

        public int levelsToUpload;
        public int numberOfLevelsToDisplay;
        public Dictionary<string, AttributeValue> lastScanned;

        public Button loadMore;

        void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;
        }

        void Start()
        {
            client = ClientFactory.GetUserIdentity();

            //StartCoroutine(TableScan());
            //uploadLevels.onClick.AddListener(() => StartCoroutine(UploadTestLevels()));
            uploadLevels.onClick.AddListener(() => StartCoroutine(BatchUploadItems(0)));
            loadMore.onClick.AddListener(() => StartCoroutine(ScanTable()));

            //StartCoroutine(BatchReadItems());
            StartCoroutine(ScanTable());
        }

        // stepStart variable is used for the "load more levels"
        IEnumerator ScanTable()
        {
            ScanRequest request = new ScanRequest
            {
                TableName = DynamoDB.tableName,
                Limit = numberOfLevelsToDisplay,
                ExclusiveStartKey = lastScanned
            };

            // Issue request
            var result = client.ScanAsync(request);

            lastScanned = result.Result.LastEvaluatedKey;

            // List all returned items
            List<Dictionary<string, AttributeValue>> items = result.Result.Items;

            // Visualize a preset number of levels
            /*for (int i = stepStart; i < stepStart + numberOfLevelsToDisplay; i++)
            {
                if (i == stepStart + numberOfLevelsToDisplay - 1) lastScanned = i;

                GameObject level = Instantiate(levelListPrefab, listTransform);
                OnlineLevelObject listObject = level.GetComponent<OnlineLevelObject>();

                foreach (var keyValuePair in items[i])
                {
                    if (keyValuePair.Key == DynamoDB.baseTablePK) listObject.levelCreator.text = keyValuePair.Value.S;
                    if (keyValuePair.Key == DynamoDB.levelName) listObject.levelName.text = keyValuePair.Value.S;
                }
                yield return null;
            }*/

            foreach (Dictionary<string, AttributeValue> item in items)
            {
                GameObject level = Instantiate(levelListPrefab, listTransform);
                OnlineLevelObject listObject = level.GetComponent<OnlineLevelObject>();

                foreach (var keyValuePair in item)
                {
                    if (keyValuePair.Key == DynamoDB.baseTablePK) listObject.levelCreator.text = keyValuePair.Value.S;
                    if (keyValuePair.Key == DynamoDB.levelName) listObject.levelName.text = keyValuePair.Value.S;
                    if (keyValuePair.Key == DynamoDB.levelName) listObject.levelName.text = keyValuePair.Value.S;
                }
                yield return null;
            }
        }

        #region // Batch Reading Items
        IEnumerator UploadTestLevels()
        {
            DevEditorData levelFile = JsonUtility.FromJson<DevEditorData>(levelToUpload.ToString());

            yield return null;

            PutItemRequest putItemRequest = new PutItemRequest()
            {
                TableName = DynamoDB.tableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                    { DynamoDB.baseTablePK, new AttributeValue { S =  "12345"} },
                    { DynamoDB.baseTableSK, new AttributeValue { N =  "1"} },
                    { DynamoDB.levelName, new AttributeValue { S =  levelFile.levelName.ToString()} },
                    { DynamoDB.levelFile, new AttributeValue { S =  levelToUpload.ToString()} },
                    { DynamoDB.levelRating, new AttributeValue { N =  Random.Range(1, 5).ToString()} },
                    { DynamoDB.numberOfRatings, new AttributeValue { N =  "12"} },
                    { DynamoDB.numberOfDownloads, new AttributeValue { N =  "1000"} },
                    { DynamoDB.creatorSetDifficulty, new AttributeValue { N =  "4"} },
                    { DynamoDB.votedDifficulty, new AttributeValue { N =  "5"} },
                    { DynamoDB.publishDate, new AttributeValue { S =  System.DateTime.Today.ToShortDateString()} }
                }
            };

            yield return null;

            var response = client.PutItemAsync(putItemRequest);

            // if something goes wrong, debug the log message from AWS
            if (response.Exception != null) Debug.Log(response.Exception.Message);

            // else, the item has successfully been uploaded
            else Debug.Log(levelFile.levelName + " has been uploaded successfully!");

        }

        IEnumerator BatchUploadItems(int stepStart)
        {
            List<WriteRequest> newBatchList = new List<WriteRequest>();

            Debug.Log("Starting point is " + stepStart);

            for (int i = stepStart; i < LevelsManager.instance.gameMasterList.Count; i++)
            {
                DevEditorData levelFile = JsonUtility.FromJson<DevEditorData>(LevelsManager.instance.gameMasterList[i].levelFile.ToString());

                Dictionary<string, AttributeValue> currLevel = new Dictionary<string, AttributeValue>();
                currLevel[DynamoDB.baseTablePK] = new AttributeValue { S = "Champ&ZeeDev" };
                currLevel[DynamoDB.baseTableSK] = new AttributeValue { N = i.ToString() };
                currLevel[DynamoDB.levelName] = new AttributeValue { S = levelFile.levelName.ToString() };
                currLevel[DynamoDB.levelFile] = new AttributeValue { S = LevelsManager.instance.gameMasterList[i].levelFile.ToString() };
                currLevel[DynamoDB.publishDate] = new AttributeValue { S = DateTime.Today.ToShortDateString() };
                currLevel[DynamoDB.levelRating] = new AttributeValue { N = Random.Range(1, 5).ToString() };
                currLevel[DynamoDB.numberOfDownloads] = new AttributeValue { N = Random.Range(1, 3000).ToString() };
                currLevel[DynamoDB.creatorSetDifficulty] = new AttributeValue { N = Random.Range(1, 5).ToString() };
                currLevel[DynamoDB.votedDifficulty] = new AttributeValue { N = Random.Range(1, 5).ToString() };


                newBatchList.Add(new WriteRequest
                {
                    PutRequest = new PutRequest
                    {
                        Item = currLevel
                    }
                });

                yield return null;

                if (i % 25 == 0 && i != 0)
                {
                    Debug.Log("I've hit a ceiling at " + i);
                    StartCoroutine(BatchUploadItems(i + 1));
                    break;
                }
            }

            BatchWriteItemRequest batchWriteRequest = new BatchWriteItemRequest()
            {
                RequestItems = new Dictionary<string, List<WriteRequest>>()
                {
                    {
                        DynamoDB.tableName, newBatchList
                    }
                }
            };

            var result = client.BatchWriteItemAsync(batchWriteRequest);

            // if something goes wrong, debug the log message from AWS
            if (result.Exception != null) Debug.Log(result.Exception.Message);

            // else, the item has successfully been uploaded
            else Debug.Log("Files have been uploaded successfully!");
        }

        IEnumerator BatchReadItems()
        {
            List<string> attributesToGet = new List<string> { DynamoDB.levelName, DynamoDB.levelRating, DynamoDB.publishDate };
            List<Dictionary<string, AttributeValue>> sampleTableKeys = new List<Dictionary<string, AttributeValue>>();

            for (int i = 0; i < 100; i++)
            {
                Dictionary<string, AttributeValue> item1 = new Dictionary<string, AttributeValue>
                {
                    { DynamoDB.baseTablePK, new AttributeValue { S = "Champ&ZeeDev" } },
                    { DynamoDB.baseTableSK, new AttributeValue { N = i.ToString() } },
                };

                sampleTableKeys.Add(item1);
            }

            // Construct get-request for first table
            KeysAndAttributes sampleTableItems = new KeysAndAttributes
            {
                AttributesToGet = attributesToGet,
                Keys = sampleTableKeys
            };

            BatchGetItemRequest batchGetRequest = new BatchGetItemRequest()
            {
                RequestItems = new Dictionary<string, KeysAndAttributes>
                {
                    { DynamoDB.tableName, sampleTableItems}
                }
            };
            yield return null;

            var result = client.BatchGetItemAsync(batchGetRequest);

            // if something goes wrong, debug the log message from AWS
            if (result.Exception != null) Debug.Log(result.Exception.Message);

            // else, the item has successfully been uploaded
            else Debug.Log("Files have been read successfully!");

            Dictionary<string, List<Dictionary<string, AttributeValue>>> responses = result.Result.Responses;

            foreach (string tableName in responses.Keys)
            {
                // Get items for each table
                List<Dictionary<string, AttributeValue>> tableItems = responses[tableName];

                // View items
                foreach (Dictionary<string, AttributeValue> item in tableItems)
                {
                    foreach (var keyValuePair in item)
                    {
                        Debug.Log(keyValuePair.Key + " is " + keyValuePair.Value.S + keyValuePair.Value.N);
                    }
                }
            }

        }
        #endregion

        #region // Reading Data
        IEnumerator TableScan()
        {
            var request = new ScanRequest
            {
                TableName = DynamoDB.tableName,
            };

            var response = client.ScanAsync(request);

            Debug.Log("I've scanned " + response.Result.ScannedCount + " items");
            Debug.Log("Last Primary Key was " + response.Result.LastEvaluatedKey);

            foreach (Dictionary<string, AttributeValue> item in response.Result.Items)
            {
                // Process the result.
                Debug.Log("Item value is " + item.Values.ToString());
                Debug.Log("Item key is " + item.Keys.ToString());
            }

            yield return null;
        }

        // searches for all levels produced by a single user
        IEnumerator GetUserLevels()
        {
            DynamoReceivedInfo receivedInfo = new DynamoReceivedInfo();

            GetItemRequest getRequest = new GetItemRequest()
            {
                TableName = DynamoDB.tableName,

                Key = new Dictionary<string, AttributeValue>()
                {
                    { DynamoDB.baseTablePK , new AttributeValue { S = "12345"} },
                    { DynamoDB.baseTableSK, new AttributeValue { S = "azesd"} },
                },
            };

            //var response = client.GetItemAsync(getRequest);

            yield return null;
            /*
            foreach (var keyValuePair in response.Result.Responses)
            {
                if (keyValuePair.Key == DynamoDB.baseTableSK) receivedInfo.kubicode = keyValuePair.Value.S;
                if (keyValuePair.Key == DynamoDB.levelName) receivedInfo.levelName = keyValuePair.Value.S;
            }

            while (receivedInfo.levelName == "" && receivedInfo.kubicode == "") yield return null;

            Debug.Log("I'm saving kubicode : " + receivedInfo.kubicode);

            GameObject newListObj = Instantiate(levelListPrefab, listTransform);
            kubiCode = newListObj.transform.GetChild(1).GetComponent<Text>();
            levelname = newListObj.transform.GetChild(0).GetComponent<Text>();

            kubiCode.text = receivedInfo.kubicode;
            levelname.text = receivedInfo.levelName;

            Button button = newListObj.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => DownloadLevel(receivedInfo.kubicode));*/
        }
        #endregion

        #region // Upload Levels
        // called by UIManager
        public IEnumerator UploadLevelFromEditor()
        {
            string jsonFile = SaveAndLoad.instance.GetLevelFile();

            yield return null;

            DevEditorData levelData = JsonUtility.FromJson<DevEditorData>(jsonFile);

            yield return null;

            Debug.Log("Creating new Request");

            var request = new PutItemRequest
            {
                TableName = DynamoDB.tableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                    { DynamoDB.baseTablePK, new AttributeValue{ S = AmazonCognito.authenticatedUserId } },
                    { DynamoDB.baseTableSK, new AttributeValue{ S = levelData.levelName} },
                    { DynamoDB.levelFile, new AttributeValue{ S = jsonFile} },
                    { DynamoDB.publishDate, new AttributeValue{ S = DateTime.Today.ToShortDateString()} },
                    // { CommunityDatabase.creatorSetDifficulty, new AttributeValue{ S = ""} 
                }
            };

            yield return null;

            Debug.Log("Uploading " + levelData.levelName);

            var response = client.PutItemAsync(request);

            yield return null;

            if (response.Exception != null) Debug.Log(response.Exception.Message);
        }
        #endregion

        // also update number of downloads in same method
        void DownloadLevel(string kubicode)
        {

        }

        void CreatorUpdateLevel()
        {

        }

        #region // Level Stats Updates
        // increment/decrement the rating of the level
        void RateLevel()
        {

        }

        // to increment the number of times the level has been downloaded
        IEnumerator IncrementDownloadCount()
        {
            Debug.Log("Updating Item");

            var request = new UpdateItemRequest
            {
                TableName = DynamoDB.tableName,

                Key = new Dictionary<string, AttributeValue>()
                {
                    { DynamoDB.baseTablePK, new AttributeValue { S = "12345" } },
                    { DynamoDB.baseTableSK, new AttributeValue{ S = "azesd" } }
                },

                ExpressionAttributeNames = new Dictionary<string, string>()
                {
                    {"#A", "LevelName"},
                    {"#P", "DatePosted"},
                    {"#NA", "NewAttribute"},
                    {"#I", "LevelFile"}
                },

                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                {
                    {":auth",new AttributeValue { SS = {"Author YY","Author ZZ"}}},
                    {":p",new AttributeValue {N = "1"}},
                    {":newattr",new AttributeValue {S = "someValue"}},
                },

                UpdateExpression = "ADD #A :auth SET #P = #P - :p, #NA = :newattr REMOVE #I"
            };

            yield return null;

            var response = client.UpdateItemAsync(request);

            yield return null;

            if (response.Exception != null) Debug.Log(response.Exception.Message);

            Debug.Log(response.Status);
        }

        // for when the player votes on how difficult the level was
        void VoteLevelDifficulty()
        {

        }
        #endregion
    }
}
