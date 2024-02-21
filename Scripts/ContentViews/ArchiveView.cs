using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

namespace Kamenote.ContentViews
{
    [RequireComponent(typeof(EventTrigger))]
    public class ArchiveView : MonoBehaviour, IContentView, IEventTriggerContent
    {
        #region フィールド・プロパティ

        [SerializeField] private bool useCondition = true;
        [SerializeField] private bool useValue = true;
        [SerializeField] private bool useSales = true;
        [SerializeField] private bool useReservation = true;
        [SerializeField] private bool useCount = true;
        [SerializeField] private bool useTime = true;
        [SerializeField] private bool useDay = true;
        [SerializeField] private bool useResetCount = false;
        [SerializeField] private bool onValidate;

        /// <summary>
        /// インタラクトされたとき（長押し）メソッド・イベント等の実行を許可するか。
        /// </summary>
        [Header("長押しによるメソッド・イベント等の実行を許可する"), SerializeField] private bool interactable = true;

        /// <summary>
        /// アーカイブの情報。
        /// </summary>
        [SerializeField] private TextView textView;

        /// <summary>
        /// インタラクトされたときに発火するイベントを登録する。
        /// </summary>
        [SerializeField] private UnityEvent<OrderArchive> onInteract;

        /// <summary>
        /// 押されているか。
        /// </summary>
        private bool onPress;

        /// <summary>
        /// 現在実行中のコルーチン<see cref="CheckLongPressing"/>を保持する。
        /// </summary>
        private Coroutine current;

        /// <summary>
        /// アーカイブの情報を表示する。
        /// </summary>
        public TextView Information => textView;

        #endregion

        #region クラス

        /// <summary>
        /// アーカイブの情報を表示する。
        /// </summary>
        [System.Serializable]
        public class TextView
        {
            /// <summary>
            /// 受け渡しの状態を表示する。
            /// </summary>
            [SerializeField] private Image condition;

            [SerializeField] private TextMeshProUGUI itemName;

            [SerializeField] private TextMeshProUGUI value;

            [SerializeField] private TextMeshProUGUI sales;

            [SerializeField] private TextMeshProUGUI reservationNumber;

            [SerializeField] private TextMeshProUGUI count;

            [SerializeField] private TextMeshProUGUI time;

            [SerializeField] private TextMeshProUGUI day;

            [SerializeField] private TextMeshProUGUI resetCount;

            [SerializeField] private Sprite uncompleted;

            [SerializeField] private Sprite completed;

            public OrderArchive Archive { get; private set; }

            public int Period { get; private set; }

            /// <summary>
            /// 初期化する。
            /// </summary>
            /// <param name="useValue"></param>
            /// <param name="useReservation"></param>
            /// <param name="useCount"></param>
            /// <param name="useTime"></param>
            /// <param name="useDay"></param>
            public void Initialize(bool useCondition, bool useValue, bool useSumValue, bool useReservation, bool useCount, bool useTime, bool useDay, bool useResetCount)
            {
                condition.gameObject.SetActive(useCondition);
                value.gameObject.SetActive(useValue);
                sales.gameObject.SetActive(useSumValue);
                reservationNumber.gameObject.SetActive(useReservation);
                count.gameObject.SetActive(useCount);
                time.gameObject.SetActive(useTime);
                day.gameObject.SetActive(useDay);
                resetCount.gameObject.SetActive(useResetCount);

                condition.sprite = uncompleted;
            }

            /// <summary>
            /// テキストを設定する。
            /// </summary>
            /// <param name="archive">注文情報</param>
            /// <param name="useYenMark">「￥」マークを使用して価格表示するか。</param>
            public TextView SetText(OrderArchive archive, bool useYenMark = true)
            {
                Archive = archive;

                itemName.text = archive.name;
                value.text = useYenMark ? $"￥{archive.value}" : $"{archive.value}";
                sales.text = useYenMark ? $"￥{archive.sales}" : $"{archive.sales}";
                reservationNumber.text = archive.reservation.ToString();
                count.text = archive.count.ToString();
                time.text = archive.time;
                day.text = archive.day.ToString();
                Period = archive.period;

                return this;
            }

            /// <summary>
            /// 商品を受け渡す。
            /// </summary>
            public TextView IsReceived(bool flag = true)
            {
                Archive.isReceived = flag;
                condition.sprite = flag ? completed : uncompleted;

                return this;
            }
        }

        #endregion

        #region MonoBehaviourメソッド

        void Awake() => Initialize();

        void OnValidate()
        {
            if (onValidate) Initialize();
        }

        #endregion

        #region メソッド

        public void Initialize() => textView.Initialize(useCondition, useValue, useSales, useReservation, useCount, useTime, useDay, useResetCount);

        /// <summary>
        /// パラメータとビューを初期化する。
        /// </summary>
        /// <param name="useValue">価格を表示するか</param>
        /// <param name="useReservation">整理券番号を表示するか</param>
        /// <param name="useCount">購入数を表示するか</param>
        /// <param name="useTime">注文された時間を表示するか</param>
        /// <param name="useDay">注文された日程を表示するか</param>
        public ArchiveView Initialize(bool useCondition, bool useValue, bool useSumValue, bool useReservation, bool useCount, bool useTime, bool useDay, bool useResetCount)
        {
            this.useValue = useValue;
            this.useReservation = useReservation;
            this.useCount = useCount;
            this.useTime = useTime;
            this.useDay = useDay;
            textView.Initialize(useCondition, useValue, useSumValue, useReservation, useCount, useTime, useDay, useResetCount);

            return this;
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

            onInteract?.Invoke(Information.Archive);
        }

        #endregion
    }
}
