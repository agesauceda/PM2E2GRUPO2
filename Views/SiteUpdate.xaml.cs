using Plugin.Maui.Audio;
using PM2E2GRUPO2.Models;

#if ANDROID
using Android.Media;
#endif

namespace PM2E2GRUPO2.Views;

public partial class SiteUpdate : ContentPage
{
	private Sitio site = new Sitio();
    private string _audioFilePath;
#if ANDROID
    private MediaRecorder _recorder;
#endif

    private bool _isRecording;
    private Byte[] VideoBase;
    private string VideoPath;
    private Byte[] AudioBase;
    private Service client = new Service();
    private Utils tools = new Utils();
    public SiteUpdate(int id, string des)
	{
		InitializeComponent();
		site.Id = id;
		site.Descripcion = des;
	}

	protected override async void OnAppearing() 
	{
		base.OnAppearing();
		txtDescripcionUpt.Text = site.Descripcion;
		await GetLocation();
        await DownloadAudio(site.Id);
        await DownloadVideo(site.Id);
	}

	private async Task GetLocation()
	{
		try
		{
			var location = await Geolocation.GetLocationAsync();
			if (location != null) {
				txtLatitudUpt.Text = location.Latitude.ToString();
				txtLongitudUpt.Text = location.Longitude.ToString();
			}
		}
		catch (Exception ex) {
			await DisplayAlert("Error", "No se pudo obtener la ubicación", "OK");
		}
	}

	private async void OnRecordUpdateVideoClicked(object sender, EventArgs e) {
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
                    Console.WriteLine($"VideoBase capturado: {Convert.ToBase64String(VideoBase).Substring(0, 100)}... (Tamaño: {VideoBase.Length} bytes)");

                    Preferences.Set("LastRecordedVideoPath", videoFile.FullPath);
                    await DisplayAlert("Éxito", "Video grabado correctamente.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo grabar el video: {ex.Message}", "OK");
        }
    }

	private async void OnPlayVideoUpdateClicked(object sender, EventArgs e) {
        try
        {
            if (string.IsNullOrEmpty(VideoPath)) {
                VideoPath = Preferences.Get("LastRecordedVideoPath", string.Empty);
            }

            if (!string.IsNullOrEmpty(VideoPath))
            {
                mediaElementUpt.Source = VideoPath;
                mediaElementUpt.Play();
            }
            else
            {
                await DisplayAlert("Error", "No hay ningún video grabado.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo reproducir el video: {ex.Message}", "OK");
        }
    }

	private async void OnRecordAudioUpdateClicked(object sender, EventArgs e) {
#if ANDROID
        var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Error", "Permiso de micrófono denegado.", "OK");
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
            }
            Console.WriteLine($"AudioBase capturado: {Convert.ToBase64String(AudioBase).Substring(0, 100)}... (Tamaño: {AudioBase.Length} bytes)");

            await DisplayAlert("Grabación", "Audio grabado correctamente.", "OK");
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

            await DisplayAlert("Grabación", "Grabando audio...", "OK");
        }
#else
        await DisplayAlert("Error", "La grabación de audio solo está disponible en Android.", "OK");
#endif
    }

    private async void OnPlayAudioUpdateClicked(object sender, EventArgs e) {
        if (string.IsNullOrEmpty(_audioFilePath))
        {
            await DisplayAlert("Error", "No hay audio grabado.", "OK");
            return;
        }

        try
        {
            audioMediaElementUpt.Source = _audioFilePath;
            audioMediaElementUpt.Play();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo reproducir el audio: {ex.Message}", "OK");
        }
    }

	private async void OnUpdateClicked(object sender, EventArgs e) {
        if (string.IsNullOrEmpty(txtDescripcionUpt.Text) ||
    string.IsNullOrEmpty(txtLatitudUpt.Text) ||
    string.IsNullOrEmpty(txtLongitudUpt.Text))
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
                Descripcion = txtDescripcionUpt.Text,
                Latitud = double.Parse(txtLatitudUpt.Text),
                Longitud = double.Parse(txtLongitudUpt.Text),
                Video = VideoBase,
                Audio = AudioBase,
                Id = site.Id
            };

            var sitioService = new Service();
            var isSuccess = await sitioService.UpdateSitioAsync(sitio);

            if (isSuccess)
            {
                await DisplayAlert("Éxito", "Los datos han sido guardados con éxito.", "OK");
            }
            else
            {
                await DisplayAlert("Error", "No se pudo guardar los datos.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            Console.WriteLine($"Detalles del error: {ex}");
        }
    }

    private async Task DownloadAudio(int id) {
        AudioBase = await client.getAudio(id);
        if (AudioBase != null) {
            for (int i = 0; i < 10; i++) {
                Console.WriteLine(AudioBase[i]);
            }
            _audioFilePath = await tools.SaveAudioAsync(AudioBase);
            Console.WriteLine(AudioBase.Length);
            Console.WriteLine(_audioFilePath);
        }
    }

    private async Task DownloadVideo(int id) {
        VideoBase = await client.getVideo(id);
        if (VideoBase != null) {
            VideoPath = await tools.SaveVideoAsync(VideoBase);
            Console.WriteLine(VideoBase.Length);
            Console.WriteLine(VideoPath);
        }
    }
}