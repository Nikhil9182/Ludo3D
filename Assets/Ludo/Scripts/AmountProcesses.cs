using System;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmountProcesses : MonoBehaviour
{
    [Header("UI Components")]

    [SerializeField] Text _totalAmount;
    [SerializeField] Text _totalAmount2;
    [SerializeField] Text _depositAmount;
    [SerializeField] Text _winAmount;
    [SerializeField] Text _bonusAmount;
    [SerializeField] InputField _addAmount;
    [SerializeField] InputField _addBonus;

    [Header("References")]

    [SerializeField] UserAuthData _userAuthData;

    int _totalAmountCoins = 0;
    int _amountToBeAdded = 0;

    IEnumerator AddDeposit(int amount)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post("http://135.181.200.141/~ludoprid/api/user/add-deposit", AddForm(amount));
        webRequest.SetRequestHeader("token", _userAuthData.sessionToken);
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
                GetAmountDetails();
                break;
        }
    }

    IEnumerator AddBonus(int a)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post("http://135.181.200.141/~ludoprid/api/user/add-bonus", AddForm(a));
        webRequest.SetRequestHeader("token", _userAuthData.sessionToken);
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
                break;
        }
    }

    IEnumerator AddWinningAmount(int a)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post("http://135.181.200.141/~ludoprid/api/user/add-winning-amount", AddForm(a));
        webRequest.SetRequestHeader("token", _userAuthData.sessionToken);
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
                break;
        }
    }

    IEnumerator WithdrawlAmount(int a)
    {
        UnityWebRequest webRequest = UnityWebRequest.Post("http://135.181.200.141/~ludoprid/api/user/withdrawal-amount", AddForm(a));
        webRequest.SetRequestHeader("token", _userAuthData.sessionToken);
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
                break;
        }
    }

    IEnumerator GetAmount()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("http://135.181.200.141/~ludoprid/api/user/get-wallet");
        webRequest.SetRequestHeader("token", _userAuthData.sessionToken);
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
                Debug.LogError("Received: " + webRequest.downloadHandler.text);
                AssignAmountDataToUI(webRequest);
                break;
        }
    }

    private void AssignAmountDataToUI(UnityWebRequest webRequest)
    {
        Amount amountDetails = JsonUtility.FromJson<Amount>(webRequest.downloadHandler.text);
        _totalAmount.text = "₹" + amountDetails.total_amount.ToString();
        _totalAmount2.text = _totalAmount.text;
        _depositAmount.text = "₹" + amountDetails.deposit_amount.ToString();
        _winAmount.text = "₹" + amountDetails.win_amount.ToString();
        _bonusAmount.text = "₹" + amountDetails.bonus_amount.ToString();
    }

    private WWWForm AddForm(int a)
    {
        WWWForm form = new WWWForm();
        form.AddField("amount", a);
        return form;
    }

    public void CoinBtn(int amount)
    {
        _amountToBeAdded = amount;
        _addAmount.text = amount.ToString();
    }

    public void EditAmount()
    {
        int amount = Int16.Parse(_addAmount.text);
        if (amount > 0)
        {
            _amountToBeAdded = amount;
        }
    }

    public void CallAddDeposit() => StartCoroutine(AddDeposit(_amountToBeAdded));
    public void CallAddBonus(int amount) => StartCoroutine(AddBonus(amount));
    public void CallAddWinningAmount(int amount) => StartCoroutine(AddWinningAmount(amount));
    public void CallWithdrawlAmount(int amount) => StartCoroutine(WithdrawlAmount(amount));
    public void GetAmountDetails() => StartCoroutine(GetAmount());



    [System.Serializable]
    public class Amount
    {
        public bool result;
        public string message;
        public int total_amount;
        public int deposit_amount;
        public int win_amount;
        public int bonus_amount;
    }
}
