using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Amazon.CognitoIdentityProvider.Model;
using Kubika.Online;
using Amazon.CodeCommit.Model;
using Amazon.Lambda.Model;
using Amazon.CognitoIdentityProvider;
using Amazon.Runtime;
using Amazon.AppSync.Model;
using System.Globalization;
using Amazon.CognitoIdentity.Model;
using Amazon.DynamoDBv2;
using UnityEditor.PackageManager;
using Amazon.CognitoIdentity;
using UnityEditor;
using Amazon.DynamoDBv2.Model;

namespace Kubika.Online
{

    public class ClientHandler : MonoBehaviour
    {
        private static ClientHandler _instance;
        public static ClientHandler instance { get { return _instance; } }

        void Awake()
        {
            if (_instance != null && _instance != this) Destroy(this);
            else _instance = this;
        }

        private void Start()
        {
            //TrySignUpRequest("Camenbert99@gmail.com", "Thesamething99");
            StartCoroutine(TrySignInRequest("Camenbert99@gmail.com", "Thesamething99",
            () =>
            {
                Debug.Log("Failed ! Check the log");
            },
                (token) =>
                {
                    Debug.Log("Success !" + token.Substring(0, 10) + "...");
                }));

        }

        private AmazonCognitoIdentityProviderClient _cognitoIdentityProvider = null;
        public AmazonCognitoIdentityProviderClient cognitoIdentityProvider
        {
            get
            {
                if (_cognitoIdentityProvider == null)
                {
                    var config = new AmazonCognitoIdentityProviderConfig();
                    config.RegionEndpoint = AmazonCognito.cognitoIdentityRegion;
                    _cognitoIdentityProvider = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), config);
                }
                return _cognitoIdentityProvider;
            }
        }

        // sign-up method, called by UIManager
        public void TrySignUpRequest(string email, string password, Action OnFailureF = null, Action OnSuccessF = null)
        {
            SignUpRequest signUpRequest = new SignUpRequest()
            {
                ClientId = AmazonCognito.appClientId,
                Password = password,
                Username = email
            };

            var emailAttribute = new AttributeType
            {
                Name = "email",
                Value = email
            };

            Debug.Log("posting signup request...");

            signUpRequest.UserAttributes.Add(emailAttribute);

            var response = cognitoIdentityProvider.SignUpAsync(signUpRequest);

            if (response.Exception != null)
            {
                Debug.Log("Failed to complete signup, returned error : " + response.Exception.ToString());
                OnFailureF?.Invoke(); // if OnFailureF != null, invoke the function call
            }

            else if (response.Exception == null)
            {
                Debug.Log("Signup Complete with results :");
                Debug.Log(response.Result.ResponseMetadata.ToString());
                Debug.Log(response.Status);
                OnSuccessF?.Invoke(); // if OnFailureF != null, invoke the function call
            }
        }

        // sign-in method, called by UIManager
        public void TrySignInRequest1(string username, string password, Action OnFailureF = null, Action<string> OnSuccessF = null)
        {
            InitiateAuthRequest authRequest = new InitiateAuthRequest()
            {
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                ClientId = AmazonCognito.appClientId
            };

            var authResponse = cognitoIdentityProvider.InitiateAuthAsync(authRequest);

            if (authResponse.Exception != null)
            {
                Debug.Log("[TrySignInRequest] exception : " + authResponse.Exception.Message);
                OnFailureF?.Invoke(); // if OnFailureF != null, invoke the function call
                return;
            }

            else
            {
                RespondToAuthChallengeRequest respondRequest = new RespondToAuthChallengeRequest()
                {
                    ClientId = AmazonCognito.appClientId,
                    ChallengeName = authResponse.Result.ChallengeName,
                };

                var finalResponse = cognitoIdentityProvider.RespondToAuthChallengeAsync(respondRequest);

                if (finalResponse.Exception != null)
                {
                    Debug.Log("[TrySignInRequest] exception : " + finalResponse.Exception.Message);
                    OnFailureF?.Invoke();
                    return;
                }

                else
                {
                    AuthenticationResultType authResult = finalResponse.Result.AuthenticationResult;
                    string idToken = authResult.IdToken;
                    string accessToken = authResult.AccessToken;
                    string refreshToken = authResult.RefreshToken;

                    Debug.Log("[TrySignInRequest] success!");
                    OnSuccessF?.Invoke(idToken);
                }
            }
        }


        public void TrySignInAdmin(string username, string password)
        {
            var authReq = new AdminInitiateAuthRequest()
            {
                UserPoolId = AmazonCognito.userPoolId,
                ClientId = AmazonCognito.appClientId,
                AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
            };

            authReq.AuthParameters.Add("USERNAME", username);
            authReq.AuthParameters.Add("PASSWORD", password);

            var response = cognitoIdentityProvider.AdminInitiateAuthAsync(authReq);

            if (response.Exception != null)
            {
                Debug.Log("Failed to complete signup, returned error : " + response.Exception.ToString());
            }

            else if (response.Exception == null)
            {
                Debug.Log("Signup Complete with results :");
                Debug.Log(response.Result.ResponseMetadata.ToString());
                Debug.Log(response.Result.ResponseMetadata.ToString());
                Debug.Log(response.Status);
            }
        }

        public void TrySignInUser(string username, string password)
        {

        }

        public IEnumerator TrySignInRequest(string username, string password, Action OnFailureF = null, Action<string> OnSuccessF = null)
        {
            //Get the SRP variables A and a
            var TupleAa = AuthenticationHelper.CreateAaTuple();

            InitiateAuthRequest authRequest = new InitiateAuthRequest()
            {
                ClientId = AmazonCognito.appClientId,
                AuthFlow = AuthFlowType.USER_SRP_AUTH,
                AuthParameters = new Dictionary<string, string>() {
                    { "USERNAME", username },
                    { "SRP_A", TupleAa.Item1.ToString(16) } }
            };

            //
            // This is a nested request / response / request. First we send the
            // InitiateAuthRequest, with some crypto things. AWS sends back
            // some of its own crypto things, in the authResponse object (this is the "challenge").
            // We combine that with the actual password, using math, and send it back (the "challenge response").
            // If AWS is happy with our answer, then it is convinced we know the password,
            // and it sends us some tokens!
            var authResponse = cognitoIdentityProvider.InitiateAuthAsync(authRequest);

            if (authResponse.Exception != null)
            {
                Debug.Log("[TrySignInRequest] exception : " + authResponse.Exception.ToString());
                if (OnFailureF != null)
                    OnFailureF();
                yield return null;
            }

            //The timestamp format returned to AWS _needs_ to be in US Culture
            DateTime timestamp = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
            CultureInfo usCulture = new CultureInfo("en-US");
            String timeStr = timestamp.ToString("ddd MMM d HH:mm:ss \"UTC\" yyyy", usCulture);

            //Do the hard work to generate the claim we return to AWS
            var challegeParams = authResponse.Result.ChallengeParameters;
            byte[] claim = AuthenticationHelper.authenticateUser(
                                challegeParams["USERNAME"],
                                password, AmazonCognito.userPoolName, TupleAa,
                                challegeParams["SALT"], challegeParams["SRP_B"],
                                challegeParams["SECRET_BLOCK"], timeStr);

            String claimBase64 = Convert.ToBase64String(claim);

            // construct the second request
            RespondToAuthChallengeRequest respondRequest = new RespondToAuthChallengeRequest()
            {
                ChallengeName = authResponse.Result.ChallengeName,
                ClientId = AmazonCognito.appClientId,
                ChallengeResponses = new Dictionary<string, string>() {
                            { "PASSWORD_CLAIM_SECRET_BLOCK", challegeParams["SECRET_BLOCK"] },
                            { "PASSWORD_CLAIM_SIGNATURE", claimBase64 },
                            { "USERNAME", username },
                            { "TIMESTAMP", timeStr } }
            };

            // send the second request
            var finalResponse = cognitoIdentityProvider.RespondToAuthChallengeAsync(respondRequest);

            if (finalResponse.Exception != null)
            {
                // Note: if you have the wrong username/password, you will get an exception.
                // It's up to you to differentiate that from other errors / etc.
                Debug.Log("[TrySignInRequest] exception : " + finalResponse.Exception.ToString());
                if (OnFailureF != null) OnFailureF();
                yield return null;
            }

            // Ok, if we got here, we logged in, and here are some tokens
            AuthenticationResultType authResult = finalResponse.Result.AuthenticationResult;
            string idToken = authResult.IdToken;
            string accessToken = authResult.AccessToken;
            string refreshToken = authResult.RefreshToken;
            Debug.Log(idToken + ", " + accessToken + ", " + refreshToken);

            Debug.Log("[TrySignInRequest] success!");
            if (OnSuccessF != null) OnSuccessF(idToken);

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            var credentials = new CognitoAWSCredentials(AmazonCognito.identityPoolId, AmazonCognito.cognitoIdentityRegion);

            //CognitoAWSCredentials credentials = new CognitoAWSCredentials(username, AmazonCognito.identityPoolId, AmazonIAM.unauthARN, AmazonIAM.authARN, AmazonCognito.cognitoIdentityRegion);

            credentials.AddLogin(cognitoIdentityProvider.ToString(), idToken);

            Debug.Log("Account ID is " + credentials.AccountId);
            Debug.Log("Auth Role ARN is " + credentials.AuthRoleArn);
            
            foreach (var item in credentials.CurrentLoginProviders) Debug.Log("Current Login Providers are " + item.ToString());

            Debug.Log("Identity Pool ID is " + credentials.IdentityPoolId);
            Debug.Log("Logins Count is " + credentials.LoginsCount);
            Debug.Log("PreemptExpirty Time is " + credentials.PreemptExpiryTime);
            Debug.Log("Unauth role ARN is " + credentials.UnAuthRoleArn);

            credentials.RemoveLogin(cognitoIdentityProvider.ToString());

            Debug.Log("Logins count is " + credentials.LoginsCount);

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            
            AmazonCognitoIdentityClient yes = new AmazonCognitoIdentityClient(idToken, accessToken, refreshToken, AmazonCognito.cognitoIdentityRegion);
            
            GetIdRequest request2 = new GetIdRequest()
            {
                IdentityPoolId = AmazonCognito.identityPoolId,
            };

            GetIdentityPoolRolesRequest request3 = new GetIdentityPoolRolesRequest()
            {
                IdentityPoolId = AmazonCognito.identityPoolId,
            };





            var requestResponse = yes.GetIdAsync(request2);

            if (requestResponse.Exception != null)
            {
                Debug.Log(requestResponse.Exception.Message);
                Debug.Log(requestResponse.Status);
            }

            else Debug.Log("Identity id is " + requestResponse.Result.IdentityId);







            GetCredentialsForIdentityRequest request = new GetCredentialsForIdentityRequest()
            {
                IdentityId = requestResponse.Result.IdentityId,                
                
            };

            var finalResultPlease = yes.GetCredentialsForIdentityAsync(request);

            if (finalResultPlease.Exception != null)
            {
                Debug.Log(finalResultPlease.Exception.Message);
                Debug.Log(finalResultPlease.Status);
            }

            else Debug.Log("Finally, credentials are " + finalResultPlease.Result.Credentials.AccessKeyId.ToString());

            MakeNewClientForAuthUser(finalResultPlease.Result.Credentials);

        }

        private void MakeNewClientForAuthUser(Credentials credentials)
        {
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(credentials, AmazonCognito.cognitoIdentityRegion);
            StartCoroutine(UploadLevelFromEditor(client)); 
        }

        public IEnumerator UploadLevelFromEditor(AmazonDynamoDBClient client)
        {
            Debug.Log("Creating new Request");

            var request = new PutItemRequest
            {
                TableName = DynamoDB.tableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                    { DynamoDB.baseTablePK, new AttributeValue{ S = AmazonCognito.authenticatedUserId } },
                    { DynamoDB.baseTableSK, new AttributeValue{ S = "PLEASE WORK"} },
                    { DynamoDB.levelFile, new AttributeValue{ S = "NOOGER"} },
                    { DynamoDB.publishDate, new AttributeValue{ S = DateTime.Today.ToShortDateString()} },
                    // { CommunityDatabase.creatorSetDifficulty, new AttributeValue{ S = ""} 
                }
            };

            yield return null;

            Debug.Log("Uploading a test item");

            var response = client.PutItemAsync(request);

            yield return null;

            if (response.Exception != null) Debug.Log(response.Exception.Message);
        }
    }
}