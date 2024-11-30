using Plugin.Maui.Audio;
using PM2E2GRUPO2.Models;
#if ANDROID
using Android.Media; 
#endif


namespace PM2E2GRUPO2.Views;

public partial class SiteRegister : ContentPage
{
    private string _audioFilePath;
#if ANDROID
    private MediaRecorder _recorder;
#endif

    private bool _isRecording;
    private Byte[] VideoBase;
    private Byte[] AudioBase;
    public SiteRegister()
	{
		InitializeComponent();
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetLocation();
    }
    private async Task GetLocation()
    {
        try
        {
            var location = await Geolocation.Default.GetLocationAsync();
            if (location != null)
            {
                txtLatitud.Text = location.Latitude.ToString();
                txtLongitud.Text = location.Longitude.ToString();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo obtener la ubicaci�n.", "OK");
        }
    }

    private async void OnRecordVideoClicked(object sender, EventArgs e)
    {
        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var videoFile = await MediaPicker.Default.CaptureVideoAsync();

                if (videoFile != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var fileStream = File.OpenRead(videoFile.FullPath))
                        {
                            await fileStream.CopyToAsync(ms);
                        }
                        VideoBase = ms.ToArray();
                    }

                    Console.WriteLine($"Video guardado en: {videoFile.FullPath}");
                    Console.WriteLine($"VideoBase capturado: {Convert.ToBase64String(VideoBase).Substring(0, 100)}... (Tama�o: {VideoBase.Length} bytes)");

                    Preferences.Set("LastRecordedVideoPath", videoFile.FullPath);
                    await DisplayAlert("�xito", "Video grabado correctamente.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo grabar el video: {ex.Message}", "OK");
        }
    }

    private void OnPlayVideoClicked(object sender, EventArgs e)
    {
        try
        {
            var videoPath = Preferences.Get("LastRecordedVideoPath", string.Empty);

            if (!string.IsNullOrEmpty(videoPath))
            {
                mediaElement.Source = videoPath;
                mediaElement.Play();
            }
            else
            {
                DisplayAlert("Error", "No hay ning�n video grabado.", "OK");
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"No se pudo reproducir el video: {ex.Message}", "OK");
        }
    }
    private async void OnRecordAudioClicked(object sender, EventArgs e)
    {
#if ANDROID
        var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Error", "Permiso de micr�fono denegado.", "OK");
                return;
            }
        }

        if (_isRecording)
        {
            _recorder.Stop();
            _recorder.Release();
            _recorder.Dispose();
            _recorder = null;

            _isRecording = false;

            using (var ms = new MemoryStream()) 
            { 
                using (var fileStream = File.OpenRead(_audioFilePath)) 
                { 
                    await fileStream.CopyToAsync(ms); 
                } 
                    AudioBase = ms.ToArray();
                for (int i = 0; i < 10; i++) {
                    Console.WriteLine(AudioBase[i]);
                }
                }
           Console.WriteLine($"AudioBase capturado: {Convert.ToBase64String(AudioBase).Substring(0, 100)}... (Tama�o: {AudioBase.Length} bytes)");

            await DisplayAlert("Grabaci�n", "Audio grabado correctamente.", "OK");
        }
        else
        {
            _audioFilePath = Path.Combine(FileSystem.AppDataDirectory, "audio_recording.mp4");

            _recorder = new MediaRecorder();
            _recorder.SetAudioSource(AudioSource.Mic);
            _recorder.SetOutputFormat(OutputFormat.Mpeg4);
            _recorder.SetAudioEncoder(AudioEncoder.Aac);
            _recorder.SetOutputFile(_audioFilePath);

            _recorder.Prepare();
            _recorder.Start();
            _isRecording = true;

            await DisplayAlert("Grabaci�n", "Grabando audio...", "OK");
        }
#else
        await DisplayAlert("Error", "La grabaci�n de audio solo est� disponible en Android.", "OK");
#endif
    }

    private async void OnPlayAudioClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_audioFilePath))
        {
            await DisplayAlert("Error", "No hay audio grabado.", "OK");
            return;
        }

        try
        {
            audioMediaElement.Source = _audioFilePath;
            audioMediaElement.Play();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo reproducir el audio: {ex.Message}", "OK");
        }
    }

    private async void OnSave(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtDescripcion.Text) ||
            string.IsNullOrEmpty(txtLatitud.Text) ||
            string.IsNullOrEmpty(txtLongitud.Text))
        {
            await DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
            return;
        }

        if (AudioBase == null || AudioBase.Length == 0)
        {
            await DisplayAlert("Error", "Debes grabar un audio antes de guardar.", "OK");
            return;
        }

        if (VideoBase == null || VideoBase.Length == 0)
        {
            await DisplayAlert("Error", "Debes grabar un video antes de guardar.", "OK");
            return;
        }

        try
        {
            var sitio = new Sitio
            {
                Descripcion = txtDescripcion.Text,
                Latitud = double.Parse(txtLatitud.Text),
                Longitud = double.Parse(txtLongitud.Text),
                Video = VideoBase,
                Audio = AudioBase
            };

            var sitioService = new Service();
            var isSuccess = await sitioService.CreateSitioAsync(sitio);
            await GetLocation();
            VideoBase = null;
            AudioBase = null;
            txtDescripcion.Text = "";
            audioMediaElement = null;
            mediaElement = null;
            if (isSuccess)
            {
                await DisplayAlert("�xito", "Los datos han sido guardados con �xito.", "OK");
            }
            else
            {
                await DisplayAlert("Error", "No se pudo guardar los datos.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurri� un error: {ex.Message}", "OK");
            Console.WriteLine($"Detalles del error: {ex}");
        }
    }

    private async void OnShowList(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SiteList());
    }
}