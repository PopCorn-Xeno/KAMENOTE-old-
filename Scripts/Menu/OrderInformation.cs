using Kamenote.ContentViews;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Kamenote.Menu
{
    /// <summary>
    /// 注文履歴に登録されている情報を閲覧する。
    /// </summary>
    public class OrderInformation : WindowContent<OrderInformation>
    {
        #region フィールド

        /// <summary>
        /// 注文キャンセルするためのボタン。
        /// </summary>
        [SerializeField] private Button cancel;

        /// <summary>
        /// 注文情報を表示する。
        /// </summary>
        [SerializeField] private ArchiveView archiveView;

        /// <summary>
        /// 受け渡し状況をテキストで表示する。
        /// </summary>
        [SerializeField] private TextMeshProUGUI conditionText;

        /// <summary>
        /// 注文情報を保持するフィールド。
        /// </summary>
        private OrderArchive order;

        #endregion

        #region メソッド

        public override void Open()
        {
            substance = Manager.window.Initialize("注文情報").SetContent<OrderInformation>(gameObject);
            substance.component.SetOrder(order);
            substance.component.Initialize();
            substance.window.Open();
        }

        public override void Initialize()
        {
            archiveView.Information.SetText(order).IsReceived(order.isReceived);
            conditionText.text = order.isReceived ? "受け渡し完了" : "未受け渡し";
            cancel.onClick.AddListener(CancelOrder);
        }

        /// <summary>
        /// 注文情報を設定する。
        /// </summary>
        /// <param name="order"></param>
        // インスペクターから登録中
        public void SetOrder(OrderArchive order) => this.order = order;

        /// <summary>
        /// 注文をキャンセルする。
        /// </summary>
        private void CancelOrder()
        {
            if (!order.isReceived)
            {
                Manager.window.attention.Initialize($"整理券番号 {order.reservation} の商品 {order.name} の受け渡しを本当に取り消しますか？", false, true)
                                        .SetSelect(RemoveOrder, Manager.window.Close)
                                        .Open(0.5f);
            }
            else Manager.window.attention.Initialize("既に受け渡しが完了しているため取り消しできません。", false).Open();
        }

        /// <summary>
        /// 注文を削除する。
        /// </summary>
        /// <remarks>
        /// 他クラスのメソッドも一括実行する。
        /// </remarks>
        private void RemoveOrder()
        {
            int target = Manager.orderArchives.IndexOf(order);
            // マネージャーのアーカイブを削除する
            Manager.orderArchives.Remove(order);

            // 削除する注文と同じ整理券番号が1つも残っていなかったら待ちリストから削除する
            var sameReservation = Manager.GetRecentOrders(order.reservation, order.period);
            if (sameReservation.Count == 0) Manager.contents.reservation.Remove(order.reservation);

            // 注文履歴を同期する
            Manager.contents.history.RemoveOrder();
            // 注文履歴一覧を同期する
            Manager.menu.orderArchive.RemoveOrder(target);
        }

        #endregion
    }
}
