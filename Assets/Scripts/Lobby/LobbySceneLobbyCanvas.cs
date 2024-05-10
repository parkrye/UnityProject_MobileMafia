using Photon.Pun;
using Photon.Realtime;

public class LobbySceneLobbyCanvas : SceneUI
{    
    protected override void AwakeSelf()
    {
        if (GetText("NameText", out var nText))
            nText.text = GameManager.Data.PlayerName;
        if (GetButton("JoinButton", out var jButton))
            jButton.onClick.AddListener(OnRandomMatchingButtonTouched);
        if (GetButton("BackButton", out var bButton))
            bButton.onClick.AddListener(OnLeaveLobbyButtonTouched);
        if (GetButton("UpAvatarButton", out var uaButton))
            uaButton.onClick.AddListener(OnUpAvatarButtonTouched);
        if (GetButton("DownAvatarButton", out var daButton))
            daButton.onClick.AddListener(OnDownAvatarButtonTouched);
    }

    void OnLeaveLobbyButtonTouched()
    {
        GameManager.Scene.LoadScene("StartScene");
    }

    void OnRandomMatchingButtonTouched()
    {
        RoomOptions roomOptions = new()
        {
            IsOpen = true,
            MaxPlayers = 8
        };
        PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: roomOptions);
    }

    void OnUpAvatarButtonTouched()
    {
        GameManager.Data.PlayerAvatar--;
        if (GameManager.Data.PlayerAvatar < 0)
            GameManager.Data.PlayerAvatar = 4;
        ChangeAvatarImage();
    }

    void OnDownAvatarButtonTouched()
    {
        GameManager.Data.PlayerAvatar++;
        if (GameManager.Data.PlayerAvatar > 4)
            GameManager.Data.PlayerAvatar = 0;
        ChangeAvatarImage();
    }

    void ChangeAvatarImage()
    {
        if (GetImage("AvatarImage", out var aImage))
            aImage.sprite = GameManager.Data.Avaters[GameManager.Data.PlayerAvatar];
        PhotonNetwork.LocalPlayer.SetAvatar(GameManager.Data.PlayerAvatar);
    }
}
