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

    /// <summary>
    /// Interface for respondants of Game Events with parameters to implement
    /// </summary>
    public interface IGameEventListener<T> : IGameEventListener
    {
        /// <summary>
        /// 'Callback' for when the event is fired
        /// </summary>
        void OnEventRaised(T Param);
    }
}
