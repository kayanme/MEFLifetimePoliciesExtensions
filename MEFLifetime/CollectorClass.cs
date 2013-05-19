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
    internal class CollectorClass
    {
      
        public readonly TestPart Part;

        public static int CrCount;

        [ImportingConstructor]
        public CollectorClass([Import]TestPart part)
        {
            Interlocked.Increment(ref CrCount);
            Part = part;
        }
    }
}
