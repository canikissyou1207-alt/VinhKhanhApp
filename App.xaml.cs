namespace VinhKhanhApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Sửa dòng này để đảm bảo không bị nhận giá trị Null
            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(MainPage);
        }
    }
}
