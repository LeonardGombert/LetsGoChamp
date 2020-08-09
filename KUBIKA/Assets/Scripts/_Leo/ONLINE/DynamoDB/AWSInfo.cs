using Amazon;

public struct AmazonCognito
{
    // user and identity pool access variables
    public static string identityPoolId = "eu-central-1:f8990c10-6786-4c01-adc7-6614e9da700b";
    public static string providerName = "cognito-idp:eu-central-1:427426409501:userpool/eu-central-1_UXwaae0M4";

    public static string appClientId = "9db7pkdnppvrlui6sackifg6d";
    public static string userPoolId = "eu-central-1_UXwaae0M4";
    public static string userPoolName = "UXwaae0M4";
    public static RegionEndpoint cognitoIdentityRegion = RegionEndpoint.EUCentral1;

    public static string authenticatedUserId = "1234567890";

    // assigned by clint handler
    public static string idToken = "";
    public static string accessToken = "";
    public static string refreshToken = "";
}

public struct AmazonIAM
{
    public static string authARN = "arn:aws:iam::427426409501:role/Cognito_KUBIKA_TestingPoolAuth_Role";
    public static string unauthARN = "arn:aws:iam::427426409501:role/Cognito_KUBIKA_TestingPoolUnauth_Role";  
}

public struct DynamoDB
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
