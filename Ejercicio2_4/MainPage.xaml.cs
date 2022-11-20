using Ejercicio2_4.Model;
using MediaManager;
using Plugin.Media;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Ejercicio2_4
{
    public partial class MainPage : ContentPage
    {
        Plugin.Media.Abstractions.MediaFile videoFile = null;
        private string videoName = "";
        private int videoCount;
        
        public MainPage()
        {
            InitializeComponent();
            CrossMediaManager.Current.MediaItemFinished += Current_MediaItemFinished;
            CrossMediaManager.Current.AutoPlay = false;
        }        

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            fillCollectionView();            
        }

        private async void fillCollectionView()
        {
            try
            {
                var videoList = await App.DBase.obtenerVideos();
                videoCount = videoList.Count + 1;
                clvVideos.ItemsSource = videoList;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Vaya!", "Algo salió mal...", "OK");
                //Console.WriteLine(ex.Message);
            }
        }

        private async void btnRec_Clicked(object sender, EventArgs e)
        {
            try
            {
                string txbName = await DisplayPromptAsync("Información", "Ingrese el título del video", initialValue: "video" + videoCount, maxLength: 25);
                if (!string.IsNullOrEmpty(txbName))
                {
                    videoName = txbName;
                    videoFile = await CrossMedia.Current.TakeVideoAsync(new Plugin.Media.Abstractions.StoreVideoOptions
                    {
                        Directory = "Mis Videos",
                        Name = videoName,
                        SaveToAlbum = true
                    });

                    if (videoFile != null)
                    {
                        vvVideo.Source = videoFile.Path;

                        btnRec.IsVisible = false;
                        controlsVideo.IsVisible = true;
                    }                    
                }
                else
                {
                    await DisplayAlert("Aviso", "Video no guardaddo! \nDebe agregar el título del video", "OK");
                }
                
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            await guardarVideo();
            fillCollectionView();
            btnSave.IsVisible = false;
        }

        private void btnPlay_Clicked(object sender, EventArgs e)
        {            
            if (!CrossMediaManager.Current.IsPlaying())
            {
                CrossMediaManager.Current.Play();
                btnPlay.Source = "pausa.png";
            }
            else
            {
                CrossMediaManager.Current.Pause();
                btnPlay.Source = "play.png";
            }            
        }

        private void btnStop_Clicked(object sender, EventArgs e)
        {
            CrossMediaManager.Current.Stop();
            btnPlay.Source = "play.png";
        }

        private void btnNew_Clicked(object sender, EventArgs e)
        {
            controlsVideo.IsVisible = false;
            btnRec.IsVisible = true;
            btnSave.IsVisible = true;
            vvVideo.Source = null;
        }

        private async void frmCard_Tapped(object sender, EventArgs e)
        {
            try
            {
                Frame frame = (Frame)sender;
                var video = (Video)frame.BindingContext;
                vvVideo.Source = video.path;
                btnRec.IsVisible = false;
                controlsVideo.IsVisible = true;
                btnSave.IsVisible = false;

            }
            catch(Exception ex)
            {
                await DisplayAlert("Vaya!", "Algo salió mal", "OK");
            }
            
        }


        private async void swpDelete_Invoked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Confirmar", "Desea Eliminar este Video? \ntambién se eliminará de su dispositivo", "Sí", "No"))
            {
                var video = (Video)(sender as SwipeItemView).CommandParameter;
                var archivo = video.path;
                var result = await App.DBase.borrarVideo(video);
                File.Delete(archivo);

                if (result == 1)
                {
                    fillCollectionView();
                    await DisplayAlert("Aviso", "Video Eliminado", "OK");
                }
                else
                {
                    await DisplayAlert("Aviso", "Ha ocurrido un error", "OK");
                }
            }
        }

        private async Task guardarVideo()
        {                       
            DateTime DateAndTime = DateTime.Now;
            string date = DateAndTime.ToString("dd-MM-yyyy hh:mm:ss");

            if (videoFile == null)
            {
                await DisplayAlert("Error", "No ha grabado un video", "OK");
                return;
            }

            var video = new Video
            {
                id = 0,
                name = videoName,
                video = ConvertVideoToByteArray(),
                path = videoFile.Path,
                date = date

            };

            var result = await App.DBase.guardarVideo(video);

            if (result > 0)
            {
                btnSave.IsVisible = false;
                await DisplayAlert("Aviso", "Video Guardado", "OK");
            }
            else
            {
                await DisplayAlert("Aviso", "Ha ocurrido un error", "OK");
            }
        }

        private Byte[] ConvertVideoToByteArray()
        {
            if (videoFile != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Stream stream = videoFile.GetStream();
                    stream.CopyTo(memory);
                    return memory.ToArray();
                }
            }
            return null;
        }

        private void Current_MediaItemFinished(object sender, MediaManager.Media.MediaItemEventArgs e)
        {
            btnPlay.Source = "play.png";
        }        

        
    }
}
