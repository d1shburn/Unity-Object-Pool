# Object Pool for Unity 🎮
Implementation of the Object Pool pattern with support for different prefab types using Zenject dependency injection.

## ✨ Features
- 🎯 Multi-type Support - Pool different prefab types simultaneously
- 📊 Organized Hierarchy - Automatic grouping in scene view
- ⚡ Performance Ready - Minimal allocations at runtime
- 🔧 Zenject Integration - Seamless dependency injection
- 🔄 Lifecycle Hooks - IPoolable interface for setup/cleanup
- 🏗️ Clean Architecture - Interface-based design

## 📦 Installation
- Ensure Zenject is installed in your project
- Clone or copy the ObjectPool folder to your Assets/ directory
- Add the included installer to your scene or project context

## 🚀 Quick Start
Implement the IPoolable interface for lifecycle management:

```
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
```

Inject and use the pool service in your components:

```
public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;

    private IObjectPoolService _pool;

    [Inject]
    public void Construct(IObjectPoolService pool) =>
        _pool = pool;

    private void Start()
    {
        // You can create 5 bullets in advance
        _pool.Prewarm(_bulletPrefab, 5);
    }

    public void Shoot()
    {
        // Here's the logic of spawn bullets
        var bullet = _pool.Spawn(
            _bulletPrefab, _firePoint.position, _firePoint.rotation);

        // Launching a coroutine to return the bullet to the pool
        StartCoroutine(DespawnRoutine(bullet));
    }

    private IEnumerator DespawnRoutine(GameObject target)
    {
        yield return new WaitForSeconds(2f);

        // Returning the bullet to the pool
        _pool.Despawn(target);
    }
}
```
