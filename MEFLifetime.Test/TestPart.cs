using System.ComponentModel.Composition;
using System.Threading;

namespace MEFLifetime.Test
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal sealed class TestPart:IPartImportsSatisfiedNotification 
    {
        
        private readonly CollectorClass _counter;

        [ImportingConstructor]
        internal TestPart(CollectorClass counter)
        {
            _counter = counter;
        }

        public void OnImportsSatisfied()
        {
            _counter.Increment();
        }

        ~TestPart()
        {
            _counter.Decrement();
        }
    }
}
