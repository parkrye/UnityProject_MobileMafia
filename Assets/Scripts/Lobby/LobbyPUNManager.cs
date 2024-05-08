using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LobbyPUNManager : MonoBehaviourPunCallbacks
{
    private enum Panel { Lobby, Room, Login }

    [SerializeField] private Panel _curPanel;

    [SerializeField] private LobbySceneLobbyCanvas _lobbyCanvas;
    [SerializeField] private LobbySceneRoomCanvas _roomCanvas;

    void Awake()
    {
        _curPanel = Panel.Lobby;
    }

    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
        }
        else
        {
            OnJoinedLobby();
        }
    }

    public override void OnConnectedToMaster()
    {
        SetActivePanel(Panel.Lobby);

        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetActivePanel(Panel.Login);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(Panel.Lobby);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(Panel.Lobby);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        SetActivePanel(Panel.Lobby);
    }

    public override void OnJoinedRoom()
    {
        SetActivePanel(Panel.Room);

        PhotonNetwork.AutomaticallySyncScene = true;
        _roomCanvas.UpdateRoomState();
    }

    public override void OnLeftRoom()
    {
        SetActivePanel(Panel.Lobby);

        PhotonNetwork.JoinLobby();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _roomCanvas.PlayerEnterRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _roomCanvas.PlayerLeftRoom(otherPlayer);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _roomCanvas.MasterClientSwitched(newMasterClient);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        _roomCanvas.PlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    public override void OnJoinedLobby()
    {
        SetActivePanel(Panel.Lobby);
    }

    public override void OnLeftLobby()
    {
        SetActivePanel(Panel.Login);
    }

    void SetActivePanel(Panel panel)
    {
        if(panel.Equals(Panel.Login))
        {
            GameManager.Scene.LoadScene("StartScene");
            return;
        }

        _curPanel = panel;

        _roomCanvas.gameObject?.SetActive(_curPanel == Panel.Room);
        _lobbyCanvas.gameObject.SetActive(_curPanel == Panel.Lobby);
    }
}