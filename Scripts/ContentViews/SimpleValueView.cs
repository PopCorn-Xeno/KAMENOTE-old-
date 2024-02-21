using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using System.Linq;

namespace Kamenote.ContentViews
{
    /// <summary>
    /// 数値表記の値をシンプルに表示するビュー。
    /// </summary>
    /* 
        [使い方]
        1. このスクリプトがアタッチされたプレハブをヒエラルキーに配置する。
        2. 別の管理スクリプトにこのクラスのフィールドをつくり、ヒエラルキーに配置したプレハブを登録する
        3. 初期化、値の受け渡しと更新のメソッドを実装する
     */
    public class SimpleValueView : MonoBehaviour, IContentView
    {
        #region フィールド

        /// <summary>
        /// キャプションテキスト。
        /// </summary>
        [SerializeField] private TextMeshProUGUI caption;

        /// <summary>
        /// 現在の値を表示する。
        /// </summary>
        [SerializeField] private TextMeshProUGUI value;

        /// <summary>
        /// 単位を表示する。
        /// </summary>
        [SerializeField] private TextMeshProUGUI unit;

        /// <summary>
        /// 押されたとき、値を１加算するボタン。
        /// </summary>
        [SerializeField] private Button add;

        /// <summary>
        /// 押されたとき、値を１減算するボタン。
        /// ※１以下にはならない
        /// </summary>
        [SerializeField] private Button remove;

        [SerializeField] private bool onValidate;

        #endregion

        #region プロパティ

        /// <summary>
        /// 現在の値を保持する。
        /// </summary>
        public decimal Value { get; set; }

        #endregion

        #region MonoBehaviourメソッド

        void Awake() => Initialize();

        void OnValidate()
        {
            if (onValidate) Initialize();
        }

        #endregion

        #region メソッド

        public void Initialize()
        {
            // ボタンを使わないときは横幅を狭める
            if (add == null && remove == null)
            {
                GetComponent<RectTransform>().sizeDelta = new(250, GetComponent<RectTransform>().sizeDelta.y);
            }
            // ボタンを使うとき、イベントを初期化する
            else
            {
                add.onClick.RemoveAllListeners();
                remove.onClick.RemoveAllListeners();

                // 値の加算、減算の処理を登録する
                add.onClick.AddListener(() => Value++);
                remove.onClick.AddListener(() => Value = Value > 1 ? Value -= 1 : Value);
            }
        }

        /// <summary>
        /// ビューを初期化する。
        /// </summary>
        /// <param name="caption">キャプションテキスト</param>
        /// <param name="value">初期値</param>
        /// <param name="unit">単位</param>
        /// <param name="onValueChanged">値が変わった時に発火するイベント</param>
        public void Initialize(string caption, decimal value, string unit, params UnityAction<decimal>[] onValueChanged)
        {
            // フィールドを初期化する
            Value = value;
            this.caption.text = caption;
            this.value.text = Value.ToString();
            this.unit.text = unit;
            
            // ボタンを使用する際、指定されたイベントを登録する
            if (add != null && remove != null && onValueChanged != null) { OnValueChanged(onValueChanged); }
        }

        /// <summary>
        /// 値を更新し、更新後の値を返す。
        /// </summary>
        /// <typeparam name="T">値型</typeparam>
        /// <param name="value">更新する値</param>
        /// <returns>更新後の値</remarks>
        public T UpdateValue<T>(T value) where T : struct
        {
            this.value.text = value.ToString();
            Value = decimal.Parse(this.value.text);
            return value;
        }

        /// <summary>
        /// 値が変わった時に行う処理。
        /// </summary>
        /// <typeparam name="T">値型</typeparam>
        /// <param name="actions">値が変わった時に行うメソッド</param>
        private void OnValueChanged<T>(params UnityAction<T>[] actions) where T : struct
        {
            actions.ToList().ForEach
            (
                action => 
                {
                    add.onClick.AddListener
                    (
                        () =>
                        {
                            action.Invoke((T)(object)Value);
                            value.text = Value.ToString();
                        }
                    );
                    remove.onClick.AddListener
                    (
                        () =>
                        {
                            action.Invoke((T)(object)Value);
                            value.text = Value.ToString();
                        }
                    );
                }
            );
        }

        #endregion
    }
}
