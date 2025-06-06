using Game.Scripts.Ecs.Components;
using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class BalanceView : MonoBehaviour
    {
        [SerializeField] private Text _text;
        
        private EcsPool<Balance> _balancePool;
        private int _balanceEntity;
        private float _prevBalance = float.NaN;

        public void Init(EcsWorld world, int balanceEnt)
        {
            _balancePool = world.GetPool<Balance>();
            _balanceEntity = balanceEnt;
        }

        private void Update()
        {
            float current = _balancePool.Get(_balanceEntity).Value;

            if (float.IsNaN(_prevBalance) || Mathf.Abs(current - _prevBalance) > 0)
            {
                _prevBalance = current;
                _text.text = $"Баланс: {current}$";
            }
        }
    }
}