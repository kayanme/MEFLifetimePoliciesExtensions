using System.Collections.Concurrent;

namespace System.ComponentModel.Composition.Extensions
{
    /// <summary>
    /// Abstract storage for context-bound part.
    /// Override with partially-closed generic with defined TAffinity.
    /// </summary>
    /// <typeparam name="TExport">Export type.</typeparam>
    /// <typeparam name="TAffinity">The type of the affinity.</typeparam>
    public abstract class AffinityStorage<TExport, TAffinity> where TExport : class
    {        
        private ConcurrentDictionary<TAffinity,TExport> _objects
            = new ConcurrentDictionary<TAffinity,TExport>();

        /// <summary>
        /// Get part for context or create and associate a new one.
        /// </summary>
        /// <param name="affinity">Context.</param>
        /// <param name="creator">Creation method for part.</param>
        /// <param name="onInitialize">Initialization hook.</param>
        /// <returns></returns>
        internal TExport GetOrAdd(TAffinity affinity, Func<TExport> creator,Action<TExport> onInitialize)
        {
            var t = _objects.GetOrAdd(affinity, (a) =>
                                                    {
                                                        var t2 = creator();
                                                        onInitialize(t2);
                                                        return t2;
                                                    });          
            return t;
        }

        /// <summary>
        /// Removes the affinity.
        /// </summary>
        /// <param name="affinity">Context.</param>
        internal void RemoveAffinity(TAffinity affinity)
        {
            TExport val;
           _objects.TryRemove(affinity, out val);
        }
    }

}
