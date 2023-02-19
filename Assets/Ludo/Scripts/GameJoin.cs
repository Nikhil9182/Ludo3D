using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameJoin : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _betAmountText;
    [SerializeField] private TextMeshProUGUI _winAmountText;
    [SerializeField] Button _2Player;
    [SerializeField] Button _4Player;
    [SerializeField] UserAuthData _userAuthData;

    int[] _2PlayerBetAmount = new int[4] { 28, 55, 83, 110 };
    int[] _2PlayerWinAmount = new int[4] { 50, 100, 150, 200 };
    int[] _4PlayerBetAmount = new int[4] { 15, 28, 43, 57 };
    int[] _4PlayerWinAmount = new int[4] { 50, 100, 150, 200 };

    int counter = 0;
    private bool isTwoPlayer = false;

    public void TwoPlayer() => isTwoPlayer = true;
    public void FourPlayer() => isTwoPlayer = false;

    public void GameMode(int i)
    {
        counter = 0;
        if (i == 1)
        {
            _betAmountText.text = _2PlayerBetAmount[0].ToString();
            _winAmountText.text = _2PlayerWinAmount[0].ToString();
        }
        else
        {
            _betAmountText.text = _4PlayerBetAmount[0].ToString();
            _winAmountText.text = _4PlayerWinAmount[0].ToString();
        }
    }

    public void IncreaseAmount()
    {
        if (isTwoPlayer)
        {
            if (counter < _2PlayerBetAmount.Length - 1)
            {
                counter++;
                _betAmountText.text = _2PlayerBetAmount[counter].ToString();
                _winAmountText.text = _2PlayerWinAmount[counter].ToString();
            }
        }
        else
        {
            if (counter < _4PlayerBetAmount.Length - 1)
            {
                counter++;
                _betAmountText.text = _4PlayerBetAmount[counter].ToString();
                _winAmountText.text = _4PlayerWinAmount[counter].ToString();
            }
        }
    }
    public void DecreaseAmount()
    {
        if (isTwoPlayer)
        {
            if (counter > 0)
            {
                counter--;
                _betAmountText.text = _2PlayerBetAmount[counter].ToString();
                _winAmountText.text = _2PlayerWinAmount[counter].ToString();
            }
        }
        else
        {
            if (counter > 0)
            {
                counter--;
                _betAmountText.text = _4PlayerBetAmount[counter].ToString();
                _winAmountText.text = _4PlayerWinAmount[counter].ToString();
            }
        }
    }

    public IEnumerator GetAllContests()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("http://135.181.200.141/~ludoprid/api/get-contests");
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
                CheckContest(webRequest);
                break;
        }
    }

    public IEnumerator JoinContest()
    {
        WWWForm form = new WWWForm();
        if (isTwoPlayer)
        {
            form.AddField("contest_id", 1);
        }
        else
        {
            form.AddField("contest_id", 2);
        }
        int amount = int.Parse(_betAmountText.text);
        form.AddField("bet_amount", amount);

        UnityWebRequest webRequest = UnityWebRequest.Post("http://135.181.200.141/~ludoprid/api/user/join-game", form);
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
                break;
        }
    }

    void CheckContest(UnityWebRequest webRequest)
    {
        UserContest _userContest = JsonUtility.FromJson<UserContest>(webRequest.downloadHandler.text);
        if (_userContest.contests.Count > 1)
        {
            _2Player.interactable = true;
            _4Player.interactable = true;
        }
        else
        {
            if(_userContest.contests[0].name == "four player")
            {
                _2Player.interactable = false;
                _4Player.interactable = true;
            }
            else
            {
                _2Player.interactable = true;
                _4Player.interactable = false;
            }
        }
    }

    [System.Serializable]
    public class Contest
    {
        public int id;
        public string name;
        public int bet_amount;
        public int player;
        public int winner;
        public bool percentage;
    }

    [System.Serializable]
    public class UserContest
    {
        public bool result;
        public string message;
        public List<Contest> contests;
        public string image_url;
    }
}
