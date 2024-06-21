namespace PM2E163
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Vistas.PaginaInicial());
        }
    }
}
