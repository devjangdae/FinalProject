using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 로그인 playfab이용
using PlayFab;
using PlayFab.ClientModels;

// 로비, 매칭 photon이용
using Photon.Pun;


// PUN 콜백이용 로그인
public class NetMgr : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update

    public GameObject DisconnectPanel;

    [Header("Login")]
    public PlayerLeaderboardEntry MyPlayFabInfo;
    public List<PlayerLeaderboardEntry> PlayFabUserList = new List<PlayerLeaderboardEntry>();
    public InputField EmailInput, PasswordInput, UsernameInput;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // 로그인 - 이메일, pw, username 필요
    public void Login()
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { print("로그인 성공함"); GetLeaderboard(result.PlayFabId); PhotonNetwork.ConnectUsingSettings(); }, (error) => print("아,, 로그인 실패함"));
    }

    // 회원가입 - 이메일, pw, username, username, displayname 필요 
    public void Register()
    {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text, DisplayName = UsernameInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => { print("회원가입 성공함"); SetStat(); SetData("default"); }, (error) => print("아,, 회원가입 실패함"));
    }

    // playfab에 IDInfo 저장
    void SetStat()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "IDInfo", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => print("아,, 값 저장실패"));
    }

    // NetMgr playfab 유저리스트에서 비교
    void GetLeaderboard(string myID)
    {
        // -비우기   로그인 다시할때 필요 
        PlayFabUserList.Clear(); 

        // 1000명까지 가능
        for (int i = 0; i < 10; i++)
        {
            var request = new GetLeaderboardRequest
            {
                StartPosition = i * 100,
                StatisticName = "IDInfo",
                MaxResultsCount = 100,
                ProfileConstraints = new PlayerProfileViewConstraints() { ShowDisplayName = true }
            };

            //호출결과를 리더보드에 넣
            PlayFabClientAPI.GetLeaderboard(request, (result) =>
            {
                if (result.Leaderboard.Count == 0) return;
                for (int j = 0; j < result.Leaderboard.Count; j++)
                {
                    // -채우기 ( 플레이어 리더보드 엔트리 )
                    PlayFabUserList.Add(result.Leaderboard[j]);
                    if (result.Leaderboard[j].PlayFabId == myID) MyPlayFabInfo = result.Leaderboard[j];
                }
            },
            (error) => { });
        }
    }

    // playfab 플레이어 데이터 저장
    void SetData(string curData)
    {
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() { { "Home", curData } },
            Permission = UserDataPermission.Public
        };
        PlayFabClientAPI.UpdateUserData(request, (result) => { print("플레이어 데이터 저장 성공함"); }, (error) => print("아,, 플레이어 데이터 저장 실패")) ;
    }
}
