using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainSessionManager : MonoBehaviour
{
    [SerializeField] private MainPunManager _pun;

    [SerializeField] private Image _timer;
    [SerializeField] private MorningSession _morning;
    [SerializeField] private EveningSession _evening;
    [SerializeField] private NightSession _night;

    private int _session;

    public float Timer { get; private set; }

    public void DataSynchronize()
    {
        _morning.EnableChatServer();
        StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        Timer = 100f;
        while (true)
        {
            yield return null;
            Timer -= Time.deltaTime;
            _timer.fillAmount = Timer * 0.01f;

            if (Timer < 0f)
            {
                Timer = 100f;
                _pun.SessionChange(_session + 1);
            }
        }
    }

    public void SetTimer(float time)
    {
        Timer = time;
    }

    public void FlowTime(int session)
    {
        if (session < 0 || session > 2)
            session = 0;
        _session = session;

        switch (session)
        {
            default:
            case 0:
                _morning.gameObject.SetActive(true);
                _evening.gameObject.SetActive(false);
                _night.gameObject.SetActive(false);
                break;
            case 1:
                _morning.gameObject.SetActive(false);
                _evening.gameObject.SetActive(true);
                _night.gameObject.SetActive(false);
                break;
            case 2:
                _morning.gameObject.SetActive(false);
                _evening.gameObject.SetActive(false);
                _night.gameObject.SetActive(true);
                break;
        }
    }
}
