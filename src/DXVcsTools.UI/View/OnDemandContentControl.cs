using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DXVcsTools.UI {
    public class OnDemandContentControl : ContentControl {
        public static readonly DependencyProperty DelayedContentProperty;

        public object DelayedContent {
            get { return GetValue(DelayedContentProperty); }
            set { SetValue(DelayedContentProperty, value); }
        }
        static OnDemandContentControl() {
            DelayedContentProperty = DependencyProperty.Register("DelayedContent", typeof(object), typeof(OnDemandContentControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, (o, args) => ((OnDemandContentControl)o).DelayedContentChanged(args.NewValue)));
        }
        readonly DispatcherTimer timer;
        public OnDemandContentControl() {
            timer = new DispatcherTimer();
            timer.Tick += TimerTick;
            timer.Interval = TimeSpan.FromSeconds(1);
        }
        void DelayedContentChanged(object p) {
            timer.Start();
        }
        void TimerTick(object sender, EventArgs e) {
            timer.Stop();
            Content = DelayedContent;
        }
    }
}
