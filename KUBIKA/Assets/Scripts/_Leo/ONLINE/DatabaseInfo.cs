using Amazon;

public struct DatabaseInfo 
{
    //AWS Cognito information
    public static string identityPoolId = "eu-central-1:f8990c10-6786-4c01-adc7-6614e9da700b";
    public static RegionEndpoint region = RegionEndpoint.EUCentral1;

    //AWS DynamoDB content Table
    public static string userContent_tableName = "KUBIKA_Testing";
    public static string userContent_pKey = "kubikaID";
    public static string userContent_levelName = "levelName";
    public static string userContent_jsonFile = "levelFile";

    public static string userContent_retrievedPKey = "";
    public static string userContent_retrievedLevel = "";
    public static string userContent_retrievedJson = "";

    //AWS DynamoDB Information Table
    public static string info_tableName = "KUBIKA_Information";
    public static string info_pKey = "id";
    public static string info_key = "00";
    public static string info_jsonFile = "info";

    public static string info_retrivedInfo = "";
    public static string info_retrivedKey = "";

    //AWS DynamoDB Levels Table
    public static string levels_tableName = "KUBIKA_Levels";
    public static string levels_pKey = "kubicode";
    public static string levels_levelName = "levelName";
    public static string levels_jsonFile = "levelFile";

    // reset retrieved info
    public static void CleanInfo()
    {
        userContent_retrievedPKey = "";
        userContent_retrievedLevel = "";
        userContent_retrievedJson = "";

        info_retrivedInfo = "";
        info_retrivedKey = "";
    }
}
