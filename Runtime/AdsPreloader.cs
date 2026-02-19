using System;
using System.Threading;
using CustomUtils.Runtime.Extensions;
using Cysharp.Threading.Tasks;
using R3;

namespace AdsIntegration.Runtime
{
    internal sealed class AdsPreloader : IDisposable
    {
        private readonly Action _preload;
        private readonly int _maxAttempts;
        private readonly float _retryDelay;

        private int _loadAttemptCount;
        private CancellationTokenSource _cancellationSource;

        private readonly IDisposable _availabilitySubscription;

        internal AdsPreloader(Observable<bool> isAvailable, Action preload, int maxAttempts, float retryDelay)
        {
            _preload = preload;
            _maxAttempts = maxAttempts;
            _retryDelay = retryDelay;

            _availabilitySubscription = isAvailable
                .Subscribe(this, static (isAvailable, self) => self.HandleAvailabilityChange(isAvailable));
        }

        private void HandleAvailabilityChange(bool isAvailable)
        {
            if (isAvailable)
            {
                _loadAttemptCount = 0;
                return;
            }

            if (_loadAttemptCount >= _maxAttempts)
                return;

            _loadAttemptCount++;

            if (_loadAttemptCount == 1)
            {
                _preload();
                return;
            }

            var token = CancellationExtensions.GetFreshToken(ref _cancellationSource);
            PreloadAsync(token).Forget();
        }

        private async UniTask PreloadAsync(CancellationToken cancellationToken)
        {
            await UniTask.WaitForSeconds(_retryDelay, cancellationToken: cancellationToken);

            _preload();
        }

        public void Dispose()
        {
            _cancellationSource?.Dispose();
            _availabilitySubscription.Dispose();
        }
    }
}