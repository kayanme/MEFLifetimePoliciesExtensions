using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace System.ComponentModel.Composition.Extensions
{
    [Export(typeof(TransactionStorage<>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class TransactionStorage<T>:AffinityStorage<T,Transaction> where T:class
    {
    }
}
