using Kamenote.Save;
using UnityEngine;
using UnityEngine.UI;

namespace Kamenote
{
    public class ApplicationSettings : WindowContent<ApplicationSettings>
    {
        #region フィールド

        [SerializeField] private Button save;

        [SerializeField] private Button reset;

        // [SerializeField] private Button autoSave;

        #endregion

        #region メソッド

        public override void Open()
        {
            substance = Manager.window.Initialize("アプリ設定").SetContent<ApplicationSettings>(gameObject);
            substance.component.Initialize();
            substance.window.Open();
        }

        public override void Initialize()
        {
            save.onClick.AddListener
            (
                () => Manager.window.attention.Initialize("セーブしますか？", false, true)
                                              .SetSelect(Manager.Save, () => Manager.window.attention.Initialize("セーブしました。", true))
                                              .Open(0.5f)
            );

            reset.onClick.AddListener
            (
                () => Manager.window.attention.Initialize("履歴・設定などのファイルを本当に全て削除しますか？\nこの操作は元に戻すことはできません。", false, true)
                                              .SetSelect
                                              (
                                                () =>
                                                {
                                                    try { Reset(); }
                                                    catch (System.Exception) { Debug.Log("削除できませんでした。"); }
                                                },

                                                () => Manager.window.attention.Initialize("削除しました。", true)
                                              )
                                              .Open(0.5f)
            );
        }

        private void Reset()
        {
            Settings.Instance.Delete();
            ItemDatas.Instance.Delete();
            OrderArchives.Instance.Delete();
            Shop.Instance.Delete();
        }

        #endregion
    }
}
