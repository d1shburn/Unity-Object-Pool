using UnityEngine;

public interface IObjectPoolService
{
    void Prewarm(GameObject prefab, int count);

    GameObject Spawn(GameObject prefab, Vector3 position,
        Quaternion rotation, Transform parent = null);
    
    void Despawn(GameObject instance);
}