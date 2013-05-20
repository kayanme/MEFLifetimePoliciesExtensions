using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Extensions
{

    /// <summary>
    /// Returns a part, bound to service call. 
    /// Throws <exception cref="InvalidOperationException">error</exception>, if one does not exists.
    /// </summary>
    /// <typeparam name="T">Export type</typeparam>
    [Export(typeof(CallPolicy<>))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class CallPolicy<T>:Policy<T,OperationContext> where T:class
    {
        [ImportingConstructor]
        internal CallPolicy([Import(RequiredCreationPolicy=CreationPolicy.Shared)]
            CallStorage<T> storage) : base(storage)
        {
        }

        protected override OperationContext GetAffinity()
        {
         
            if (OperationContext.Current == null)
                throw new InvalidOperationException();
             return OperationContext.Current;            
        }

        protected override void OnInitialize(T obj)
        {          
            OperationContext.Current.OperationCompleted += Current_OperationCompleted;
        }

        void Current_OperationCompleted(object sender, EventArgs e)
        {
            DestroyAffinity(OperationContext.Current);
            OperationContext.Current.OperationCompleted -= Current_OperationCompleted;
        }
    }
}
