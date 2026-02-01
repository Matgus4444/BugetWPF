using System;
using System.Globalization;
using System.Windows.Data;
using BudgetPlanner.Models;

namespace BudgetPlanner.Views.Converters
{
    public class ItemTypeToDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ItemType type)
            {
                return type switch
                {
                    ItemType.Income => "Inkomst",
                    ItemType.Expense => "Utgift",
                    _ => value.ToString()
                };
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return str switch
                {
                    "Inkomst" => ItemType.Income,
                    "Utgift" => ItemType.Expense,
                    _ => Binding.DoNothing
                };
            }
            return Binding.DoNothing;
        }
    }
}
