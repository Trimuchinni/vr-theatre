using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;


public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher launcherInstance;

    private void Awake()
    {
        launcherInstance = this;
    }

    public GameObject loadingScreen;

    public TMP_Text loadingText;

    public GameObject menuButtons;

    public GameObject createRoomScreen;
    public TMP_InputField roomNameInput;

    public GameObject roomScreen;
    public TMP_Text roomNameText,playerNameLabel;
    private List<TMP_Text> allPlayerNames = new List<TMP_Text>();

    public GameObject errorScreen;
    public TMP_Text errorText;

    public GameObject roomBrowserScreen;
    public RoomButton roomButton;
    private List<RoomButton> allRoomButtons = new List<RoomButton>();


    public GameObject nameInputScreen;
    public TMP_InputField nameInput;
    private bool hasSetNickName;

    public string levelToPlay;

    public GameObject startButton;
    // Start is called before the first frame update
    void Start()
    {
        closeMenus();

        loadingScreen.SetActive(true);

        loadingText.text = "Connecting to network...";

        PhotonNetwork.ConnectUsingSettings();
    }

    void closeMenus()
    {
        loadingScreen.SetActive(false);

        menuButtons.SetActive(false);

        createRoomScreen.SetActive(false);

        roomScreen.SetActive(false);

        errorScreen.SetActive(false);

        roomBrowserScreen.SetActive(false);

        nameInputScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
       

        PhotonNetwork.JoinLobby();

        PhotonNetwork.AutomaticallySyncScene = true;

        loadingText.text = "Joining to lobby .....";
    }

    public override void OnJoinedLobby()
    {
        closeMenus();

        menuButtons.SetActive(true);

        PhotonNetwork.NickName = Random.Range(0, 1000).ToString();

        if(!hasSetNickName)
        {
            closeMenus();
            nameInputScreen.SetActive(true);

            if(PlayerPrefs.HasKey("playerName"))
            {
                nameInput.text = PlayerPrefs.GetString("playerName");
            }

        }
        else
        {
            PhotonNetwork.NickName=PlayerPrefs.GetString("playerName");
        }
    }
    public void openRoomCreate()
    {
        closeMenus();

        createRoomScreen.SetActive(true);
    }
    
    public void createRoom()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;

        if(!string.IsNullOrEmpty(roomNameInput.text))
        {
            PhotonNetwork.CreateRoom(roomNameInput.text,options);

            closeMenus();

            loadingText.text = "Creating Room...";
            loadingScreen.SetActive(true);
        }
    }

    public override void OnJoinedRoom()
    {
        closeMenus();

        roomScreen.SetActive(true);

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        listAllPlayers();

        if(PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }

    private void listAllPlayers()
    {
        foreach(TMP_Text player in allPlayerNames)
        {
            Destroy(player.gameObject);
        }
        allPlayerNames.Clear();

        Player[] players = PhotonNetwork.PlayerList;
        for(int i=0;i<players.Length;i++)
        {
            TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
            newPlayerLabel.text = players[i].NickName;
            newPlayerLabel.gameObject.SetActive(true);

            allPlayerNames.Add(newPlayerLabel);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
        newPlayerLabel.text = newPlayer.NickName;
        newPlayerLabel.gameObject.SetActive(true);

        allPlayerNames.Add(newPlayerLabel);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        listAllPlayers();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Failed to create room:" + message;
        closeMenus();
        errorScreen.SetActive(true);
    }

    public void closeErrorScreen()
    {
        closeMenus();
        menuButtons.SetActive(true);
    }

    public void leaveRoom()
    {

        PhotonNetwork.LeaveRoom();
        closeMenus();
        loadingText.text = "leaving room..";
        loadingScreen.SetActive(true);
        
    }

    public override void OnLeftRoom()
    {
        closeMenus();
        menuButtons.SetActive(true);
    }

    public void openRoomBrowser()
    {
        closeMenus();
        roomBrowserScreen.SetActive(true);
    }

    public void closeRoomBrowser()
    {
        closeMenus();
        menuButtons.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomButton rb in allRoomButtons)
        {
            Destroy(rb.gameObject);
        }
        allRoomButtons.Clear();


        roomButton.gameObject.SetActive(false);

        Debug.Log(roomList.Count);
        for(int i=0;i<roomList.Count;i++)
        {
            if(roomList[i].PlayerCount!=roomList[i].MaxPlayers && !roomList[i].RemovedFromList)
            {
                RoomButton newButton = Instantiate(roomButton, roomButton.transform.parent);
                
                newButton.setButtonDetails(roomList[i]);
                //Debug.Log(newButton.buttonText.text);
                newButton.gameObject.SetActive(true);

                allRoomButtons.Add(newButton);
            }
        }
    }
    
    public void joinRoom(RoomInfo inputInfo)
    {
        PhotonNetwork.JoinRoom(inputInfo.Name);

        closeMenus();
        loadingText.text = "Joining Room...";
        loadingScreen.SetActive(true);
    }

    public void setNickName()
    {
        if(!string.IsNullOrEmpty(nameInput.text))
        {
            PhotonNetwork.NickName = nameInput.text;
            PlayerPrefs.SetString("playerName", nameInput.text);

            closeMenus();
            menuButtons.SetActive(true);

            hasSetNickName = true;
        }
    }

    public void startGame()
    {
        PhotonNetwork.LoadLevel(levelToPlay);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
