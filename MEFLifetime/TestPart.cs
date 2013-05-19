using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MEFLifetime
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class TestPart
    {
        public static int CrCount;
        public TestPart()
        {
            Interlocked.Increment(ref CrCount);
        }
    }
}
