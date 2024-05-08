using Photon.Pun;
using Photon.Realtime;

public class LobbySceneLobbyCanvas : SceneUI
{    
    protected override void AwakeSelf()
    {
        if (GetText("NameText", out var nText))
            nText.text = GameManager.Data._playerName;
        if (GetButton("JoinButton", out var jButton))
            jButton.onClick.AddListener(OnRandomMatchingButtonTouched);
        if (GetButton("BackButton", out var bButton))
            bButton.onClick.AddListener(OnLeaveLobbyButtonTouched);
        if (GetButton("UpAvatarButton", out var uaButton))
            uaButton.onClick.AddListener(OnUpAvatarButtonTouched);
        if (GetButton("DownAvatarButton", out var daButton))
            daButton.onClick.AddListener(OnDownAvatarButtonTouched);
    }

    void OnEnable()
    {
        PhotonNetwork.JoinLobby();
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
        GameManager.Data._playerAvatar--;
        if (GameManager.Data._playerAvatar < 0)
            GameManager.Data._playerAvatar = 4;
        ChangeAvatarImage();
    }

    void OnDownAvatarButtonTouched()
    {
        GameManager.Data._playerAvatar++;
        if (GameManager.Data._playerAvatar > 4)
            GameManager.Data._playerAvatar = 0;
        ChangeAvatarImage();
    }

    void ChangeAvatarImage()
    {
        if (GetImage("AvatarImage", out var aImage))
            aImage.sprite = GameManager.Data._avaters[GameManager.Data._playerAvatar];
        PhotonNetwork.LocalPlayer.SetAvatar(GameManager.Data._playerAvatar);
    }
}
