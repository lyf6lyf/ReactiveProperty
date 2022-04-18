using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReactiveProperty;

namespace Test
{
    [TestClass]
    public class PropertyExTest
    {
        [DataRow(true)]
        [DataRow(false)]
        [DataTestMethod]
        public void ToProperty_WhenPropertyIsNotUsed_ShouldBeCollected(bool deferSubscription)
        {
            var source = new BehaviorSubject<int>(1);
            WeakReference weakRef = null;

            void Scope()
            {
                var property = source.ToProperty(deferSubscription);
                weakRef = new WeakReference(property);
            }

            Scope();
            Assert.IsTrue(weakRef.IsAlive);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.IsFalse(weakRef.IsAlive);

            Assert.IsNotNull(source, "pin source");
        }

        [TestMethod]
        public void ToProperty_WithDeferSubscription_WhenNoPropertyChangedEventHandler_ShouldBeCollected()
        {
            var source = new BehaviorSubject<int>(1);
            WeakReference weakRef = null;

            void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
            }

            void Scope()
            {
                var property = source.ToProperty(true);
                weakRef = new WeakReference(property);

                property.PropertyChanged += OnPropertyChanged;
                property.PropertyChanged -= OnPropertyChanged;
            }

            Scope();
            Assert.IsTrue(weakRef.IsAlive);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.IsFalse(weakRef.IsAlive);

            Assert.IsNotNull(source, "pin source");
        }

        [TestMethod]
        public void ToProperty_WithDeferSubscription_ShouldSubscribeWhenAddHandlerToPropertyChanged()
        {
            var source = new BehaviorSubject<int>(1);
            var count = 0;
            var property = source.Select(x =>
            {
                count++;
                return x;
            }).ToProperty(true);

            void OnPropertyChanged1(object sender, PropertyChangedEventArgs e)
            {
            }
            void OnPropertyChanged2(object sender, PropertyChangedEventArgs e)
            {
            }

            property.PropertyChanged += OnPropertyChanged1;
            property.PropertyChanged += OnPropertyChanged2;

            Assert.AreEqual(1, count); // Should Only subscribe once;

            property.PropertyChanged -= OnPropertyChanged1;
            property.PropertyChanged -= OnPropertyChanged2;

            source.OnNext(2);
            Assert.AreEqual(1, count); // shouldn't be updated.
        }
    }
}
