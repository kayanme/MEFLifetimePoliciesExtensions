using System.ComponentModel.Composition.Extensions.MefPolicies;
using System.ComponentModel.Composition.Extensions.Thread;
using System.Threading;

namespace System.ComponentModel.Composition.Extensions
{             

    [Export(typeof (ThreadPolicy<>))]
    [Export(typeof (IDisposable))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal sealed class ThreadPolicy<T> : Policy<T, Thread> where T : class
    {
        private Thread thread;
        protected override Thread GetAffinity()
        {
            return thread;
        }

        [ImportingConstructor]
        internal ThreadPolicy(
            [Import(RequiredCreationPolicy = CreationPolicy.Shared)] 
            ThreadStorage<T> storage)
            : base(storage)
        {
           thread = Thread.CurrentThread;
        }
    }

}
