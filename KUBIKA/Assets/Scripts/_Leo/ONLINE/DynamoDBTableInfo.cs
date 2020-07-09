using Amazon;

public struct DynamoDBTableInfo 
{
    //AWS Cognito information
    public static string identityPoolId = "eu-central-1:f8990c10-6786-4c01-adc7-6614e9da700b";
    public static RegionEndpoint region = RegionEndpoint.EUCentral1;

    //AWS DynamoDB Levels Table
    public static string table_Name = "KUBIKA_Testing";
    public static string table_PPKey = "kubikaID";
    public static string table_level = "levelName";
    public static string table_Json = "levelFile";

    public static string table_retrievedPPKey = "";
    public static string table_retrievedLevel = "";
    public static string table_retrievedJson = "";

    //AWS DynamoDB Information Table
    public static string infoTable_Name = "KUBIKA_Information";
    public static string infoTable_PPKey = "id";
    public static string infoTable_info = "info";
    public static string infoTable_key = "00";

    public static string infoTable_retrivedInfo = "";
    public static string infoTable_retrivedKey = "";

    // reset retrieved info
    public static void CleanInfo()
    {
        table_retrievedPPKey = "";
        table_retrievedLevel = "";
        table_retrievedJson = "";

        infoTable_retrivedInfo = "";
        infoTable_retrivedKey = "";
    }
}
