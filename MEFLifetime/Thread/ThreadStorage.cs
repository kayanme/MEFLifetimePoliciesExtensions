using System.ComponentModel.Composition.Extensions.MefPolicies;

namespace System.ComponentModel.Composition.Extensions
{
    [Export(typeof(ThreadStorage<>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class ThreadStorage<T> : AffinityStorage<T, Threading.Thread> where T : class
    {
    }
}
