using BudgetPlanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace BudgetPlanner.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        private readonly BudgetDbContext _context;
        private BudgetItem _selectedItem;
        private IncomeData _incomeData;

        #endregion

        #region Collections

        public ObservableCollection<BudgetItem> Items { get; }
        public ObservableCollection<MonthItem> MonthItems { get; }
        public ObservableCollection<ItemType> ItemTypes { get; }
        public ObservableCollection<RecurrenceType> RecurrenceTypes { get; }
        public List<int> Months { get; }

        public ObservableCollection<ExpenseCategory> ExpenseCategories { get; } =
            new ObservableCollection<ExpenseCategory>
            {
                ExpenseCategory.Mat,
                ExpenseCategory.Hus,
                ExpenseCategory.Transport,
                ExpenseCategory.Fritid,
                ExpenseCategory.Barn,
                ExpenseCategory.Streaming,
                ExpenseCategory.SaaS,
                ExpenseCategory.Övrigt
            };

        public ObservableCollection<IncomeCategory> IncomeCategories { get; } =
            new ObservableCollection<IncomeCategory>
            {
                IncomeCategory.Lön,
                IncomeCategory.Bidrag,
                IncomeCategory.Hobby,
                IncomeCategory.Gåva
            };

        #endregion

        #region Commands

        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand YearlyIncomeLostFocusCommand { get; }

        #endregion

        #region Constructor

        public MainViewModel()
        {
            _context = new BudgetDbContext();

            // Commands
            AddItemCommand = new RelayCommand(AddItem);
            RemoveItemCommand = new RelayCommand(RemoveItem);
            YearlyIncomeLostFocusCommand = new RelayCommand(UpdateMonthlyIncome);

            // Month lookup
            MonthItems = new ObservableCollection<MonthItem>(
                Enumerable.Range(1, 12)
                    .Select(i => new MonthItem
                    {
                        Number = i,
                        Name = new DateTime(1, i, 1).ToString("MMMM", new CultureInfo("sv-SE"))
                    })
            );

            // Enum collections
            ItemTypes = new ObservableCollection<ItemType>((ItemType[])Enum.GetValues(typeof(ItemType)));
            RecurrenceTypes = new ObservableCollection<RecurrenceType>((RecurrenceType[])Enum.GetValues(typeof(RecurrenceType)));
            Months = Enumerable.Range(1, 12).ToList();

            // Load data from DB
            _incomeData = _context.IncomeDatas.FirstOrDefault() ?? new IncomeData { Id = 1, YearlyIncome = 0 };
            if (_incomeData.Id == 0)
            {
                _context.IncomeDatas.Add(_incomeData);
                _context.SaveChanges();
            }

            Items = new ObservableCollection<BudgetItem>(_context.BudgetItems.ToList());
            Items.CollectionChanged += Items_CollectionChanged;

            foreach (var item in Items)
                item.PropertyChanged += Item_PropertyChanged;
        }

        #endregion

        #region Properties

        public BudgetItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    if (_selectedItem != null)
                        _selectedItem.PropertyChanged -= Item_PropertyChanged;

                    _selectedItem = value;

                    if (_selectedItem != null)
                        _selectedItem.PropertyChanged += Item_PropertyChanged;

                    OnPropertyChanged();
                }
            }
        }

        public decimal YearlyIncome
        {
            get => _incomeData.YearlyIncome;
            set
            {
                if (_incomeData.YearlyIncome != value)
                {
                    _incomeData.YearlyIncome = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(MonthlyIncome));
                    OnPropertyChanged(nameof(CurrentMonthIncome));
                    OnPropertyChanged(nameof(CurrentMonthTotal));
                    _context.SaveChanges();
                }
            }
        }

        public decimal MonthlyIncome => YearlyIncome / 12;

        public decimal CurrentMonthIncome => CalculateIncomeForMonth(DateTime.Now.Month);
        public decimal NextMonthIncome => CalculateIncomeForMonth(NextMonth);

        public decimal CurrentMonthExpenses => CalculateExpensesForMonth(DateTime.Now.Month);
        public decimal NextMonthExpenses => CalculateExpensesForMonth(NextMonth);

        public decimal CurrentMonthTotal => CurrentMonthIncome - CurrentMonthExpenses;
        public decimal NextMonthTotal => NextMonthIncome - NextMonthExpenses;

        private int NextMonth => DateTime.Now.Month == 12 ? 1 : DateTime.Now.Month + 1;

        #endregion

        #region Private Methods

        private decimal CalculateIncomeForMonth(int month)
        {
            return Items
                .Where(i => i.Type == ItemType.Income)
                .Sum(i =>
                {
                    return i.Recurrence switch
                    {
                        RecurrenceType.Monthly => i.Amount,
                        RecurrenceType.Yearly => i.YearlyMonth == month ? i.Amount : 0,
                        RecurrenceType.OneTime => i.Recurrence == RecurrenceType.OneTime && month == DateTime.Now.Month ? i.Amount : 0,
                        _ => 0
                    };
                });
        }

        private decimal CalculateExpensesForMonth(int month)
        {
            return Items
                .Where(i => i.Type == ItemType.Expense)
                .Sum(i =>
                {
                    return i.Recurrence switch
                    {
                        RecurrenceType.Monthly => i.Amount,
                        RecurrenceType.Yearly => i.YearlyMonth == month ? i.Amount : 0,
                        RecurrenceType.OneTime => i.Recurrence == RecurrenceType.OneTime && month == DateTime.Now.Month ? i.Amount : 0,
                        _ => 0
                    };
                });
        }

        private void UpdateMonthlyIncome()
        {
            var monthlyIncomeItem = Items.FirstOrDefault(i =>
                i.Name == "Lön" &&
                i.Type == ItemType.Income &&
                i.Recurrence == RecurrenceType.Monthly);

            decimal monthlyAmount = YearlyIncome / 12;

            if (monthlyIncomeItem != null)
            {
                monthlyIncomeItem.Amount = monthlyAmount; // Update existing
            }
            else
            {
                // Add new monthly income item
                var newItem = new BudgetItem
                {
                    Name = "Lön",
                    Amount = monthlyAmount,
                    Type = ItemType.Income,
                    Recurrence = RecurrenceType.Monthly,
                    Category = IncomeCategory.Lön.ToString()
                };
                newItem.PropertyChanged += Item_PropertyChanged;
                Items.Add(newItem);
            }

            Refresh();
        }

        private void AddItem()
        {
            var newItem = new BudgetItem
            {
                Name = "Ny post",
                Amount = 0,
                Category = ExpenseCategory.Övrigt.ToString(),
                Type = ItemType.Expense,
                Recurrence = RecurrenceType.OneTime
            };
            newItem.PropertyChanged += Item_PropertyChanged;
            Items.Add(newItem);
            Refresh();
        }

        private void RemoveItem()
        {
            if (SelectedItem == null) return;

            _context.BudgetItems.Remove(SelectedItem);
            Items.Remove(SelectedItem);
            _context.SaveChanges();
            Refresh();
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (BudgetItem item in e.NewItems)
                {
                    _context.BudgetItems.Add(item);
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (BudgetItem item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;
            }

            Refresh();
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _context.SaveChanges();
            Refresh();
        }

        private void Refresh()
        {
            OnPropertyChanged(nameof(CurrentMonthIncome));
            OnPropertyChanged(nameof(CurrentMonthExpenses));
            OnPropertyChanged(nameof(CurrentMonthTotal));
            OnPropertyChanged(nameof(NextMonthIncome));
            OnPropertyChanged(nameof(NextMonthExpenses));
            OnPropertyChanged(nameof(NextMonthTotal));
        }

        #endregion
    }
}
