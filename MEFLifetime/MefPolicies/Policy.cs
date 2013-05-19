using System.Threading;

namespace System.ComponentModel.Composition.Extensions
{

    public abstract class Policy<T, TAffinity>  where T : class
    {

        private T GetExportedValue()
        {
            _wasCreated = true;
            return _storage.GetOrAdd(GetAffinity(), () => _lazyPart.Value,OnInitialize);
        }

        private readonly AffinityStorage<T, TAffinity> _storage;

        [Import(AllowRecomposition = true, RequiredCreationPolicy = CreationPolicy.NonShared)]
        private Lazy<T> _lazyPart;
        
        private bool _wasCreated;       

        protected abstract TAffinity GetAffinity();

        protected virtual void OnInitialize(T obj)
        {
            
        }

        public static implicit operator T(Policy<T, TAffinity> threadPolicy)
        {
            return threadPolicy.GetExportedValue();
        }

        protected Policy(AffinityStorage<T, TAffinity> storage)
        {
            _storage = storage;
        }

        private int _wasDisposed;

        protected void DestroyAffinity(TAffinity affinity)
        {
            var wasDisposed = Interlocked.CompareExchange(ref _wasDisposed, 1, 0);
            if (_wasCreated && wasDisposed == 0)
            {
                _storage.RemoveAffinity(affinity);              
            } 
        }

      
      
    }
}
