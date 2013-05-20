using System;
using System.ComponentModel.Composition.Extensions;
using System.ComponentModel.Composition.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MEFLifetime.Test
{
    [TestClass]
    public class ThreadTest
    {
        private CompositionContainer _container;

        private CollectorClass _collector;

        [TestInitialize]
        public void Initialize()
        {
            _container = new CompositionContainer(
                new TypeCatalog(typeof (TestPart), typeof (CollectorClass), typeof (ThreadPolicy<>),
                                typeof (ThreadStorage<>))
                );
            _collector = _container.GetExportedValue<CollectorClass>();
        }

        [TestMethod]
        public void SingleThreadOneTake()
        {
            TestPart part = _container.GetExportedValue<ThreadPolicy<TestPart>>();
            Assert.AreEqual(1, _collector.PartCount);
        }

        [TestMethod]
        public void SingleThreadTwoTakes()
        {
            TestPart part = _container.GetExportedValue<ThreadPolicy<TestPart>>();
            TestPart part2 = _container.GetExportedValue<ThreadPolicy<TestPart>>();
            Assert.AreSame(part, part2);
            Assert.AreEqual(1, _collector.PartCount);
        }

        [TestMethod]
        public void TwoThreadsTwoTakes()
        {
            TestPart part = _container.GetExportedValue<ThreadPolicy<TestPart>>();
            TestPart part2 = null;
            Task.Run(() => part2 = _container.GetExportedValue<ThreadPolicy<TestPart>>()).Wait();
            Assert.AreNotSame(part, part2);
            Assert.AreEqual(2, _collector.PartCount);
        }

        [TestMethod]
        public void TwoThreadsFourTakes()
        {
            TestPart part1 = _container.GetExportedValue<ThreadPolicy<TestPart>>();
            TestPart part2 = _container.GetExportedValue<ThreadPolicy<TestPart>>();
            TestPart part3 = null;
            TestPart part4 = null;
            Task.Run(() =>
                         {
                             part3 = _container.GetExportedValue<ThreadPolicy<TestPart>>();
                             part4 = _container.GetExportedValue<ThreadPolicy<TestPart>>();
                         }).Wait();
            Assert.AreSame(part1, part2);
            Assert.AreSame(part3, part4);
            Assert.AreNotSame(part3, part2);
            Assert.AreEqual(2, _collector.PartCount);
        }

        [TestMethod]
        public void SingleThreadWithDisposal()
        {
            TestPart part;
            var t = new Thread(() =>
                                   {
                                       part = _container.GetExportedValue<ThreadPolicy<TestPart>>();
                                   });
            t.Start();
            SpinWait.SpinUntil(() => !t.IsAlive);
            part = null;
            Thread.Sleep(1000);
            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            Assert.AreEqual(0, _collector.PartCount);
        }

        [TestMethod]
        public void TwoThreadWithDisposal()
        {
            TestPart part;
            TestPart part2;
            var t = new Thread(() =>
                                   {
                                       part = _container.GetExportedValue<ThreadPolicy<TestPart>>();
                                   });
            var t2 = new Thread(() =>
                                    {
                                        part2 = _container.GetExportedValue<ThreadPolicy<TestPart>>();
                                    });
            t.Start();
            t2.Start();
            SpinWait.SpinUntil(() => !t.IsAlive && !t2.IsAlive);
            Thread.Sleep(1000);

            part = null;
            part2 = null;
            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            Assert.AreEqual(0, _collector.PartCount);
        }
    }
}
