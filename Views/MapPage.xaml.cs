using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using PM2E2GRUPO2.Models;

namespace PM2E2GRUPO2.Views;

public partial class MapPage : ContentPage
{
    private Sitio sitio; 

    public MapPage(Sitio sitio)
    {
        InitializeComponent();
        this.sitio = sitio;

        Location loc = new Location(sitio.Latitud, sitio.Longitud);
        MapSpan mapSpan = MapSpan.FromCenterAndRadius(loc, Distance.FromKilometers(1));

        Pin pin = new Pin
        {
            Label = sitio.Descripcion,
            Location = loc,
            Type = PinType.Place
        };
        map.Pins.Add(pin);
        map.MoveToRegion(mapSpan);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await SolicitarPermisosUbicacion();
    }

    private async Task<PermissionStatus> SolicitarPermisosUbicacion()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            {
                await DisplayAlert("Permisos necesarios", "Se necesitan permisos para mostrar el mapa.", "OK");
            }
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        return status;
    }

    private async Task NavigateToLocation(double latitude, double longitude)
    {
        try
        {
            // Esto me permite navegar y abrir la app de mapa
            string uri = $"https://www.google.com/maps/dir/?api=1&destination={latitude},{longitude}&travelmode=driving";
            await Launcher.OpenAsync(new Uri(uri));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "No se pudo abrir la aplicación de mapas: " + ex.Message, "OK");
        }
    }
    private async void OnNavigateClicked(object sender, EventArgs e)
    {
        await NavigateToLocation(sitio.Latitud, sitio.Longitud);
    }

}


