using UnityEngine;

namespace Game.Scripts.Data
{
    [CreateAssetMenu(menuName = "Clicker/BusinessNamesCatalog")]
    public class BusinessNamesSO : ScriptableObject 
    {
        [SerializeField] private string[] _businessNames;
        [SerializeField] private string[] _upgrade1Names;
        [SerializeField] private string[] _upgrade2Names;
        
        public string[] BusinessNames => _businessNames;
        public string[] Upgrade1Names => _upgrade1Names;
        public string[] Upgrade2Names => _upgrade2Names;
    }
}