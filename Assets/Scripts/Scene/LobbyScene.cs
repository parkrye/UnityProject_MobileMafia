using System.Collections;

public class LobbyScene : BaseScene
{
    protected override IEnumerator LoadingRoutine()
    {
        yield return null;
        Progress = 1f;
    }
}

