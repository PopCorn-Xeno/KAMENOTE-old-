using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Kamenote.Menu
{
    public class ShopSetting : WindowContent<ShopSetting>
    {
        #region フィールド

        /// <summary>
        /// 出店クラス名を入力する。
        /// </summary>
        [SerializeField] private TMP_InputField className;

        /// <summary>
        /// 現在の年を入力する。
        /// </summary>
        [SerializeField] private TMP_InputField year;

        /// <summary>
        /// 模擬店名を入力する。
        /// </summary>
        [SerializeField] private TMP_InputField shopName;

        /// <summary>
        /// 模擬店名を入力する。
        /// </summary>
        [SerializeField] private TMP_InputField maxReservation;

        /// <summary>
        /// 現在設定しようとしているクラス名を表示する。
        /// </summary>
        [SerializeField] private TextMeshProUGUI temporaryClassName;

        /// <summary>
        /// 現在設定しようとしている年を表示する。
        /// </summary>
        [SerializeField] private TextMeshProUGUI temporaryYear;

        /// <summary>
        /// 現在設定しようとしている模擬店名を表示する。
        /// </summary>
        [SerializeField] private TextMeshProUGUI temporaryStoreName;

        /// <summary>
        /// 現在設定しようとしている模擬店名を表示する。
        /// </summary>
        [SerializeField] private TextMeshProUGUI temporaryMaxReservation;

        /// <summary>
        /// 設定を確定するボタン。
        /// </summary>
        [SerializeField] private Button apply;

        #endregion

        #region メソッド

        public override void Open()
        {
            substance = Manager.window.Initialize("模擬店準備").SetContent<ShopSetting>(gameObject);
            substance.component.Initialize();
            substance.window.Open();
        }

        public override void Initialize()
        {
            className.text = Manager.shop.className;
            year.text = Manager.shop.GetYear();
            shopName.text = Manager.shop.shopName;
            maxReservation.text = Manager.shop.maxReservation > 0 
                                         ? Manager.shop.maxReservation.ToString()
                                         : "1";

            temporaryClassName.text = Manager.shop.className;
            temporaryYear.text = Manager.shop.year ?? Manager.shop.GetYear();
            temporaryStoreName.text = Manager.shop.shopName;
            temporaryMaxReservation.text = Manager.shop.maxReservation.ToString();

            className.onEndEdit.AddListener(value => temporaryClassName.text = value);
            year.onEndEdit.AddListener(value => temporaryYear.text = value);
            shopName.onEndEdit.AddListener(value => temporaryStoreName.text = value);
            maxReservation.onEndEdit.AddListener(value => temporaryMaxReservation.text = value);

            apply.onClick.AddListener(Apply);
            apply.onClick.AddListener(Manager.other.information.UpdateValue);
        }

        private void Apply()
        {
            string className = temporaryClassName.text;
            string year = temporaryYear.text;
            string storeName = temporaryStoreName.text;
            int maxReservation = ParseValue(temporaryMaxReservation.text, "不正な形式です。");
            
            if (maxReservation <= 0)
            {
                Manager.window.attention.Initialize("上限は1以上の数を指定してください。", false).Open();
                return;
            }

            if (className == null || className == "")
            {
                Manager.window.attention.Initialize("クラス名が入力されていません。", false).Open();
                return;
            }
            else if (year == null || year == "")
            {
                Manager.window.attention.Initialize("年が入力されていません。", false).Open();
                return;
            }
            else if (storeName == null || storeName == "")
            {
                string temp = $"{temporaryYear.text} - {temporaryClassName.text}";

                Manager.window.attention.Initialize($"模擬店名が入力されていません。\n「{temp}」として名前を自動生成しますか？", false, true)
                                        .SetSelect
                                        (
                                            () => Manager.shop = new(className, temp, maxReservation, year),
                                            () => Manager.window.attention.Initialize("適用しました。", true),
                                            Manager.other.information.UpdateValue
                                        )
                                        .Open(0.5f);
                return;
            }

            Manager.shop = new(className, storeName, maxReservation, year);

            Manager.window.attention.Initialize("適用しました。", true).Open();
        }

        #endregion
    }
}
