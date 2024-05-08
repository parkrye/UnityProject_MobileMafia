using Photon.Pun;
using System.Collections;
using UnityEngine;

public class TitleSceneCanvas : SceneUI
{
    private bool _quitButtonClick;

    protected override void AwakeSelf()
    {
        _quitButtonClick = false;

        if (GetButton("StartButton", out var sButton))
            sButton.onClick.AddListener(OnStartButtonTouched);
        if (GetButton("QuitButton", out var qButton))
            qButton.onClick.AddListener(OnQuitButtonTouched);
        if (GetImage("QuitImage", out var qImage))
            qImage.gameObject.SetActive(false);
    }

    void OnStartButtonTouched()
    {
        var playerName = string.Empty;
        if (GetInputField("NameInputField", out var nInputField))
            playerName = nInputField.text;

        if (playerName == "")
            playerName = $"Mob {Random.Range(1000, 5000)}";

        GameManager.Data._playerName = playerName;
        GameManager.Data._playerAvatar = 0;

        ExitGames.Client.Photon.Hashtable props = new()
        {
            { GameData.PLAYER_NAME, playerName },
            { GameData.PLAYER_AVATAR, 0 },
            { GameData.PLAYER_READY, false },
            { GameData.PLAYER_LOAD, false },
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        if (nInputField != null)
            nInputField.text = "";
        PhotonNetwork.LocalPlayer.NickName = playerName;
        PhotonNetwork.ConnectUsingSettings();
        GameManager.Scene.LoadScene("LobbyScene");
    }

    void OnQuitButtonTouched()
    {
        if (_quitButtonClick)
        {
            Application.Quit();
        }
        else
        {
            StartCoroutine(QuitButtonRoutine());
        }
    }

    IEnumerator QuitButtonRoutine()
    {
        _quitButtonClick = true;
        if (GetImage("QuitImage", out var qImage))
            qImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        _quitButtonClick = false;
        if (qImage != null)
            qImage.gameObject.SetActive(false);
    }
}
