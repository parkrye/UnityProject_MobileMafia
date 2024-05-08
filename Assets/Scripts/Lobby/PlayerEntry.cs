using Photon.Realtime;

public class PlayerEntry : SceneUI
{
    public Player _player;
    public bool _isUsing;

    public void Initialize(Player player)
    {
        base.Initialize();

        _player = player;

        if (GetText("PlayerName", out var pnText))
            pnText.text = _player.NickName;
        if (GetImage("PlayerImage", out var pImage))
            pImage.sprite = GameManager.Data._avaters[_player.GetAvatar()];

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
}