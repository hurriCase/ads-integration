using System;
using System.Threading;
using AdsIntegration.Runtime.Providers.Unity.Data;
using CustomUtils.Runtime.Extensions;
using Cysharp.Threading.Tasks;

namespace AdsIntegration.Runtime.Providers.Unity.Wrappers
{
    internal sealed class AdLoadRetryHandler : IDisposable
    {
        private readonly Action _loadCallback;
        private readonly RetryConfig _retryConfig;
        private CancellationTokenSource _cancellationSource;
        private int _attemptCount;

        internal AdLoadRetryHandler(Action loadCallback, RetryConfig retryConfig)
        {
            _loadCallback = loadCallback;
            _retryConfig = retryConfig;
        }

        internal void OnLoadFailed()
        {
            if (_attemptCount >= _retryConfig.MaxAttempts)
                return;

            _attemptCount++;

            if (_attemptCount == 1)
            {
                _loadCallback();
                return;
            }

            var token = CancellationExtensions.GetFreshToken(ref _cancellationSource);
            RetryAsync(token).Forget();
        }

        internal void OnLoadSuccess()
        {
            _attemptCount = 0;
        }

        private async UniTask RetryAsync(CancellationToken token)
        {
            await UniTask.WaitForSeconds(_retryConfig.RetryDelay, cancellationToken: token);
            _loadCallback();
        }

        public void Dispose()
        {
            _cancellationSource?.Dispose();
        }
    }
}