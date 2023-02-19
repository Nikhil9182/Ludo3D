using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;
using System;
//using UnityEditor;

public class ProfileUpdater : MonoBehaviour
{
    [Header("Profile Profile")]

    [SerializeField] InputField _playerName;
    [SerializeField] InputField _emailAddress;
    [SerializeField] InputField _phoneNumber;
    [SerializeField] Text _refferalCode;
    [SerializeField] List<Image> _profileImage = new List<Image>();
    int _playerUniqueId;

    [Header("Player History")]
    [SerializeField] Text _winRate;
    [SerializeField] Text _playedMatches;
    [SerializeField] Text _winMatches;
    [SerializeField] Text _twoPlayerWins;
    [SerializeField] Text _fourPlayerWins;
    [SerializeField] Text _twoVsTwoWins;
    [SerializeField] Text _totalEarnings;

    [Header("Player Leaderboard")]
    [SerializeField] GameObject _leaderboardContent;
    [SerializeField] GameObject _playerDataPrefab;
    [SerializeField] GameObject _userLeaderboardData;


    [Header("References")]

    [SerializeField] UserAuthData _userAuthData;

    List<GameObject> _leaderboardData = new List<GameObject> ();
    Texture2D _downloadImage;

    private void Awake()
    {
        _downloadImage = new Texture2D(100, 100);
    }

    IEnumerator UpdateProfileDetails()
    {
        UnityWebRequest webRequest = UnityWebRequest.Post("http://135.181.200.141/~ludoprid/api/user/update-profile", GiveForm());
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

    private WWWForm GiveForm()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", _playerName.text);
        form.AddField("email", _emailAddress.text);
        form.AddField("phone", _phoneNumber.text);
        //form.AddField("image", _profileImage.sprite.texture.ToString());

        return form;
    }

    IEnumerator GetProfileDetails()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("http://135.181.200.141/~ludoprid/api/user/get-profile");
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
                AssignProfileDataToUI(webRequest);
                break;
        }
    }

    private void AssignProfileDataToUI(UnityWebRequest webRequest)
    {
        UserProfile playerProfile = JsonUtility.FromJson<UserProfile>(webRequest.downloadHandler.text);
        _playerName.text = playerProfile.user.name;
        _emailAddress.text = playerProfile.user.email;
        _phoneNumber.text = playerProfile.user.phone;
        _refferalCode.text = playerProfile.user.refer_code;
        _playerUniqueId = playerProfile.user.id;
        StartCoroutine(DownloadImage(playerProfile.user.image));
        StopCoroutine("DownloadImage");
    }

    private IEnumerator DownloadImage(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            _downloadImage = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
        Rect rec = new Rect(0, 0, _downloadImage.width, _downloadImage.height);
        foreach(var image in _profileImage)
        {
            image.sprite = Sprite.Create(_downloadImage, rec, new Vector2(0.5f, 0.5f));
        }

    }

    IEnumerator GetPlayerHistory()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("http://135.181.200.141/~ludoprid/api/user/get-history");
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
                AssignStatsData(webRequest);
                break;
        }
    }

    void AssignStatsData(UnityWebRequest webRequest)
    {
        StatsData playerStats = JsonUtility.FromJson<StatsData>(webRequest.downloadHandler.text);
        _winRate.text = playerStats.data.win_rate.ToString();
        _playedMatches.text = playerStats.data.played_matches.ToString();
        //_winMatches.text = playerStats.data.win_matches.ToString();
        _twoPlayerWins.text = playerStats.data.two_player_wins.ToString();
        _fourPlayerWins.text = playerStats.data.four_player_wins.ToString();
        _twoVsTwoWins.text = playerStats.data.two_vs_two_wins.ToString();
        _totalEarnings.text = playerStats.data.total_earnings.ToString();
    }

    IEnumerator GetPlayerLeaderboard()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("http://135.181.200.141/~ludoprid/api/user/get-leaderboard");
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
                AssignLeaderboardData(webRequest);
                break;
        }
    }

    private void AssignLeaderboardData(UnityWebRequest webRequest)
    {
        Leaderboard leaderboard = JsonUtility.FromJson<Leaderboard>(webRequest.downloadHandler.text);
        if (leaderboard.data.Count != _leaderboardData.Count + 1)
        {
            foreach (LeaderboardData player in leaderboard.data)
            {
                if(player.user_id != _playerUniqueId)
                {
                    GameObject _player = Instantiate(_playerDataPrefab, _leaderboardContent.transform);
                    _player.GetComponent<LeaderboardDataAssigning>().AssignText(player.name, player.rank, player.win_amount.ToString());
                    _leaderboardData.Add(_player);
                }
                else
                {
                    _userLeaderboardData.GetComponent<LeaderboardDataAssigning>().AssignText(player.name, player.rank, player.win_amount.ToString());
                }
            }
        }
        //else
        //{
        //    for(int i = 0; i < )
        //}
    }

    public void CallGetProfile() => StartCoroutine(GetProfileDetails());

    public void CallUpdateProfile() => StartCoroutine(UpdateProfileDetails());

    public void CallGetHistory() => StartCoroutine(GetPlayerHistory());

    public void CallGetLeaderboard() => StartCoroutine(GetPlayerLeaderboard());


    [System.Serializable]
    public class UserProfile
    {
        public bool result;
        public string message;
        public User user;
    }
    [System.Serializable]
    public class User
    {
        public int id;
        public string name;
        public string email;
        public string phone;
        public string image;
        public string refer_code;
    }
    [System.Serializable]
    public class Stats
    {
        public int win_rate;
        public int played_matches;
        public int win_matches;
        public int two_player_wins;
        public int four_player_wins;
        public int two_vs_two_wins;
        public int total_earnings;
    }
    [System.Serializable]
    public class StatsData
    {
        public bool result;
        public Stats data;
        public string message;
    }

    [System.Serializable]
    public class LeaderboardData
    {
        public int user_id;
        public string name;
        public string rank;
        public int win_amount;
    }
    [System.Serializable]
    public class Leaderboard
    {
        public bool result;
        public List<LeaderboardData> data = new List<LeaderboardData>();
        public string message;
    }

}
