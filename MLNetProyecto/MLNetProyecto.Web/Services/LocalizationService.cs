using System.Globalization;
using System.Resources;

namespace MLNetProyecto.Web.Services
{
    public class LocalizationService
    {
        private readonly ResourceManager _resourceManager;

        public LocalizationService()
        {
            _resourceManager = new ResourceManager("MLNetProyecto.Web.Resources.Resources", typeof(LocalizationService).Assembly);
        }

        public string GetString(string name, CultureInfo culture = null)
        {
            return _resourceManager.GetString(name, culture ?? CultureInfo.CurrentUICulture);
        }
    }
}
