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
using ProductsDishes.DataAccess.Postgres.Repositories;

namespace ProductsDishes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ProductsDishesDbContext _db;

        public MainWindow(ProductsDishesDbContext db)
        {
            InitializeComponent();

            _db = db;

            var dishesRepository = new DishesRepository(_db);
            var productsRepository = new ProductsRepository(_db);
            var dishIngredientsRepository = new DishIngradientsRepository(_db);
            var userRepository = new UsersRepository(_db);
            var dailyRationsRepository = new DailyRationsRepository(_db);

            DataContext = new MainWindowViewModel(
                dishesRepository,
                productsRepository,
                dishIngredientsRepository,
                userRepository,
                dailyRationsRepository);
        }
    }
}