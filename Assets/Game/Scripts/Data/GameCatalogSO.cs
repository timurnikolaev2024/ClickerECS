using UnityEngine;

namespace Game.Scripts.Data
{
    [CreateAssetMenu(menuName = "Clicker/GameCatalog")]
    public class GameCatalogSO : ScriptableObject
    {
        [SerializeField] private BusinessConfigSO[] _businesses;
        [SerializeField] private BusinessNamesSO _names;

        public BusinessConfigSO[] Businesses => _businesses;
        public BusinessNamesSO Names => _names;
    }
}