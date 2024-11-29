using PM2E2GRUPO2.Models;
namespace PM2E2GRUPO2.Views;

public partial class SiteVideo : ContentPage
{
	private Service client = new Service();
	private int id;
	private Utils tools = new Utils();
	private Byte[]? video;
	public SiteVideo(int id)
	{
		InitializeComponent();
		this.id = id;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
	}

	private async void OnPlayVideoChargeClicked(Object sender, EventArgs e){
		video = await client.getVideo(id);
		if(video != null)
        {
			Console.WriteLine(video);
			Console.WriteLine(video.Length);
            string filePath = await tools.SaveVideoAsync(video);
			Console.WriteLine(filePath);
			if (File.Exists(filePath) && !string.IsNullOrEmpty(filePath)) {
				Console.WriteLine("Existe el archivo, bueno se crea");
				string videoContent = Convert.ToBase64String(video);
				Console.WriteLine($"Contenido del archivo base64 (truncado):\n{videoContent} \n...");
				videoPlayer.Source = filePath;
				videoPlayer.Play();
			}
			else {
				Console.WriteLine("No se pudo crear el archivo o no existe");
				await DisplayAlert("Error", "No se pudo guardar o reproducir el video", "OK");
			}
        }
        else
        {
            Console.WriteLine("Video es null");
            await DisplayAlert("Error", "No se pudo cargar el video", "OK");
        }
    }


}