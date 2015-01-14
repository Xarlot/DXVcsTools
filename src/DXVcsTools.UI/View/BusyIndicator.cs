using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;

namespace DXVcsTools.UI {
    public class BusyIndicator {
        static ProgressBarBusyIndicatorThreadWrapper Wrapper { get; set; }
        public static readonly object Locker = new object();
        static Thread Thread { get; set; }
        public static void Show() {
            lock (Locker) {
                if (Wrapper != null)
                    return;
                Wrapper = new ProgressBarBusyIndicatorThreadWrapper();
                Thread = new Thread(Wrapper.Show);
                Thread.Priority = ThreadPriority.AboveNormal;
                Thread.SetApartmentState(ApartmentState.STA);
                Thread.Start();
            }
        }
        public static void Close() {
            lock (Locker) {
                if (Wrapper == null)
                    return;
                Wrapper.Close();
                Wrapper = null;
                Thread = null;
            }
        }
        public static void UpdateText(string text) {
            lock (Locker) {
                if (Wrapper == null)
                    return;
                Wrapper.Text = text;
            }
        }
        public static void UpdateProgress(int current, int count) {
            lock (Locker) {
                if (Wrapper == null)
                    return;
                Wrapper.Progress = current;
                Wrapper.Count = count;
                Wrapper.SupportProgress = true;
            }
        }
    }

    public enum BusyIndicatorStyle {
        Immediate,
        Marquee,
    }

    class ProgressBarBusyIndicatorThreadWrapper {
        public ProgressBusyIndicator Indicator { get; set; }
        public int Progress { get; set; }
        public int Count { get; set; }
        public bool SupportProgress { get; set; }
        public string Text { get; set; }
        public volatile bool ShouldStop;
        public void Show() {
            Indicator = new ProgressBusyIndicator();
            Indicator.Tag = this;
            Indicator.ShowDialog();
        }
        public void Close() {
            ShouldStop = true;
        }
    }
    class ProgressBusyIndicator : Window {
        public string Text { get { return ((ProgressBarBusyIndicatorThreadWrapper)Tag).Text; } }
        public int Progress { get { return ((ProgressBarBusyIndicatorThreadWrapper)Tag).Progress; } }
        public int Count { get { return ((ProgressBarBusyIndicatorThreadWrapper)Tag).Count; } }
        public bool SupportProgress { get { return ((ProgressBarBusyIndicatorThreadWrapper)Tag).SupportProgress; } }
        public bool ShouldStop { get { return ((ProgressBarBusyIndicatorThreadWrapper)Tag).ShouldStop; } }
        DispatcherTimer Timer { get; set; }
        public ProgressBusyIndicator() {
            BackgroundPanel panel = new BackgroundPanel();
            TextBlock tb = new TextBlock() { Text = "Loading...", Margin = new Thickness(30) };
            panel.Content = tb;
            Content = panel;
            Topmost = true;
            ThemeManager.SetThemeName(this, ThemeProvider.Instance.ThemeName);
            BorderThickness = new Thickness(1);
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowStyle = WindowStyle.None;
            SizeToContent = SizeToContent.WidthAndHeight;
            Timer = new DispatcherTimer();
            Timer.Tick += Timer_Tick;
            Timer.Interval = TimeSpan.FromMilliseconds(100);
            Timer.Start();
        }
        protected override void OnClosing(CancelEventArgs e) {
            Timer.Stop();
            base.OnClosing(e);
        }
        TextBlock tb;
        void Timer_Tick(object sender, EventArgs e) {
            lock (BusyIndicator.Locker) {
                if (ShouldStop)
                    Close();
                if (tb == null)
                    tb = (TextBlock)LayoutHelper.FindElementByType(this, typeof(TextBlock));
                tb.Text = SupportProgress ? string.Format(Text, Progress, Count) : Text;
            }
        }
    }
}
