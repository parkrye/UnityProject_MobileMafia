using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPunManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private MainSessionManager _session;

    private enum Session { Morning, Evening, Night }

    private int[] _voteArray;
    private bool[] _aliveStudents;
    public bool[] AliveStudents { get { return _aliveStudents; } }
    private List<int> _normalStudents = new List<int>();
    private List<int> _spyStudents = new List<int>();
    public List<int> SpyStudents { get { return _spyStudents; } }

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
        _voteArray = new int[PhotonNetwork.PlayerList.Length];
        _aliveStudents = new bool[PhotonNetwork.PlayerList.Length];

        foreach (var player in PhotonNetwork.PlayerList)
        {
            _aliveStudents[player.ActorNumber - 1] = true;
            if (spyArray[player.ActorNumber - 1])
                _spyStudents.Add(player.ActorNumber - 1);
            else
                _normalStudents.Add(player.ActorNumber - 1);
        }

        if (_spyStudents.Contains(PhotonNetwork.LocalPlayer.ActorNumber - 1))
            GameManager.Data.PlayerState = GameData.PlayerState.Spy;

        _session.Initialize();
        SessionChange((int)Session.Morning);
    }

    [PunRPC]
    private void RequestSynchronizeTimer(float time)
    {
        _session.SetTimer(time);
    }

    [PunRPC]
    private void RequestSessionChange(int session)
    {
        _session.SessionChange(session);
    }

    public void SessionChange(int nextSession)
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        if (nextSession < 0 || nextSession > 2)
            nextSession = 0;

        if (nextSession != 1)
            WorkVoting();

        photonView.RPC("RequestSessionChange", RpcTarget.AllBufferedViaServer, nextSession);
    }

    public void ResetVoteData()
    {
        for(int i = 0; i < _voteArray.Length; i++)
        {
            _voteArray[i] = 0;
        }
    }

    public void Vote(int targetNumber, int prevNumber = -1)
    {
        photonView.RPC("RequestVote", RpcTarget.AllBufferedViaServer, targetNumber, prevNumber);
    }

    [PunRPC]
    private void RequestVote(int targetNumber, int prevNumber)
    {
        if (prevNumber >= 0)
            _voteArray[prevNumber]--;
        _voteArray[targetNumber]++;
        _session.DrawVoteCount(_voteArray);
    }

    public void WorkVoting()
    {
        var mostIndex = -1;
        var mostCount = 0;
        for (int i = 0; i < _voteArray.Length; i++)
        {
            if (_voteArray[i] > mostCount)
            {
                mostIndex = i;
                mostCount = _voteArray[i];
            }
        }

        if (mostIndex >= 0)
            photonView.RPC("KillPlayer", RpcTarget.AllBufferedViaServer, mostIndex);
    }

    [PunRPC]
    private void KillPlayer(int index)
    {
        if (index == PhotonNetwork.LocalPlayer.ActorNumber - 1)
            GameManager.Data.PlayerState = GameData.PlayerState.Deadman;
        _aliveStudents[index] = false;

        var normalCount = 0;
        var spyCount = 0;
        foreach (var normal in _normalStudents)
        {
            if (_aliveStudents[normal])
                normalCount++;
        }
        foreach (var spy in _spyStudents)
        {
            if (_aliveStudents[spy])
                spyCount++;
        }

        if (normalCount <= spyCount)
        {
            _session.EndGame(_spyStudents, "스파이 승리!");
        }
        else if (spyCount <= 0)
        {
            _session.EndGame(_normalStudents, "시민 승리!");
        }
    }
}
