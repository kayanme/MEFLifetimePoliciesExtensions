using System.Threading;

namespace System.ComponentModel.Composition.Extensions
{

    /// <summary>
    /// Base class for policies. 
    /// Override with partially-closed generic with TAffinity defined.
    /// </summary>
    /// <typeparam name="TExport">The type of the export.</typeparam>
    /// <typeparam name="TAffinity">The type of the context.</typeparam>
    /// <remarks>It always returns different instances for different metadata, even if it describes one part.</remarks>
    public abstract class Policy<TExport, TAffinity>  where TExport : class
    {

     
        private readonly AffinityStorage<TExport, TAffinity> _storage;

        [Import(AllowRecomposition = true, RequiredCreationPolicy = CreationPolicy.NonShared)]
        private Lazy<TExport> _lazyPart;
        
        private bool _wasCreated;
        private int _wasDisposed;

        protected abstract TAffinity GetAffinity();

        private TExport GetExportedValue()
        {
            _wasCreated = true;
            var affinity = GetAffinity();
            if (Equals(affinity, default(TExport)))
                return _lazyPart.Value;

            if (_storage == null)//it have null for some unit-tests for example, when we have no need in composition container
                throw new InvalidOperationException("Policy has no affinity storage.");
            return _storage.GetOrAdd(affinity, () => _lazyPart.Value, OnInitialize);
        }


        protected virtual void OnInitialize(TExport obj)
        {
            
        }

        public static implicit operator TExport(Policy<TExport, TAffinity> threadPolicy)
        {
            return threadPolicy.GetExportedValue();
        }

        public TExport Target
        {
            get
            {
                return GetExportedValue();
            }
        }

        protected Policy(AffinityStorage<TExport, TAffinity> storage)
        {
            _storage = storage;
        }

        protected void DestroyAffinity(TAffinity affinity)
        {
            if (_storage == null)//it have null for some unit-tests for example, when we have no need in composition container
                throw new InvalidOperationException("Policy has no affinity storage.");
            var wasDisposed = Interlocked.CompareExchange(ref _wasDisposed, 1, 0);
            if (_wasCreated && wasDisposed == 0)
            {
                _storage.RemoveAffinity(affinity);              
            } 
        }

      
      
    }
}
