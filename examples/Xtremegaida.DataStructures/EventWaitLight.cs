using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xtremegaida.DataStructures
{
   [System.Diagnostics.DebuggerDisplay("TriggerState = {triggerState}")]
   public struct EventWaitLight
   {
      private SpinLock taskLock;
      private TaskCompletionSource taskSource;
      private Exception lastError;
      private volatile int triggerState;

      public bool Triggered => triggerState != 0;

      public Task WaitAsync(CancellationToken cancellationToken = default)
      {
         if (cancellationToken.IsCancellationRequested) { return Task.FromCanceled(cancellationToken); }
         Task task;
         bool locked = false;
         try
         {
            taskLock.TryEnter(ref locked);
            if (triggerState != 0)
            {
               var error = lastError;
               if (triggerState > 0) { triggerState = 0; lastError = null; }
               if (error != null) { return Task.FromException(error); }
               return Task.CompletedTask;
            }
            if (taskSource == null) { taskSource = new TaskCompletionSource(); }
            task = taskSource.Task;
         }
         finally
         {
            if (locked) { taskLock.Exit(); }
         }
         if (cancellationToken.CanBeCanceled) { return task.WaitAsync(cancellationToken); }
         return task;
      }

      public void Trigger(Exception error = null, bool manualReset = false, bool withReEntrantSafety = false)
      {
         // WARNING: TrySetResult runs continuations synchronously on the same thread.
         // This can cause issues with non-reentrant locking.
         TaskCompletionSource source = null;
         bool locked = false;
         try
         {
            taskLock.Enter(ref locked);
            lastError = error; triggerState = 1;
            if (taskSource != null)
            {
               source = taskSource; taskSource = null;
               lastError = null; triggerState = 0;
            }
            if (manualReset) { triggerState = -1; }
         }
         finally
         {
            if (locked) { taskLock.Exit(); }
         }
         if (source != null)
         {
            if (withReEntrantSafety)
            {
               RunAsTask(source, error);
            }
            else
            {
               if (error != null) { source.TrySetException(error); }
               else { source.TrySetResult(); }
            }
         }
      }

      private void RunAsTask(TaskCompletionSource source, Exception error)
      {
         _ = Task.Run(() =>
         {
            if (error != null) { source.TrySetException(error); }
            else { source.TrySetResult(); }
         });
      }

      public void Reset()
      {
         bool locked = false;
         try
         {
            taskLock.Enter(ref locked);
            if (triggerState != 0)
            {
               taskSource = null;
               lastError = null;
               triggerState = 0;
            }
         }
         finally
         {
            if (locked) { taskLock.Exit(); }
         }
      }
   }
}
