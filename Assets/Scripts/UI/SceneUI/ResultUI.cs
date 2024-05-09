using Photon.Pun;

public class ResultUI : SceneUI
{
    protected override void AwakeSelf()
    {
        base.AwakeSelf();

        if (GetButton("QuitButton", out var qButton))
        {
            qButton.onClick.AddListener(() => PhotonNetwork.LeaveRoom());
        }
    }

    public void SetText(string text)
    {
        if (GetText("ResultText", out var rText))
        {
            rText.text = text;
        }
    }
}
