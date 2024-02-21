using Kamenote.ContentViews;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace Kamenote.Contents
{
    /// <summary>
    /// 注文する。
    /// </summary>
    public class Order : Content
    {
        #region フィールド

        /// <summary>
        /// 注文メニューのドロップダウンリスト。
        /// </summary>
        [SerializeField] private TMP_Dropdown orderItemList;

        /// <summary>
        /// 整理券番号の入力フィールド。
        /// </summary>
        [SerializeField] private ReservationRegister reservationRegister;

        /// <summary>
        /// 注文する個数の表示。
        /// </summary>
        [SerializeField] private SimpleValueView orderCount;

        /// <summary>
        /// 注文確定ボタン。
        /// </summary>
        [SerializeField] private Button enter;

        /// <summary>
        /// 整理券機能を使うか。
        /// </summary>
        [SerializeField] private bool useReservation = false;

        /// <summary>
        /// 選択している整理券番号を保持する。
        /// </summary>
        [SerializeField] private int reservationNumber = 1;

        /// <summary>
        /// 選択している注文個数を保持する。
        /// </summary>
        [SerializeField] private int count = 1;

        [SerializeField] private int resetCount = 0;

        #endregion

        #region クラス

        /// <summary>
        /// 整理券番号登録機能。
        /// </summary>
        [System.Serializable]
        private class ReservationRegister
        {
            /// <summary>
            /// キャプションテキスト。
            /// </summary>
            [SerializeField] private TextMeshProUGUI caption;

            /// <summary>
            /// 整理券番号の入力フィールド。
            /// </summary>
            [SerializeField] private TMP_InputField inputField;

            /// <summary>
            /// 整理券機能が無効な場合<see cref="caption"/>に表示するメッセージ。
            /// </summary>
            private const string NOT_USE_RESERVATION = "整理券機能は無効になっています";

            /// <summary>
            /// 整理券機能が有効な場合<see cref="caption"/>に表示するメッセージ。
            /// </summary>
            private const string USE_RESERVATION = "整理券番号を入力";

            /// <summary>
            /// 番号の入力が終わった時実行するメソッドを登録する。
            /// </summary>
            /// <param name="actions"></param>
            public void RegisterOnEndEdit(params UnityAction<string>[] actions) => actions.ToList().ForEach(action => inputField.onEndEdit.AddListener(action));

            /// <summary>
            /// この機能のアクティブ状態を設定する。
            /// </summary>
            /// <param name="useReservation">整理券機能を使うか</param>
            public void SetActive(bool useReservation)
            {
                caption.text = useReservation ? USE_RESERVATION : NOT_USE_RESERVATION;
                inputField.gameObject.SetActive(useReservation);
            }

            /// <summary>
            /// 入力された番号を取得する。
            /// </summary>
            /// <param name="target">出力用変数</param>
            /// <returns>入力された番号</returns>
            public int GetNumber(out int target) => target = int.TryParse(inputField.text, out target) ? target : Reset(0);

            /// <summary>
            /// 指定した番号で入力フィールドをリセットする。
            /// </summary>
            /// <param name="number">指定した整理券番号</param>
            /// <returns></returns>
            public int Reset(int number)
            {
                inputField.text = number.ToString();
                return number;
            }
        }

        #endregion

        #region MonoBehaviourメソッド

        void Start()
        {
            Initialize();
            SetActive(Manager.shop.day1Started);
        }

        void OnValidate()
        {
            if (onValidate)
            {
                Initialize();
            }
        }

        #endregion

        #region メソッド

        public override void Initialize()
        {
            base.Initialize();

            // 注文メニューリストの初期化
            UpdateValue();

            // 整理券機能の初期化
            reservationNumber = Manager.setting.currentReservation;
            reservationRegister.SetActive(useReservation);
            reservationRegister.Reset(reservationNumber);
            reservationRegister.RegisterOnEndEdit
            (
                value => reservationNumber = reservationRegister.GetNumber(out reservationNumber),
                value =>
                {
                    if (reservationNumber == 0)
                    {
                        Manager.window.attention.Initialize("整理券番号が入力されませんでした。番号を強制リセットしますか？", false, true)
                                                .SetSelect(Reset, () => Manager.window.attention.Initialize("リセットしました。", true))
                                                .Open(0.25f);
                    }

                    else if (reservationNumber > Manager.shop.maxReservation)
                    {
                        Manager.window.attention.Initialize("整理券番号が上限を超過しました。\nリセットしますか？\n(未受け渡しの商品は受け渡し不可能になります)", false, true)
                                                .SetSelect(Reset, () => Manager.window.attention.Initialize("リセットしました。", true))
                                                .Open(0.25f);
                    }
                }
            );

            // 注文個数管理の初期化
            orderCount.Initialize("個数", count, "個", value => count = (int)value);

            // 注文確定ボタンの初期化
            enter.onClick.AddListener(Add);
            enter.onClick.AddListener(BlockOrder);
        }

        /// <summary>
        /// 注文を追加する。
        /// </summary>
        private void Add()
        {
            // マネージャーに値を渡す
            Manager.orderArchives.Add
            (
                new
                (
                    Manager.itemDatas[orderItemList.value].name,
                    count,
                    reservationNumber,
                    false,
                    Manager.itemDatas[orderItemList.value].value,
                    Manager.Time,
                    Manager.setting.currentDay,
                    resetCount
                )
            );
            Manager.setting.currentReservation = reservationNumber;
            Manager.setting.currentReset = resetCount;

            // マネージャーに直近の注文で使った整理券番号を渡した後、番号を1つ増やす
            reservationNumber = reservationRegister.Reset(reservationNumber + 1);

            if (reservationNumber > Manager.shop.maxReservation)
            {
                Manager.window.attention.Initialize("整理券番号が上限を超過しました。\nリセットしますか？\n(未受け渡しの商品は受け渡し不可能になります)", false, true)
                                        .SetSelect
                                        (
                                            Reset,
                                            () => Manager.window.attention.Initialize("リセットしました。", true),
                                            () => Manager.setting.currentReservation = reservationNumber,
                                            () => Manager.setting.currentReset = resetCount,
                                            Manager.contents.receive.UpdateValue
                                        )
                                        .Open(0.25f);
            }

            // 受け渡しの整理券番号を更新する
            Manager.contents.receive.UpdateValue();
            // 待ち状況の整理券番号のリストに追加する
            Manager.contents.reservation.UpdateValue();
            // 直近の注文履歴を取得して表示する
            Manager.contents.history.UpdateValue();
            // 注文履歴一覧に新しい注文を追加する
            Manager.menu.orderArchive.AddOrder();
        }

        /// <summary>
        /// 注文できる商品のリストを更新する。
        /// </summary>
        public override void UpdateValue()
        {
            orderItemList.ClearOptions();

            if (Manager.itemDatas.Count > 0)
            {
                orderItemList.interactable = true;
                enter.interactable = true;
                orderItemList.AddOptions(Manager.itemDatas.Select(itemData => itemData.name).ToList());
            }


            else
            {
                orderItemList.interactable = false;
                enter.interactable = false;
                orderItemList.AddOptions(new List<string>() { "商品未登録" });
            }
        }

        /// <summary>
        /// 注文に使用する整理券をリセットする。
        /// </summary>
        private void Reset()
        {
            resetCount++;
            reservationNumber = reservationRegister.Reset(1);

            Manager.setting.currentReservation = reservationNumber;
            Manager.setting.currentReset = resetCount;
        }

        /// <summary>
        /// 注文を無効にする。
        /// </summary>
        public void BlockOrder()
        {
            if (!orderItemList.interactable && !enter.interactable)
                Manager.window.attention.Initialize("商品が1つも登録されていません。\nメニューの「商品の登録」から登録してください。", false).Open();
        }

        #endregion
    }
}
