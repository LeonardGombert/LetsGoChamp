using Amazon;

public struct DynamoDBTableInfo 
{
    //AWS Cognito information
    public static string identityPoolId = "eu-central-1:f8990c10-6786-4c01-adc7-6614e9da700b";
    public static RegionEndpoint region = RegionEndpoint.EUCentral1;

    //AWS DynamoDB Testing Table
    public static string testingTable_tableName = "KUBIKA_Testing";
    public static string testingTable_pKey = "kubikaID";
    public static string testingTable_levelName = "levelName";
    public static string testingTable_jsonFile = "levelFile";

    public static string testingTable_retrievedPKey = "";
    public static string testingTable_retrievedLevel = "";
    public static string testingTable_retrievedJson = "";

    //AWS DynamoDB Information Table
    public static string infoTable_tableName = "KUBIKA_Information";
    public static string infoTable_pKey = "id";
    public static string infoTable_key = "00";
    public static string infoTable_jsonFile = "info";

    public static string infoTable_retrivedInfo = "";
    public static string infoTable_retrivedKey = "";

    //AWS DynamoDB Levels Table
    public static string levelsTable_tableName = "KUBIKA_Levels";
    public static string levelsTable_pKey = "kubicode";
    public static string levelsTable_levelName = "levelName";
    public static string levelsTable_jsonFile = "levelFile";



    // reset retrieved info
    public static void CleanInfo()
    {
        testingTable_retrievedPKey = "";
        testingTable_retrievedLevel = "";
        testingTable_retrievedJson = "";

        infoTable_retrivedInfo = "";
        infoTable_retrivedKey = "";
    }
}
