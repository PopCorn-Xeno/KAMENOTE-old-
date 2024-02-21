using UnityEngine;

namespace Kamenote.Menu
{
    public class Shift : WindowContent<Shift>
    {
        #region フィールド

        #endregion

        #region メソッド

        public override void Open()
        {
            substance = Manager.window.Initialize("シフト確認（β版）", size: new(960, 650))
                                      .SetContent<Shift>(gameObject);
            // substance.component.Initialize();
            substance.window.Open();
        }

        public override void Initialize()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
