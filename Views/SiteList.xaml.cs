using PM2E2GRUPO2.Models;

namespace PM2E2GRUPO2.Views;

public partial class SiteList : ContentPage
{
    private Service client = new Service();
    private List<Sitio> list = new List<Sitio>();
    private Sitio sitioSeleccionado;

    public SiteList()
	{
		InitializeComponent();
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        list =  await client.GetAllSitiosAsync();
        siteList.ItemsSource = list;
    }

    private async void OnPlayVideoClicked(Object sender, EventArgs e)
    {
        var button = (Button)sender;
        var site = (Sitio)button.BindingContext;
        await Navigation.PushAsync(new SiteVideo(site.Id));

    }
    private async void OnPlayAudioClicked(Object sender, EventArgs e)
    {
        var button = (Button)sender;
        var site = (Sitio)button.BindingContext;
    }
    private async void OnLocationClicked(Object sender, EventArgs e)
    {
        var button = (Button)sender;
        var site = (Sitio)button.BindingContext;

    }
    private async void OnEliminarClicked(Object sender, EventArgs e)
    {
        if (sitioSeleccionado != null)
        {
            bool eliminado = await client.DeleteSitioAsync(sitioSeleccionado);

            if (eliminado)
            {
                await DisplayAlert("Éxito", "El sitio se eliminó correctamente.", "OK");

                list.Remove(sitioSeleccionado);
                siteList.ItemsSource = null;
                siteList.ItemsSource = list;
            }
            else
            {
                await DisplayAlert("Error", "No se pudo eliminar el sitio.", "OK");
            }
        }
        else
        {

            await DisplayAlert("Error", "No se ha seleccionado un sitio.", "OK");
        }
    }


    private async void OnSeleccionarElemento(object sender, ItemTappedEventArgs e)
    {
        if (e.Item == null)
            return;
        sitioSeleccionado = (Sitio)e.Item;

    }
    private async void OnActualizarClicked(Object sender, EventArgs e)
    {

        if (sitioSeleccionado == null)
        {
            await DisplayAlert("Error", "Debe seleccionar un elemento", "OK");
            return;
        }

        await DisplayAlert("Información del Sitio",
                          $"Descripción: {sitioSeleccionado.Descripcion}",
                          "OK");
    }

}
