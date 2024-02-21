using UnityEngine;

namespace Kamenote.Menu
{
    public class Credit : WindowContent<Credit>
    {
        #region フィールド

        #endregion

        #region メソッド

        public override void Open()
        {
            substance = Manager.window.Initialize("クレジット")
                                      .SetContent<Credit>(gameObject);
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
