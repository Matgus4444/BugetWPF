using System.Windows.Controls;
using BudgetPlanner.ViewModels;

namespace BudgetPlanner.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
                InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
