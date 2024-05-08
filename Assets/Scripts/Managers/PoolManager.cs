using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    private Dictionary<string, ObjectPool<GameObject>> _poolDic = new Dictionary<string, ObjectPool<GameObject>>();
    private Dictionary<string, Transform> _poolContainer = new Dictionary<string, Transform>();
    private Canvas _canvasRoot;
    private Transform _sceneTransform;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        _canvasRoot = GameManager.Resource.Instantiate<Canvas>("UI/Canvas");
        _canvasRoot.name = "CanvasRoot";
        _canvasRoot.transform.SetParent(transform, false);
    }

    public void Reset()
    {
        _poolDic = new Dictionary<string, ObjectPool<GameObject>>();
        _poolContainer = new Dictionary<string, Transform>();
    }

    private Transform SceneTransform()
    {
        if (_sceneTransform)
        {
            return _sceneTransform;
        }
        else
        {
            _sceneTransform = GameManager.Resource.Instantiate<Transform>("Container");
            return _sceneTransform;
        }
    }

    public T Get<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
    {
        if (original is GameObject)
        {
            GameObject prefab = original as GameObject;
            string key = prefab.name;

            if (!_poolDic.ContainsKey(key))
                CreatePool(key, prefab);

            GameObject obj = _poolDic[key].Get();
            obj.transform.parent = parent;
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj as T;
        }
        else if (original is Component)
        {
            Component component = original as Component;
            string key = component.gameObject.name;

            if (!_poolDic.ContainsKey(key))
                CreatePool(key, component.gameObject);

            GameObject obj = _poolDic[key].Get();
            obj.transform.parent = parent;
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj.GetComponent<T>();
        }
        else
        {
            return null;
        }
    }

    public T Get<T>(T original, Vector3 position, Quaternion rotation) where T : Object
    {
        return Get<T>(original, position, rotation, null);
    }

    public T Get<T>(T original, Transform parent) where T : Object
    {
        return Get<T>(original, Vector3.zero, Quaternion.identity, parent);
    }

    public T Get<T>(T original) where T : Object
    {
        return Get<T>(original, Vector3.zero, Quaternion.identity, null);
    }

    public bool Release<T>(T instance) where T : Object
    {
        if (instance is GameObject)
        {
            GameObject go = instance as GameObject;
            string key = go.name;

            if (!_poolDic.ContainsKey(key))
                return false;

            try
            {
                _poolDic[key].Release(go);
            }
            catch
            {
                return false;
            }
            return true;
        }
        else if (instance is Component)
        {
            Component component = instance as Component;
            string key = component.gameObject.name;

            if (!_poolDic.ContainsKey(key))
                return false;

            try
            {
                _poolDic[key].Release(component.gameObject);
            }
            catch
            {
                return false;
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsContain<T>(T original) where T : Object
    {
        if (original is GameObject)
        {
            GameObject prefab = original as GameObject;
            string key = prefab.name;

            if (_poolDic.ContainsKey(key))
                return true;
            else
                return false;

        }
        else if (original is Component)
        {
            Component component = original as Component;
            string key = component.gameObject.name;

            if (_poolDic.ContainsKey(key))
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }

    private void CreatePool(string key, GameObject prefab)
    {
        GameObject root = new GameObject();
        root.gameObject.name = $"{key}Container";
        root.transform.parent = transform;
        _poolContainer.Add(key, root.transform);

        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject obj = GameManager.Resource.Instantiate(prefab);
                obj.gameObject.name = key;
                return obj;
            },
            actionOnGet: (GameObject obj) =>
            {
                obj.gameObject.SetActive(true);
                if (!obj.GetComponent<RectTransform>())
                    obj.transform.parent = SceneTransform();
            },
            actionOnRelease: (GameObject obj) =>
            {
                obj.gameObject.SetActive(false);
                obj.transform.parent = _poolContainer[key];
            },
            actionOnDestroy: (GameObject obj) =>
            {
                GameManager.Resource.Destroy(obj);
            }
        );
        _poolDic.Add(key, pool);
    }

    public T GetUI<T>(T original, Vector3 position) where T : Object
    {
        if (original is GameObject)
        {
            GameObject prefab = original as GameObject;
            string key = prefab.name;

            if (!_poolDic.ContainsKey(key))
                CreateUIPool(key, prefab);

            GameObject obj = _poolDic[key].Get();
            obj.transform.position = position;
            return obj as T;
        }
        else if (original is Component)
        {
            Component component = original as Component;
            string key = component.gameObject.name;

            if (!_poolDic.ContainsKey(key))
                CreateUIPool(key, component.gameObject);

            GameObject obj = _poolDic[key].Get();
            obj.transform.position = position;
            return obj.GetComponent<T>();
        }
        else
        {
            return null;
        }
    }

    public T GetUI<T>(T original) where T : Object
    {
        if (original is GameObject)
        {
            GameObject prefab = original as GameObject;
            string key = prefab.name;

            if (!_poolDic.ContainsKey(key))
                CreateUIPool(key, prefab);

            GameObject obj = _poolDic[key].Get();
            return obj as T;
        }
        else if (original is Component)
        {
            Component component = original as Component;
            string key = component.gameObject.name;

            if (!_poolDic.ContainsKey(key))
                CreateUIPool(key, component.gameObject);

            GameObject obj = _poolDic[key].Get();
            return obj.GetComponent<T>();
        }
        else
        {
            return null;
        }
    }

    public T GetUI<T>(string path) where T : Object
    {
        T original = GameManager.Resource.Load<T>(path);
        if (original is GameObject)
        {
            GameObject prefab = original as GameObject;
            string key = prefab.name;

            if (!_poolDic.ContainsKey(key))
                CreateUIPool(key, prefab);

            GameObject obj = _poolDic[key].Get();
            return obj as T;
        }
        else if (original is Component)
        {
            Component component = original as Component;
            string key = component.gameObject.name;

            if (!_poolDic.ContainsKey(key))
                CreateUIPool(key, component.gameObject);

            GameObject obj = _poolDic[key].Get();
            return obj.GetComponent<T>();
        }
        else
        {
            return null;
        }
    }

    public bool ReleaseUI<T>(T instance) where T : Object
    {
        if (instance is GameObject)
        {
            GameObject go = instance as GameObject;
            string key = go.name;

            if (!_poolDic.ContainsKey(key))
                return false;

            try
            {
                _poolDic[key].Release(go);
            }
            catch
            {
                return false;
            }
            return true;
        }
        else if (instance is Component)
        {
            Component component = instance as Component;
            string key = component.gameObject.name;

            if (!_poolDic.ContainsKey(key))
                return false;

            try
            {
                _poolDic[key].Release(component.gameObject);
            }
            catch
            {
                return false;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CreateUIPool(string key, GameObject prefab)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject obj = GameManager.Resource.Instantiate(prefab);
                obj.gameObject.name = key;
                return obj;
            },
            actionOnGet: (GameObject obj) =>
            {
                obj.gameObject.SetActive(true);
            },
            actionOnRelease: (GameObject obj) =>
            {
                obj.gameObject.SetActive(false);
                obj.transform.SetParent(_canvasRoot.transform, false);
            },
            actionOnDestroy: (GameObject obj) =>
            {
                GameManager.Resource.Destroy(obj);
            }
        );
        _poolDic.Add(key, pool);
    }
}