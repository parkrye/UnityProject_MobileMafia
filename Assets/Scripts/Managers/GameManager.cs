using UnityEngine;

public class GameManager : BaseManager
{
    private static GameManager _instance;

    private static PoolManager _poolManager;
    private static ResourceManager _resourceManager;
    private static DataManager _dataManager;
    private static SceneManager _sceneManager;
    private static AudioManager _audioManager;

    public static GameManager Instance { get { return _instance; } }
    public static PoolManager Pool { get { return _poolManager; } }
    public static ResourceManager Resource { get { return _resourceManager; } }
    public static DataManager Data { get { return _dataManager; } }
    public static SceneManager Scene { get { return _sceneManager; } }
    public static AudioManager Audio { get { return _audioManager; } }

    private void Awake()
    {
        if (_instance)
        {
            Destroy(this);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this);
        Initialize();
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    public override void Initialize()
    {
        base.Initialize();

        GameObject resourceObj = new GameObject();
        resourceObj.name = "ResourceManager";
        resourceObj.transform.parent = transform;
        _resourceManager = resourceObj.AddComponent<ResourceManager>();
        _resourceManager.Initialize();

        GameObject poolObj = new GameObject();
        poolObj.name = "PoolManager";
        poolObj.transform.parent = transform;
        _poolManager = poolObj.AddComponent<PoolManager>();
        _poolManager.Initialize();

        GameObject dataObj = new GameObject();
        dataObj.name = "DataManager";
        dataObj.transform.parent = transform;
        _dataManager = dataObj.AddComponent<DataManager>();
        _dataManager.Initialize();

        GameObject sceneObj = new GameObject();
        sceneObj.name = "SceneManager";
        sceneObj.transform.parent = transform;
        _sceneManager = sceneObj.AddComponent<SceneManager>();
        _sceneManager.Initialize();

        GameObject audioObj = new GameObject();
        audioObj.name = "AudioManager";
        audioObj.transform.parent = transform;
        _audioManager = audioObj.AddComponent<AudioManager>();
        _audioManager.Initialize();
    }
}