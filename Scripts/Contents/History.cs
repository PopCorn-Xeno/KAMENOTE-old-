using Kamenote.ContentViews;
using UnityEngine;

namespace Kamenote.Contents
{
    /// <summary>
    /// 直近の履歴を表示する。
    /// </summary>
    public class History : Content
    {
        #region フィールド

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private ScrollValueView recentHistory;

        [SerializeField] private GameObject archiveViewPrefab;

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
            RemoveOrder();
        }

        public override void UpdateValue()
        {
            var archive = Manager.orderArchives[^1];
            recentHistory.Add<ArchiveView>(archiveViewPrefab, $"Achive - {archive.reservation}[{archive.period}]")
                         .Initialize(true, false, false, true, true, true, false, false)
                         .Information
                         .SetText(archive);
        }

        /// <summary>
        /// 注文履歴の受け取り状況を同期する。
        /// </summary>
        public void SyncReceive()
        {
            // ApplicationManagerの注文履歴と同期する
            // リストの後ろ側から照合する
            for (int i = 1; i <= recentHistory.ContentCount; i++)
            {
                if (i <= Manager.orderArchives.Count)
                {
                    // isReceived が受け渡されたかどうかの指標
                    recentHistory.ContentAt<ArchiveView>(recentHistory.ContentCount - i)
                                 .Information.IsReceived(Manager.orderArchives[^i].isReceived);
                }
                else break;
            }
        }

        /// <summary>
        /// <see cref="ApplicationManager.orderArchives"/> のどれかが削除されたときに更新する。
        /// </summary>
        // めんどくさくなっちゃった
        public void RemoveOrder()
        {
            recentHistory.Clear();

            // ApplicationManagerの注文履歴と同期する
            // リストの後ろ側から照合する
            for (int i = 0; i < recentHistory.contentsLimit; i++)
            {
                if (i < Manager.orderArchives.Count)
                {
                    var archive = Manager.orderArchives[i];
                    recentHistory.Add<ArchiveView>(archiveViewPrefab, $"Achive - {archive.reservation}[{archive.period}]")
                                 .Initialize(true, false, false, true, true, true, false, false)
                                 .Information
                                 .SetText(archive);
                }
                else break;
            }
        }

        #endregion
    }
}
