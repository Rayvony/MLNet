using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MLNetProyecto.Web.Areas.Identity.Data;
using MLNetProyecto.Web.Services;
using MLNetProyecto.Web.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MLNetProyecto.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ApiService _apiService;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, ApiService apiService)
        {
            _logger = logger;
            _userManager = userManager;
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            string userId = _userManager.GetUserId(this.User);
            ViewData["UserId"] = userId;

            try
            {
                var efficiencyData = await _apiService.GetAsync<EfficiencyResponse>("page_1_current_year/search_efficiency");

                ViewData["EfficiencyData"] = efficiencyData.Efficiency;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Error al llamar a la API: {ex.Message}");
                ViewData["EfficiencyData"] = "Error al obtener datos de eficiencia.";
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


}
