using System.ComponentModel.Composition;
using System.Threading;

namespace MEFLifetime.Test
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class TestPart:IPartImportsSatisfiedNotification 
    {
        [Import]
        private CollectorClass _counter;       

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
