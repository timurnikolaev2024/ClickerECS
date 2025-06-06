using Game.Scripts.Data;
using Game.Scripts.Ecs.Bootstrap;
using Game.Scripts.Ecs.Components;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.UI
{
    public class UiSpawner : MonoBehaviour
    {
        [SerializeField] private EcsBootstrap _bootstrap;
        [SerializeField] private GameCatalogSO _catalog;
        [SerializeField] private BusinessView _itemPrefab;
        [SerializeField] private Transform _parent;
        [SerializeField] private BalanceView _balanceView;

        private void Start()
        {
            EcsWorld world = _bootstrap.World;
            int balEnt = _bootstrap.BalanceEntity;
            _balanceView.Init(world, balEnt);
            
            foreach (int entity in world.Filter<BusinessId>().End())
            {
                var view = Instantiate(_itemPrefab, _parent);
                view.Init(world, entity, _catalog);
            }
        }
    }

}