using Game.Scripts.Data;
using Game.Scripts.Ecs.Components;
using Leopotam.EcsLite;

namespace Game.Scripts.Ecs.Systems
{
    public sealed class IncomeApplySystem : IEcsRunSystem
    {
        private readonly EcsWorld _world;
        private readonly EcsFilter _events;
        private readonly GameCatalogSO _catalog;
        private readonly int _balEnt;
        
        private readonly EcsPool<IncomeEvent> _incomeEvtPool;
        private readonly EcsPool<BusinessId> _idPool;
        private readonly EcsPool<Level> _levelPool;
        private readonly EcsPool<BaseIncome> _incomePool;
        private readonly EcsPool<Upgrade1PurchasedTag> _upgrade1Pool;
        private readonly EcsPool<Upgrade2PurchasedTag> _upgrade2Pool;
        private readonly EcsPool<Balance> _balancePool;

        public IncomeApplySystem(EcsWorld world, GameCatalogSO catalog, int balanceEntity)
        {
            _world = world;
            _catalog = catalog;
            _balEnt = balanceEntity;
            _events = world.Filter<IncomeEvent>().End();
            _incomeEvtPool = world.GetPool<IncomeEvent>();
            
            _idPool = world.GetPool<BusinessId>();
            _levelPool = world.GetPool<Level>();
            _incomePool = world.GetPool<BaseIncome>();
            _upgrade1Pool = world.GetPool<Upgrade1PurchasedTag>();
            _upgrade2Pool = world.GetPool<Upgrade2PurchasedTag>();
            _balancePool = world.GetPool<Balance>();
        }

        public void Run(IEcsSystems _) 
        {
            foreach (int evEnt in _events) 
            {
                int bizId = _incomeEvtPool.Get(evEnt).BusinessId;
                
                foreach (int e in _world.Filter<BusinessId>().End()) 
                {
                    if (_idPool.Get(e).Id != bizId) 
                        continue;
                    
                    int lvl = _levelPool.Get(e).Value;
                    float baseInc = _incomePool.Get(e).Value;
                    float mult = 1f;
                    var config = _catalog.Businesses[bizId];
                    
                    if (_upgrade1Pool.Has(e)) 
                        mult += config.Upgrade1Multiplier / 100f;
                    
                    if (_upgrade2Pool.Has(e)) 
                        mult += config.Upgrade2Multiplier / 100f;

                    float income = lvl * baseInc * mult;
                    _balancePool.Get(_balEnt).Value += income;
                    break;
                }
                
                _incomeEvtPool.Del(evEnt);
                _world.DelEntity(evEnt);
            }
        }
    }
}