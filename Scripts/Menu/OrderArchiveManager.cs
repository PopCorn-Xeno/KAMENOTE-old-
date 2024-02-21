using Kamenote.ContentViews;
using UnityEngine;
using System.Linq;

namespace Kamenote.Menu
{
    /// <summary>
    /// 注文履歴一覧を表示し、管理する。
    /// </summary>
    public class OrderArchiveManager : MonoBehaviour
    {
        #region フィールド

        [SerializeField] private bool isOpen;

        [SerializeField] private CanvasGroup canvasGroup;

        [Space, SerializeField] private ScrollValueView view;

        [SerializeField] private GameObject archiveViewMiniPrefab;

        #endregion

	    #region プロパティ

	

	    #endregion

	    #region MonoBehaviourメソッド

        void Start()
        {
            Close();
            Initialize();
        }

	    #endregion

        #region メソッド

        public void Initialize()
        {
            ApplicationManager.Instance.orderArchives.ForEach
            (
                archive => view.Add<ArchiveView>(archiveViewMiniPrefab, $"{archive.name} - [{archive.reservation}]({archive.period})")
                               .Initialize(true, false, false, true, true, false, false, false)
                               .Information
                               .SetText(archive)
            );
        }

        /// <summary>
        /// 直近の注文を追加する。
        /// </summary>
        public void AddOrder()
        {
            // 直近の注文を取得
            var archive = ApplicationManager.Instance.orderArchives[^1];
            string name = $"{archive.name} - [{archive.reservation}]({archive.period})";
            
            // アーカイブビューを作成する
            view.Add<ArchiveView>(archiveViewMiniPrefab, name)
                .Initialize(true, false, false, true, true, false, false, false)
                .Information
                .SetText(archive);
        }

        /// <summary>
        /// 注文状況を同期する。
        /// </summary>
        /// <param name="reservation">同期する注文に紐づいた整理券番号</param>
        public void SyncOrder(int reservation)
        {
            // 指定された整理券番号に該当する注文のリスト内でのインデックスを取得する
            var indices = ApplicationManager.Instance
                                      .GetRecentOrders(reservation)
                                      .Select(order => ApplicationManager.Instance.orderArchives.IndexOf(order))
                                      .ToList();

            // そのインデックスはアーカイブビューのリストのインデックスに等しい
            indices.ForEach(index => view.ContentAt<ArchiveView>(index).Information.IsReceived(true));
        }

        /// <summary>
        /// 指定したインデックスの注文履歴を削除する。
        /// </summary>
        /// <param name="index">削除したいインデックス（<see cref="ApplicationManager.orderArchives"/>のインデックスと一致する ）</param>
        public void RemoveOrder(int index)
        {
            Debug.Log(index);
            view.Remove(view.ContentAt<ArchiveView>(index).gameObject.name);
        }

        /// <summary>
        /// 注文履歴一覧を開く。
        /// </summary>
        // インスペクターで登録中
        public void Open()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            isOpen = true;
        }

        /// <summary>
        /// 注文履歴一覧を閉じる。
        /// </summary>
        private void Close()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            isOpen = false;
        }

        /// <summary>
        /// 注文履歴一覧を閉じる。
        /// </summary>
        /// <param name="trigger"><c>false</c>の時のみ実行される</param>
        // インスペクターから登録中
        public void Close(bool trigger = false)
        {
            if (!trigger) Close();
        }

        #endregion
    }
}
