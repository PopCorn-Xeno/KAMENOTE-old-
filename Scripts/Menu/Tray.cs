using Kamenote.ContentViews;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;

namespace Kamenote.Menu
{
    [RequireComponent(typeof(Animator))]
    public class Tray : MonoBehaviour
    {
        #region フィールド
        
        /// <summary>
        /// 基準にする左右の端。
        /// </summary>
        [SerializeField] private PivotMode pivotMode;

        /// <summary>
        /// トレイが有効になっているときに、閉じる機能のみを使う場合。
        /// </summary>
        [SerializeField] private bool closeOnly;

        /// <summary>
        /// トレイを開くボタン。
        /// </summary>
        [SerializeField] private InteractiveButton activateButton;

        /// <summary>
        /// トレイをピン留めするボタン。
        /// </summary>
        [SerializeField] private InteractiveButton pinButton;

        /// <summary>
        /// トレイのアニメーションを制御するアニメーター。
        /// </summary>
        [SerializeField] private Animator animator;

        /// <summary>
        /// メインウィンドウ（インフォメーション）
        /// </summary>
        [SerializeField] private InformationAnimator information;

        /// <summary>
        /// トレイが開閉されたときに発火するイベントを登録する。
        /// </summary>
        /// <remarks>
        /// トレイが開いた場合は<c>true</c>、閉じた場合は<c>false</c>を送る。
        /// </remarks>
        [Space, SerializeField] private UnityEvent<bool> onInteract;

        /// <summary>
        /// トレイ内に表示する項目（<see cref="items"/>）の数。
        /// </summary>
        [Space, SerializeField] private int itemCount;

        /// <summary>
        /// トレイ内に表示する項目。
        /// </summary>
        [SerializeField] private TrayItemView[] items;

        /// <summary>
        /// トレイ内の項目をクリックしたときに発火するイベントを登録する。
        /// </summary>
        [Tooltip("イベントのインデックスは上のトレイ内項目のインデックスと紐づいています"), SerializeField] private UnityEvent[] itemEvents;

        [SerializeField] private bool onValidate;

        private static readonly int IsOpen = Animator.StringToHash("IsOpen");

        #endregion

	    #region クラス

        /// <summary>
        /// トレイをピン留めしたときにメインウィンドウ（インフォメーション）の位置を変える
        /// </summary>
        [Serializable]
        private class InformationAnimator
        {
            [SerializeField] private Animator animator;

            private static readonly int IsTransformLeft = Animator.StringToHash("IsTransformLeft");
            private static readonly int EnablePin = Animator.StringToHash("EnablePin");

            /// <summary>
            /// トレイのピン留め状態に合わせてウィンドウをずらす。
            /// </summary>
            /// <param name="direction">ずらす方向</param>
            /// <param name="active">ピン留め状態</param>
            public void Move(PivotMode direction, bool active)
            {
                animator.SetBool(IsTransformLeft, direction == PivotMode.Left);
                Move(active);
            }

            /// <summary>
            /// ウィンドウのピン留め状態を変更する。
            /// </summary>
            /// <param name="active">ウィンドウのピン留め状態</param>
            public void Move(bool active) => animator.SetBool(EnablePin, active);
        }

        /// <summary>
        /// 基準位置
        /// </summary>
        private enum PivotMode { Left, Right }

	    #endregion

	    #region MonoBehaviourメソッド

        void Start()
        {
            // トレイに共通する処理を書いているため、追加でボタンにイベントを登録したくなった場合は
            // 各ボタンのイベントにインスペクタから登録しておく
            if (activateButton != null)
            {
                activateButton.OnOpened += Open;
                activateButton.OnClosed += Close;
            }
            if (pinButton != null)
            {
                pinButton.OnOpened += pivotMode == PivotMode.Right 
                                      ? () => information.Move(PivotMode.Right, true)
                                      : () => information.Move(PivotMode.Left, true);
                pinButton.OnClosed += pivotMode == PivotMode.Right
                                      ? () => information.Move(PivotMode.Right, false)
                                      : () => information.Move(PivotMode.Left, false); 
            }
            
            if (closeOnly)
            {
                activateButton.Disable();
                activateButton.IsOpen = !activateButton.IsOpen;
                pinButton.IsOpen = !pinButton.IsOpen;
            }
            pinButton.Disable();

            RegisterItems();
        }

        void OnValidate()
        {
            if (onValidate) RegisterItems(true);
        }

	    #endregion

        #region メソッド

        /// <summary>
        /// トレイを開く。
        /// </summary>
        public void Open()
        {
            animator.SetBool(IsOpen, true);
            pinButton.Enable();
            onInteract.Invoke(true);
        }

        /// <summary>
        /// トレイを閉じる。
        /// </summary>
        public void Close()
        {
            animator.SetBool(IsOpen, false);
            information.Move(false);
            pinButton.Close();
            pinButton.Disable();
            onInteract.Invoke(false);
        }

        /// <summary>
        /// ピン留め状態ではないときのみトレイを閉じる。
        /// </summary>
        /// トレイ内アイテムを展開した時に使う。インスペクターからイベントに指定(<see cref="itemEvents"/>)
        public void CloseIf()
        {
            if (!pinButton.IsOpen)
            {
                Close();
                activateButton.Close();
            }
        }

        private void RegisterItems(bool onValidate = false)
        {
            // 配列の長さが変わってもクローンを作って値を保持する
            items = KeepElements(items, itemCount);
            itemEvents = KeepElements(itemEvents, itemCount);

            if (!onValidate)
            {
                Enumerable.Range(0, itemCount).ToList().ForEach(i => items[i].AddListener(itemEvents[i].Invoke));
            }
        }

        /// <summary>
        /// 配列の長さを変更しても保持できる要素をコピーした新しい配列を返す。
        /// </summary>
        /// <typeparam name="T">コピー元の配列の型</typeparam>
        /// <param name="array">コピー元配列</param>
        /// <param name="length">変更後の配列の長さ</param>
        /// <returns>長さを変更した配列</returns>
        private T[] KeepElements<T>(T[] array, int length)
        {
            T[] copy = new T[length];
            int newLength = array.Length > length ? length : array.Length;
            Array.Copy(array, copy, newLength);
            return copy;
        }

        #endregion
    }
}
