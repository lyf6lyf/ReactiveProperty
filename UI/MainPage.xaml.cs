using System;
using System.Reactive;
using System.Reactive.Linq;
using Windows.UI.Xaml.Controls;
using ReactiveProperty;

using static ReactiveProperty.PropertyEx;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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

        public IReadOnlyProperty<Timestamped<long>> Timer { get; }

        public ObservableCollection<string> Collection { get; }

        // Command
        public IProperty<bool> IsCommandEnabled { get; }

        public IObservableCommand<string, int> Command { get; }

        public IReadOnlyProperty<int> FinalResult { get; }

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

            IsCommandEnabled = CreateProperty(false);
            Command = CreateCommand<string, int>(async s =>
                {
                    await Task.Delay(2000);
                    return int.Parse(s) * 2;
                },
                IsCommandEnabled,
                s => int.TryParse(s, out _),
                6);

            Collection = new ObservableCollection<string>();
            Collection.Add("row1");
            Collection.Add("row2");
            var collectionCount = Collection.ToProperty(nameof(Collection.Count), x => x.Count);

            FinalResult = Number3.CombineLatest(Command, collectionCount, (x, y, z) => x + y + z).ToProperty(true);
        }

        private void AddItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Collection.Add("row" + (Collection.Count + 1));
        }

        private void RemoveItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (Collection.Count > 0)
            {
                Collection.RemoveAt(Collection.Count - 1);
            }
        }
    }
}
