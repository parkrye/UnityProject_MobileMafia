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
    private bool[] _aliveArray;
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
        _aliveArray = new bool[PhotonNetwork.PlayerList.Length];

        foreach (var player in PhotonNetwork.PlayerList)
        {
            _aliveArray[player.ActorNumber - 1] = true;
            if (spyArray[player.ActorNumber - 1])
                _spyStudents.Add(player.ActorNumber - 1);
            else
                _normalStudents.Add(player.ActorNumber - 1);
        }

        if (_spyStudents.Contains(PhotonNetwork.LocalPlayer.ActorNumber - 1))
            GameManager.Data._playerState = GameData.PlayerState.Spy;

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

    public void SessionChange(int session)
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        photonView.RPC("RequestSessionChange", RpcTarget.AllBufferedViaServer, session);
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

    public int GetMostVoted()
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

        return mostIndex;
    }
}
