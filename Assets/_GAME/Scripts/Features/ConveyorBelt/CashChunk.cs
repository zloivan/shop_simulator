using Sim.Features.InteractionSystem.Base;
using TMPro;
using UnityEngine;

namespace Sim.Features.ConveyorBeltSystem
{
    public class CashChunk : InteractableBase
    {
        [SerializeField] private int _amount;
        [SerializeField] private CurrencyType _currencyType;
        [SerializeField] private TextMeshPro _amountLabel;

        private void OnValidate()
        {
            var curIcon = _currencyType switch 
            {
                CurrencyType.Dollar => "$",
                CurrencyType.Cent => "¢",
                _ => "$"
            };
            
            _amountLabel.text = $"{curIcon}{_amount}";
        }

        public override void InteractInternal(IInteractor playerFacade)
        {
            Debug.Log($"Clicked into {_amount} {_currencyType}");
        }

        enum CurrencyType
        {
            Dollar,
            Cent
        }
    }
}