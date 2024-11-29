using CommunityToolkit.Maui.Views;
using PM2E2GRUPO2.Models;

namespace PM2E2GRUPO2.Views;

public partial class PopupVideo : Popup
{
	public PopupVideo(Sitio sitio)
	{
		InitializeComponent();
        reproducirVideo(sitio);
    }

    private async void reproducirVideo(Sitio sitio)
    {

        try
        {
            Console.WriteLine("Debug********** " + sitio.Descripcion);

            var videoPath = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}.mp4");

            if (string.IsNullOrEmpty(videoPath))
            {
                Console.WriteLine("No existe el archivo");
                return;
            }

            if (sitio.Video == null)
            {
                Console.WriteLine("El sitio no contiene audio.");
                return;
            }

            await File.WriteAllBytesAsync(videoPath, sitio.Video);

            Console.WriteLine("Debug********** " + videoPath);
            VideoPlayer.Source = MediaSource.FromFile(videoPath);
            VideoPlayer.Play();
        }
        catch (Exception ex)
        {
            //await DisplayAlert("Error", $"No se pudo reproducir el audio: {ex.Message}", "OK");
            Console.WriteLine(ex.Message);
        }
    }
    private void CloseVideoPopup(object sender, EventArgs e)
    {
        Close();
    }
}