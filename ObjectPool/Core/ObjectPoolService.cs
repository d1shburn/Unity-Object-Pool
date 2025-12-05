using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObjectPoolService : IObjectPoolService
{
    private readonly IInstantiator _instantiator;

    private readonly Dictionary<GameObject, Stack<GameObject>> _pools = new();
    private readonly Dictionary<int, GameObject> _origins = new();
    private readonly Dictionary<GameObject, Transform> _groups = new();

    private Transform _container;

    public ObjectPoolService(IInstantiator instantiator)
    {
        _instantiator = instantiator;

        CreateContainer();
    }

    public void Prewarm(GameObject prefab, int count)
    {
        ValidatePool(prefab);

        for (int i = 0; i < count; i++)
        {
            GameObject instance = CreateInstance(prefab);
            ReturnToPool(instance, prefab);
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        ValidatePool(prefab);

        GameObject instance = null;

        var stack = _pools[prefab];

        while (stack.Count > 0)
        {
            instance = stack.Pop();

            if (instance != null)
                break;
        }

        if (instance == null)
            instance = CreateInstance(prefab);

        _origins[instance.GetInstanceID()] = prefab;

        Transform transform = instance.transform;

        transform.SetPositionAndRotation(position, rotation);
        transform.SetParent(parent != null ? parent : _groups[prefab]);

        instance.SetActive(true);

        foreach (var poolable in instance.GetComponents<IPoolable>())
            poolable.OnSpawned();

        return instance;
    }

    public void Despawn(GameObject instance)
    {
        if (instance == null)
            return;

        int identifier = instance.GetInstanceID();

        if (_origins.TryGetValue(identifier, out GameObject prefab) == false)
        {
            Object.Destroy(instance);
            return;
        }

        _origins.Remove(identifier);
        ReturnToPool(instance, prefab);
    }

    private GameObject CreateInstance(GameObject prefab)
    {
        Transform group = _groups[prefab];
        GameObject instance = _instantiator.InstantiatePrefab(prefab, group);
        
        instance.SetActive(false);
        return instance;
    }

    private void ReturnToPool(GameObject instance, GameObject prefab)
    {
        foreach (var poolable in instance.GetComponents<IPoolable>())
            poolable.OnDespawned();

        instance.SetActive(false);

        Transform group = _groups[prefab];

        if (instance.transform.parent != group)
            instance.transform.SetParent(group);

        _pools[prefab].Push(instance);
    }

    private void ValidatePool(GameObject prefab)
    {
        if (_pools.ContainsKey(prefab))
            return;

        _pools.Add(prefab, new Stack<GameObject>());

        GameObject poolGroup = new GameObject($"Pool_{prefab.name}");
        poolGroup.transform.SetParent(_container);

        _groups.Add(prefab, poolGroup.transform);
    }

    private void CreateContainer()
    {
        if (_container != null)
            return;

        var containerObject = new GameObject("[ObjectPool]");
        Object.DontDestroyOnLoad(containerObject);
        _container = containerObject.transform;
    }
}