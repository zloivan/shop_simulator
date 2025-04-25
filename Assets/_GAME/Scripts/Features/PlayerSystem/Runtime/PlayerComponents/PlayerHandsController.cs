using System;
using IKhom.EventBusSystem.Runtime;
using Sim.Features.InteractionSystem.Base;
using Sim.Features.PlayerSystem.Base;
using UnityEngine;

namespace Sim.Features.PlayerSystem.PlayerComponents
{
    public class PlayerHandsController : MonoBehaviour, IPlayerComponent
    {
        [SerializeField] private Transform _handsTransform;
        [SerializeField] private float _throwForce = 3f;

        private Player _facade;
        private GameObject _itemInHands;
        public bool HasItemInHands => _itemInHands != null;

        public void Initialize(Player facade)
        {
            _facade = facade;

            EventBus<PlayerEvents.PlayerInteractInput>.Register(
                new EventBinding<PlayerEvents.PlayerInteractInput>(HandleInteraction));

            SetupHands();
        }

        private void SetupHands()
        {
            if (_handsTransform == null)
            {
                var handsObj = new GameObject("HandsPosition");
                _handsTransform = handsObj.transform;
                _handsTransform.SetParent(transform);

                // Размещаем перед игроком
                var cameraTransform = _facade.LookController.Camera.transform;
                if (cameraTransform != null)
                {
                    _handsTransform.position = cameraTransform.position + cameraTransform.forward * 0.5f;
                }
                else
                {
                    _handsTransform.localPosition = new Vector3(0, 0, 1f);
                }
            }
        }

        private void HandleInteraction(PlayerEvents.PlayerInteractInput playerInteractInput)
        {
            if (playerInteractInput.InteractionType == InteractionType.DropItem && HasItemInHands)
            {
                DropItem();
            }
        }

        public bool TakeItem(GameObject item)
        {
            Debug.Log("Taking item: " + item.name);
            if (HasItemInHands)
                return false;

            _itemInHands = item;

            // Отключаем физику
            if (item.TryGetComponent<Rigidbody>(out var rb))
                rb.isKinematic = true;

            if (item.TryGetComponent<Collider>(out var collider))
                collider.enabled = false;

            // Помещаем в руки
            item.transform.SetParent(_handsTransform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
            EventBus<PlayerEvents.OnItemTaken>.Raise(new PlayerEvents.OnItemTaken(item));
           // OnItemTaken?.Invoke(item);
            return true;
        }

        public GameObject DropItem()
        {
            if (!HasItemInHands)
                return null;

            Debug.Log("Dropping item: " + _itemInHands.name);
            var droppedItem = _itemInHands;
            droppedItem.transform.SetParent(null);

            // Возвращаем физику
            if (droppedItem.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = false;
                rb.AddForce(_facade.LookController.Camera.transform.forward * _throwForce, ForceMode.Impulse);
            }

            if (droppedItem.TryGetComponent<Collider>(out var collider))
                collider.enabled = true;

            _itemInHands = null;
            EventBus<PlayerEvents.ItemDropped>.Raise(new PlayerEvents.ItemDropped(droppedItem));
           // OnItemDropped?.Invoke(droppedItem);
            return droppedItem;
        }
    }
}