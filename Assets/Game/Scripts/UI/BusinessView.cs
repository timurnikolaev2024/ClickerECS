using Game.Scripts.Data;
using Game.Scripts.Ecs.Components;
using Leopotam.EcsLite;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public sealed class BusinessView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _incomeText;
        [SerializeField] private Image _progress;
        [SerializeField] private Button _levelUpBtn;
        [SerializeField] private TextMeshProUGUI _levelUpPriceText;
        [SerializeField] private Button _up1Btn;
        [SerializeField] private TextMeshProUGUI _up1PriceText;
        [SerializeField] private Button _up2Btn;
        [SerializeField] private TextMeshProUGUI _up2PriceText;

        private EcsWorld _world;
        private GameCatalogSO _catalog;
        private int _entity;
        private EcsPool<BusinessId> _id;
        private EcsPool<Level> _lvl;
        private EcsPool<Progress> _prog;
        private EcsPool<IncomeDelay> _delay;
        private EcsPool<Upgrade1PurchasedTag> _up1;
        private EcsPool<Upgrade2PurchasedTag> _up2;
        private EcsPool<UiIncomePreview> _uiInc;
        private EcsPool<UiNextLevelPrice> _uiPrice;
        private EcsPool<UiCanBuyLevelTag> _canLvl;
        private EcsPool<UiCanBuyUp1Tag> _canUp1;
        private EcsPool<UiCanBuyUp2Tag> _canUp2;

        private int _prevLevel = -1;
        private float _prevIncome = float.NaN;
        private float _prevPrice = float.NaN;

        private void Awake()
        {
            _levelUpBtn.onClick.AddListener(OnLevelUp);
            _up1Btn.onClick.AddListener(OnUpgrade1);
            _up2Btn.onClick.AddListener(OnUpgrade2);
        }

        public void Init(EcsWorld world, int entity, GameCatalogSO catalog)
        {
            _world = world;
            _entity = entity;
            _catalog = catalog;

            _id = world.GetPool<BusinessId>();
            _lvl = world.GetPool<Level>();
            _prog = world.GetPool<Progress>();
            _delay = world.GetPool<IncomeDelay>();
            _up1 = world.GetPool<Upgrade1PurchasedTag>();
            _up2 = world.GetPool<Upgrade2PurchasedTag>();

            _uiInc = world.GetPool<UiIncomePreview>();
            _uiPrice = world.GetPool<UiNextLevelPrice>();
            _canLvl = world.GetPool<UiCanBuyLevelTag>();
            _canUp1 = world.GetPool<UiCanBuyUp1Tag>();
            _canUp2 = world.GetPool<UiCanBuyUp2Tag>();

            _nameText.SetText(_catalog.Names.BusinessNames[_id.Get(_entity).Id]);
        }

        private void Update()
        {
            if (_world == null) 
                return;

            int level = _lvl.Get(_entity).Value;
            float income = _uiInc.Get(_entity).Value;
            float price = _uiPrice.Get(_entity).Value;

            if (level != _prevLevel)
            {
                _prevLevel = level;
                _levelText.SetText("Lvl\n {0}", level);
            }

            if (float.IsNaN(_prevIncome) || Mathf.Abs(income - _prevIncome) > 0)
            {
                _prevIncome = income;

                if (level > 0)
                {
                    _incomeText.SetText("Доход: {0}$", income);
                }
                else
                {
                    var id = _id.Get(_entity).Id;
                    var baseIncome = _catalog.Businesses[id].BaseIncome;
                    _incomeText.SetText("Доход: {0}$", baseIncome);
                }
            }

            if (float.IsNaN(_prevPrice) || Mathf.Abs(price - _prevPrice) > 0.009f)
            {
                _prevPrice = price;
                _levelUpPriceText.SetText("LVL UP\n Цена: {0}$", price);
            }

            if (level == 0)
            {
                if (_progress.gameObject.activeSelf)
                    _progress.gameObject.SetActive(false);
            }
            else
            {
                if (!_progress.gameObject.activeSelf)
                    _progress.gameObject.SetActive(true);
                
                _progress.fillAmount = _prog.Get(_entity).Value / _delay.Get(_entity).Value;
            }

            _levelUpBtn.interactable = _canLvl.Has(_entity);
            _up1Btn.interactable = _canUp1.Has(_entity);
            _up2Btn.interactable = _canUp2.Has(_entity);

            var config = _catalog.Businesses[_id.Get(_entity).Id];
            
            if (_up1.Has(_entity))
            {
                _up1PriceText.SetText(
                    $"{_catalog.Names.Upgrade1Names[_id.Get(_entity).Id]}\nДоход: +{config.Upgrade1Multiplier}%\nКуплено"
                );
            }
            else
            {
                _up1PriceText.SetText(
                    $"{_catalog.Names.Upgrade1Names[_id.Get(_entity).Id]}\nДоход: +{config.Upgrade1Multiplier}%\nЦена: {config.Upgrade1Cost}$"
                );
            }
            
            if (_up2.Has(_entity))
            {
                _up2PriceText.SetText(
                    $"{_catalog.Names.Upgrade2Names[_id.Get(_entity).Id]}\nДоход: +{config.Upgrade2Multiplier}%\nКуплено"
                );
            }
            else
            {
                _up2PriceText.SetText(
                    $"{_catalog.Names.Upgrade2Names[_id.Get(_entity).Id]}\nДоход: +{config.Upgrade2Multiplier}%\nЦена: {config.Upgrade2Cost}$"
                );
            }
        }

        private void OnLevelUp() 
        {
            int e = _world.NewEntity();
            _world.GetPool<LevelUpRequest>().Add(e).BusinessId = _id.Get(_entity).Id;
        }
        
        private void OnUpgrade1() 
        {
            int e = _world.NewEntity();
            _world.GetPool<Upgrade1Request>().Add(e).BusinessId = _id.Get(_entity).Id;
        }
        
        private void OnUpgrade2() 
        {
            int e = _world.NewEntity();
            _world.GetPool<Upgrade2Request>().Add(e).BusinessId = _id.Get(_entity).Id;
        }
    }
}