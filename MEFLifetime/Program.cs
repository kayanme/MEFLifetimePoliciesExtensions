using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MEFLifetime
{
    class Program
    {
       

        static void Main(string[] args)
        {
            var c = new CompositionContainer(
                new AggregateCatalog(
                    new AssemblyCatalog(typeof(TestPart).Assembly)),
                    CompositionOptions.ExportCompositionService|CompositionOptions.IsThreadSafe,
                    new ExtendedProvider());
           
         

            var t = new Thread(() =>
                                   {
                                       var part2 = c.GetExportedValue<CollectorClass>();
                                       var e2 = part2.Part;
                                   });
            var t2 = new Thread(() =>
                                    {
                                        var part2 = c.GetExportedValue<CollectorClass>();
                                        var e2 = part2.Part;
                                    });
            
            t.Start();
            t2.Start();
            var part = c.GetExportedValue<CollectorClass>();
            var e = part.Part;

            part = c.GetExportedValue<CollectorClass>();
            e = part.Part;
            t.Join();
            t2.Join();

         
           
        }
   

    }
}
