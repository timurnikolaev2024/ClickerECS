using Game.Scripts.Data;
using Game.Scripts.Ecs.Components;
using Game.Scripts.Ecs.Systems;
using Game.Scripts.Save;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game.Scripts.Ecs.Bootstrap
{
    public class EcsBootstrap : MonoBehaviour
    {
        [SerializeField] private GameCatalogSO _catalog;

        private EcsWorld _world;
        private IEcsSystems _systems;
        private int _balanceEnt;

        public EcsWorld World => _world;
        public int BalanceEntity => _balanceEnt;

        private void Awake()
        {
            _world = new EcsWorld();
            _systems = new EcsSystems(_world);
            _balanceEnt = _world.NewEntity();
            _world.GetPool<Balance>().Add(_balanceEnt).Value = 0;

            InitBusinesses();

            _systems
                .Add(new IncomeProgressSystem(_world))
                .Add(new IncomeApplySystem(_world, _catalog, _balanceEnt))
                .Add(new LevelUpSystem(_world, _balanceEnt))
                .Add(new UpgradeSystem<Upgrade1Request, Upgrade1PurchasedTag>(_world, _catalog, _balanceEnt))
                .Add(new UpgradeSystem<Upgrade2Request, Upgrade2PurchasedTag>(_world, _catalog, _balanceEnt))
                .Add(new UiPrepareSystem(_world, _catalog, _balanceEnt));

            _systems.Init();

            SaveService.Load(_world, _catalog, _balanceEnt);
        }

        private void Update()
        {
            _systems?.Run();
        }

        private void OnDestroy()
        {
            SaveService.Save(_world, _catalog, _balanceEnt);
            _systems?.Destroy();
            _world?.Destroy();
        }

        private void InitBusinesses()
        {
            for (int i = 0; i < _catalog.Businesses.Length; i++)
            {
                var config = _catalog.Businesses[i];
                int entity = _world.NewEntity();
                _world.GetPool<BusinessId>().Add(entity).Id = i;
                _world.GetPool<IncomeDelay>().Add(entity).Value = config.IncomeDelay;
                _world.GetPool<BaseCost>().Add(entity).Value = config.BaseCost;
                _world.GetPool<BaseIncome>().Add(entity).Value = config.BaseIncome;
                _world.GetPool<Progress>().Add(entity).Value = 0;
                _world.GetPool<Level>().Add(entity).Value = config.UnlockedByDefault ? 1 : 0;
            }
        }
    }
}