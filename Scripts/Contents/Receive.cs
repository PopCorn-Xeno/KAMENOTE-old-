using Kamenote.ContentViews;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Kamenote.Contents
{
    public class Receive : Content
    {
        #region フィールド

        [SerializeField] private SimpleValueView reservationSelecter;

        [SerializeField] private TMP_InputField resetCountInput;

        [SerializeField] private Button enter;

        private int resetCount;

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

            resetCount = Manager.setting.currentReset;
            ResetInputField(resetCount);
            reservationSelecter.Initialize("整理券番号を選択", Manager.setting.currentReservation, "", value => Manager.setting.currentReservation = (int)value);

            resetCountInput.onEndEdit.AddListener(value => GetResetCount(out resetCount));
            resetCountInput.onEndEdit.AddListener
            (
                value =>
                {
                    if (resetCount > Manager.setting.currentReset || resetCount < 0)
                    {
                        Manager.window.attention.Initialize("入力されたリセット回数が現状をオーバーしています。強制的に戻します。", false).Open();
                    }
                }
            );
            enter.onClick.AddListener(ReceiveOrder);
        }

        public override void UpdateValue()
        {
            reservationSelecter.UpdateValue(Manager.setting.currentReservation);
            resetCountInput.text = Manager.setting.currentReset.ToString();
        }

        private void ReceiveOrder()
        {
            // 直近の注文からまだ受け取りが済んでいないものを抽出する
            var uncomplishedOrders = Manager.GetRecentOrders(Manager.setting.currentReservation, resetCount)
                                            .Where(order => !order.isReceived)
                                            .ToList();

            // 該当する直近の注文が見つかった場合
            if (uncomplishedOrders.Count > 0)
            {
                uncomplishedOrders.ForEach(order => order.Receive());

                // 注文回数を加算
                Manager.contents.sales.UpdateValue();
                // 現在の整理券番号をリストから削除
                Manager.contents.reservation.Remove((int)reservationSelecter.Value);
                // 注文履歴を同期する
                Manager.contents.history.SyncReceive();
                // 注文履歴一覧を同期する
                Manager.menu.orderArchive.SyncOrder((int)reservationSelecter.Value);
            }
            else
                Manager.window.attention.Initialize("該当する注文が見つかりません。\n整理券番号やリセット回数が正しいか確認してください。", false).Open();
        }

        /// <summary>
        /// 入力された番号を取得する。
        /// </summary>
        /// <param name="target">出力用変数</param>
        /// <returns>入力された番号</returns>
        private int GetResetCount(out int target) => target = int.TryParse(resetCountInput.text, out target)
                                                 ? target
                                                 : ResetInputField(Manager.setting.currentReset);

        /// <summary>
        /// 指定した番号で入力フィールドをリセットする。
        /// </summary>
        /// <param name="number">指定した整理券番号</param>
        /// <returns></returns>
        private int ResetInputField(int number)
        {
            resetCountInput.text = number.ToString();
            return number;
        }

        #endregion
    }
}
