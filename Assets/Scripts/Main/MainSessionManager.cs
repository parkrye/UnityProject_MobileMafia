using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainSessionManager : MonoBehaviour
{
    [SerializeField] private MainPunManager _pun;

    [SerializeField] private Image _timer;
    [SerializeField] private Session[] _sessions = new Session[3];
    [SerializeField] private Image _blockImage;
    [SerializeField] private ResultUI _resultUI;
    private int _currentSession;

    public float Timer { get; private set; }

    [SerializeField] private RectTransform _entryRoot;
    [SerializeField] private PlayerEntry[] _playerEntryList;
    private int _prevVoteNumber = -1;

    public void Initialize()
    {
        (_sessions[0] as MorningSession).EnableChatServer();
        (_sessions[1] as EveningSession).EmoticonEvent.AddListener(EmoticonAction);
        (_sessions[2] as NightSession).EmoticonEvent.AddListener(EmoticonAction);
        _currentSession = 0;

        _playerEntryList = _entryRoot.GetComponentsInChildren<PlayerEntry>().OrderBy(t => t.name).ToArray();
        for (int i = 0; i < _playerEntryList.Length; i++)
        {
            _playerEntryList[i].InitializeInGame(PhotonNetwork.PlayerList.Length > i ? PhotonNetwork.PlayerList[i] : null);
            _playerEntryList[i].SetVoteActionOnEvent(VoteAction);
        }

        _resultUI.gameObject.SetActive(false);
    }

    private void VoteAction(int voteNumber)
    {
        _pun.Vote(voteNumber, _prevVoteNumber);
        _prevVoteNumber = voteNumber;
    }

    public void DrawVoteCount(int[] voteCounts)
    {
        for (int i = 0; i < voteCounts.Length; i++)
        {
            _playerEntryList[i].SetCountText(voteCounts[i]);
        }
    }

    private IEnumerator TimerRoutine()
    {
        Timer = _sessions[_currentSession].Time;
        while (Timer > 0f)
        {
            yield return null;
            Timer -= Time.deltaTime;
            _timer.fillAmount = Timer / _sessions[_currentSession].Time;
        }
        Timer = 0f;

        if (PhotonNetwork.IsMasterClient)
            _pun.SessionChange(_currentSession + 1);
    }

    public void SetTimer(float time)
    {
        Timer = time;
    }

    public void SessionChange(int sessionIndex)
    {
        if (_resultUI.isActiveAndEnabled)
            return;

        if (_currentSession != sessionIndex)
        {
            _sessions[_currentSession].EndSession();
            for (int i = 0; i < _pun.AliveStudents.Length; i++)
            {
                if (_pun.AliveStudents[i] == false)
                    _playerEntryList[i].OnDead();
            }
            _currentSession = sessionIndex;
        }

        if (_currentSession == 0)
        {
            (_sessions[_currentSession] as MorningSession).EnableChatServer();
            _blockImage.gameObject.SetActive(false);
        }
        else
        {
            foreach (var entry in _playerEntryList)
            {
                entry.SetCountText(0);
            }

            if (_currentSession == 2)
            {
                if (_pun.SpyStudents.Contains(PhotonNetwork.LocalPlayer.ActorNumber - 1))
                    _blockImage.gameObject.SetActive(false);
                else
                    _blockImage.gameObject.SetActive(true);
            }
        }
        for (int i = 0; i < 3; i++)
        {
            _sessions[i].gameObject.SetActive(i == _currentSession);
        }
        _entryRoot.gameObject.SetActive(_currentSession != 0);
        _sessions[_currentSession].StartSession();
        StopAllCoroutines();
        StartCoroutine(TimerRoutine());
    }

    public void EndGame(List<int> winners, string text)
    {
        StopAllCoroutines();
        foreach (var session in _sessions)
        {
            session.gameObject.SetActive(false);
        }
        foreach (var entry in _playerEntryList)
        {
            entry.SetCountText(0);
        }
        _blockImage.gameObject.SetActive(false);
        _timer.transform.parent.gameObject.SetActive(false);
        _entryRoot.gameObject.SetActive(true);
        _resultUI.gameObject.SetActive(true);
        foreach (var winner in winners)
        {
            if (_playerEntryList[winner].GetImage("BG", out var bg))
                bg.color = Color.green;
        }
        _resultUI.SetText(text);
    }

    private void EmoticonAction(int index)
    {
        _pun.ShowEmoticon(index);
    }

    public void ShowEmoticon(int entry, int icon)
    {
        _playerEntryList[entry].ShowEmoticon(icon);
    }
}
