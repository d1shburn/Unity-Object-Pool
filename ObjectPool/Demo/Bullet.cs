using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    public void OnSpawned()
    {
        // The spawn logic
    }

    public void OnDespawned()
    {
        // The despawn logic
    }
}