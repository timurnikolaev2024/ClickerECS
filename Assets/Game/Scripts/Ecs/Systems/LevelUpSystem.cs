using Game.Scripts.Ecs.Components;
using Leopotam.EcsLite;

namespace Game.Scripts.Ecs.Systems
{
    public sealed class LevelUpSystem : IEcsRunSystem
    {
        private readonly EcsWorld _world;
        private readonly int _balEnt;

        private readonly EcsFilter _reqFilter;
        private readonly EcsPool<LevelUpRequest> _levelUpReqPool;
        private readonly EcsPool<BusinessId> _idPool;
        private readonly EcsPool<Level> _levelPool;
        private readonly EcsPool<BaseCost> _costPool;
        private readonly EcsPool<Balance> _balancePool;

        public LevelUpSystem(EcsWorld world, int balanceEntity)
        {
            _world = world;
            _balEnt = balanceEntity;

            _reqFilter = world.Filter<LevelUpRequest>().End();
            _levelUpReqPool = world.GetPool<LevelUpRequest>();
            _idPool = world.GetPool<BusinessId>();
            _levelPool = world.GetPool<Level>();
            _costPool = world.GetPool<BaseCost>();
            _balancePool = world.GetPool<Balance>();
        }

        public void Run(IEcsSystems _) 
        {
            ref var bal = ref _balancePool.Get(_balEnt);
            foreach (int reqEnt in _reqFilter) 
            {
                int bizId = _levelUpReqPool.Get(reqEnt).BusinessId;
                foreach (int e in _world.Filter<BusinessId>().End()) 
                {
                    if (_idPool.Get(e).Id != bizId) 
                        continue;
                    
                    ref var lvl = ref _levelPool.Get(e);
                    float price = (lvl.Value + 1) * _costPool.Get(e).Value;
                    
                    if (bal.Value >= price) 
                    {
                        bal.Value -= price;
                        lvl.Value++;
                    }
                    break;
                }
                _levelUpReqPool.Del(reqEnt);
                _world.DelEntity(reqEnt);
            }
        }
    }
}