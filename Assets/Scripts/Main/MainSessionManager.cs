using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainSessionManager : MonoBehaviour
{
    [SerializeField] private MainPunManager _pun;

    [SerializeField] private Image _timer;
    [SerializeField] private Session[] _sessions = new Session[3];
    private int _currentSession;

    public float Timer { get; private set; }

    [SerializeField] private RectTransform _entryRoot;
    [SerializeField] private PlayerEntry[] _playerEntryList;

    public void Initialize()
    {
        (_sessions[0] as MorningSession).EnableChatServer();
        _currentSession = 0;

        _playerEntryList = _entryRoot.GetComponentsInChildren<PlayerEntry>().OrderBy(t => t.name).ToArray();
        for (int i = 0; i < _playerEntryList.Length; i++)
        {
            _playerEntryList[i].InitializeInGame(PhotonNetwork.PlayerList.Length > i ? PhotonNetwork.PlayerList[i] : null);
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
        _pun.SessionChange(_currentSession + 1);
    }

    public void SetTimer(float time)
    {
        Timer = time;
    }

    public void SessionChange(int sessionIndex)
    {
        if (sessionIndex < 0 || sessionIndex > 2)
            sessionIndex = 0;

        if (_currentSession != sessionIndex)
        {
            _sessions[_currentSession].EndSession();
            _currentSession = sessionIndex;
        }
        for (int i = 0; i < 3; i++)
        {
            _sessions[i].gameObject.SetActive(i == _currentSession);
        }
        _entryRoot.gameObject.SetActive(_currentSession != 0);
        _sessions[_currentSession].StartSession();
        StartCoroutine(TimerRoutine());
    }
}
