using BudgetPlanner.Models;
using System.Globalization;
using System.Windows.Data;

namespace BudgetPlanner.Views.Converters
{
    public class CategoryListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ItemType type)
            {
                return type switch
                {
                    ItemType.Income => Enum.GetNames(typeof(IncomeCategory)),
                    ItemType.Expense => Enum.GetNames(typeof(ExpenseCategory)),
                    _ => null
                };
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }

}