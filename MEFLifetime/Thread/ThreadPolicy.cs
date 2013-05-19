using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Extensions
{             

    [Export(typeof (ThreadPolicy<>))]  
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class ThreadPolicy<T> : Policy<T, Thread> where T : class
    {
        
        protected override Thread GetAffinity()
        {
            return Thread.CurrentThread;
        }

        private void Tt(Thread thread)
        {         
            thread.Join();
            DestroyAffinity(thread); 
        }

        protected override void OnInitialize(T obj)
        {
            var thread = Thread.CurrentThread;
            new Thread(()=> Tt(thread)).Start();
          
        }                      

        [ImportingConstructor]
        internal ThreadPolicy(
            [Import(RequiredCreationPolicy = CreationPolicy.Shared)] 
            ThreadStorage<T> storage)
            : base(storage)
        {
          
        }
    }

}
