using UnityEngine;
using System.Collections;

#if UNITY_EDITOR || UNITY_STANDALONE
using System.Threading;
#endif

#if NETFX_CORE
using System;
using System.Threading.Tasks;
#endif

namespace HoloGroup.Threading
{
    /// <summary>
    /// Class that represent async process.
    /// </summary>
    public class AsyncProcess
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// Thread  that used when run in standalone platform.
        /// </summary>
        private Thread _thread;
#endif

#if NETFX_CORE
    // UWP not support Thread class, so we use Task.
    private Task _task;
#endif

#if UNITY_EDITOR || UNITY_STANDALONE
        /// <summary>
        /// Just construct with thread object.
        /// </summary>
        /// <param name="thread">Thread with passed method.</param>
        public AsyncProcess(Thread thread)
        {
            _thread = thread;
        }
#endif

#if NETFX_CORE
    /// <summary>
    /// Construct with task.
    /// </summary>
    /// <param name="task">Task with passed method.</param>
    public AsyncProcess(Task task)
    {
        _task = task;
    }
#endif

        /// <summary>
        /// Start process in new thread.
        /// </summary>
        public void Start()
        {
            #region Standalone
#if UNITY_EDITOR || UNITY_STANDALONE
            // If already runned, we can't run again.
            if (_thread.ThreadState == ThreadState.Running)
            {
                Debug.LogError("Thread already started.");
                return;
            }

            // Run thread.
            _thread.Start();
#endif
            #endregion

            #region UWP
#if NETFX_CORE
        // If already runned, we can't run again.
        if(_task.Status == TaskStatus.Running)
        {
            return;
        }

        // Run task.
        _task.Start();
#endif
            #endregion
        }

        /// <summary>
        /// Simple wait while milliseconds elapsed.
        /// </summary>
        /// <param name="milliseconds">Target milliseconds.</param>
        public static void Sleep(int milliseconds)
        {
            #region Standalone
#if UNITY_EDITOR || UNITY_STANDALONE
            Thread.Sleep(milliseconds);
#endif
            #endregion

            #region UWP
#if NETFX_CORE
        // UWP not support Thread class.
        // Below we emulate sleeping (Thread.Sleep(int milliseconds)) with simple timer.
        
        // Save system time at start moment.
        DateTime startTime = DateTime.Now;

        // Wait target time.
        while((DateTime.Now - startTime).TotalMilliseconds < milliseconds) { }
#endif
            #endregion
        }
    }
}