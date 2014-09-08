﻿namespace AngleSharp.DOM
{
    using AngleSharp.DOM.Events;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Event target base of all DOM nodes.
    /// </summary>
    public abstract class EventTarget : IEventTarget
    {
        #region Fields

        readonly List<RegisteredEventListener> _listeners;

        #endregion

        #region ctor

        public EventTarget()
        {
            _listeners = new List<RegisteredEventListener>();
        }

        #endregion

        #region Events

        /// <summary>
        /// Register an event handler of a specific event type on the Node.
        /// </summary>
        /// <param name="type">A string representing the event type to listen for.</param>
        /// <param name="callback">The listener parameter indicates the EventListener function to be added.</param>
        /// <param name="capture">True indicates that the user wishes to initiate capture. After initiating
        /// capture, all events of the specified type will be dispatched to the registered listener before being
        /// dispatched to any Node beneath it in the DOM tree. Events which are bubbling upward through the tree
        /// will not trigger a listener designated to use capture.</param>
        public void AddEventListener(String type, EventListener callback = null, Boolean capture = false)
        {
            if (callback == null)
                return;

            _listeners.Add(new RegisteredEventListener
            {
                Type = type,
                Callback = callback,
                IsCaptured = capture
            });
        }

        /// <summary>
        /// Removes an event listener from the Node.
        /// </summary>
        /// <param name="type">A string representing the event type being removed.</param>
        /// <param name="callback">The listener parameter indicates the EventListener function to be removed.</param>
        /// <param name="capture">Specifies whether the EventListener being removed was registered as a capturing listener or not.</param>
        public void RemoveEventListener(String type, EventListener callback = null, Boolean capture = false)
        {
            if (callback == null)
                return;

            _listeners.Remove(new RegisteredEventListener
            {
                Type = type,
                Callback = callback,
                IsCaptured = capture
            });
        }

        /// <summary>
        /// Calls the listener registered for the given event.
        /// </summary>
        /// <param name="ev">The event that asks for the listeners.</param>
        internal void CallEventListener(Event ev)
        {
            foreach (var listener in _listeners)
            {
                if (ev.Flags.HasFlag(EventFlags.StopImmediatePropagation))
                    break;

                if (listener.Type != ev.Type || listener.IsCaptured && ev.Phase == EventPhase.Bubbling || !listener.IsCaptured && ev.Phase == EventPhase.Capturing)
                    continue;

                listener.Callback(ev.CurrentTarget, ev);
            }
        }

        /// <summary>
        /// Dispatch an event to this Node.
        /// </summary>
        /// <param name="ev">The event to dispatch.</param>
        /// <returns>False if at least one of the event handlers, which handled this event called preventDefault(). Otherwise true.</returns>
        public Boolean Dispatch(IEvent ev)
        {
            var impl = ev as Event;

            if (impl == null || impl.Flags.HasFlag(EventFlags.Dispatch) || !impl.Flags.HasFlag(EventFlags.Initialized))
                throw new DomException(ErrorCode.InvalidState);

            impl.IsTrusted = false;
            return impl.Dispatch(this);
        }

        #endregion

        #region Event Listener Structure

        struct RegisteredEventListener
        {
            public String Type;
            public EventListener Callback;
            public Boolean IsCaptured;
        }

        #endregion
    }
}
