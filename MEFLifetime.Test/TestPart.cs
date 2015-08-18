using System.ComponentModel.Composition;
using System.Threading;

namespace MEFLifetime.Test
{

    internal interface ITestPart
    {}


[Export]
[Export(typeof(ITestPart))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal sealed class TestPart : IPartImportsSatisfiedNotification, ITestPart
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
