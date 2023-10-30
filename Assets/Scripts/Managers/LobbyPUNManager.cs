using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPUNManager : MonoBehaviourPunCallbacks
{
    public enum Panel { Lobby, Room, Login }

    [SerializeField] Panel curPanel;

    [SerializeField] LobbySceneLobbyCanvas lobbyCanvas;
    [SerializeField] LobbySceneRoomCanvas roomCanvas;

    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
        }
        else if (PhotonNetwork.InLobby)
        {
            OnJoinedLobby();
        }
        else
        {
            OnDisconnected(DisconnectCause.None);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
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
        roomCanvas.UpdateRoomState();
    }

    public override void OnLeftRoom()
    {
        SetActivePanel(Panel.Lobby);

        PhotonNetwork.JoinLobby();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        roomCanvas.PlayerEnterRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomCanvas.PlayerLeftRoom(otherPlayer);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        roomCanvas.MasterClientSwitched(newMasterClient);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        roomCanvas.PlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    public override void OnJoinedLobby()
    {
        SetActivePanel(Panel.Lobby);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (lobbyCanvas.isActiveAndEnabled)
            lobbyCanvas.UpdateRoomList(roomList);
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

        curPanel = panel;

        roomCanvas.gameObject?.SetActive(curPanel == Panel.Room);
        lobbyCanvas.gameObject.SetActive(curPanel == Panel.Lobby);
    }
}