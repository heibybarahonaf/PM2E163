using GeolocatorPlugin;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace PM2E163.Vistas;

public partial class PaginaInicial : ContentPage
{
    FileResult photo; //Objeto Global
    private Controles.SitiosControl sitiosBD;
    public PaginaInicial(Controles.SitiosControl dbPath)
	{
		InitializeComponent();
        sitiosBD = dbPath;
    }
    public PaginaInicial()
    {
        InitializeComponent();
        sitiosBD = new Controles.SitiosControl();

    }

    public async Task GetLocationAsync()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium);
            var location = await Geolocation.GetLocationAsync(request);

            if (location != null)
            {
                txtLatitud.Text = "" + location.Latitude;
                txtLongitud.Text = "" + location.Longitude;
            }

        }
        catch (FeatureNotSupportedException fnsEx)
        {
            await DisplayAlert("Advertencia", fnsEx + "", "OK");
        }
        catch (PermissionException pEx)
        {
            await DisplayAlert("Advertencia", "Activa permisos", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Advertencia", "GPS Desactivado", "OK");
        }
    }

    private async Task CheckAndRequestLocationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (status == PermissionStatus.Granted)
        {
            GetLocationAsync();
        }
        else if (status == PermissionStatus.Denied)
        {
            await DisplayAlert("Advertencia", "Activa los permisos", "OK");
        }
    }
    public async Task CheckAndRequestPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (status == PermissionStatus.Granted)
        {
            await DisplayAlert("Aviso", "Permiso otorgado", "OK");
        }
        else if (status == PermissionStatus.Denied)
        {
            await DisplayAlert("Aviso", "Permiso denegado", "OK");
        }
    }

    private async void btnAgregar_Clicked(object sender, EventArgs e)
    {
        var sitio = new ModeloSQL.Sitios
        {
            Imagen = GetImage64(),
            latitud = txtLatitud.Text,
            longitud = txtLongitud.Text,
            descripcion = txtDescripcion.Text
        };

        if (validar() == true)
        {
            if (await sitiosBD.StoreSitio(sitio) > 0) //
            {
                await DisplayAlert("Aviso", "Agregado exitosamente", "OK");
                txtDescripcion.Text = string.Empty;
                foto.Source = null;
                photo = null;
            }
            else
            {
                await DisplayAlert("Aviso", "No se ha agregado", "OK");
            }
        }     

    }

    public Boolean validar()
    {
        Boolean campoVacio = true;
        if(string.IsNullOrEmpty(txtLatitud.Text))
        {
            campoVacio = false;
            DisplayAlert("Advertencia", "Campo de Latitud vacío", "OK");
        }else if(string.IsNullOrEmpty(txtLongitud.Text))
        {
            campoVacio = false;
            DisplayAlert("Advertencia", "Campo de Longitud vacío", "OK");
        }
        else if (string.IsNullOrEmpty(txtDescripcion.Text))
        {
            campoVacio = false;
            DisplayAlert("Advertencia", "Complete Descripción", "OK");
        }else if(photo == null)
        {
            campoVacio = false;
            DisplayAlert("Advertencia", "Imagen del Sitio Vacía", "OK");
        }

        return campoVacio;
    }
    private async void btnLista_Clicked(object sender, EventArgs e)
    {
        var items = await sitiosBD.GetListSitios();
        await Navigation.PushAsync(new Vistas.SelectSitios(items));
    }

    private void btnSalir_Clicked(object sender, EventArgs e)
    {
        Environment.Exit(0);    
    }

    private async void btnFoto_Clicked(object sender, EventArgs e)
    {
        photo = await MediaPicker.CapturePhotoAsync(); 

        if (photo != null)
        {
            string path = Path.Combine(FileSystem.CacheDirectory, photo.FileName); 
            using Stream sourcephoto = await photo.OpenReadAsync();
            using FileStream StreamLocal = File.OpenWrite(path);

            foto.Source = ImageSource.FromStream(() => photo.OpenReadAsync().Result);

            await sourcephoto.CopyToAsync(StreamLocal);
        }

    }

    public String GetImage64()
    {
        if (photo != null)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Stream stream = photo.OpenReadAsync().Result;
                stream.CopyTo(ms);
                byte[] data = ms.ToArray(); 

                String Base64 = Convert.ToBase64String(data);
                return Base64;
            }
        }
        return null;
    }
    protected async override void OnAppearing()
    {
        base.OnAppearing();

        var connection = Connectivity.NetworkAccess;
        var local = CrossGeolocator.Current;

        if (connection == NetworkAccess.Internet)
        {
            if (local == null || !local.IsGeolocationAvailable || !local.IsGeolocationEnabled)
            {

                CheckAndRequestLocationPermissionAsync();
            }
            else
            {
                GetLocationAsync();
            }
        }
        else
        {
            await DisplayAlert("Sin Acceso a internet", "Por favor, revisa tu conexion", "OK");
        }
    }
}