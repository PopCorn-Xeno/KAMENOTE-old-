using UnityEngine;

namespace Kamenote
{
    /// <summary>
    /// ウィンドウに表示するコンテンツの抽象基底クラス。
    /// </summary>
    /// <typeparam name="T">このクラスを継承したクラス（ウィンドウに表示するコンテンツ）</typeparam>
    public abstract class WindowContent<T> : MonoBehaviour where T : WindowContent<T>
    {
        #region フィールド

        /// <summary>
        /// ウィンドウ本体と内包するコンテンツを保持するフィールド。
        /// </summary>
        protected Window.Substance<T> substance;

        #endregion

	    #region プロパティ

        /// <summary>
        /// <see cref="Application.Manager"/>のショートカット。 
        /// </summary>
        protected ApplicationManager Manager => ApplicationManager.Instance;

	    #endregion

        #region メソッド

        /// <summary>
        /// 初期化する。
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// ウィンドウを開く。
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// 入力された数字を数値に変換する。
        /// </summary>
        /// <param name="value">数値（<c>string</c>）</param>
        /// <returns>
        /// 正常変換された場合はその数値（<c>int</c>）。エラーが発生した場合<c>-1</c>
        /// </returns>
        protected int ParseValue(string value, string errorMessage)
        {
            try { return int.Parse(value); }
            catch (System.Exception)
            {
                Manager.window.attention.Initialize(errorMessage, false).Open();
            }
            return -1;
        }

        #endregion
    }
}
