using ProductsDishes.DataAccess.Postgres.Models;
using ProductsDishes.DataAccess.Postgres.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProductsDishes
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // ======= Repositories & common =======

        private readonly DishesRepository _dishesRepository;
        private readonly ProductsRepository _productsRepository;
        private readonly DishIngradientsRepository _dishIngredientsRepository;
        private readonly UsersRepository _usersRepository;
        private readonly DailyRationsRepository _dailyRationsRepository;

        public event PropertyChangedEventHandler? PropertyChanged;

        // ======= Collections =======

        public ObservableCollection<DishEntity> Dishes { get; } = new();
        public ObservableCollection<ProductEntity> Products { get; } = new();
        public ObservableCollection<DishEntity> DishesForView { get; } = new();
        public ObservableCollection<DishIngradientEntity> SelectedDishIngredientsForView { get; } = new();
        public ObservableCollection<UserEntity> Users { get; } = new();
        public ObservableCollection<DailyRationEntity> DailyRations { get; } = new();
        public ObservableCollection<UserEntity> SavedUsers { get; } = new();

        // ======= Ration collections =======

        public ObservableCollection<RationDishViewModel> BreakfastDishes { get; } = new();
        public ObservableCollection<RationDishViewModel> LunchDishes { get; } = new();
        public ObservableCollection<RationDishViewModel> DinnerDishes { get; } = new();
        public ObservableCollection<string> SavedRationDates { get; } = new();

        // ======= Selected entities =======

        private DishEntity? _selectedDish;
        public DishEntity? SelectedDish
        {
            get => _selectedDish;
            set
            {
                _selectedDish = value;
                OnPropertyChanged();
                LoadDishToEdit(_selectedDish);
                _ = ReloadDishAndRecalculateAsync(_selectedDish);
            }
        }

        private DishEntity? _selectedDishForView;
        public DishEntity? SelectedDishForView
        {
            get => _selectedDishForView;
            set
            {
                _selectedDishForView = value;
                OnPropertyChanged();
                IngredientsCurrentPage = 1;
                _ = LoadIngredientsForViewPageAsync();
            }
        }

        private ProductEntity? _selectedProduct;
        public ProductEntity? SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(); LoadProductToEdit(value); }
        }

        private UserEntity? _selectedUser;
        public UserEntity? SelectedUser
        {
            get => _selectedUser;
            set { _selectedUser = value; OnPropertyChanged(); }
        }

        private string? _selectedRationDate;
        public string? SelectedRationDate
        {
            get => _selectedRationDate;
            set { _selectedRationDate = value; OnPropertyChanged(); }
        }

        // ======= Dish edit fields & totals =======

        public string EditDishName { get; set; } = string.Empty;
        public string EditDishDescription { get; set; } = string.Empty;

        public decimal DishTotalCalories { get; private set; }
        public decimal DishTotalProtein { get; private set; }
        public decimal DishTotalFat { get; private set; }
        public decimal DishTotalCarbs { get; private set; }

        // ======= Product edit fields =======

        public string EditProductName { get; set; } = string.Empty;
        public string EditProductCalories { get; set; } = string.Empty;
        public string EditProductProtein { get; set; } = string.Empty;
        public string EditProductFat { get; set; } = string.Empty;
        public string EditProductCarbs { get; set; } = string.Empty;

        // ======= User profile fields =======

        public string UserName { get; set; } = string.Empty;
        public string UserAge { get; set; } = string.Empty;
        public string UserWeight { get; set; } = string.Empty;
        public string UserHeight { get; set; } = string.Empty;
        public string UserGender { get; set; } = "Male";
        public string UserActivityLevel { get; set; } = "Sedentary";
        public string UserGoal { get; set; } = "Maintain weight";

        private UserEntity? _currentUser;

        // ======= Daily norm (calculated) =======

        public decimal DailyCaloriesNorm { get; private set; }
        public decimal DailyProteinNorm { get; private set; }
        public decimal DailyFatNorm { get; private set; }
        public decimal DailyCarbsNorm { get; private set; }

        // ======= Ration totals =======

        public decimal RationTotalCalories { get; private set; }
        public decimal RationTotalProtein { get; private set; }
        public decimal RationTotalFat { get; private set; }
        public decimal RationTotalCarbs { get; private set; }

        // ======= Paging =======

        public int ProductsPageSize { get; } = 25;
        private int _productsCurrentPage = 1;
        public int ProductsCurrentPage
        {
            get => _productsCurrentPage;
            set
            {
                if (_productsCurrentPage == value) return;
                _productsCurrentPage = value;
                OnPropertyChanged();
            }
        }

        public int DishesPageSize { get; } = 10;
        private int _dishesCurrentPage = 1;
        public int DishesCurrentPage
        {
            get => _dishesCurrentPage;
            set
            {
                if (_dishesCurrentPage == value) return;
                _dishesCurrentPage = value;
                OnPropertyChanged();
            }
        }

        public int DishesForViewPageSize { get; } = 17;
        private int _dishesForViewCurrentPage = 1;
        public int DishesForViewCurrentPage
        {
            get => _dishesForViewCurrentPage;
            set
            {
                if (_dishesForViewCurrentPage == value) return;
                _dishesForViewCurrentPage = value;
                OnPropertyChanged();
            }
        }

        public int IngredientsPageSize { get; } = 17;
        private int _ingredientsCurrentPage = 1;
        public int IngredientsCurrentPage
        {
            get => _ingredientsCurrentPage;
            set
            {
                if (_ingredientsCurrentPage == value) return;
                _ingredientsCurrentPage = value;
                OnPropertyChanged();
            }
        }

        // ======= Search state =======

        public string ProductSearchText { get; set; } = string.Empty;
        private bool _isProductSearchMode;

        public string DishSearchText { get; set; } = string.Empty;
        private bool _isDishSearchMode;

        // ======= Commands: dishes =======

        public ICommand AddDishCommand { get; }
        public ICommand UpdateDishCommand { get; }
        public ICommand DeleteDishCommand { get; }

        public ICommand NextDishesPageCommand { get; }
        public ICommand PrevDishesPageCommand { get; }

        public ICommand SearchDishesCommand { get; }
        public ICommand ClearDishesSearchCommand { get; }

        public ICommand NextDishesForViewPageCommand { get; }
        public ICommand PrevDishesForViewPageCommand { get; }

        public ICommand EditDishIngredientsCommand { get; }

        // ======= Commands: products =======

        public ICommand AddProductCommand { get; }
        public ICommand UpdateProductCommand { get; }
        public ICommand DeleteProductCommand { get; }

        public ICommand NextProductsPageCommand { get; }
        public ICommand PrevProductsPageCommand { get; }

        public ICommand SearchProductsCommand { get; }
        public ICommand ClearProductsSearchCommand { get; }

        // ======= Commands: ingredients (view tab) =======

        public ICommand NextIngredientsPageCommand { get; }
        public ICommand PrevIngredientsPageCommand { get; }

        // ======= Commands: profile & ration =======

        public ICommand SaveUserCommand { get; }
        public ICommand GenerateDailyRationCommand { get; }
        public ICommand SaveRationCommand { get; }
        public ICommand LoadUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand LoadRationCommand { get; }
        public ICommand DeleteRationCommand { get; }

        // ======= Ctor =======

        public MainWindowViewModel(
            DishesRepository dishesRepository,
            ProductsRepository productsRepository,
            DishIngradientsRepository dishIngredientsRepository,
            UsersRepository usersRepository,
            DailyRationsRepository dailyRationsRepository)
        {
            _dishesRepository = dishesRepository;
            _productsRepository = productsRepository;
            _dishIngredientsRepository = dishIngredientsRepository;

            // dishes
            AddDishCommand = new RelayCommand(async _ => await AddDishAsync());
            UpdateDishCommand = new RelayCommand(async _ => await UpdateDishAsync(), _ => SelectedDish != null);
            DeleteDishCommand = new RelayCommand(async _ => await DeleteDishAsync(), _ => SelectedDish != null);
            NextDishesPageCommand = new RelayCommand(async _ => await NextDishesPageAsync());
            PrevDishesPageCommand = new RelayCommand(async _ => await PrevDishesPageAsync());
            SearchDishesCommand = new RelayCommand(async _ => await SearchDishesAsync());
            ClearDishesSearchCommand = new RelayCommand(async _ => await ClearDishesSearchAsync());
            NextDishesForViewPageCommand = new RelayCommand(async _ => await NextDishesForViewPageAsync());
            PrevDishesForViewPageCommand = new RelayCommand(async _ => await PrevDishesForViewPageAsync());
            EditDishIngredientsCommand = new RelayCommand(
                _ => OpenEditDishIngredients(),
                _ => SelectedDish != null);

            // products
            AddProductCommand = new RelayCommand(async _ => await AddProductAsync());
            UpdateProductCommand = new RelayCommand(async _ => await UpdateProductAsync(), _ => SelectedProduct != null);
            DeleteProductCommand = new RelayCommand(async _ => await DeleteProductAsync(), _ => SelectedProduct != null);
            NextProductsPageCommand = new RelayCommand(async _ => await NextProductsPageAsync());
            PrevProductsPageCommand = new RelayCommand(async _ => await PrevProductsPageAsync());
            SearchProductsCommand = new RelayCommand(async _ => await SearchProductsAsync());
            ClearProductsSearchCommand = new RelayCommand(async _ => await ClearProductsSearchAsync());

            // ingredients (view tab)
            NextIngredientsPageCommand = new RelayCommand(async _ => await NextIngredientsPageAsync());
            PrevIngredientsPageCommand = new RelayCommand(async _ => await PrevIngredientsPageAsync());

            _usersRepository = usersRepository;
            _dailyRationsRepository = dailyRationsRepository;

            SaveUserCommand = new RelayCommand(async _ => await SaveUserAsync());
            GenerateDailyRationCommand = new RelayCommand(async _ => await GenerateDailyRationAsync());
            SaveRationCommand = new RelayCommand(async _ => await SaveRationAsync());

            LoadUserCommand = new RelayCommand(async _ => await LoadSelectedUserAsync(), _ => SelectedUser != null);
            DeleteUserCommand = new RelayCommand(async _ => await DeleteSelectedUserAsync(), _ => SelectedUser != null);
            LoadRationCommand = new RelayCommand(async _ => await LoadSelectedRationAsync(), _ => SelectedRationDate != null);
            DeleteRationCommand = new RelayCommand(async _ => await DeleteSelectedRationAsync(), _ => SelectedRationDate != null);

            _ = LoadAllAsync();
        }

        // ======= Load methods =======

        private async Task LoadAllAsync()
        {
            await LoadDishesPageAsync();
            await LoadDishesForViewPageAsync();
            await LoadProductsPageAsync();
            await LoadSavedUsersAsync();
            await LoadSavedRationDatesAsync();
        }

        private async Task LoadProductsPageAsync()
        {
            Products.Clear();
            var list = await _productsRepository.GetByPage(ProductsCurrentPage, ProductsPageSize);
            foreach (var p in list)
                Products.Add(p);
        }

        private async Task LoadProductsSearchPageAsync()
        {
            Products.Clear();

            var list = await _productsRepository.GetByNamePaged(
                ProductSearchText, ProductsCurrentPage, ProductsPageSize);

            foreach (var p in list)
                Products.Add(p);
        }

        private async Task LoadDishesPageAsync()
        {
            Dishes.Clear();
            var list = await _dishesRepository.GetByPage(DishesCurrentPage, DishesPageSize);
            foreach (var d in list)
                Dishes.Add(d);
        }

        private async Task LoadDishesSearchPageAsync()
        {
            Dishes.Clear();

            var list = await _dishesRepository.GetByNamePaged(
                DishSearchText, DishesCurrentPage, DishesPageSize);

            foreach (var d in list)
                Dishes.Add(d);
        }

        private async Task LoadDishesForViewPageAsync()
        {
            DishesForView.Clear();
            var list = await _dishesRepository.GetByPage(DishesForViewCurrentPage, DishesForViewPageSize);
            foreach (var d in list)
                DishesForView.Add(d);
        }

        private async Task LoadIngredientsForViewPageAsync()
        {
            SelectedDishIngredientsForView.Clear();
            if (SelectedDishForView == null) return;

            var pageItems = await _dishIngredientsRepository
                .GetByDishPageAsync(SelectedDishForView.Id, IngredientsCurrentPage, IngredientsPageSize);

            foreach (var di in pageItems)
                SelectedDishIngredientsForView.Add(di);
        }

        private async Task ReloadDishAndRecalculateAsync(DishEntity? dish)
        {
            if (dish == null)
            {
                RecalculateDishTotals(null);
                return;
            }

            var fullDish = await _dishesRepository.GetWithIngredientsAsync(dish.Id);
            RecalculateDishTotals(fullDish);
        }

        // ======= Dishes: CRUD & validation =======

        private bool TryValidateDish()
        {
            if (string.IsNullOrWhiteSpace(EditDishName))
                return false;

            return true;
        }

        private async Task AddDishAsync()
        {
            if (!TryValidateDish())
            {
                MessageBox.Show("Dish name is required.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var nameExists = await _dishesRepository.ExistsByNameAsync(EditDishName.Trim());
            if (nameExists)
            {
                MessageBox.Show("Dish with the same name already exists.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var tempDish = new DishEntity
            {
                Id = Guid.NewGuid(),
                Name = EditDishName.Trim(),
                Description = EditDishDescription?.Trim() ?? string.Empty,
                Ingredients = new List<DishIngradientEntity>()
            };

            var vm = new EditDishIngredientsViewModel(
                tempDish,
                _productsRepository,
                _dishIngredientsRepository,
                isNewDish: true);

            var window = new EditDishIngredientsWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };
            window.ShowDialog();

            if (!vm.HasIngredients)
            {
                MessageBox.Show(
                    "Dish must have at least one ingredient.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _dishesRepository.Add(
                    tempDish.Id,
                    tempDish.Name,
                    tempDish.Description,
                    vm.IngredientsToSave);

                await LoadAllAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task UpdateDishAsync()
        {
            if (SelectedDish == null) return;

            if (!TryValidateDish())
            {
                MessageBox.Show("Dish name is required.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                await _dishesRepository.Update(SelectedDish.Id,
                    EditDishName,
                    EditDishDescription,
                    SelectedDish.Ingredients);

                await LoadAllAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            await LoadAllAsync();
        }

        private async Task DeleteDishAsync()
        {
            if (SelectedDish == null) return;
            await _dishesRepository.Delete(SelectedDish.Id);
            await LoadAllAsync();
        }

        private void LoadDishToEdit(DishEntity? dish)
        {
            if (dish == null) return;
            EditDishName = dish.Name;
            EditDishDescription = dish.Description;
            OnPropertyChanged(nameof(EditDishName));
            OnPropertyChanged(nameof(EditDishDescription));
        }

        // ======= Dishes: paging & search =======

        private async Task NextDishesPageAsync()
        {
            DishesCurrentPage++;

            if (_isDishSearchMode)
                await LoadDishesSearchPageAsync();
            else
                await LoadDishesPageAsync();
        }

        private async Task PrevDishesPageAsync()
        {
            if (DishesCurrentPage <= 1)
                return;

            DishesCurrentPage--;

            if (_isDishSearchMode)
                await LoadDishesSearchPageAsync();
            else
                await LoadDishesPageAsync();
        }

        private async Task NextDishesForViewPageAsync()
        {
            DishesForViewCurrentPage++;
            await LoadDishesForViewPageAsync();
        }

        private async Task PrevDishesForViewPageAsync()
        {
            if (DishesForViewCurrentPage <= 1)
                return;

            DishesForViewCurrentPage--;
            await LoadDishesForViewPageAsync();
        }

        private async Task SearchDishesAsync()
        {
            if (string.IsNullOrWhiteSpace(DishSearchText))
            {
                _isDishSearchMode = false;
                DishesCurrentPage = 1;
                await LoadDishesPageAsync();
                return;
            }

            _isDishSearchMode = true;
            DishesCurrentPage = 1;
            await LoadDishesSearchPageAsync();
        }

        private async Task ClearDishesSearchAsync()
        {
            DishSearchText = string.Empty;
            OnPropertyChanged(nameof(DishSearchText));

            _isDishSearchMode = false;
            DishesCurrentPage = 1;
            await LoadDishesPageAsync();
        }

        // ======= Products: CRUD & validation =======

        private bool TryGetProductNumbers(
            out decimal cal, out decimal prot,
            out decimal fat, out decimal carbs)
        {
            cal = prot = fat = carbs = 0;

            var culture = System.Globalization.CultureInfo.InvariantCulture;

            if (!decimal.TryParse(EditProductCalories, System.Globalization.NumberStyles.Any, culture, out cal))
                return false;
            if (!decimal.TryParse(EditProductProtein, System.Globalization.NumberStyles.Any, culture, out prot))
                return false;
            if (!decimal.TryParse(EditProductFat, System.Globalization.NumberStyles.Any, culture, out fat))
                return false;
            if (!decimal.TryParse(EditProductCarbs, System.Globalization.NumberStyles.Any, culture, out carbs))
                return false;

            return true;
        }

        private async Task AddProductAsync()
        {
            var id = Guid.NewGuid();

            if (!TryGetProductNumbers(out var cal, out var prot, out var fat, out var carbs))
            {
                MessageBox.Show("Invalid format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                await _productsRepository.Add(id,
                    EditProductName,
                    cal,
                    prot,
                    fat,
                    carbs);

                await LoadAllAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            await LoadAllAsync();
        }

        private async Task UpdateProductAsync()
        {
            if (SelectedProduct == null) return;

            if (!TryGetProductNumbers(out var cal, out var prot, out var fat, out var carbs))
            {
                MessageBox.Show("Invalid format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                await _productsRepository.Update(SelectedProduct.Id,
                    EditProductName,
                    cal,
                    prot,
                    fat,
                    carbs);

                await LoadAllAsync();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            await LoadAllAsync();
        }

        private async Task DeleteProductAsync()
        {
            if (SelectedProduct == null) return;
            await _productsRepository.Delete(SelectedProduct.Id);
            await LoadAllAsync();
        }

        private void LoadProductToEdit(ProductEntity? product)
        {
            if (product == null) return;
            EditProductName = product.Name;
            EditProductCalories = product.CaloriesPer100.ToString();
            EditProductProtein = product.ProteinPer100.ToString();
            EditProductFat = product.FatPer100.ToString();
            EditProductCarbs = product.CarbsPer100.ToString();

            OnPropertyChanged(nameof(EditProductName));
            OnPropertyChanged(nameof(EditProductCalories));
            OnPropertyChanged(nameof(EditProductProtein));
            OnPropertyChanged(nameof(EditProductFat));
            OnPropertyChanged(nameof(EditProductCarbs));
        }

        // ======= Products: paging & search =======

        private async Task NextProductsPageAsync()
        {
            ProductsCurrentPage++;

            if (_isProductSearchMode)
                await LoadProductsSearchPageAsync();
            else
                await LoadProductsPageAsync();
        }

        private async Task PrevProductsPageAsync()
        {
            if (ProductsCurrentPage <= 1)
                return;

            ProductsCurrentPage--;

            if (_isProductSearchMode)
                await LoadProductsSearchPageAsync();
            else
                await LoadProductsPageAsync();
        }

        private async Task SearchProductsAsync()
        {
            if (string.IsNullOrWhiteSpace(ProductSearchText))
            {
                _isProductSearchMode = false;
                ProductsCurrentPage = 1;
                await LoadProductsPageAsync();
                return;
            }

            _isProductSearchMode = true;
            ProductsCurrentPage = 1;
            await LoadProductsSearchPageAsync();
        }

        private async Task ClearProductsSearchAsync()
        {
            ProductSearchText = string.Empty;
            OnPropertyChanged(nameof(ProductSearchText));

            _isProductSearchMode = false;
            ProductsCurrentPage = 1;
            await LoadProductsPageAsync();
        }

        // ======= Ingredients: paging & edit window =======

        private async Task NextIngredientsPageAsync()
        {
            if (SelectedDishForView == null) return;

            IngredientsCurrentPage++;
            await LoadIngredientsForViewPageAsync();
        }

        private async Task PrevIngredientsPageAsync()
        {
            if (SelectedDishForView == null) return;
            if (IngredientsCurrentPage <= 1)
                return;

            IngredientsCurrentPage--;
            await LoadIngredientsForViewPageAsync();
        }

        private void OpenEditDishIngredients()
        {
            if (SelectedDish == null) return;

            var vm = new EditDishIngredientsViewModel(
                SelectedDish,
                _productsRepository,
                _dishIngredientsRepository);

            var window = new EditDishIngredientsWindow
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow
            };

            window.ShowDialog();

            _ = ReloadDishAndRecalculateAsync(SelectedDish);
        }

        // ======= Dish totals calculation =======

        private void RecalculateDishTotals(DishEntity? dish)
        {
            DishTotalCalories = DishTotalProtein = DishTotalFat = DishTotalCarbs = 0m;
            if (dish == null || dish.Ingredients == null) { RaiseTotalsChanged(); return; }

            foreach (var di in dish.Ingredients)
            {
                if (di.Product == null) continue;
                var factor = di.QuantityGrams / 100m;

                DishTotalCalories += di.Product.CaloriesPer100 * factor;
                DishTotalProtein += di.Product.ProteinPer100 * factor;
                DishTotalFat += di.Product.FatPer100 * factor;
                DishTotalCarbs += di.Product.CarbsPer100 * factor;
            }

            RaiseTotalsChanged();
        }

        private void RaiseTotalsChanged()
        {
            OnPropertyChanged(nameof(DishTotalCalories));
            OnPropertyChanged(nameof(DishTotalProtein));
            OnPropertyChanged(nameof(DishTotalFat));
            OnPropertyChanged(nameof(DishTotalCarbs));
        }

        // ======= User: save =======

        private async Task SaveUserAsync()
        {
            if (!TryParseUserFields(out var age, out var weight, out var height))
            {
                MessageBox.Show("Invalid numeric fields.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (_currentUser == null)
                {
                    var id = Guid.NewGuid();
                    await _usersRepository.Add(id, UserName, age, height, weight,
                        UserGender, UserActivityLevel, UserGoal);

                    _currentUser = await _usersRepository.GetById(id);
                }
                else
                {
                    await _usersRepository.Update(_currentUser.Id, UserName, age, height, weight,
                        UserGender, UserActivityLevel, UserGoal);

                    _currentUser = await _usersRepository.GetById(_currentUser.Id);
                }

                RecalculateNorms(weight, height, age, UserGender, UserActivityLevel, UserGoal);

                await LoadSavedUsersAsync();
                await LoadSavedRationDatesAsync();

                MessageBox.Show("Profile saved!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool TryParseUserFields(out int age, out decimal weight, out decimal height)
        {
            age = 0; weight = 0; height = 0;

            var culture = System.Globalization.CultureInfo.InvariantCulture;

            if (!int.TryParse(UserAge, out age)) return false;
            if (!decimal.TryParse(UserWeight, System.Globalization.NumberStyles.Any, culture, out weight)) return false;
            if (!decimal.TryParse(UserHeight, System.Globalization.NumberStyles.Any, culture, out height)) return false;

            return true;
        }

        // ======= Norm calculation (Mifflin-St Jeor) =======

        private void RecalculateNorms(
            decimal weightKg, decimal heightCm, int age,
            string gender, string activityLevel, string goal)
        {
            double bmr = gender == "Male"
                ? 10 * (double)weightKg + 6.25 * (double)heightCm - 5 * age + 5
                : 10 * (double)weightKg + 6.25 * (double)heightCm - 5 * age - 161;

            double activityFactor = activityLevel switch
            {
                "Light" => 1.375,
                "Moderate" => 1.55,
                "Active" => 1.725,
                "Very Active" => 1.9,
                _ => 1.2
            };

            double tdee = bmr * activityFactor;

            tdee = goal switch
            {
                "Lose weight" => tdee - 500,
                "Gain weight" => tdee + 500,
                _ => tdee
            };

            DailyCaloriesNorm = (decimal)tdee;
            DailyProteinNorm = DailyCaloriesNorm * 0.25m / 4m;
            DailyFatNorm = DailyCaloriesNorm * 0.30m / 9m;
            DailyCarbsNorm = DailyCaloriesNorm * 0.45m / 4m;

            OnPropertyChanged(nameof(DailyCaloriesNorm));
            OnPropertyChanged(nameof(DailyProteinNorm));
            OnPropertyChanged(nameof(DailyFatNorm));
            OnPropertyChanged(nameof(DailyCarbsNorm));
        }

        // ======= Daily ration generation =======

        private async Task GenerateDailyRationAsync()
        {
            if (DailyCaloriesNorm == 0)
            {
                MessageBox.Show("Save your profile first to calculate norms.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var allDishes = await _dishesRepository.GetWithProductsAsync();

            var scored = allDishes
                .Select(d => ToRationVm(d))
                .Where(d => d.TotalCalories > 0)
                .ToList();

            if (scored.Count == 0)
            {
                MessageBox.Show("No dishes with ingredients found.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            decimal breakfastTarget = DailyCaloriesNorm * 0.25m;
            decimal lunchTarget = DailyCaloriesNorm * 0.40m;
            decimal dinnerTarget = DailyCaloriesNorm * 0.35m;

            BreakfastDishes.Clear();
            LunchDishes.Clear();
            DinnerDishes.Clear();

            var rng = new Random();
            var shuffled = scored.OrderBy(_ => rng.Next()).ToList();

            BreakfastDishes.Clear();
            LunchDishes.Clear();
            DinnerDishes.Clear();

            var usedIds = new HashSet<Guid>();

            FillMealNoDupes(BreakfastDishes, shuffled, breakfastTarget, "Breakfast", usedIds);
            FillMealNoDupes(LunchDishes, shuffled, lunchTarget, "Lunch", usedIds);
            FillMealNoDupes(DinnerDishes, shuffled, dinnerTarget, "Dinner", usedIds);

            RecalculateRationTotals();
        }

        private static RationDishViewModel ToRationVm(DishEntity d)
        {
            decimal cal = 0, prot = 0, fat = 0, carbs = 0;

            foreach (var ing in d.Ingredients ?? new List<DishIngradientEntity>())
            {
                if (ing.Product == null) continue;
                var f = ing.QuantityGrams / 100m;
                cal += ing.Product.CaloriesPer100 * f;
                prot += ing.Product.ProteinPer100 * f;
                fat += ing.Product.FatPer100 * f;
                carbs += ing.Product.CarbsPer100 * f;
            }

            return new RationDishViewModel
            {
                DishId = d.Id,
                Name = d.Name,
                TotalCalories = cal,
                TotalProtein = prot,
                TotalFat = fat,
                TotalCarbs = carbs
            };
        }

        private static void FillMealNoDupes(
            ObservableCollection<RationDishViewModel> collection,
            List<RationDishViewModel> pool,
            decimal targetCalories,
            string mealType,
            HashSet<Guid> usedIds)
        {
            decimal accumulated = 0;
            decimal tolerance = targetCalories * 0.10m;

            foreach (var dish in pool)
            {
                if (usedIds.Contains(dish.DishId)) continue;
                if (accumulated >= targetCalories + tolerance) break;

                decimal remaining = targetCalories - accumulated;
                if (dish.TotalCalories > remaining + tolerance && accumulated > 0)
                    continue;

                collection.Add(new RationDishViewModel
                {
                    DishId = dish.DishId,
                    Name = dish.Name,
                    TotalCalories = dish.TotalCalories,
                    TotalProtein = dish.TotalProtein,
                    TotalFat = dish.TotalFat,
                    TotalCarbs = dish.TotalCarbs,
                    MealType = mealType
                });

                usedIds.Add(dish.DishId);
                accumulated += dish.TotalCalories;
            }

            if (collection.Count == 0)
            {
                var fallback = pool
                    .Where(d => !usedIds.Contains(d.DishId))
                    .OrderBy(d => Math.Abs(d.TotalCalories - targetCalories))
                    .FirstOrDefault();

                if (fallback != null)
                {
                    collection.Add(new RationDishViewModel
                    {
                        DishId = fallback.DishId,
                        Name = fallback.Name,
                        TotalCalories = fallback.TotalCalories,
                        TotalProtein = fallback.TotalProtein,
                        TotalFat = fallback.TotalFat,
                        TotalCarbs = fallback.TotalCarbs,
                        MealType = mealType
                    });
                    usedIds.Add(fallback.DishId);
                }
            }
        }
        private void RecalculateRationTotals()
        {
            var all = BreakfastDishes.Concat(LunchDishes).Concat(DinnerDishes);
            RationTotalCalories = all.Sum(d => d.TotalCalories);
            RationTotalProtein = all.Sum(d => d.TotalProtein);
            RationTotalFat = all.Sum(d => d.TotalFat);
            RationTotalCarbs = all.Sum(d => d.TotalCarbs);

            OnPropertyChanged(nameof(RationTotalCalories));
            OnPropertyChanged(nameof(RationTotalProtein));
            OnPropertyChanged(nameof(RationTotalFat));
            OnPropertyChanged(nameof(RationTotalCarbs));
        }

        // ======= Ration: save to DB =======

        private async Task SaveRationAsync()
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Save your profile first.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var all = BreakfastDishes.Concat(LunchDishes).Concat(DinnerDishes).ToList();
            if (all.Count == 0)
            {
                MessageBox.Show("Generate a ration first.", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var entries = all
                .Select(d => (d.DishId, d.MealType))
                .ToList();

            await _dailyRationsRepository.SaveRationAsync(
                _currentUser.Id,
                DateOnly.FromDateTime(DateTime.Today),
                entries);

            await LoadSavedRationDatesAsync();
            MessageBox.Show("Ration saved!", "Success",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ======= Load lists =======

        private async Task LoadSavedUsersAsync()
        {
            SavedUsers.Clear();
            var users = await _usersRepository.GetAllAsync();
            foreach (var u in users)
                SavedUsers.Add(u);
        }

        private async Task LoadSavedRationDatesAsync()
        {
            SavedRationDates.Clear();
            var entries = await _dailyRationsRepository.GetAllGroupedAsync();
            foreach (var e in entries)
                SavedRationDates.Add(e);
        }

        // ======= Load selected profile =======

        private async Task LoadSelectedUserAsync()
        {
            if (SelectedUser == null) return;

            _currentUser = SelectedUser;

            UserName = SelectedUser.Name;
            UserAge = SelectedUser.Age.ToString();
            UserWeight = SelectedUser.WeightKg.ToString();
            UserHeight = SelectedUser.HeightCm.ToString();
            UserGender = SelectedUser.Gender;
            UserActivityLevel = SelectedUser.ActivityLevel;
            UserGoal = SelectedUser.Goal;

            OnPropertyChanged(nameof(UserName));
            OnPropertyChanged(nameof(UserAge));
            OnPropertyChanged(nameof(UserWeight));
            OnPropertyChanged(nameof(UserHeight));
            OnPropertyChanged(nameof(UserGender));
            OnPropertyChanged(nameof(UserActivityLevel));
            OnPropertyChanged(nameof(UserGoal));

            RecalculateNorms(
                SelectedUser.WeightKg,
                SelectedUser.HeightCm,
                SelectedUser.Age,
                SelectedUser.Gender,
                SelectedUser.ActivityLevel,
                SelectedUser.Goal);
        }

        // ======= Delete selected profile =======

        private async Task DeleteSelectedUserAsync()
        {
            if (SelectedUser == null) return;

            var result = MessageBox.Show(
                $"Delete profile \"{SelectedUser.Name}\"?",
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            await _usersRepository.Delete(SelectedUser.Id);

            if (_currentUser?.Id == SelectedUser.Id)
                _currentUser = null;

            await LoadSavedUsersAsync();
            await LoadSavedRationDatesAsync();
        }

        // ======= Load selected ration =======

        private async Task LoadSelectedRationAsync()
        {
            if (SelectedRationDate == null) return;

            var parts = SelectedRationDate.Split(" | ");
            var date = DateOnly.Parse(parts[0]);
            var userName = parts.Length > 1 ? parts[1] : string.Empty;

            var user = SavedUsers.FirstOrDefault(u => u.Name == userName);
            if (user == null) return;

            // GetByUserAndDateAsync тепер повертає List<DailyRationEntity>
            var rations = await _dailyRationsRepository.GetByUserAndDateAsync(user.Id, date);

            BreakfastDishes.Clear();
            LunchDishes.Clear();
            DinnerDishes.Clear();

            foreach (var ration in rations)
            {
                foreach (var rd in ration.RationDishes)
                {
                    if (rd.Dish == null) continue;

                    var vm = ToRationVm(rd.Dish);
                    vm.MealType = rd.MealType;

                    switch (rd.MealType)
                    {
                        case "Breakfast": BreakfastDishes.Add(vm); break;
                        case "Lunch": LunchDishes.Add(vm); break;
                        case "Dinner": DinnerDishes.Add(vm); break;
                    }
                }
            }

            RecalculateRationTotals();
        }

        // ======= Delete selected ration =======

        private async Task DeleteSelectedRationAsync()
        {
            if (SelectedRationDate == null) return;

            var result = MessageBox.Show(
                $"Delete ration \"{SelectedRationDate}\"?",
                "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            var parts = SelectedRationDate.Split(" | ");
            var date = DateOnly.Parse(parts[0]);
            var userName = parts.Length > 1 ? parts[1] : string.Empty;
            var user = SavedUsers.FirstOrDefault(u => u.Name == userName);

            if (user == null) return;

            await _dailyRationsRepository.DeleteByUserAndDateAsync(user.Id, date);

            BreakfastDishes.Clear();
            LunchDishes.Clear();
            DinnerDishes.Clear();
            RecalculateRationTotals();

            await LoadSavedRationDatesAsync();
        }
        // ======= INotifyPropertyChanged =======

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
