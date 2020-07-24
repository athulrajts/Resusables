using System;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace KEI.Infrastructure
{
    /// <summary>
    /// Helper class that performs common threading-related functions.
    /// </summary>
    public class ThreadingHelper
    {
        private static Dispatcher m_Dispatcher;

        public static Dispatcher CurrentDispatcher
        {
            get
            {
                return m_Dispatcher;
            }
        }

        static ThreadingHelper()
        {
            if (Application.Current != null)
            {
                m_Dispatcher = Application.Current.Dispatcher;
            }
            else
            {
                m_Dispatcher = Dispatcher.CurrentDispatcher;
            }
        }

        /// <summary>
        /// Dummy initializer to force Dispatcher instantiation.
        /// </summary>
        public static void Init() { }

        /// <summary>
        /// Asynchronously execute the specified action via the Dispatcher. 
        /// </summary>
        public static void AsyncDispatcherInvoke(Action p_Delegate)
        {
            CurrentDispatcher.BeginInvoke(p_Delegate);
        }

        /// <summary>
        /// Synchronously execute the specified action via the Dispatcher. 
        /// </summary>
        public static void DispatcherInvoke(Action p_Delegate)
        {
            try
            {
                if (!CurrentDispatcher.CheckAccess())
                {
                    CurrentDispatcher.Invoke(p_Delegate);
                }
                else
                {
                    // we are the dispatcher thread, just call the delegate...
                    p_Delegate();
                }
            }
            finally
            {
            }
        }

        /// Returns whether or not the current thread is the dispatch thread
        public static bool IsDispatchThread
        {
            get
            {
                return Thread.CurrentThread == CurrentDispatcher.Thread;
            }
        }

        /// <summary>
        /// Run the specified action in a background thread.
        /// </summary>
        /// <param name="p_Action">Action to run.</param>
        /// <param name="p_OnComplete">Action to run when the task completes (optional).</param>
        public static void RunInBackgroundTread(Action action) => Task.Run(action);

        public static async Task<TReturn> RunInBackgroundThread<TReturn>(Func<TReturn> func) => await Task.Run(func);

        /// <summary>
        /// Asynchronously invokes the specified action and waits for its completion.
        /// The wait is non-blocking, and can thus be invoked on the UI thread without locking it.
        /// </summary>
        /// <param name="p_Action">Action to execute</param>
        public static void AsyncInvokeAndPumpWaitFor(Action p_Action)
        {
            DispatcherFrame dispFrame = new DispatcherFrame();

            Task actionTask = new Task(p_Action);
            actionTask.ContinueWith(_ => dispFrame.Continue = false);
            actionTask.Start();

            Dispatcher.PushFrame(dispFrame);

            actionTask.Wait();
        }

        /// <summary>
        /// Asynchronously invokes the specified action and waits for its completion.
        /// The wait is non-blocking, and can thus be invoked on the UI thread without locking it. 
        /// </summary>
        /// <typeparam name="TReturn">Return value type</typeparam>
        /// <param name="p_Func">Action to execute</param>
        public static TReturn AsyncInvokeAndPumpWaitFor<TReturn>(Func<TReturn> p_Func)
        {
            DispatcherFrame dispFrame = new DispatcherFrame();

            Task<TReturn> actionTask = new Task<TReturn>(p_Func);
            actionTask.ContinueWith(_ => dispFrame.Continue = false);
            actionTask.Start();

            Dispatcher.PushFrame(dispFrame);

            return actionTask.Result;
        }
    }
}
