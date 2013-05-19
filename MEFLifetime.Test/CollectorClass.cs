using System.ComponentModel.Composition;
using System.Threading;

namespace MEFLifetime.Test
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class CollectorClass
    {
        private int _partCount = 0;


        public int PartCount
        {
            get { return _partCount; }

        }

        public void Increment()
        {
            Interlocked.Increment(ref _partCount);
        }

        public void Decrement()
        {
            Interlocked.Decrement(ref _partCount);
        }
    }
}
