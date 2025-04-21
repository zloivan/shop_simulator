using Sim.Features.PlayerSystem.Base;
using UnityEngine;
using System;
using Sim.Features.InteractionSystem.Base;

namespace Sim.Features.PlayerSystem.PlayerConponents
{
    public class PlayerHandsController : MonoBehaviour, IPlayerComponent
    {
        [SerializeField] private Transform _handsTransform;

        private PlayerFacade _facade;
        private GameObject _itemInHands;

        public event Action<GameObject> OnItemTaken;
        public event Action<GameObject> OnItemDropped;

        public bool HasItemInHands => _itemInHands != null;

        public void Initialize(PlayerFacade facade)
        {
            _facade = facade;

            // Подписываемся на события ввода через фасад
            _facade.OnInteractPressed += HandleInteraction;

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
                var cameraTransform = _facade.PlayerCamera.transform;
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


        private void OnDisable()
        {
            if (_facade != null)
            {
                _facade.OnInteractPressed -= HandleInteraction;
            }
        }

        private void HandleInteraction(InteractionType interactionType)
        {
            if (interactionType == InteractionType.DropItem && HasItemInHands)
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

            OnItemTaken?.Invoke(item);
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
                rb.AddForce(_facade.PlayerCamera.transform.forward * 3f, ForceMode.Impulse);
            }

            if (droppedItem.TryGetComponent<Collider>(out var collider))
                collider.enabled = true;

            _itemInHands = null;
            OnItemDropped?.Invoke(droppedItem);
            return droppedItem;
        }
    }
}