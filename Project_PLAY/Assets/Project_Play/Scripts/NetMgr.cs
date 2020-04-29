using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 로그인 playfab이용
using PlayFab;
using PlayFab.ClientModels;

// 로비, 매칭 photon이용
using Photon.Pun;
using Photon.Realtime;


// PUN 콜백이용 로그인
public class NetMgr : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update

    public GameObject DisconnectPanel, LobbyPanel;

    [Header("Login")]
    public PlayerLeaderboardEntry MyPlayFabInfo;
    public List<PlayerLeaderboardEntry> PlayFabUserList = new List<PlayerLeaderboardEntry>();
    public InputField EmailInput, PasswordInput, UsernameInput;

    [Header("Lobby")]
    public InputField UserNickNameInput;
    public Text LobbyInfoText, UserNickNameText;

    bool isLoaded;

    //void Start()
    //{

    //}

    // Update is called once per frame
    //void Update()
    //{
    //    LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";
    //}

    #region 

    //네트워크속도
    void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }
    // 로그인 - 이메일, pw, username 필요
    public void Login()
    {
        var request = new LoginWithEmailAddressRequest { Email = EmailInput.text, Password = PasswordInput.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, (result) => { Debug.Log("로그인 성공함"); GetLeaderboard(result.PlayFabId); PhotonNetwork.ConnectUsingSettings(); }, (error) => Debug.Log("아,, 로그인 실패함"));
    }

    // 회원가입 - 이메일, pw, username, username, displayname 필요 
    public void Register()
    {
        var request = new RegisterPlayFabUserRequest { Email = EmailInput.text, Password = PasswordInput.text, Username = UsernameInput.text, DisplayName = UsernameInput.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, (result) => { Debug.Log("회원가입 성공함"); SetStat(); SetData("default"); }, (error) => Debug.Log("아,, 회원가입 실패함"));
    }

    // playfab에 IDInfo 저장
    void SetStat()
    {
        var request = new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "IDInfo", Value = 0 } } };
        PlayFabClientAPI.UpdatePlayerStatistics(request, (result) => { }, (error) => Debug.Log("아,, 값 저장실패"));
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
        PlayFabClientAPI.UpdateUserData(request, (result) => { Debug.Log("플레이어 데이터 저장 성공함"); }, (error) => Debug.Log("아,, 플레이어 데이터 저장 실패")) ;
    }
    #endregion




    #region 로비
    // 로비
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    // 로비 접속 PlayFabUserList 네트워크시간동안 1초 딜레이 필요
    // 로비로 돌아올 때는 딜레이없게
    public override void OnJoinedLobby()
    {
        if (isLoaded)
        {
            ShowPanel(LobbyPanel);
            ShowUserNickName();
        }
        else Invoke("OnJoinedLobbyDelay", 1);
    }

    // 닉네임을 display으로 네임설정
    void OnJoinedLobbyDelay()
    {
        isLoaded = true;
        PhotonNetwork.LocalPlayer.NickName = MyPlayFabInfo.DisplayName;

        ShowPanel(LobbyPanel);
        ShowUserNickName();
    }

    // 패널 변경
    void ShowPanel(GameObject CurPanel)
    {
        LobbyPanel.SetActive(false);
        DisconnectPanel.SetActive(false);

        CurPanel.SetActive(true);
    }

    // 유저 닉네임 text변경
    void ShowUserNickName()
    {
        UserNickNameText.text = "";
        for (int i = 0; i < PlayFabUserList.Count; i++) UserNickNameText.text += PlayFabUserList[i].DisplayName + "\n";
    }

    // 뒤로가기 버튼
    // 로비에서 시작화면은 로그아웃
    // 로비로 돌아오는것은 개인 방 떠나기
    public void XBtn()
    {
        if (PhotonNetwork.InLobby) PhotonNetwork.Disconnect();
        else if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
    }

    // 로비에서 시작화면으로 로그아웃시 초기화, 패널끄기, 시작화면이동(추가)
    public override void OnDisconnected(DisconnectCause cause)
    {

        isLoaded = false;
        ShowPanel(DisconnectPanel);

    }
    #endregion


    //유저 방
    public void JoinOrCreateRoom(string roomName) { }



}
