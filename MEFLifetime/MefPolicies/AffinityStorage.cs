using System.Collections.Concurrent;

namespace System.ComponentModel.Composition.Extensions
{
    public abstract class AffinityStorage<T, TAffinity> where T : class
    {        
        private ConcurrentDictionary<TAffinity,T> _objects
            = new ConcurrentDictionary<TAffinity,T>();

        internal T GetOrAdd(TAffinity affinity, Func<T> creator,Action<T> onInitialize)
        {
            var t = _objects.GetOrAdd(affinity, (a) =>
                                                    {
                                                        var t2 = creator();
                                                        onInitialize(t2);
                                                        return t2;
                                                    });          
            return t;
        }

        internal void RemoveAffinity(TAffinity affinity)
        {
            T val;
           _objects.TryRemove(affinity, out val);
        }
    }

}
