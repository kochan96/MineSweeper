using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MineSweeper
{
    public enum Language
    {
        English,
        Polish
    }

    

    public class Localizer
    {
        public  Properties.Resources GetResourceInstance()
        {
            return new Properties.Resources();
        }

        readonly static string ResourceKey = "CultureResources";
        public static void SetLanguage(Language lang)
        {
            switch (lang)
            {
                case MineSweeper.Language.English:
                    {
                        System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
                        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                        break;
                    }
                case MineSweeper.Language.Polish:
                    {
                        System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("pl-PL");
                        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("pl-PL");
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }
            ObjectDataProvider provider = Application.Current.TryFindResource(ResourceKey) as ObjectDataProvider;
            if (provider == null)
                MessageBox.Show(Properties.Resources.ProviderNullError);

            provider?.Refresh();//refresh text
        }
    }
}
