namespace ZeroProgress.Common
{
    /// <summary>
    /// Interface for respondants of Game Events to implement
    /// </summary>
    public interface IGameEventListener
    {
        /// <summary>
        /// 'Callback' for when the event is fired
        /// </summary>
        void OnEventRaised();
    }
}
