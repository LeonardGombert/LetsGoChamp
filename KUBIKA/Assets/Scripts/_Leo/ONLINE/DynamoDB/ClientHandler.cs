using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Amazon.CognitoIdentityProvider.Model;
using Kubika.Online;
using Amazon.CodeCommit.Model;
using Amazon.Lambda.Model;

public class ClientHandler : MonoBehaviour
{
    private static ClientHandler _instance;
    public static ClientHandler instance { get { return _instance; } }

    ClientFactory clientFactory = new ClientFactory();

    void Awake()
    {
        if (_instance != null && _instance != this) Destroy(this);
        else _instance = this;
    }

    // sign-up method, called by UIManager
    public void TrySignUpRequest(string email, string password, Action OnFailureF = null, Action OnSuccessF = null)
    {
        SignUpRequest signUpRequest = new SignUpRequest
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

        var response = clientFactory.CognitoIdentityProvider.SignUpAsync(signUpRequest);

        if(response.Exception != null)
        {
            Debug.Log("Failed to complete signup, returned error : \n " + response.Exception.ToString());
            if(OnSuccessF == null) OnFailureF();
        }

        else if (response.Exception == null)
        {
            Debug.Log("Signup Complete");
            if (OnSuccessF == null) OnSuccessF();
        }
    }

    // sign-in method, called by UIManager
    public void TrySignInRequest()
    {

    }
}
