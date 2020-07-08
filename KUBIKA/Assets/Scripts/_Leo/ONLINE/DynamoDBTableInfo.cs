using Amazon;

public struct DynamoDBTableInfo 
{
    //AWS Cognito information
    public static string identityPoolId = "eu - central - 1:f8990c10-6786-4c01-adc7-6614e9da700b";
    public static RegionEndpoint region = RegionEndpoint.EUCentral1;

    //AWS DynamoDB information
    public static string table_Name = "KUBIKA_Testing";
    public static string table_PPKey = "kubikaID";
    public static string table_levelName = "kubikaID";
    public static string table_Json = "levelFile";
}
