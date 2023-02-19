using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;

public class AuthHandler : MonoBehaviour
{
    public TMP_InputField nameTxt;
    public TMP_InputField phoneTxt;
    public TMP_InputField emailTxt;
    public TMP_InputField refrelCodeTxt;
    public TMP_InputField otpTxt;
    public Button submitBtn;
    [SerializeField] string countryCode = "+91";
    [SerializeField] GameObject login;
    [SerializeField] GameObject register;
    [SerializeField] GameJoin _game;
    [SerializeField] UserAuthData userauthdata;
    [SerializeField] GameObject LoginPanel;
    [SerializeField] GameObject PlayerSelectionPanel;

    PhoneAuthProvider provider;
    FirebaseAuth firebaseAuth;
    private uint phoneAuthTimeoutMs = 60 * 1000;
    private string verificationId;

    private void Start()
    {
        firebaseAuth = FirebaseAuth.DefaultInstance;
    }

    void OnClickSignIn()
    {
        provider = PhoneAuthProvider.GetInstance(firebaseAuth);
        provider.VerifyPhoneNumber(countryCode+phoneTxt.text, phoneAuthTimeoutMs, null,
        verificationCompleted: (credential) => {
            Debug.Log("done");
        // Auto-sms-retrieval or instant validation has succeeded (Android only).
        // There is no need to input the verification code.
        // `credential` can be used instead of calling GetCredential().
        },
        verificationFailed: (error) => {
            Debug.Log(error);
        // The verification code was not sent.
        // `error` contains a human readable explanation of the problem.
        },
        codeSent: (id, token) => {
            verificationId = id;
            Debug.Log(id + " : " + token);
            submitBtn.interactable = true;
        // Verification code was successfully sent via SMS.
        // `id` contains the verification id that will need to passed in with
        // the code from the user when calling GetCredential().
        // `token` can be used if the user requests the code be sent again, to
        // tie the two requests together.
        },
        codeAutoRetrievalTimeOut: (id) => {
        // Called when the auto-sms-retrieval has timed out, based on the given
        // timeout parameter.
        // `id` contains the verification id of the request that timed out.
        });
    }

    public void VerifyOTP()
    {
        Credential credential = provider.GetCredential(verificationId, otpTxt.text);
        firebaseAuth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }
            FirebaseUser newUser = task.Result;
            Debug.Log("User signed in successfully");
            // This should display the phone number.
            Debug.Log("Phone number: " + newUser.PhoneNumber);
            // The phone number providerID is 'phone'.
            Debug.Log("Phone provider ID: " + newUser.ProviderId);
        });
        if (credential.IsValid())
        {
            SignIn();
        }
    }

    private void SignIn()
    {
        StartCoroutine(LoginWithNumber(phoneTxt.text));
    }

    IEnumerator LoginWithNumber(string Number)
    {
        WWWForm form = new WWWForm();
        form.AddField("phone", Number);

        UnityWebRequest www = UnityWebRequest.Post("http://135.181.200.141/~ludoprid/api/login-register", form);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");

            UserAuth userauth = JsonUtility.FromJson<UserAuth>(www.downloadHandler.text);

            Debug.LogError(www.downloadHandler.text);

            userauthdata.sessionToken = userauth.access_token;
            userauthdata.userName = userauth.user.name;
            userauthdata.userId = userauth.user.id;
            userauthdata.phoneNumber = userauth.user.phone;
            userauthdata.emailId = userauth.user.email;
            userauthdata.is_completed = userauth.is_completed;
            userauthdata.refCode = userauth.user.refer_code;

            if (userauthdata.is_completed)
            {
                PlayerSelectionPanel.SetActive(true);
                LoginPanel.SetActive(false);
                StartCoroutine(_game.GetAllContests());
            }
            else
            {
                register.SetActive(true);
                login.SetActive(false);
            }
        }

    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();

        form.AddField("name", nameTxt.text);
        form.AddField("email", emailTxt.text);
        form.AddField("phone", phoneTxt.text);
        form.AddField("refer_code", refrelCodeTxt.text);

        UnityWebRequest webRequest = UnityWebRequest.Post("http://135.181.200.141/~ludoprid/api/user/update-profile", form);
        webRequest.SetRequestHeader("token", userauthdata.sessionToken);
        yield return webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
                Debug.LogError("No Internet");
                break;
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError(": Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError(": HTTP Error: " + webRequest.error);
                break;
            case UnityWebRequest.Result.Success:
                Debug.Log("Received: " + webRequest.downloadHandler.text);
                PlayerSelectionPanel.SetActive(true);
                LoginPanel.SetActive(false);
                StartCoroutine(_game.GetAllContests());
                break;
        }
    }

    public void MobileNumberSubmit()
    {
        if (!string.IsNullOrEmpty(phoneTxt.text.Trim()))
        {
            OnClickSignIn();
        }
    }

    public void OTPSubmit()
    {
        if (!string.IsNullOrEmpty(otpTxt.text.Trim()))
        {
            VerifyOTP();
        }
    }

    public void RegisterSubmit() => StartCoroutine(Register());

}

[System.Serializable]
public class UserAuth
{
    public bool result;
    public string message;
    public string access_token;
    public string token_type;
    public object expires_at;
    public bool is_completed;
    public User user;
}

[System.Serializable]
public class User
{
    public int id;
    public string name;
    public string email;
    public string phone;
    public string refer_code;
}
