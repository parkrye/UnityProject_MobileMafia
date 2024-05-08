using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPunManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private MainSessionManager _session;

    private enum Phase { Morning, Evening, Night }

    private Dictionary<int, bool> _aliveDictionary = new Dictionary<int, bool>();
    private List<int> _normalStudents = new List<int>();
    private List<int> _spyStudents = new List<int>();

    void Awake()
    {

        PhotonNetwork.LocalPlayer.SetLoad(true);

        if (!PhotonNetwork.IsMasterClient)
            return;

        StartCoroutine(StartGameRoutine());
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
            StartCoroutine(MasterWorkRoutine());
        }
    }

    private int PlayerLoadCount()
    {
        int loadCount = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetLoad())
                loadCount++;
        }
        return loadCount;
    }

    private IEnumerator StartGameRoutine()
    {
        var waitCondition = new WaitUntil(() => PlayerLoadCount() < PhotonNetwork.PlayerList.Length);
        yield return waitCondition;

        GameSetting();
    }

    private void GameSetting()
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

    private IEnumerator MasterWorkRoutine()
    {
        var countDown = new WaitForSeconds(10f);
        while (true)
        {
            yield return countDown;
            photonView.RPC("RequestSynchronizeTimer", RpcTarget.AllBufferedViaServer, _session.Timer);
        }
    }

    [PunRPC]
    private void RequestSynchronizeData(bool[] spyArray)
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

        _session.DataSynchronize();
        _session.FlowTime((int)Phase.Morning);
    }

    [PunRPC]
    private void RequestSynchronizeTimer(float time)
    {
        _session.SetTimer(time);
    }
}
