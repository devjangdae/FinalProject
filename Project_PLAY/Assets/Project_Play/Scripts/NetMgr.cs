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

    public GameObject DisconnectPanel, LobbyPanel, RoomPanel, LeaderBoardPanel;

    [Header("Login")]
    public PlayerLeaderboardEntry MyPlayFabInfo;
    public List<PlayerLeaderboardEntry> PlayFabUserList = new List<PlayerLeaderboardEntry>();
    public InputField EmailInput, PasswordInput, UsernameInput;


    public GameObject Billboard;


    [Header("Lobby")]
    public InputField UserNickNameInput;
    public Text LobbyInfoText, UserNickNameText;//크아 로비에 있는 플레이어들 모티브
    bool isLoaded;

    public Text WelcomeText;
    public InputField RoomInput;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;


    [Header("RoomPanel")]
    public Text[] ChatText;
    public InputField ChatInput;
    public Text ListText;
    public Text RoomInfoText;

    public Button ReadyOrStartBtn;
    public Sprite[] ReadyOrStartSprite;

    [Header("ETC")]
    public Text StatusText;
    public PhotonView PV;
    public Text LogText;


    [Header("Private")]
    private bool readyDelay;
    private bool counting;
    private WaitForSeconds delay1 = new WaitForSeconds(1f);


    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //현재 포톤 상태 출력, 로비에 (1로비 1접속) 상태텍스트변경
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
        LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";
    }

    #region 로그인 playfab

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
    //로비에서만 방리스트 갱신가능 마스터->조인로비
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();
    

    // 로비 접속 PlayFabUserList 네트워크시간동안 1초 딜레이 필요
    // 로비로 돌아올 때는 딜레이없게
    public override void OnJoinedLobby()
    {
        //업데이트(콘솔)
        print("---디버그 온조인로비---");

        if (isLoaded)
        {
            ShowPanel(LobbyPanel);
            ShowUserNickName();
            myList.Clear();//빈방 제거 (버그수정)
        }
        else Invoke("OnJoinedLobbyDelay", 1);
    }

    // 닉네임을 playfab display으로 네임설정
    void OnJoinedLobbyDelay()
    {

        //업데이트(콘솔)
        print("---디버그 온조인로비딜레이---"); // 빈방제거 조인로비?(버그)

        isLoaded = true;
        PhotonNetwork.LocalPlayer.NickName = MyPlayFabInfo.DisplayName;
        PhotonNetwork.LocalPlayer.NickName = UsernameInput.text;
        WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다";
        ShowPanel(LobbyPanel);
        ShowUserNickName();
        myList.Clear();//빈방 제거 (버그수정)
    }

    // 패널 변경
    void ShowPanel(GameObject CurPanel)
    {
        LobbyPanel.SetActive(false);
        DisconnectPanel.SetActive(false);
        RoomPanel.SetActive(false);
        LeaderBoardPanel.SetActive(false);

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
    // 로비로 돌아오는것은 개인 방 떠나기  // ㄱOnDisconnected콜벡
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




    #region 방
    //방 생성
    public void CreateRoom() => PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text, new RoomOptions { MaxPlayers = 2 });
    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom(); // 빠른시작
    public void LeaveRoom() => PhotonNetwork.LeaveRoom();
    //방 조인
    public override void OnJoinedRoom()
    {
        ShowPanel(RoomPanel);
        RoomRenewal();
        ChatInput.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = ""; //채팅창도 비우기#
    }

    //방 생성실패 , 빠른시작 실패
    public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }
    public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }

    //방갱신
    void RoomRenewal()
    {
        ListText.text = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");
    }


    #endregion




    #region 채팅,RPC
    public void Send()
    {
        //방에서만 가능함 포톤 뷰 rpc PV.PRC
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
    }

    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }
    #endregion




    #region 크레이지 아케이드 방식 모티브_방리스트 갱신
    // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        // 최대페이지
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // 이전, 다음버튼
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // 페이지에 맞는 리스트 대입
        // 페이지를 대표하는 첫번째 인덱스 멀티플임
        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : ""; //방이름
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : ""; //현재인원, 최대인원
        }
    }

    //룸 리스트 업데이트 (버그수정)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //업데이트(콘솔)
        print("---디버그 룸리스트업데이트---");
        for (int i = 0; i < roomList.Count; i++)
        {
            print("방이름:" + roomList[i].Name + ", 플레이어수:" + roomList[i].PlayerCount + ", 제거될건가?" + roomList[i].RemovedFromList );
        }


        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];// 인덱스 반환 후 mylist로 넣기 갱신
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion





    //유저 상점,통계 추가개발
    public void JoinOrCreateRoom(string roomName) { }

    //통계 playfab 리더보드를 리더보드 패널에 통계 출력
    public void GetLeaderboard()
    {
        ShowPanel(LeaderBoardPanel);
        var request = new GetLeaderboardRequest { StartPosition = 0, StatisticName = "IDInfo", MaxResultsCount = 20, ProfileConstraints = new PlayerProfileViewConstraints() { ShowLocations = true, ShowDisplayName = true } };
        PlayFabClientAPI.GetLeaderboard(request, (result) =>
        {
            for (int i = 0; i < result.Leaderboard.Count; i++)
            {
                var curBoard = result.Leaderboard[i];
                LogText.text += curBoard.Profile.Locations[0].CountryCode.Value + " / " + curBoard.DisplayName + " / " + curBoard.StatValue + "\n";
            }
        },
        (error) => print("리더보드 불러오기 실패"));
    }
    public void LeaveBoardBtn()
    {
        ShowPanel(LobbyPanel);
    }

    //#################################################################################################
    #region Set Get
    private void SetRoomTag(int slotIndex, int value)
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;
        ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable();
        propertiesToSet.Add((object)slotIndex.ToString(), (object)value);
        currentRoom.SetCustomProperties(propertiesToSet, (ExitGames.Client.Photon.Hashtable)null, (WebFlags)null);
    }

    private int GetRoomTag(int slotIndex)
    {
        object customProperty = PhotonNetwork.CurrentRoom.CustomProperties[(object)slotIndex.ToString()];
        return customProperty == null ? 0 : (int)customProperty;
    }

    private Player GetPlayer(int slotIndex)
    {
        int roomTag = this.GetRoomTag(slotIndex);
        for (int index = 0; index < PhotonNetwork.PlayerList.Length; ++index)
        {
            if (PhotonNetwork.PlayerList[index].ActorNumber == roomTag)
                return PhotonNetwork.PlayerList[index];
        }
        return (Player)null;
    }

    private void SetLocalTag(string key, bool value)
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;
        ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable();
        propertiesToSet.Add((object)key, (object)value);
        localPlayer.SetCustomProperties(propertiesToSet, (ExitGames.Client.Photon.Hashtable)null, (WebFlags)null);
    }

    private bool GetLocalTag(string key)
    {
        object customProperty = PhotonNetwork.LocalPlayer.CustomProperties[(object)key];
        return customProperty != null && (bool)customProperty;
    }

    private void SetTag(string key, object value, Player player = null)
    {
        if (player == null)
            player = PhotonNetwork.LocalPlayer;
        Player player1 = player;
        ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable();
        propertiesToSet.Add((object)key, value);
        player1.SetCustomProperties(propertiesToSet, (ExitGames.Client.Photon.Hashtable)null, (WebFlags)null);
    }

    private object GetTag(string key, Player player = null)
    {
        if (player == null)
            player = PhotonNetwork.LocalPlayer;
        return player.CustomProperties[(object)key];
    }

    private bool isMaster()
    {
        return PhotonNetwork.LocalPlayer.IsMasterClient;
    }

    //private void SetItemTag()
    //{
    //    Item obj = this.AllItems.Find((Predicate<Item>)(x => x.isUsing));
    //    Player localPlayer = PhotonNetwork.LocalPlayer;
    //    ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable();
    //    propertiesToSet.Add((object)"Car", (object)obj.Name);
    //    localPlayer.SetCustomProperties(propertiesToSet, (ExitGames.Client.Photon.Hashtable)null, (WebFlags)null);
    //    this.PreviewCarImage.sprite = this.CarSprites[int.Parse(obj.Index)];
    //    for (int index = 0; index < this.ItemSlot.Length; ++index)
    //        this.ItemSlot[index].GetChild(2).gameObject.SetActive(this.AllItems[index].isUsing);
    //}
    #endregion

    //###################################################################################################################

    #region 게임시작
    public void ReadyOrStart(bool isClick)
    {
        if (isClick && this.readyDelay)
            return;

        // 방장은 준비상태 false, 사람들은 클릭시 준비상태 변경
        bool flag1 = this.GetLocalTag("isReady");
        if (this.isMaster())
        {
            flag1 = false;
            //this.MyItemBtn.interactable = true;
            this.SetLocalTag("isReady", flag1);
            if (isClick)
                this.GameStart();
        }
        else if (isClick)
        {
            flag1 = !flag1;
            this.SetLocalTag("isReady", flag1);
            this.ReadyOrStartBtn.interactable = false;
            this.readyDelay = true;
            this.Invoke("ReadyDelay", 0.5f);
        }


        // 방장이 아님 || 2인이상 모두 준비될 때 시작가능
        bool flag2 = true;
        for (int index = 0; index < PhotonNetwork.PlayerList.Length; ++index)
        {
            Player player = PhotonNetwork.PlayerList[index];
            if (!player.IsMasterClient && (this.GetTag("isReady", player) != null && !(bool)this.GetTag("isReady", player) || this.GetTag("isRepair", player) != null && (bool)this.GetTag("isRepair", player)))
            {
                flag2 = false;
                break;
            }
        }
        this.ReadyOrStartBtn.interactable = !this.isMaster() || flag2 && PhotonNetwork.PlayerList.Length > 1;
        this.ReadyOrStartBtn.GetComponent<Image>().sprite = this.isMaster() ? this.ReadyOrStartSprite[0] : (flag1 ? this.ReadyOrStartSprite[2] : this.ReadyOrStartSprite[1]);


        // 모두 준비됐고, 빈 방이 없어야 카운트 다운
        bool flag3 = false;
        for (int slotIndex = 0; slotIndex < 8; ++slotIndex)
        {
            if (this.GetRoomTag(slotIndex) == 0)
                flag3 = true;
        }
        if (flag2 && !flag3 && !this.counting)
        {
            this.StartCoroutine("Countdown");
        }
        else
        {
            if (!this.counting || !(!flag2 | flag3))
                return;
            //타이머 내려가는 중에, 취소하면
            this.counting = false;
            this.StopCoroutine("Countdown");
            this.ReadyOrStartBtn.transform.GetChild(0).gameObject.SetActive(true);
            this.ReadyOrStartBtn.transform.GetChild(1).GetComponent<Text>().text = "";
        }
    }

    private void ReadyDelay()
    {
        this.readyDelay = false;
        this.ReadyOrStartBtn.interactable = true;
    }

    private IEnumerator Countdown()
    {
        this.counting = true;
        this.ReadyOrStartBtn.transform.GetChild(0).gameObject.SetActive(false);
        Text CountdownText = this.ReadyOrStartBtn.transform.GetChild(1).GetComponent<Text>();
        CountdownText.text = "9";
        yield return (object)this.delay1;
        CountdownText.text = "8";
        yield return (object)this.delay1;
        CountdownText.text = "7";
        yield return (object)this.delay1;
        CountdownText.text = "6";
        yield return (object)this.delay1;
        CountdownText.text = "5";
        yield return (object)this.delay1;
        CountdownText.text = "4";
        yield return (object)this.delay1;
        CountdownText.text = "3";
        yield return (object)this.delay1;
        CountdownText.text = "2";
        yield return (object)this.delay1;
        CountdownText.text = "1";
        yield return (object)this.delay1;
        this.counting = false;
        this.GameStart();
    }

    private void GameStart()
    {
        if (!this.isMaster())
            return;
        this.PV.RPC("BillboardRPC", RpcTarget.All);
        for (int index = 0; index < PhotonNetwork.PlayerList.Length; ++index)
        {
            Player player = PhotonNetwork.PlayerList[index];
            ExitGames.Client.Photon.Hashtable propertiesToSet = new ExitGames.Client.Photon.Hashtable();
            propertiesToSet.Add((object)"isReady", (object)false);
            player.SetCustomProperties(propertiesToSet, (ExitGames.Client.Photon.Hashtable)null, (WebFlags)null);
        }
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(1);
    }

    [PunRPC]
    private void BillboardRPC()
    {
        this.Billboard.SetActive(true);
    }
    #endregion
}
