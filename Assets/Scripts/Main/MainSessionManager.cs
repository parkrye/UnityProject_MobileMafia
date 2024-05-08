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

    public float Timer { get; private set; }

    public void DataSynchronize()
    {
        _morning.EnableChatServer();
        StartCoroutine(TimerRoutine());
    }

    private IEnumerator TimerRoutine()
    {
        Timer = 100f;
        while (Timer > 0f)
        {
            yield return null;
            Timer -= Time.deltaTime;
            _timer.fillAmount = Timer * 0.01f;
        }
    }

    public void SetTimer(float time)
    {
        Timer = time;
    }

    public void FlowTime(int time)
    {
        if (time < 0 || time > 2)
            time = 0;

        switch (time)
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
