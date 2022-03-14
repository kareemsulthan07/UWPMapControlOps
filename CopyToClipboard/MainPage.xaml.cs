using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CopyToClipboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // new york
                BasicGeoposition geoposition = new BasicGeoposition()
                {
                    Latitude = 40.7128,
                    Longitude = -74.0060,
                };

                Geopoint center = new Geopoint(geoposition);
                mapControl.Center = center;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void copyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();
                await renderTargetBitmap.RenderAsync(mapControl);
                var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
                var pixels = pixelBuffer.ToArray();

                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("clipboardImage.png", CreationCollisionOption.ReplaceExisting);
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Straight,
                        (uint)renderTargetBitmap.PixelWidth,
                        (uint)renderTargetBitmap.PixelHeight,
                        displayInformation.LogicalDpi,
                        displayInformation.LogicalDpi,
                        pixels);
                    await encoder.FlushAsync();
                }

                DataPackage dataPackage = new DataPackage();
                dataPackage.RequestedOperation = DataPackageOperation.Copy;
                dataPackage.SetBitmap(RandomAccessStreamReference.CreateFromFile(file));
                Clipboard.SetContent(dataPackage);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
