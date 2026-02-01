using System.ComponentModel;
using System.Runtime.CompilerServices;

public enum ItemType
{
    Income,
    Expense
}

public enum RecurrenceType
{
    OneTime,
    Monthly,
    Yearly
}

public class BudgetItem : INotifyPropertyChanged
{
    public int Id { get; set; }

    private string _name;
    private decimal _amount;
    private string _category;
    private ItemType _type;
    private RecurrenceType _recurrence;
    private int? _yearlyMonth;

    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    public decimal Amount
    {
        get => _amount;
        set { _amount = value; OnPropertyChanged(); }
    }

    public string  Category
    {
        get => _category;
        set { _category = value; OnPropertyChanged(); }
    }

    public ItemType Type
    {
        get => _type;
        set { _type = value; OnPropertyChanged(); }
    }

    public RecurrenceType Recurrence
    {
        get => _recurrence;
        set { _recurrence = value; OnPropertyChanged(); }
    }

    public int? YearlyMonth
    {
        get => _yearlyMonth;
        set
        {
            if (_yearlyMonth != value)
            {
                _yearlyMonth = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}

