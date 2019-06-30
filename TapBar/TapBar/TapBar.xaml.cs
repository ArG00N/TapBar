using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace TapBar
{
    public partial class TapBar : ContentView
    {

        public static readonly BindableProperty TapsProperty =
            BindableProperty.Create("Taps",
            typeof(TapItemCollection),
            typeof(TapBar),default(TapItemCollection),
            BindingMode.TwoWay,propertyChanged:TapsChanged);

        public TapItemCollection Taps
        {
            get
            {
                return (TapItemCollection)GetValue(TapsProperty);
            }
            set { SetValue(TapsProperty, value); }
        }

        private static void TapsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            
        }

        public TapBar()
        {
            InitializeComponent();
            this.LayoutChanged += TapBar_LayoutChanged;
        }

        private async void TapBar_LayoutChanged(object sender, EventArgs e)
        {
            await this.UpdateSelectedFrame(0);
            var first = this.Taps.Items.FirstOrDefault();

            ContentView.Content = first?.TapTemplate?.CreateContent() as View;
            ContentView.BindingContext = this.BindingContext;

            this.LayoutChanged -= TapBar_LayoutChanged;
        }

        private async Task UpdateSelectedFrame(int index)
        {
            var width = this.Width / this.Taps.Items.Count;
            var x = width * index;
            await this.BoxView.LayoutTo(new Rectangle(x, 0, width, this.BarGrid.Height),300, Easing.CubicIn);
        }


        async void Handle_Tapped(object sender, System.EventArgs e)
        {
            if(sender is Frame frame && frame.BindingContext is TapItem tap)
            {
                this.FlexLayout.Children.Cast<Frame>().ToList()
                    .ForEach(f =>
                    {
                        f.HasShadow = false;
                        f.BackgroundColor = Color.Transparent;
                        (f.Content as Label).TextColor = Color.FromHex("#006eb4");

                    });

                var i = this.Taps.Items.IndexOf(tap);

                ContentView.Scale = 0;
                ContentView.Opacity = 0;
                ContentView.BackgroundColor = Color.FromHex("#006eb4"); 
                ContentView.BindingContext = this.BindingContext;
                ContentView.Content = tap.TapTemplate.CreateContent() as View;

                await Task.WhenAll(
                    this.UpdateSelectedFrame(i),
                    this.ContentView.FadeTo(1),
                    this.ContentView.ScaleTo(1)
                    );
                (frame.Content as Label).TextColor = Color.White;
            }
        }
    }

    [ContentProperty("Items")]
    public class TapItemCollection : BindableObject
    {
        public static readonly BindableProperty ItemsProperty =
            BindableProperty.Create("Items",
            typeof(IList<TapItem>),
            typeof(TapItemCollection), new List<TapItem>(),
            BindingMode.TwoWay);
        public IList<TapItem> Items
        {
            get
            {
                return (IList<TapItem>)GetValue(ItemsProperty);
            }
            set { SetValue(ItemsProperty, value); }
        }

    }

        public class TapItem : BindableObject
    {

        public static readonly BindableProperty TapTemplateProperty =
            BindableProperty.Create("TapTemplate",
            typeof(DataTemplate),
            typeof(TapItem), default(DataTemplate),
            BindingMode.TwoWay);

        public DataTemplate TapTemplate
        {
            get
            {
                return (DataTemplate)GetValue(TapTemplateProperty);
            }
            set { SetValue(TapTemplateProperty, value); }
        }

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create("Title",
            typeof(string),
            typeof(TapItem), default(string),
            BindingMode.TwoWay);

        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly BindableProperty ContextProperty =
            BindableProperty.Create("Context",
            typeof(object),
            typeof(TapItem), default(object),
            BindingMode.TwoWay);

        public object Context
        {
            get
            {
                return (object)GetValue(ContextProperty);
            }
            set { SetValue(ContextProperty, value); }
        }

    }
}
