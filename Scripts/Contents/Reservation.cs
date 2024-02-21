using Kamenote.ContentViews;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Kamenote.Contents
{
    public class Reservation : Content
    {
        #region フィールド

        /// <summary>
        /// 受け取りを待機している人数の表示。
        /// </summary>
        [SerializeField] private SimpleValueView reservedCountView;

        /// <summary>
        /// 受け取り待機状態にある整理券番号のスクロールビュー。
        /// </summary>
        [SerializeField] private ScrollValueView reservedNumbersView;

        /// <summary>
        /// 受け取り待機状態の整理券番号のリスト。
        /// </summary>
        [SerializeField] private List<int> reservedNumbers;

        /// <summary>
        /// 整理券番号を記入して表示するためのプレハブ。
        /// </summary>
        [SerializeField] private GameObject reservedPrefab;

        #endregion

        #region MonoBehaviourメソッド

        void Start()
        {
            Initialize();
            SetActive(Manager.shop.day1Started);
        }

        void OnValidate()
        {
            if (onValidate) Initialize();
        }

        #endregion

        #region メソッド

        public override void Initialize()
        {
            base.Initialize();
            var orders = Manager.orderArchives.Where(order => !order.isReceived).Select(order => order.reservation);

            reservedNumbers = orders.Distinct().ToList();
            reservedNumbers.ForEach
            (
                number => reservedNumbersView.Add<TMPro.TextMeshProUGUI>
                                              (reservedPrefab, $"Reservation ({number})").text = $"{number}"
            );
            reservedCountView.Initialize("待ち人数", reservedNumbers.Count, "人");
        }

        public override void UpdateValue()
        {
            // 待ちリストに現在の整理券番号がなかったら追加
            if (!reservedNumbers.Contains(Manager.setting.currentReservation))
            {
                reservedNumbers.Add(Manager.setting.currentReservation);
                reservedNumbersView.Add<TMPro.TextMeshProUGUI>(reservedPrefab, $"Reservation ({reservedNumbers[^1]})")
                                   .text = $"{reservedNumbers[^1]}";
            }
            // ビューを更新
            reservedCountView.UpdateValue(reservedNumbers.Count);
        }

        /// <summary>
        /// 指定された整理券番号の予約を消す。
        /// </summary>
        /// <param name="number"></param>
        public void Remove(int number)
        {
            // 指定された番号をリストから削除して、ビューを更新
            reservedNumbers.Remove(number);
            reservedNumbersView.Remove($"Reservation ({number})");
            reservedCountView.UpdateValue(reservedNumbers.Count);
        }

        #endregion
    }
}
