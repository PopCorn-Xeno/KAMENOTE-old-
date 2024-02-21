using System.Collections;

namespace Kamenote.ContentViews
{
    /// <summary>
    /// 押下によってイベントを発火するコンテンツ。
    /// </summary>
    public interface IEventTriggerContent
    {
        /// <summary>
        /// <b>押下されたとき</b><br/>
        /// 押下されたかどうかのフラグの管理をする。
        /// </summary>
        /// <remarks>
        /// <c>EventTrigger</c>の<c>PointerDown</c>に登録する。
        /// </remarks>
        public void OnPointerDown();

        /// <summary>
        /// <b>押下が解除されたとき</b><br/>
        /// 押下されたかどうかのフラグの管理をする。
        /// </summary>
        /// <remarks>
        /// <c>EventTrigger</c>の<c>PointerDown</c>に登録する。
        /// </remarks>
        public void OnPointerUp();

        /// <summary>
        /// 長押し判定をとり、指定時間秒経ったときの処理を実行する。
        /// </summary>
        /// <param name="duration">長押しさせる時間</param>
        public IEnumerator CheckLongPressing(float duration);
    }
}
