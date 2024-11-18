using Microsoft.AspNetCore.Mvc;
using MLNetProyecto.Entidades;
using MLNetProyecto.Logica;
using X.PagedList.Extensions;
using MLNetProyecto.Web.Models;
using MLNetProyecto.Web.Services;


namespace MLNetProyecto.Web.Controllers
{
    public class PredictionController : Controller
    {
        private readonly IMLNetLogica _mlNetLogica;
        private readonly ApiService _apiService;
        private readonly ILogger<HomeController> _logger;


        public PredictionController(ILogger<HomeController> logger,IMLNetLogica mlNetLogica, ApiService apiService)
        {
            _logger = logger;
            _mlNetLogica = mlNetLogica;
            _apiService = apiService;
        }

        public async Task<ActionResult> AnalizarImagen()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AnalizarImagen(IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                var modelo = _mlNetLogica.GenerateModel();
                var imagenAnalizada = await _mlNetLogica.ClassifySingleImage(imageFile, modelo);

                try
                {
                    var rekognitionResults = await _apiService.PostAsync("detect-labels", imageFile);

                    ViewBag.RekognitionLabels = rekognitionResults;
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError($"Error al llamar a la API: {ex.Message}");
                    ViewBag.RekognitionLabels = "Error al obtener clasificaciones.";
                }

                return View(imagenAnalizada);
            }

            return View(imageFile);
        }

        [HttpPost]
        public async Task<ActionResult> AgregarElemento(string  FileName, string ImagePath, float Score, string PredictedLabelValue)
        {
            if (ModelState.IsValid)
            {


                await _mlNetLogica.AgregarElementoAsync(FileName, ImagePath, Score, PredictedLabelValue);

                return RedirectToAction("AnalizarImagen");
            }
            return RedirectToAction("AnalizarImagen");
        }

        [HttpPost]
        public ActionResult CorregirElementoView(string FileName, string ImagePath)
        {
            ViewBag.ImagePath = ImagePath;
            ViewBag.FileName = FileName;

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CorregirElemento(string FileName, string ImagePath, string label)
        {
            await _mlNetLogica.CorregirElementoAsync(FileName,ImagePath, label);

            return RedirectToAction("AnalizarImagen");
        }

        public async Task<ActionResult> MostrarResultados(int? page)
        {
            var predicciones = await _mlNetLogica.MostrarResultados();
            int pageSize = 15;
            int pageNumber = (page ?? 1);

            var pagedPredicciones = predicciones.ToPagedList(pageNumber, pageSize);

            return View(pagedPredicciones);
        }
    }
}
