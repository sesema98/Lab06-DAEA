using System.Windows;
using Semana06.Data;
using Semana04_XS.ViewModels;

namespace Semana04_XS
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel(new NeptunoRepository(DbConfig.ConnectionString));
            DataContext = _viewModel;
            Loaded += async (_, _) => await _viewModel.CargarTodoCommand.ExecuteAsync(null);
        }
    }
}
