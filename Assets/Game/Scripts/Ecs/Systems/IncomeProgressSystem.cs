using Game.Scripts.Ecs.Components;
using Leopotam.EcsLite;
using UnityEngine;

namespace Game.Scripts.Ecs.Systems
{
    public sealed class IncomeProgressSystem : IEcsRunSystem
    {
        private readonly EcsFilter _filter;
        private readonly EcsPool<Progress> _progressPool;
        private readonly EcsPool<IncomeDelay> _delayPool;
        private readonly EcsPool<BusinessId> _idPool;
        private readonly EcsPool<Level> _levelPool;
        private readonly EcsPool<IncomeEvent> _incomeEvtPool;
        private readonly EcsWorld _world;

        public IncomeProgressSystem(EcsWorld world)
        {
            _world = world;
            _filter = world.Filter<Progress>().Inc<IncomeDelay>().Inc<BusinessId>().Inc<Level>().End();
            _progressPool = world.GetPool<Progress>();
            _delayPool = world.GetPool<IncomeDelay>();
            _idPool = world.GetPool<BusinessId>();
            _levelPool = world.GetPool<Level>();
            _incomeEvtPool = world.GetPool<IncomeEvent>();
        }

        public void Run(IEcsSystems _)
        {
            float dt = Time.deltaTime;
            foreach (int e in _filter)
            {
                if (_levelPool.Get(e).Value == 0) 
                    continue;

                ref var pr = ref _progressPool.Get(e);
                pr.Value += dt;
                float delay = _delayPool.Get(e).Value;
                
                if (pr.Value >= delay) 
                {
                    pr.Value -= delay;
                    ref var ev = ref _incomeEvtPool.Add(_world.NewEntity());
                    ev.BusinessId = _idPool.Get(e).Id;
                }
            }
        }
    }

}