using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Kubika.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Kubika.Online
{
    public struct CommunityDatabase
    {
        // Base table info
        public static string tableName = "KUBIKA_CommunityDatabse";
        public static string baseTablePK = "CreatorId";
        public static string baseTableSK = "LevelName";

        // Table Attributes
        public static string levelName = "LevelName";
        public static string levelFile = "LevelFile";
        public static string levelRating = "Rating";
        public static string numberOfRatings = "RatingsNo";
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
        }

        // when the player wants to upload his level directly from the editor
        public void UploadLevelFromEditor()
        {
            string json = SaveAndLoad.instance.GetLevelFile();
            LevelEditorData level = JsonUtility.FromJson<LevelEditorData>(json);

            StartCoroutine(UploadLevel(json, level));
        }

        // called by UIManager
        IEnumerator UploadLevel(string jsonFile, LevelEditorData levelData)
        {
            Debug.Log("Craeting new Request");

            var request = new PutItemRequest
            {
                TableName = CommunityDatabase.tableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                    { CommunityDatabase.baseTablePK, new AttributeValue{ S = "12345"} },
                    { CommunityDatabase.baseTableSK, new AttributeValue{ S = levelData.levelName} },
                    /*{ CommunityDatabase.levelFile, new AttributeValue{ S = jsonFile} },
                    { CommunityDatabase.publishDate, new AttributeValue{ S = DateTime.Today.ToShortDateString()} },
                    // { CommunityDatabase.creatorSetDifficulty, new AttributeValue{ S = ""} */
                }
            };

            Debug.Log("Uploading " + levelData.levelName);

            var response = client.PutItemAsync(request);

            //Debug.Log("Status is " + response.Result.HttpStatusCode.ToString());
            if (response.Exception != null) Debug.Log(response.Exception.Message);
            else Debug.Log("Uploaded successfully");
            
            yield return null;
        }

        void Update()
        {

        }
    }
}
