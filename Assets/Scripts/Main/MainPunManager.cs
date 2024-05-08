using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPunManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _morningCanvas, _eveningCanvas, _nightCanvas;

    private MainChat _chat;

    private enum Time { Morning, Evening, Night}
    private Time _time;

    private Dictionary<int, bool> _aliveDictionary = new Dictionary<int, bool>();
    private List<int> _normalStudents = new List<int>();
    private List<int> _spyStudents = new List<int>();

    void Awake()
    {
        _chat = _morningCanvas.GetComponent<MainChat>();

        PhotonNetwork.LocalPlayer.SetLoad(true);

        if (!PhotonNetwork.IsMasterClient)
            return;

        StartCoroutine(GameSettingRoutine());
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected : {cause}");
        GameManager.Scene.LoadScene("LobbyScene");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // 방장 작업 대신 수행
        if (newMasterClient.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {

        }
    }

    int PlayerLoadCount()
    {
        int loadCount = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetLoad())
                loadCount++;
        }
        return loadCount;
    }

    IEnumerator GameSettingRoutine()
    {
        var waitCondition = new WaitUntil(() => PlayerLoadCount() < PhotonNetwork.PlayerList.Length);
        yield return waitCondition;

        GameSetting();
    }

    void GameSetting()
    {
        bool[] spyArray = new bool[PhotonNetwork.PlayerList.Length];

        int spyCount = (PhotonNetwork.PlayerList.Length >> 2) > 0 ? (PhotonNetwork.PlayerList.Length >> 2) : 1;

        while(spyCount > 0)
        {
            int nextSpy = Random.Range(0, PhotonNetwork.PlayerList.Length);
            if (spyArray[nextSpy])
                continue;
            spyArray[nextSpy] = true;
            spyCount--;
        }

        photonView.RPC("RequestSynchronizeData", RpcTarget.AllBufferedViaServer, spyArray);
    }

    [PunRPC]
    void RequestSynchronizeData(bool[] spyArray)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            _aliveDictionary.Add(i, true);
            if (spyArray[i])
                _spyStudents.Add(i);
            else
                _normalStudents.Add(i);
        }

        if (_spyStudents.Contains(PhotonNetwork.LocalPlayer.ActorNumber))
            GameManager.Data._playerState = GameData.PlayerState.Spy;

        _chat.EnableChatServer();
        TimeFlow(Time.Morning);
    }

    [PunRPC]
    void TimeFlow(Time _time)
    {
        this._time = _time;

        switch (this._time)
        {
            default:
            case Time.Morning:
                _morningCanvas.SetActive(true);
                _eveningCanvas.SetActive(false);
                _nightCanvas.SetActive(false);
                break;
            case Time.Evening:
                _morningCanvas.SetActive(false);
                _eveningCanvas.SetActive(true);
                _nightCanvas.SetActive(false);
                break;
            case Time.Night:
                _morningCanvas.SetActive(false);
                _eveningCanvas.SetActive(false);
                _nightCanvas.SetActive(true);
                break;
        }
    }
}
