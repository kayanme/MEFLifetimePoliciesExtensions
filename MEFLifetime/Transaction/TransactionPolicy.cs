using System.Transactions;

namespace System.ComponentModel.Composition.Extensions
{

    [Export(typeof(TransactionPolicy<>))]   
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class TransactionPolicy<T>:Policy<T,Transaction> where T:class
    {
        [ImportingConstructor]
        internal TransactionPolicy(
            [Import(RequiredCreationPolicy = CreationPolicy.Shared)] 
            TransactionStorage<T> storage) : base(storage)
        {
        }

        protected override Transaction GetAffinity()
        {
            if (Transaction.Current == null)
                throw new InvalidOperationException();
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
            DestroyAffinity(e.Transaction);
            e.Transaction.TransactionCompleted -=Current_TransactionCompleted;
        }
    }
}
