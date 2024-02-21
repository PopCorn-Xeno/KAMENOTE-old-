using Kamenote.ContentViews;
using UnityEngine;

namespace Kamenote
{
    /// <summary>
    /// 商品情報一覧。
    /// </summary>
    public class ItemInformation : WindowContent<ItemInformation>
    {
        #region フィールド

        /// <summary>
        /// 商品一覧を表示する。
        /// </summary>
        [SerializeField] private ScrollValueView view;

        /// <summary>
        /// 商品情報を表示するプレハブ。
        /// </summary>
        [SerializeField] private GameObject itemViewPrefab;

        #endregion

        #region メソッド

        public override void Open()
        {
            substance = Manager.window.Initialize("登録商品一覧", size: new(880, 500))
                                      .SetContent<ItemInformation>(gameObject);
            substance.component.Initialize();
            substance.window.Open();
        }

        public override void Initialize()
        {
            Manager.itemDatas.ForEach
            (
                item => view.Add<ItemView>(itemViewPrefab, $"{item.name}[{item.id}]")
                            .Initialize(item)
            );
        }

        public void UpdateItem(ItemData item)
            => view.ContentAt<ItemView>(item.id).Initialize(item);
        

        /// <summary>
        /// 商品一覧から削除された商品を消す。
        /// </summary>
        /// <param name="removed">削除された商品</param>
        public void RemoveItem(ItemData removed)
            => view.Remove($"{removed.name}[{removed.id}]");

        #endregion
    }
}
