using Grid;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameSceneLifeTimeScope : LifetimeScope
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private BlockObjectPool _blockObjectPool;

    protected override void Configure(IContainerBuilder builder)
    {
        // 씬의 하이어라키(Hierarchy)에서 GridManager 컴포넌트를 찾아
        // 싱글톤으로 등록합니다.
        builder.RegisterInstance(_gridManager)
                .As<GridManager>();

        builder.RegisterInstance(_blockObjectPool)
                .As<BlockObjectPool>();
    }
}
