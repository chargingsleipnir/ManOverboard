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
        /// <param name="eventId">The Id of the event (in the case multiple events
        /// are handled, we need a way to identify which is the current)</param>
        void OnEventRaised(string eventId);
    }

    /// <summary>
    /// Interface for respondants of Game Events with parameters to implement
    /// </summary>
    public interface IGameEventListener<T> : IGameEventListener
    {
        /// <summary>
        /// 'Callback' for when the event is fired
        /// </summary>
        /// <param name="eventId">The Id of the event (in the case multiple events
        /// are handled, we need a way to identify which is the current)</param>
        /// <param name="Param">The parameter information</param>
        void OnEventRaised(string eventId, T Param);
    }
}
