using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainSessionManager : MonoBehaviour
{
    [SerializeField] private MainPunManager _pun;

    [SerializeField] private Image _timer;
    [SerializeField] private Session[] _sessions = new Session[3];
    private int _currentSession;

    public float Timer { get; private set; }

    public void Initialize()
    {
        (_sessions[0] as MorningSession).EnableChatServer();
        _currentSession = 0;
    }

    private IEnumerator TimerRoutine()
    {
        Timer = _sessions[_currentSession].Time;
        while (Timer > 0f)
        {
            yield return null;
            Timer -= Time.deltaTime;
            _timer.fillAmount = Timer * 0.01f;
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
        _sessions[_currentSession].StartSession();
        StartCoroutine(TimerRoutine());
    }
}
