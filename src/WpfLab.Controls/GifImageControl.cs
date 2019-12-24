using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfLab.Controls.Localization;
using Image = System.Windows.Controls.Image;

namespace WpfLab.Controls
{
    public class GifImageControl : Image
    {
        public static readonly DependencyProperty AllowClickToPauseProperty =
            DependencyProperty.Register("AllowClickToPause", typeof(bool), typeof(GifImageControl),
                                        new UIPropertyMetadata(true));

        public static readonly DependencyProperty GifSourceProperty =
            DependencyProperty.Register("GifSource", typeof(string), typeof(GifImageControl),
                                        new UIPropertyMetadata("", GIFSource_Changed));

        public static readonly DependencyProperty PlayAnimationProperty =
            DependencyProperty.Register("PlayAnimation", typeof(bool), typeof(GifImageControl),
                                        new UIPropertyMetadata(true, PlayAnimation_Changed));

        private Bitmap _Bitmap;

        private bool _mouseClickStarted;

        public GifImageControl()
        {
            MouseLeftButtonDown += GIFImageControl_MouseLeftButtonDown;
            MouseLeftButtonUp += GIFImageControl_MouseLeftButtonUp;
            MouseLeave += GIFImageControl_MouseLeave;
            Click += GIFImageControl_Click;
            //TODO:Future feature: Add a Play/Pause graphic on mouse over, and possibly a context menu
        }

        public bool AllowClickToPause
        {
            get { return (bool)GetValue(AllowClickToPauseProperty); }
            set { SetValue(AllowClickToPauseProperty, value); }
        }

        public bool PlayAnimation
        {
            get { return (bool)GetValue(PlayAnimationProperty); }
            set { SetValue(PlayAnimationProperty, value); }
        }

        public string GifSource
        {
            get { return (string)GetValue(GifSourceProperty); }
            set { SetValue(GifSourceProperty, value); }
        }

        private void GIFImageControl_Click(object sender, RoutedEventArgs e)
        {
            if (AllowClickToPause)
                PlayAnimation = !PlayAnimation;
        }

        private void GIFImageControl_MouseLeave(object sender, MouseEventArgs e)
        {
            _mouseClickStarted = false;
        }

        private void GIFImageControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_mouseClickStarted)
                FireClickEvent(sender, e);
            _mouseClickStarted = false;
        }

        private void GIFImageControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mouseClickStarted = true;
        }

        private void FireClickEvent(object sender, RoutedEventArgs e)
        {
            if (null != Click)
                Click(sender, e);
        }

        public event RoutedEventHandler Click;

        private static void PlayAnimation_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gic = (GifImageControl)d;
            if ((bool)e.NewValue)
            {
                //StartAnimation if GIFSource is properly set
                if (null != gic._Bitmap)
                    ImageAnimator.Animate(gic._Bitmap, gic.OnFrameChanged);
            }
            else
                //Pause Animation
                ImageAnimator.StopAnimate(gic._Bitmap, gic.OnFrameChanged);
        }


        private void SetImageGIFSource()
        {
            if (null != _Bitmap)
            {
                ImageAnimator.StopAnimate(_Bitmap, OnFrameChanged);
                _Bitmap = null;
            }
            if (String.IsNullOrEmpty(GifSource))
            {
                //Turn off if GIF set to null or empty
                Source = null;
                InvalidateVisual();
                return;
            }
            if (File.Exists(GifSource))
                _Bitmap = (Bitmap)System.Drawing.Image.FromFile(GifSource);
            else
            {
                //Support looking for embedded resources
                Assembly assemblyToSearch = Assembly.GetAssembly(GetType());
                _Bitmap = GetBitmapResourceFromAssembly(assemblyToSearch);
                if (null == _Bitmap)
                {
                    assemblyToSearch = Assembly.GetCallingAssembly();
                    _Bitmap = GetBitmapResourceFromAssembly(assemblyToSearch);
                    if (null == _Bitmap)
                    {
                        assemblyToSearch = Assembly.GetEntryAssembly();
                        _Bitmap = GetBitmapResourceFromAssembly(assemblyToSearch);
                        if (null == _Bitmap)
                            throw new FileNotFoundException(Messages.GifSourceNotFound, GifSource);
                    }
                }
            }
            if (PlayAnimation)
                ImageAnimator.Animate(_Bitmap, OnFrameChanged);
        }

        private Bitmap GetBitmapResourceFromAssembly(Assembly assemblyToSearch)
        {
            List<string> resourselist = assemblyToSearch.GetManifestResourceNames().ToList();
            if (null != assemblyToSearch.FullName)
            {
                string searchName = $"{assemblyToSearch.FullName.Split(',')[0]}.{GifSource}";
                if (resourselist.Contains(searchName))
                {
                    Stream bitmapStream = assemblyToSearch.GetManifestResourceStream(searchName);
                    if (null != bitmapStream)
                        return (Bitmap)System.Drawing.Image.FromStream(bitmapStream);
                }
            }
            return null;
        }

        private static void GIFSource_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GifImageControl)d).SetImageGIFSource();
        }


        private void OnFrameChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                   new OnFrameChangedDelegate(OnFrameChangedInMainThread));
        }

        private void OnFrameChangedInMainThread()
        {
            if (PlayAnimation)
            {
                ImageAnimator.UpdateFrames(_Bitmap);
                Source = GetBitmapSource(_Bitmap);
                InvalidateVisual();
            }
        }


        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        private static extern IntPtr DeleteObject(IntPtr hDc);

        private static BitmapSource GetBitmapSource(Bitmap gdiBitmap)
        {
            IntPtr hBitmap = gdiBitmap.GetHbitmap();
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
                                                                              IntPtr.Zero,
                                                                              Int32Rect.Empty,
                                                                              BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(hBitmap);
            return bitmapSource;
        }

        #region Nested type: OnFrameChangedDelegate

        private delegate void OnFrameChangedDelegate();

        #endregion
    }
}