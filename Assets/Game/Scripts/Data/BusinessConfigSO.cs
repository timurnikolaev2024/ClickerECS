using UnityEngine;

namespace Game.Scripts.Data
{
    [CreateAssetMenu(menuName = "Clicker/BusinessConfig")]
    public class BusinessConfigSO : ScriptableObject
    {
        [SerializeField] private float _incomeDelay;
        [SerializeField] private float _baseCost;
        [SerializeField] private float _baseIncome;
        [SerializeField] private bool _unlockedByDefault;

        [Header("Upgrade 1")]
        [SerializeField] private float _upgrade1Cost;
        [SerializeField] private float _upgrade1Multiplier;

        [Header("Upgrade 2")]
        [SerializeField] private float _upgrade2Cost;
        [SerializeField] private float _upgrade2Multiplier;
        
        public float IncomeDelay => _incomeDelay;
        public float BaseCost => _baseCost;
        public float BaseIncome => _baseIncome;
        public bool UnlockedByDefault => _unlockedByDefault;

        public float Upgrade1Cost => _upgrade1Cost;
        public float Upgrade1Multiplier => _upgrade1Multiplier;

        public float Upgrade2Cost => _upgrade2Cost;
        public float Upgrade2Multiplier => _upgrade2Multiplier;
    }
}