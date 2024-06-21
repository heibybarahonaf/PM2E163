using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Maps;
using GeolocatorPlugin;
namespace PM2E163.Vistas;

public partial class PageMap : ContentPage
{
    ModeloSQL.Sitios sitios;
    public PageMap()
	{
		InitializeComponent();
	}
    public PageMap(ModeloSQL.Sitios itemSeleccionado)
    {
        InitializeComponent();
        sitios = new ModeloSQL.Sitios();
        sitios = itemSeleccionado;

    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        var connection = Connectivity.NetworkAccess;
        var local = CrossGeolocator.Current;

        if (connection == NetworkAccess.Internet)
        {
            if (local != null && local.IsGeolocationAvailable && local.IsGeolocationEnabled)
            {
                var pinEstatico = new Pin
                {
                    Type = PinType.Place,
                    Location = new Location(Convert.ToDouble(sitios.latitud), Convert.ToDouble(sitios.longitud)),
                    Label = sitios.descripcion,
                };

                mapa.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(Convert.ToDouble(sitios.latitud), Convert.ToDouble(sitios.longitud)), Distance.FromKilometers(1)));
                mapa.Pins.Add(pinEstatico);
                mapa.IsShowingUser = true; 
            }
            else
            {
                if (!local.IsGeolocationEnabled)
                {
                    await DisplayAlert("GPS desactivado", "Por favor, activa el GPS", "OK");
                }
            }
        }
        else
        {
            await DisplayAlert("Sin Acceso a internet", "Por favor, revisa tu conexion", "OK");
        }
    }

    private async Task ShareImage(byte[] imageData, string filename)
    {
        var file = Path.Combine(FileSystem.CacheDirectory, filename);
        File.WriteAllBytes(file, imageData);

        await Share.RequestAsync(new ShareFileRequest
        {
            Title = "Compartir Imagen de Sitio",
            File = new ShareFile(file)
        });
    }

    private async void LoadImage_Clicked(object sender, EventArgs e)
    {
        byte[] array_image = Convert.FromBase64String(sitios.Imagen);
        await ShareImage(array_image, "ubicacion.jpg");
    }
}