using System;
using System.ComponentModel.Composition.Extensions;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MEFLifetime.Test
{
    [TestClass]
    public class TransactionTest
    {
        private CompositionContainer _container;

        private CollectorClass _collector;

        [TestInitialize]
        public void Initialize()
        {
            _container = new CompositionContainer(
                new TypeCatalog(typeof(TestPart), typeof(CollectorClass), typeof(TransactionPolicy<>),
                                typeof (TransactionStorage<>))
                );
            _collector = _container.GetExportedValue<CollectorClass>();
        }

        [TestMethod]
        public void SingleTransactionOneTake()
        {
            using (new TransactionScope())
            {
                TestPart part = _container.GetExportedValue<TransactionPolicy<TestPart>>();
            }
            Assert.AreEqual(1, _collector.PartCount);
        }

        [TestMethod]
        public void SingleTransactionTwoTakes()
        {
            TestPart part;
            TestPart part2;
            using (new TransactionScope())
            {
                part = _container.GetExportedValue<TransactionPolicy<TestPart>>();
                part2 = _container.GetExportedValue<TransactionPolicy<TestPart>>();
               
            }           
            Assert.AreSame(part, part2);
            Assert.AreEqual(1, _collector.PartCount);
        }

        [TestMethod]
        public void TwoTransactionsTwoTakes()
        {
            TestPart part;
            TestPart part2;
            using (new TransactionScope())
            {
                part = _container.GetExportedValue<TransactionPolicy<TestPart>>();
                using(new TransactionScope(TransactionScopeOption.RequiresNew))
                  part2 = _container.GetExportedValue<TransactionPolicy<TestPart>>();

            }         
            Assert.AreNotSame(part, part2);
            Assert.AreEqual(2, _collector.PartCount);
        }

        [TestMethod]
        public void TwoTransactionsFourTakes()
        {
            TestPart part1;
            TestPart part2;
            TestPart part3;
            TestPart part4;
            using (new TransactionScope())
            {
                part1 = _container.GetExportedValue<TransactionPolicy<TestPart>>();
                part2 = _container.GetExportedValue<TransactionPolicy<TestPart>>();
                using (new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    part3 = _container.GetExportedValue<TransactionPolicy<TestPart>>();
                    part4 = _container.GetExportedValue<TransactionPolicy<TestPart>>();
                }
            }         
            Assert.AreSame(part1, part2);
            Assert.AreSame(part3, part4);
            Assert.AreNotSame(part3, part2);
            Assert.AreEqual(2, _collector.PartCount);
        }

        [TestMethod]
        public void NoTransactionOneTake()
        {
            TestPart part = _container.GetExportedValue<TransactionPolicy<TestPart>>();            
            Assert.IsNotNull(part);
            Assert.AreEqual(1, _collector.PartCount);    
        }

        [TestMethod]
        public void NoTransaction_TwoTakes()
        {
            TestPart part = _container.GetExportedValue<TransactionPolicy<TestPart>>();
            TestPart part2 = _container.GetExportedValue<TransactionPolicy<TestPart>>();
            Assert.IsNotNull(part);
            Assert.IsNotNull(part2);
            Assert.AreNotSame(part,part2);
            Assert.AreEqual(2, _collector.PartCount);
        }

        [TestMethod]
        public void NoTransactionAndTransaction_TwoTakes()
        {
            TestPart part = _container.GetExportedValue<TransactionPolicy<TestPart>>();
            using (var scope = new TransactionScope())
            {
                TestPart part2 = _container.GetExportedValue<TransactionPolicy<TestPart>>();
                Assert.IsNotNull(part);
                Assert.IsNotNull(part2);
                Assert.AreNotSame(part, part2);
                Assert.AreEqual(2, _collector.PartCount);
            }
           
        }

        [TestMethod]
        public void SingleTransactionWithDisposal()
        {
            TestPart part;
            using (new TransactionScope())
            {
                part = _container.GetExportedValue<TransactionPolicy<TestPart>>();               
            }
            part = null;
            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            Assert.AreEqual(0, _collector.PartCount);
        }

        [TestMethod]
        public void TwoTransactionWithDisposal()
        {
            TestPart part;
            TestPart part2;

            using (new TransactionScope())
            {
                part = _container.GetExportedValue<TransactionPolicy<TestPart>>();
                using (new TransactionScope(TransactionScopeOption.RequiresNew))
                    part2 = _container.GetExportedValue<TransactionPolicy<TestPart>>();
            }
            part = null;
            part2 = null;
            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            Assert.AreEqual(0, _collector.PartCount);
        }
    }
}
