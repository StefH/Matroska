namespace System.Threading.Tasks
{
   public static class PolyfillTaskExtensions // Polyfill for < .NET 6
   {
      public static Task WaitAsync(this Task task, int millisecondsTimeout) =>
            WaitAsync(task, TimeSpan.FromMilliseconds(millisecondsTimeout), default);

      public static Task WaitAsync(this Task task, TimeSpan timeout) =>
          WaitAsync(task, timeout, default);

      public static async Task WaitAsync(this Task task, CancellationToken cancellationToken)
      {
         var tcs = new TaskCompletionSource();
         using (cancellationToken.Register(s => ((TaskCompletionSource)s).TrySetCanceled(), tcs))
         {
            await (await Task.WhenAny(task, tcs.Task).ConfigureAwait(false)).ConfigureAwait(false);
         }
      }

      public static async Task WaitAsync(this Task task, TimeSpan timeout, CancellationToken cancellationToken)
      {
         var tcs = new TaskCompletionSource<bool>();
         using (new Timer(s => ((TaskCompletionSource<bool>)s).TrySetException(new TimeoutException()), tcs, timeout, Timeout.InfiniteTimeSpan))
         using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetCanceled(), tcs))
         {
            await (await Task.WhenAny(task, tcs.Task).ConfigureAwait(false)).ConfigureAwait(false);
         }
      }

      public static Task<TResult> WaitAsync<TResult>(this Task<TResult> task, int millisecondsTimeout) =>
          WaitAsync(task, TimeSpan.FromMilliseconds(millisecondsTimeout), default);

      public static Task<TResult> WaitAsync<TResult>(this Task<TResult> task, TimeSpan timeout) =>
          WaitAsync(task, timeout, default);

      public static async Task<TResult> WaitAsync<TResult>(this Task<TResult> task, CancellationToken cancellationToken)
      {
         var tcs = new TaskCompletionSource<TResult>();
         using (cancellationToken.Register(s => ((TaskCompletionSource<TResult>)s).TrySetCanceled(), tcs))
         {
            return await (await Task.WhenAny(task, tcs.Task).ConfigureAwait(false)).ConfigureAwait(false);
         }
      }

      public static async Task<TResult> WaitAsync<TResult>(this Task<TResult> task, TimeSpan timeout, CancellationToken cancellationToken)
      {
         var tcs = new TaskCompletionSource<TResult>();
         using (new Timer(s => ((TaskCompletionSource<TResult>)s).TrySetException(new TimeoutException()), tcs, timeout, Timeout.InfiniteTimeSpan))
         using (cancellationToken.Register(s => ((TaskCompletionSource<TResult>)s).TrySetCanceled(), tcs))
         {
            return await (await Task.WhenAny(task, tcs.Task).ConfigureAwait(false)).ConfigureAwait(false);
         }
      }
   }
}
