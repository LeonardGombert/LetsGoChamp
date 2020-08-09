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

            //TrySignUpRequest("Camenbert99@gmail.com", "Thesamething99");
            TrySignInAdmin("Camenbert99@gmail.com", "Thesamething99");
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
        public void TrySignInRequest(string username, string password, Action OnFailureF = null, Action<string> OnSuccessF = null)
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
    }
}