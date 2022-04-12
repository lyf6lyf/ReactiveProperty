using System;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveProperty;

namespace Test
{
    [TestClass]
    public class PropertyExTest
    {
        [TestMethod]
        public void ToProperty_WhenPropertyIsNotUsed_ShouldBeCollected()
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

        [TestMethod]
        public void ToProperty_WhenDeferSubscription_ShouldSubscribeWhenAccessValue()
        {
            var source = Observable.Range(1, 5);
            var subscribed = false;
            var property = source.Select(x =>
            {
                subscribed = true;
                return x;
            }).ToProperty(true);

            Assert.IsFalse(subscribed);

            _ = property.Value;

            Assert.IsTrue(subscribed);
        }
    }
}