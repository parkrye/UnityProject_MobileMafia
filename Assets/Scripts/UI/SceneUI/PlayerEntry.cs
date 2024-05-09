using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEntry : SceneUI
{
    public Player _player;
    public bool _isUsing;

    private UnityEvent<int> _voteEvent = new UnityEvent<int>();

    public void InitializeInLobby(Player player)
    {
        base.Initialize();

        _player = player;

        if (GetText("PlayerName", out var pnText))
            pnText.text = _player.NickName;
        if (GetImage("PlayerImage", out var pImage))
            pImage.sprite = GameManager.Data.Avaters[_player.GetAvatar()];

        SetPlayerReady(CustomProperty.GetReady(_player));

        _isUsing = true;
    }

    public void ResetEntry()
    {
        _player = null;
        if (GetText("PlayerName", out var pnText))
            pnText.text = string.Empty;
        if (GetImage("PlayerImage", out var pImage))
            pImage.sprite = null;
        if (GetButton("Button", out var button))
            button.enabled = false;
        SetPlayerReady(false);

        _isUsing = false;
    }

    public void SetPlayerReady(bool ready)
    {
        if (GetImage("ReadyImage", out var rImage))
            rImage.gameObject.SetActive(ready);
    }

    public void Ready()
    {
        bool isPlayerReady = !CustomProperty.GetReady(_player);
        CustomProperty.SetReady(_player, isPlayerReady);

        SetPlayerReady(isPlayerReady);
    }

    public void InitializeInGame(Player player)
    {
        base.Initialize();

        _player = player;

        if (GetText("PlayerName", out var pnText))
            pnText.text = _player == null ? string.Empty : _player.NickName;
        if (GetImage("PlayerImage", out var pImage))
            pImage.sprite = _player == null ? null : GameManager.Data.Avaters[_player.GetAvatar()];
        if (GetImage("ReadyImage", out var rImage))
            rImage.gameObject.SetActive(false);
        if (GetButton("Button", out var button))
        {
            button.enabled = true;
            if (_player != null)
                button.onClick.AddListener(() => _voteEvent?.Invoke(_player.ActorNumber - 1));
        }
    }

    public void SetVoteActionOnEvent(UnityAction<int> action)
    {
        _voteEvent.RemoveAllListeners();
        _voteEvent.AddListener(action);
    }

    public void SetCountText(int count)
    {
        if (GetText("Count", out var countText))
        {
            if (count == 0)
                countText.text = string.Empty;
            else
                countText.text = $"{count}";
        }
    }

    public void OnDead()
    {
        if (GetButton("Button", out var button))
            button.enabled = false;
        if (GetText("Count", out var countText))
            countText.text = string.Empty;
        if (GetImage("BG", out var bg))
            bg.color = Color.black;
    }
}