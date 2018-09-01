using System;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extensions for the System.EventHandler class
    /// </summary>
    public static class EventHandlerExtensions
    {
        /// <summary>
        /// Performs a null check to ensure that the handler isn't null
        /// Passes empty event args
        /// </summary>
        /// <param name="thisHandler">The handler to invoke</param>
        /// <param name="sender">The sender of the event</param>
        public static void SafeInvoke(this EventHandler thisHandler, Object sender)
        {
            if (thisHandler != null)
                thisHandler(sender, EventArgs.Empty);
        }

        /// <summary>
        /// Performs a null check to ensure that the handler isn't null
        /// </summary>
        /// <param name="thisHandler">The handler to invoke</param>
        /// <param name="sender">The sender of the event</param>
        /// <param name="args">The arguments to pass along</param>
        public static void SafeInvoke(this EventHandler thisHandler, Object sender, EventArgs args)
        {
            if (thisHandler != null)
                thisHandler(sender, args);
        }

        /// <summary>
        /// Performs a null check to ensure that the handler isn't null
        /// </summary>
        /// <param name="thisHandler">The handler to invoke</param>
        /// <param name="sender">The sender of the event</param>
        /// <param name="args">The arguments to pass along</param>
        public static void SafeInvoke<T>(this EventHandler<T> thisHandler, Object sender, T args) where T : EventArgs
        {
            if (thisHandler != null)
                thisHandler(sender, args);
        }
    }
}