using System.Collections;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class SceneManager : BaseManager
{
    private BaseScene _curScene;
    private LoadingUI _loadingUI;

    public bool ReadyToPlay { get; private set; }

    public override void Initialize()
    {
        base.Initialize();

        _loadingUI = GameManager.Pool.GetUI<LoadingUI>("UI/LoadingUI");
        GameManager.Pool.ReleaseUI(_loadingUI);
    }

    public BaseScene CurScene
    {
        get
        {
            if (!_curScene)
                _curScene = GameObject.FindObjectOfType<BaseScene>();

            return _curScene;
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadingRoutine(sceneName));
    }

    private IEnumerator LoadingRoutine(string sceneName)
    {
        ReadyToPlay = false;
        GameManager.Pool.GetUI(_loadingUI);
        yield return new WaitForSeconds(1f);
        AsyncOperation oper = UnitySceneManager.LoadSceneAsync(sceneName);
        while (!oper.isDone)
        {
            yield return null;
        }

        if (CurScene)
        {
            CurScene.LoadAsync();
            while (CurScene.Progress < 1f)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(1f);
        GameManager.Pool.ReleaseUI(_loadingUI);
        ReadyToPlay = true;
    }
}