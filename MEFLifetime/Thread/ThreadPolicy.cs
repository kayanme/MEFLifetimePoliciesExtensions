using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Extensions
{

    /// <summary>
    /// Returns a part bound to calling thread.
    /// </summary>
    /// <typeparam name="T">Export type</typeparam>
    [Export(typeof (ThreadPolicy<>))]  
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class ThreadPolicy<T> : Policy<T, Thread> where T : class
    {
        
        protected override Thread GetAffinity()
        {
            return Thread.CurrentThread;
        }

        private void KillThread(Thread thread)
        {         
            thread.Join();
            DestroyAffinity(thread); 
        }

        protected override void OnInitialize(T obj)
        {
            var thread = Thread.CurrentThread;
            new Thread(() => KillThread(thread)).Start();          
        }                      

        [ImportingConstructor]
        internal ThreadPolicy(
            [Import(RequiredCreationPolicy = CreationPolicy.Shared)] 
            ThreadStorage<T> storage)
            : base(storage)
        {
          
        }

        public ThreadPolicy(T part)
            : base(null)
        {
            
        }
    }

}
