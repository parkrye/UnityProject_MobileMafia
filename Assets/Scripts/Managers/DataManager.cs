using UnityEngine;

public class DataManager : BaseManager
{
    public string _playerName = "";
    public int _playerAvatar = 0;

    public Sprite[] _avaters;
    public GameData.PlayerState _playerState;

    public override void Initialize()
    {
        base.Initialize();

        Application.runInBackground = true;

        _avaters = new Sprite[5];
        _avaters = GameManager.Resource.LoadAll<Sprite>($"Materials/ch");
    }
}