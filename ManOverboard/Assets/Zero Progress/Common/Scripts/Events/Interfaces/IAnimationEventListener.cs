namespace ZeroProgress.Common
{
    /// <summary>
    /// Interface for animation event responders to implement
    /// </summary>
    public interface IAnimationEventListener
    {
        /// <summary>
        /// The 'callback' for when an animation event has been fired
        /// </summary>
        /// <param name="EventLabel">An identifier for the event that's been fired</param>
        void ReceiveEvent(string EventLabel);
    }
}