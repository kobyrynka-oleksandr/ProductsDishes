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
    public class EditDishIngredientsViewModel : INotifyPropertyChanged
    {
        private readonly DishEntity _dish;
        private readonly ProductsRepository _productsRepository;
        private readonly DishIngradientsRepository _dishIngredientsRepository;

        public string DishName => _dish.Name;

        public ObservableCollection<DishIngradientEntity> Ingredients { get; } = new();
        public ObservableCollection<ProductEntity> Products { get; } = new();

        public DishIngradientEntity? SelectedIngredient { get; set; }
        public ProductEntity? SelectedProduct { get; set; }
        public string EditQuantity { get; set; } = string.Empty;
        public string ProductSearchText { get; set; } = string.Empty;
        private List<ProductEntity> _allProducts = new();

        private readonly bool _isNewDish;

        public ICommand SearchProductsCommand { get; }
        public ICommand ClearProductsSearchCommand { get; }

        public ICommand AddIngredientCommand { get; }
        public ICommand UpdateIngredientCommand { get; }
        public ICommand DeleteIngredientCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public List<DishIngradientEntity> IngredientsToSave { get; private set; } = new();
        public bool HasIngredients => IngredientsToSave.Count > 0 || Ingredients.Count > 0;

        public EditDishIngredientsViewModel(
                DishEntity dish,
                ProductsRepository productsRepository,
                DishIngradientsRepository dishIngredientsRepository,
                bool isNewDish = false)
        {
            _dish = dish;
            _productsRepository = productsRepository;
            _dishIngredientsRepository = dishIngredientsRepository;

            _dish = dish;
            _isNewDish = isNewDish;

            AddIngredientCommand = new RelayCommand(async _ => await AddAsync());
            UpdateIngredientCommand = new RelayCommand(async _ => await UpdateAsync(), _ => SelectedIngredient != null);
            DeleteIngredientCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedIngredient != null);

            SearchProductsCommand = new RelayCommand(_ => ApplyProductFilter());
            ClearProductsSearchCommand = new RelayCommand(_ => ClearProductFilter());

            _ = LoadAsync();
        }


        private async Task LoadAsync()
        {
            _allProducts = await _productsRepository.Get();
            Products.Clear();
            foreach (var p in _allProducts)
                Products.Add(p);

            if (!_isNewDish)
            {
                Ingredients.Clear();
                foreach (var di in await _dishIngredientsRepository.GetByDishAsync(_dish.Id))
                    Ingredients.Add(di);
            }
        }


        private bool TryGetQuantity(out decimal quantity)
        {
            quantity = 0;

            if (!decimal.TryParse(EditQuantity, out quantity))
                return false;

            return true;
        }

        private async Task AddAsync()
        {
            if (SelectedProduct == null) return;

            if (!TryGetQuantity(out var q))
            {
                MessageBox.Show("Invalid quantity format.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_isNewDish)
            {
                var existing = IngredientsToSave
                    .FirstOrDefault(i => i.ProductId == SelectedProduct.Id);

                if (existing != null)
                    existing.QuantityGrams = q;
                else
                    IngredientsToSave.Add(new DishIngradientEntity
                    {
                        DishId = _dish.Id,
                        ProductId = SelectedProduct.Id,
                        QuantityGrams = q,
                        Product = SelectedProduct  
                    });

                Ingredients.Clear();
                foreach (var i in IngredientsToSave)
                    Ingredients.Add(i);
            }
            else
            {
                await _dishIngredientsRepository.AddAsync(_dish.Id, SelectedProduct.Id, q);
                await LoadAsync();
            }
        }

        private async Task UpdateAsync()
        {
            if (SelectedIngredient == null) return;

            if (!TryGetQuantity(out var q))
            {
                MessageBox.Show("Invalid quantity format.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await _dishIngredientsRepository.UpdateAsync(
                SelectedIngredient.DishId,
                SelectedIngredient.ProductId,
                q);

            await LoadAsync();
        }
        private async Task DeleteAsync()
        {
            if (SelectedIngredient == null) return;

            if (_isNewDish)
            {
                IngredientsToSave.RemoveAll(i => i.ProductId == SelectedIngredient.ProductId);
                Ingredients.Clear();
                foreach (var i in IngredientsToSave)
                    Ingredients.Add(i);
            }
            else
            {
                await _dishIngredientsRepository.DeleteAsync(
                    SelectedIngredient.DishId,
                    SelectedIngredient.ProductId);
                await LoadAsync();
            }
        }
        private void ApplyProductFilter()
        {
            Products.Clear();

            IEnumerable<ProductEntity> filtered = _allProducts;

            if (!string.IsNullOrWhiteSpace(ProductSearchText))
                filtered = filtered.Where(p => p.Name.Contains(ProductSearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var p in filtered)
                Products.Add(p);
        }

        private void ClearProductFilter()
        {
            ProductSearchText = string.Empty;
            OnPropertyChanged(nameof(ProductSearchText));

            Products.Clear();
            foreach (var p in _allProducts)
                Products.Add(p);
        }
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
