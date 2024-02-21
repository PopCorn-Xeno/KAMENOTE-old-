using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

namespace Kamenote.ContentViews
{
    [RequireComponent(typeof(EventTrigger))]
    public class ItemView : MonoBehaviour, IContentView, IEventTriggerContent
    {
        #region フィールド

        [SerializeField] private TextMeshProUGUI itemName;

        [SerializeField] private TextMeshProUGUI value;

        [SerializeField] private TextMeshProUGUI id;

        /// <summary>
        /// インタラクトされたとき（長押し）メソッド・イベント等の実行を許可するか。
        /// </summary>
        [Header("長押しによるメソッド・イベント等の実行を許可する"), SerializeField] private bool interactable = true;

        /// <summary>
        /// インタラクトされたときに発火するイベントを登録する。
        /// </summary>
        [Tooltip("発火したとき登録された商品情報を受け渡す"), SerializeField] private UnityEvent<ItemData> onInteract;

        /// <summary>
        /// 押下されているか。
        /// </summary>
        private bool onPress;

        /// <summary>
        /// 現在実行中のコルーチン<see cref="CheckLongPressing"/>を保持する。
        /// </summary>
        private Coroutine current;

        private ItemData item;

        #endregion

        #region メソッド

        public void Initialize() => throw new System.NotImplementedException();

        public void Initialize(ItemData item)
        {
            this.item = item;
            itemName.text = item.name;
            value.text = $"￥{item.value}";
            id.text = $"{item.id}";
        }

        // EventTrigger登録用
        public void OnPointerDown()
        {
            if (interactable) onPress = true;

            current = StartCoroutine(CheckLongPressing(1.5f));
        }

        // EventTrigger登録用
        public void OnPointerUp()
        {
            if (interactable) onPress = false;

            if (!onPress) StopCoroutine(current);
        }

        public IEnumerator CheckLongPressing(float duration)
        {
            float deltaTime = 0;
            
            if (onPress)
            {
                while (deltaTime <= duration)
                {
                    deltaTime += Time.deltaTime;

                    yield return null;
                }
            }
            else yield break;

            onInteract?.Invoke(item);
        }

        #endregion
    }
}
