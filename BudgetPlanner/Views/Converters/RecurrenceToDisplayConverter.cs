using System;
using System.Globalization;
using System.Windows.Data;
using BudgetPlanner.Models;

namespace BudgetPlanner.Views.Converters
{
    public class RecurrenceToDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RecurrenceType recurrence)
            {
                return recurrence switch
                {
                    RecurrenceType.OneTime => "Engång",
                    RecurrenceType.Monthly => "Månatlig",
                    RecurrenceType.Yearly => "Årlig",
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
                    "Engång" => RecurrenceType.OneTime,
                    "Månatlig" => RecurrenceType.Monthly,
                    "Årlig" => RecurrenceType.Yearly,
                    _ => Binding.DoNothing
                };
            }
            return Binding.DoNothing;
        }
    }
}
