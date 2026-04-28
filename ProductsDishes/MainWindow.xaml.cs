using ProductsDishes.DataAccess.Postgres.Repositories;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProductsDishes.DataAccess.Postgres;

namespace ProductsDishes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ProductsDishesDbContext _db;

        private bool _isUpdatingRationSelection = false;

        public MainWindow(ProductsDishesDbContext db)
        {
            InitializeComponent();

            _db = db;

            var dishesRepository = new DishesRepository(_db);
            var productsRepository = new ProductsRepository(_db);
            var dishIngredientsRepository = new DishIngradientsRepository(_db);
            var userRepository = new UsersRepository(_db);
            var dailyRationsRepository = new DailyRationsRepository(_db);
            var normCoefficientsRepository = new NormCoefficientsRepository(_db);

            DataContext = new MainWindowViewModel(
                dishesRepository,
                productsRepository,
                dishIngredientsRepository,
                userRepository,
                dailyRationsRepository,
                normCoefficientsRepository);
        }

        private void RationGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingRationSelection) return;
            _isUpdatingRationSelection = true;

            var current = (DataGrid)sender;

            foreach (var grid in new[] { BreakfastGrid, LunchGrid, DinnerGrid })
            {
                if (grid != current)
                    grid.SelectedItem = null;
            }

            if (DataContext is MainWindowViewModel vm)
                vm.SelectedRationDish = current.SelectedItem as RationDishViewModel;

            _isUpdatingRationSelection = false;
        }
    }
}