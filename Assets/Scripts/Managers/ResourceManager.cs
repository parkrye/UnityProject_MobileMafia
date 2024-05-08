using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 풀을 이용한 인스턴스 생성 및 삭제
/// </summary>
public class ResourceManager : BaseManager
{
    private Dictionary<string, Object> _resources = new Dictionary<string, Object>();

    public T[] LoadAll<T>(string path) where T : Object
    {
        T[] allResource = Resources.LoadAll<T>(path);
        return allResource;
    }

    public T Load<T>(string path) where T : Object
    {
        string key = $"{typeof(T)}.{path}";

        if (_resources.ContainsKey(key))
            return _resources[key] as T;

        T resource = Resources.Load<T>(path);
        _resources.Add(key, resource);
        return resource;
    }

    public T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent, bool pooling = false) where T : Object
    {
        if (pooling)
            return GameManager.Pool.Get(original, position, rotation, parent);
        else
            return Object.Instantiate(original, position, rotation, parent);
    }

    public T Instantiate<T>(T original, Vector3 position, Quaternion rotation, bool pooling = false) where T : Object
    {
        return Instantiate<T>(original, position, rotation, null, pooling);
    }

    public new T Instantiate<T>(T original, Transform parent, bool pooling = false) where T : Object
    {
        return Instantiate<T>(original, Vector3.zero, Quaternion.identity, parent, pooling);
    }

    public T Instantiate<T>(T original, bool pooling = false) where T : Object
    {
        return Instantiate<T>(original, Vector3.zero, Quaternion.identity, null, pooling);
    }

    public T Instantiate<T>(string path, Vector3 position, Quaternion rotation, Transform parent, bool pooling = false) where T : Object
    {
        T original = Load<T>(path);
        return Instantiate<T>(original, position, rotation, parent, pooling);
    }

    public T Instantiate<T>(string path, Vector3 position, Quaternion rotation, bool pooling = false) where T : Object
    {
        return Instantiate<T>(path, position, rotation, null, pooling);
    }

    public T Instantiate<T>(string path, Transform parent, bool pooling = false) where T : Object
    {
        return Instantiate<T>(path, Vector3.zero, Quaternion.identity, parent, pooling);
    }

    public T Instantiate<T>(string path, bool pooling = false) where T : Object
    {
        return Instantiate<T>(path, Vector3.zero, Quaternion.identity, null, pooling);
    }

    public T InstantiateDontDestroyOnLoad<T>(string path, Transform parent = null, bool pooling = false) where T : Object
    {
        T instantiated = Instantiate<T>(path, Vector3.zero, Quaternion.identity, parent, pooling);
        DontDestroyOnLoad(instantiated);
        return instantiated;
    }

    public void Destroy(GameObject go, float delay = 0f)
    {
        if (go)
        {
            if (GameManager.Pool.IsContain(go))
                StartCoroutine(DelayReleaseRoutine(go, delay));
            else
                GameObject.Destroy(go, delay);
        }
    }

    private IEnumerator DelayReleaseRoutine(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (go)
        {
            GameManager.Pool.Release(go);
        }
    }

    public void Destroy(Component component, float delay = 0f)
    {
        Destroy(component.gameObject, delay);
    }
}