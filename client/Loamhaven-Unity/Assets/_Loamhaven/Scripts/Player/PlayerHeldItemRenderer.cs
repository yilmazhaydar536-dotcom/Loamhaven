using UnityEngine;
using Loamhaven.World;

namespace Loamhaven.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerHeldItemRenderer : MonoBehaviour
    {
        [SerializeField] Vector2 _handOffset = new Vector2(0.55f, 1.1f);

        SpriteRenderer _itemSr;
        int            _lastId = int.MinValue;

        void Awake()
        {
            var child = new GameObject("HeldItem");
            child.transform.SetParent(transform, false);
            child.transform.localPosition = new Vector3(_handOffset.x, _handOffset.y, 0f);

            _itemSr                  = child.AddComponent<SpriteRenderer>();
            _itemSr.sortingLayerName = "Characters";
            _itemSr.sortingOrder     = 1;
            _itemSr.enabled          = false;
        }

        public void SetEquippedItem(int itemId)
        {
            if (itemId == _lastId) return;
            _lastId = itemId;

            if (itemId <= 0) { _itemSr.enabled = false; return; }

            _itemSr.sprite  = ItemIconFactory.GetIcon(itemId);
            _itemSr.enabled = true;
        }

        public void SetFacingRight(bool right)
        {
            float x = Mathf.Abs(_handOffset.x) * (right ? 1f : -1f);
            _itemSr.transform.localPosition = new Vector3(x, _handOffset.y, 0f);
            _itemSr.flipX = !right;
        }
    }
}