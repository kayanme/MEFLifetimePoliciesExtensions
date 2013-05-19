using System.Transactions;

namespace System.ComponentModel.Composition.Extensions
{

    [Export(typeof(ThreadPolicy<>))]
    [Export(typeof(IDisposable))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class TransactionPolicy<T>:Policy<T,Transaction> where T:class
    {
        [ImportingConstructor]
        public TransactionPolicy(AffinityStorage<T, Transaction> storage) : base(storage)
        {
        }

        protected override Transaction GetAffinity()
        {
            return Transaction.Current;
        }

        protected override void OnInitialize(T obj)
        {
            if (obj is ISinglePhaseNotification)
                Transaction.Current.EnlistVolatile(obj as ISinglePhaseNotification,EnlistmentOptions.None);
            Transaction.Current.TransactionCompleted += Current_TransactionCompleted;
        }

        void Current_TransactionCompleted(object sender, TransactionEventArgs e)
        {
            Dispose();
        }
    }
}
