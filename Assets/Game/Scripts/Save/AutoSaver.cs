using Game.Scripts.Ecs.Bootstrap;
using UnityEngine;

namespace Game.Scripts.Save
{
    public class AutoSaver : MonoBehaviour
    {
        [SerializeField] private EcsBootstrap _ecsBootstrap;

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                if (_ecsBootstrap != null && _ecsBootstrap.World != null && _ecsBootstrap.World.IsAlive())
                {
                    SaveService.Save(
                        _ecsBootstrap.World,
                        _ecsBootstrap.Catalog,
                        _ecsBootstrap.BalanceEntity
                    );
                }
            }
        }
    }
}