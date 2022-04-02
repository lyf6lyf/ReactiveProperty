using System;
using System.Reactive;
using System.Reactive.Linq;
using Windows.UI.Xaml.Controls;
using ReactiveProperty;

using static ReactiveProperty.PropertyEx;

namespace UI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public IProperty<int> Number1 { get; }
        public IProperty<int> Number2 { get; }

        public IReadOnlyProperty<int> Number3 { get; }

        public IReadOnlyProperty<Timestamped<long>> Timer { get; set; }

        public MainPage()
        {
            InitializeComponent();

            Number1 = CreateProperty(1);
            Number2 = CreateProperty(2);

            Number3 = Number1.CombineLatest(Number2, (x, y) => x + y)
                .ToProperty();

            Timer = Observable.Timer(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(1))
                .Timestamp()
                .ObserveOnCoreDispatcher()
                .ToProperty();
        }

    }
}
