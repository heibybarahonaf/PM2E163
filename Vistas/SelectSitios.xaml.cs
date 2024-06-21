using System.Collections.ObjectModel;

namespace PM2E163.Vistas;

public partial class SelectSitios : ContentPage
{
    private Controles.SitiosControl rutaTarea;
    public ObservableCollection<ModeloSQL.Sitios> Items { get; set; }
    ModeloSQL.Sitios itemSeleccionado = null;
    public SelectSitios(IEnumerable<ModeloSQL.Sitios> ItemPersonas)
    {
        InitializeComponent();
        BindingContext = new ModeloSQL.ModeloSelect(ItemPersonas);
        rutaTarea = new Controles.SitiosControl();
        Items = new ObservableCollection<ModeloSQL.Sitios>();

        var viewModel = new ModeloSQL.ModeloSelect(ItemPersonas);
        this.BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        load_new_data();
    }

    private async void load_new_data()
    {
        var listaSitios = await rutaTarea.GetListSitios(); 
        Items.Clear();


        foreach (var persona in listaSitios) 
        {
            Items.Add(persona); 
        }
        this.BindingContext = this;
    }

    private async void btnAtras_Clicked(object sender, EventArgs e)
    {
        var page = new Vistas.PaginaInicial();
        await Navigation.PushAsync(page);
    }

    private async void btnEliminar_Clicked(object sender, EventArgs e)
    {
        if (validate_item()) return;

        bool answer = await DisplayAlert("Confirmación", "¿Está seguro de eliminar este registro?", "Sí", "No");
        if (answer == true)
        {
            await rutaTarea.DeleteSitio(itemSeleccionado);
            itemSeleccionado = null;
            load_new_data();
        }
    }

    private async void btnVerMapa_Clicked(object sender, EventArgs e)
    {
        if (validate_item()) return;

        bool answer = await DisplayAlert("Confirmación", "¿Desea ir a la ubicación indicada?", "Sí", "No");
        if (answer == true)
        {
            var mapa = new Vistas.PageMap(itemSeleccionado);
            await Navigation.PushAsync(mapa);
        }
    }

    private async void btnActualizar_Clicked(object sender, EventArgs e)
    {
        if (validate_item()) return;

        var actualizarPantalla = new Vistas.UpdateSitios();
        actualizarPantalla.BindingContext = itemSeleccionado;
        await Navigation.PushAsync(actualizarPantalla);
    }

    private bool validate_item()
    {
        if (itemSeleccionado == null)
        {
            DisplayAlert("Advertencia", "Selecciona un sitio de la lista", "OK");
            return true;
        }

        return false;
    }

    private async void OnTapped(object sender, SelectionChangedEventArgs e)
    {
        itemSeleccionado = e.CurrentSelection.FirstOrDefault() as ModeloSQL.Sitios;
    }
}