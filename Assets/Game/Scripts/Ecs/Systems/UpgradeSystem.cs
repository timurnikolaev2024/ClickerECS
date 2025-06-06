using Game.Scripts.Data;
using Game.Scripts.Ecs.Components;
using Leopotam.EcsLite;

namespace Game.Scripts.Ecs.Systems
{
    public sealed class UpgradeSystem<TRequest, TPurchasedTag> : IEcsRunSystem
        where TRequest : struct, IUpgradeRequest
        where TPurchasedTag : struct
    {
        private readonly EcsWorld _world;
        private readonly GameCatalogSO _catalog;
        private readonly int _balEnt;
        private readonly EcsFilter _filter;
        private readonly EcsPool<TRequest> _reqPool;
        private readonly EcsPool<TPurchasedTag> _purchasedPool;
        private readonly EcsPool<BusinessId> _idPool;
        private readonly EcsPool<Balance> _balancePool;

        public UpgradeSystem(EcsWorld world, GameCatalogSO catalog, int balanceEntity)
        {
            _world = world;
            _catalog = catalog;
            _balEnt = balanceEntity;

            _filter = world.Filter<TRequest>().End();
            _reqPool = world.GetPool<TRequest>();
            _purchasedPool = world.GetPool<TPurchasedTag>();
            _idPool = world.GetPool<BusinessId>();
            _balancePool = world.GetPool<Balance>();
        }

        public void Run(IEcsSystems _) 
        {
            ref var bal = ref _balancePool.Get(_balEnt);
            foreach (int reqEnt in _filter) 
            {
                int bizId = _reqPool.Get(reqEnt).BusinessId;
                foreach (int e in _world.Filter<BusinessId>().End()) 
                {
                    if (_idPool.Get(e).Id != bizId || _purchasedPool.Has(e)) 
                        continue;

                    float price = typeof(TPurchasedTag) == typeof(Upgrade1PurchasedTag)
                        ? _catalog.Businesses[bizId].Upgrade1Cost
                        : _catalog.Businesses[bizId].Upgrade2Cost;

                    if (bal.Value >= price) 
                    {
                        bal.Value -= price;
                        _purchasedPool.Add(e);
                    }
                    break;
                }
                _reqPool.Del(reqEnt);
                _world.DelEntity(reqEnt);
            }
        }
    }
}