using Zenject;

public class ObjectPoolInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IObjectPoolService>()
            .To<ObjectPoolService>()
            .AsSingle();
    }
}