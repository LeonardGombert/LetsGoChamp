using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Kubika.Saving;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using UnityEditor.PackageManager.Requests;
using Amazon.SimpleWorkflow.Model;

namespace Kubika.Online
{
    public struct CommunityDatabase
    {
        // Base table info
        public static string tableName = "KUBIKA_CommunityDatabase";
        public static string baseTablePK = "CreatorId";
        public static string baseTableSK = "LevelName";

        // Table Attributes
        public static string levelName = "LevelName";
        public static string levelFile = "LevelFile";
        public static string levelRating = "Rating";
        public static string numberOfRatings = "RatingsNo";
        public static string numberOfDownloads = "DownloadsNo";
        public static string publishDate = "DatePosted";
        public static string creatorSetDifficulty = "CreatorDifficulty";
        public static string votedDifficulty = "PlayerDifficulty";

        // View by Ratings Index --> global secondary index
        public static string GSI_1_PK = "Rating";
        public static string GSI_1_SK = "RatingsNo";

        // View by Difficulty Index --> global secondary index
        public static string GSI_2_PK = "Difficulty";
        public static string GSI_2_SK = "LevelName";
    }

    public class DatabaseBridge : MonoBehaviour
    {
        private static DatabaseBridge _instance;
        public static DatabaseBridge instance { get { return _instance; } }

        AmazonDynamoDBClient client;

        void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;
        }

        void Start()
        {
            client = ClientFactory.GetUserIdentity();
            StartCoroutine(IncrementDownloadCount());
        }

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
                TableName = CommunityDatabase.tableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                    { CommunityDatabase.baseTablePK, new AttributeValue{ S = AmazonCognito.authenticatedUserId } },
                    { CommunityDatabase.baseTableSK, new AttributeValue{ S = levelData.levelName} },
                    { CommunityDatabase.levelFile, new AttributeValue{ S = jsonFile} },
                    { CommunityDatabase.publishDate, new AttributeValue{ S = DateTime.Today.ToShortDateString()} },
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
        void DownloadLevel()
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
                TableName = CommunityDatabase.tableName,

                Key = new Dictionary<string, AttributeValue>()
                {
                    { CommunityDatabase.baseTablePK, new AttributeValue { S = "12345" } },
                    { CommunityDatabase.baseTableSK, new AttributeValue{ S = "azesd" } }
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
