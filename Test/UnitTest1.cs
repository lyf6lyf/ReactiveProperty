using System;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveProperty;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var source = Observable.Range(1, 5);
            WeakReference weakRef = null;

            void Scope()
            {
                var property = source.ToProperty();
                weakRef = new WeakReference(property);
            }

            Scope();
            Assert.IsTrue(weakRef.IsAlive);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.IsFalse(weakRef.IsAlive);
        }
    }
}