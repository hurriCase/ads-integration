using System;
using System.Threading;
using CustomUtils.Runtime.Extensions;
using Cysharp.Threading.Tasks;
using R3;

namespace AdsIntegration.Runtime
{
    internal sealed class AdsPreloader : IDisposable
    {
        private readonly ReadOnlyReactiveProperty<bool> _isAvailable;
        private readonly Action _preload;
        private readonly int _maxAttempts;
        private readonly float _retryDelay;

        private int _loadAttemptCount;
        private CancellationTokenSource _cancellationSource;

        private readonly IDisposable _availabilitySubscription;

        internal AdsPreloader(
            ReadOnlyReactiveProperty<bool> isAvailable,
            Action preload,
            int maxAttempts,
            float retryDelay)
        {
            _isAvailable = isAvailable;
            _preload = preload;
            _maxAttempts = maxAttempts;
            _retryDelay = retryDelay;

            _availabilitySubscription = _isAvailable
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

            var token = CancellationExtensions.GetFreshToken(ref _cancellationSource);
            PreloadAsync(token).Forget();
        }

        private async UniTask PreloadAsync(CancellationToken cancellationToken)
        {
            await UniTask.WaitForSeconds(_retryDelay, cancellationToken: cancellationToken);

            if (_isAvailable.CurrentValue)
                return;

            _preload();
            _loadAttemptCount++;
        }

        public void Dispose()
        {
            _cancellationSource?.Dispose();
            _availabilitySubscription.Dispose();
        }
    }
}