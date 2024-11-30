using CommunityToolkit.Maui.Views;
using PM2E2GRUPO2.Models;
using Plugin.Maui.Audio;

namespace PM2E2GRUPO2.Views;

public partial class PopupAudio : Popup
{
	public PopupAudio(Sitio sitio)
	{
		InitializeComponent();

        Console.WriteLine(sitio);

        reproducirAudio(sitio);
	}

    private async void reproducirAudio(Sitio sitio)
    {
        
        try
        {
            Console.WriteLine("Debug********** "+ sitio.Descripcion);
       
            var audioPath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}.mp3");
           
            if (string.IsNullOrEmpty(audioPath))
            {
                Console.WriteLine("No existe el archivo");
                return;
            }

            if (sitio.Audio == null)
            {
                Console.WriteLine("El sitio no contiene audio.");
                return;
            }

            await File.WriteAllBytesAsync(audioPath, sitio.Audio);
         
            Console.WriteLine("Debug********** " + audioPath);
            AudioPlayer.Source = MediaSource.FromFile(audioPath);
            AudioPlayer.Play();
        }
        catch (Exception ex)
        {
            //await DisplayAlert("Error", $"No se pudo reproducir el audio: {ex.Message}", "OK");
            Console.WriteLine(ex.Message);
        }         
    }
    private void CloseAudioPopup(object sender, EventArgs e)
    {
        Close();
    }
}