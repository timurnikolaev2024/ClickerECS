using Game.Scripts.Data;
using Game.Scripts.Ecs.Components;
using Leopotam.EcsLite;

namespace Game.Scripts.Ecs.Systems
{
public sealed class UiPrepareSystem : IEcsRunSystem
{
    private readonly GameCatalogSO _catalog;
    private readonly int _balanceEntity;

    private readonly EcsPool<BusinessId> _idPool;
    private readonly EcsPool<Level> _levelPool;
    private readonly EcsPool<BaseIncome> _incomePool;
    private readonly EcsPool<Upgrade1PurchasedTag> _upgrade1Pool;
    private readonly EcsPool<Upgrade2PurchasedTag> _upgrade2Pool;
    private readonly EcsPool<UiIncomePreview> _uiIncomePool;
    private readonly EcsPool<UiNextLevelPrice> _uiPricePool;
    private readonly EcsPool<UiCanBuyLevelTag> _uiTagLevelPool;
    private readonly EcsPool<UiCanBuyUp1Tag> _uiCanBuy1Pool;
    private readonly EcsPool<UiCanBuyUp2Tag> _uiCanBuy2Pool;
    private readonly EcsPool<Balance> _balancePool;

    private readonly EcsFilter _filter;

    public UiPrepareSystem(EcsWorld world, GameCatalogSO catalog, int balanceEntity)
    {
        _catalog = catalog;
        _balanceEntity  = balanceEntity;

        _idPool = world.GetPool<BusinessId>();
        _levelPool = world.GetPool<Level>();
        _incomePool = world.GetPool<BaseIncome>();
        _upgrade1Pool = world.GetPool<Upgrade1PurchasedTag>();
        _upgrade2Pool = world.GetPool<Upgrade2PurchasedTag>();

        _uiIncomePool = world.GetPool<UiIncomePreview>();
        _uiPricePool = world.GetPool<UiNextLevelPrice>();
        _uiTagLevelPool = world.GetPool<UiCanBuyLevelTag>();
        _uiCanBuy1Pool = world.GetPool<UiCanBuyUp1Tag>();
        _uiCanBuy2Pool = world.GetPool<UiCanBuyUp2Tag>();
        _balancePool = world.GetPool<Balance>();

        _filter = world.Filter<BusinessId>().Inc<Level>().Inc<BaseIncome>().End();
    }

    public void Run(IEcsSystems _) 
    {
        float balance = _balancePool.Get(_balanceEntity).Value;

        foreach (int e in _filter) 
        {
            int id = _idPool.Get(e).Id;
            var config = _catalog.Businesses[id];
            int lvl = _levelPool.Get(e).Value;

            float mult = 1f;
            
            if (_upgrade1Pool.Has(e))
                mult += config.Upgrade1Multiplier / 100f;
            
            if (_upgrade2Pool.Has(e)) 
                mult += config.Upgrade2Multiplier / 100f;
            
            float income = lvl * _incomePool.Get(e).Value * mult;
            if (!_uiIncomePool.Has(e))
                _uiIncomePool.Add(e);
            _uiIncomePool.Get(e).Value = income;

            float price = (lvl + 1) * config.BaseCost;
            if (!_uiPricePool.Has(e))
                _uiPricePool.Add(e);
            _uiPricePool.Get(e).Value = price;

            SetTag(_uiTagLevelPool, e, balance >= price);
            SetTag(_uiCanBuy1Pool, e, !_upgrade1Pool.Has(e) && balance >= config.Upgrade1Cost && lvl > 0);
            SetTag(_uiCanBuy2Pool, e, !_upgrade2Pool.Has(e) && balance >= config.Upgrade2Cost && lvl > 0);
        }
    }

    private static void SetTag<T>(EcsPool<T> pool, int e, bool present) where T : struct 
    {
        if (present) 
        {
            if (!pool.Has(e)) 
                pool.Add(e);
        } 
        else 
        {
            if (pool.Has(e)) 
                pool.Del(e);
        }
    }
}
}