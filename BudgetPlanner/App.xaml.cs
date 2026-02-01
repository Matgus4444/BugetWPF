using System.Configuration;
using System.Data;
using System.Globalization;
using System.Windows;

namespace BudgetPlanner
{

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
   
            var culture = new CultureInfo("sv-SE");

    
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    System.Windows.Markup.XmlLanguage.GetLanguage(culture.IetfLanguageTag))
            );

            base.OnStartup(e);
        }
    }

}
