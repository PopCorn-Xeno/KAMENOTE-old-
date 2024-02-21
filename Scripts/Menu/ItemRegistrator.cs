using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Kamenote.Menu
{
    /// <summary>
    /// 商品の追加登録、編集を行うクラス。
    /// </summary>
    public class ItemRegistrator : WindowContent<ItemRegistrator>
    {
        #region フィールド

        /// <summary>
        /// 実行モード。
        /// </summary>
        [SerializeField] private Mode mode;

        /// <summary>
        /// 商品名を入力する。
        /// </summary>
        [SerializeField] private TMP_InputField itemName;

        /// <summary>
        /// 商品の単体価格を入力する。
        /// </summary>
        [SerializeField] private TMP_InputField itemValue;

        /// <summary>
        /// 現在設定しようとしている商品名を表示する。
        /// </summary>
        [SerializeField] private TextMeshProUGUI temporaryName;

        /// <summary>
        /// 現在設定しようとしている価格を表示する。
        /// </summary>
        [SerializeField] private TextMeshProUGUI temporaryValue;

        /// <summary>
        /// 商品を削除するボタン。
        /// </summary>
        [SerializeField] private Button remove;

        /// <summary>
        /// 商品の登録を確定するボタン。
        /// </summary>
        [SerializeField] private Button enter;

        /// <summary>
        /// 商品情報。
        /// </summary>
        private ItemData item;

        #endregion

	    #region 列挙型

        /// <summary>
        /// 実行モード。
        /// </summary>
        public enum Mode { Register, Edit }

	    #endregion

        #region メソッド

        public override void Open()
        {
            // コンテンツを初期化し、ウィンドウを開く
            substance = Manager.window.Initialize("商品の登録").SetContent<ItemRegistrator>(gameObject);
            substance.component.SetItem(item);
            substance.component.Initialize();
            substance.window.Open();
        }

        public override void Initialize()
        {
            // 追加モードか編集モードかで分岐
            switch (mode)
            {
                case Mode.Register:
                    temporaryName.text = "";
                    temporaryValue.text = "";
                    remove.gameObject.SetActive(false);
                    break;

                case Mode.Edit:
                    itemName.text = item?.name;
                    itemValue.text = item?.value.ToString();
                    temporaryName.text = item?.name;
                    temporaryValue.text = item?.value.ToString();
                    remove.gameObject.SetActive(true);
                    break;
            }

            // 入力フィールドの編集が終わった時にプレビューする
            itemName.onEndEdit.AddListener(name => temporaryName.text = name);
            itemValue.onEndEdit.AddListener(value => temporaryValue.text = value);

            // 登録・削除ボタンの挙動を設定する
            enter.onClick.AddListener(RegisterItem);
            remove.onClick.AddListener(RemoveItem);
        }

        /// <summary>
        /// 実行モードを設定する。
        /// </summary>
        /// <param name="mode">実行モード（インスペクター上ではEnumに見える）</param>
        /// インスペクターから登録中（<see cref="Tray"/>）
        [EnumAction(typeof(Mode))]
        public void SetMode(int mode) => this.mode = (Mode)mode;

        /// <summary>
        /// 商品情報を設定する。
        /// </summary>
        /// <param name="item">受け取る商品情報</param>
        public void SetItem(ItemData item) => this.item = item;

        /// <summary>
        /// 商品を登録する。
        /// </summary>
        private void RegisterItem()
        {
            // 設定しようとしている情報を確認する
            string name = temporaryName.text;
            int value = ParseValue(temporaryValue.text, "価格が入力されていないか、不正な形式です。");

            // 入力形式が誤っている場合、処理を抜ける
            if (value == -1) return;

            switch (mode)
            {
                // 追加登録モード
                case Mode.Register:

                    // 商品データのインスタンスを作成する
                    item = new(name, value, Manager.itemDatas.Count);

                    // 商品リスト内に同名、または同名かつ同価格の商品があるか調べる
                    var sameName = Manager.itemDatas.Where(item => item.name == name);
                    var sameNameAndValue = Manager.itemDatas.Where(item => item.name == name && item.value == value);

                    // 同名同価格があった場合、登録をキャンセル
                    if (sameNameAndValue.Count() == 1)
                    {
                        Manager.window.attention.Initialize("名前と価格が同じ商品が既に存在するため、登録できません。", false).Open();
                        return;
                    }
                    // 同名があった場合、上書き登録するか尋ねる
                    else if (sameName.Count() == 1)
                    {
                        Manager.window.attention.Initialize("名前が同じ商品が既に存在します。\n価格を変更しますか？", false, true)
                                                .SetSelect
                                                (
                                                    () => Manager.itemDatas[sameName.ElementAt(0).id].value = value,
                                                    () => Manager.window.attention.Initialize("価格を変更しました。", true)
                                                )
                                                .Open(0.5f);
                        return;
                    }
                    // 重複がない場合、新しくリストに追加する
                    else Manager.itemDatas.Add(item);
                                        
                    break;

                    // 編集モード
                    case Mode.Edit:
                        // 商品情報を上書きする
                        if (Manager.itemDatas.Contains(item))
                        {
                            int index = Manager.itemDatas.IndexOf(item);
                            item = new(name, value, index);
                            Manager.itemDatas[index] = item;
                            // アテンションウィンドウを表示する
                            Manager.window.attention.Initialize("情報を変更しました。", true).Open();
                            /// 商品情報（<see cref="ItemInformation"/>）ウィンドウ内のアイテムを更新する
                            Manager.window.GetContent<ItemInformation>()?.component.UpdateItem(item);
                        }

                        break;
            }

            // アテンションウィンドウを表示する
            Manager.window.attention.Initialize("商品の登録が完了しました。", true).Open();
            // 注文可能な商品情報を更新する
            Manager.contents.order.UpdateValue();
        }

        /// <summary>
        /// 商品を削除する。
        /// </summary>
        private void RemoveItem()
        {
            // 編集モードの時のみ有効になる
            if (mode == Mode.Edit)
            {
                Manager.window.attention.Initialize($"商品「{item.name}」（ID: {item.id}）を本当に削除しますか？", false, true)
                                        .SetSelect
                                        (
                                            // マネージャーのリストから削除する
                                            () => Manager.itemDatas.Remove(item),
                                            // 商品IDを再登録する
                                            () => Manager.itemDatas.ForEach(item => item.id = Manager.itemDatas.IndexOf(item)),
                                            // 商品一覧から消す
                                            () => Manager.window.GetContent<ItemInformation>().component.RemoveItem(item),
                                            // アテンションを出す
                                            () => Manager.window.attention.Initialize("削除しました。", true),
                                            // 前のウィンドウに戻る
                                            Manager.window.MovePreviousContent,
                                            // 注文可能な商品情報を更新する
                                            Manager.contents.order.UpdateValue
                                        )
                                        .Open(0.5f);
            }
        }

        #endregion
    }
}
