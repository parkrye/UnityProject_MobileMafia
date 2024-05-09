using UnityEngine;

public class DataManager : BaseManager
{
    public string PlayerName = "";
    public int PlayerAvatar = 0;

    public Sprite[] Avaters;
    public GameData.PlayerState PlayerState;

    public override void Initialize()
    {
        base.Initialize();

        Application.runInBackground = true;

        Avaters = new Sprite[5];
        Avaters = GameManager.Resource.LoadAll<Sprite>($"Materials/ch");
    }
}