using MoveTheBoxSolver.Prediction;
using Plugin.Media;
using Plugin.Media.Abstractions;
using ReadWriteCsv;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MoveTheBoxSolver.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SolveByImagePage : ContentPage
    {
        private MediaFile _mediaFile;
        public MediaFile _MediaFile
        {
            get
            {
                return _mediaFile;
            }
            set
            {
                _mediaFile = value;
                OnPropertyChanged("IsUploadBtnEnable");
            }
        }

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                OnPropertyChanged("IsLoading");
                OnPropertyChanged("IsUploadBtnEnable");
                OnPropertyChanged("IsSelectBtnEnable");
            }
        }

        public bool IsSelectBtnEnable
        {
            get
            {
                return !IsLoading;
            }
        }

        public bool IsUploadBtnEnable
        {
            get
            {
                return _MediaFile != null && !IsLoading;
            }
        }

        public SolveByImagePage()
        {
            InitializeComponent();
            this.BindingContext = this;
        }

        //Picture choose from device    
        private async void btnSelectPic_Clicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Error", "This is not support on your device.", "OK");
                return;
            }
            else
            {
                var mediaOption = new PickMediaOptions()
                {
                    PhotoSize = PhotoSize.MaxWidthHeight
                };
                _MediaFile = await CrossMedia.Current.PickPhotoAsync();
            }
        }



        private async void btnSolve_Clicked(object sender, EventArgs e)
        {
            //BinaryReader reader = new BinaryReader(GetImageStream());

            try
            {
                if (_MediaFile == null)
                {
                    return;
                }

                IsLoading = true;

                var listbyte = await Task.Run(() =>
                {
                    SKBitmap myBitmap = new SKBitmap();
                    myBitmap = SKBitmap.Decode(_MediaFile.GetStream());
                    List<double> listbyte_wait = new List<double>();
                    for (int j = 0; j < myBitmap.Height; j++)
                    {
                        for (int i = 0; i < myBitmap.Width; i++)
                        {
                            //for (int j = 0; j < myBitmap.Height; j++)
                            //{
                            var color = myBitmap.GetPixel(i, j);
                            listbyte_wait.Add(((int)color.Red) / 255.00);
                            listbyte_wait.Add(((int)color.Green) / 255.00);
                            listbyte_wait.Add(((int)color.Blue) / 255.00);
                        }
                    }

                    return listbyte_wait;
                });

                if (listbyte.Count == 7581600)
                {
                    Predictor predictor = new Predictor(App.assembly);
                    var UnUsedMove = await predictor.PredictUnUsedMoveAsync(listbyte.ToArray());
                    var MoveLimit = await predictor.PredictMoveLimitAsync(listbyte.ToArray());
                    await DisplayAlert("Prediction", $"UnUsedMove : {UnUsedMove}{Environment.NewLine}MoveLimit : {MoveLimit}", "OK");
                }
                else
                {
                    await DisplayAlert("Not Support This Image", listbyte.Count.ToString(), "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsLoading = false;
            }

        }

        private int[] GetImageArray()
        {
            var stream = _MediaFile.GetStream();
            byte[] byte_arr = ReadFully(stream);
            int[] bytesAsInts = byte_arr.Select(x => (int)x).ToArray();
            return bytesAsInts;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

    }
}