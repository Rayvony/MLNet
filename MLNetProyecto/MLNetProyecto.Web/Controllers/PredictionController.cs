using Microsoft.AspNetCore.Mvc;
using MLNetProyecto.Entidades.VM;
using MLNetProyecto.Logica;
using MLNetProyecto.Web.Services;
using X.PagedList.Extensions;

namespace MLNetProyecto.Web.Controllers
{
    public class PredictionController : Controller
    {
        private readonly IMLNetLogica _mlNetLogica;
        private readonly ApiService _apiService;
        private readonly ILogger<HomeController> _logger;
        private readonly LocalizationService _localizer;


        public PredictionController(ILogger<HomeController> logger, IMLNetLogica mlNetLogica, ApiService apiService, LocalizationService localizationService)
        {
            _logger = logger;
            _mlNetLogica = mlNetLogica;
            _apiService = apiService;
            _localizer = localizationService;
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
        public async Task<ActionResult> AgregarElemento(string FileName, string ImagePath, float Score, string PredictedLabelValue)
        {
            if (ModelState.IsValid)
            {


                await _mlNetLogica.AgregarElementoAsync(FileName, ImagePath, Score, PredictedLabelValue);

                return RedirectToAction("TratamientoReciclaje", new { predictedLabelValue = PredictedLabelValue });
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
            if (ModelState.IsValid)
            {
                await _mlNetLogica.CorregirElementoAsync(FileName, ImagePath, label);
                return RedirectToAction("TratamientoReciclaje", new { predictedLabelValue = label });
            }
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

        public ActionResult TratamientoReciclaje(string predictedLabelValue)
        {
            TratamientoVM texto = new TratamientoVM()
            {
                RecicladoTitulo = _localizer.GetString("RecicladoTitulo"),
                CreatividadTitulo = _localizer.GetString("CreatividadTitulo"),
                CuriosidadesTitulo = _localizer.GetString("CuriosidadesTitulo"),
            };

            switch (predictedLabelValue)
            {
                case "plastico":
                    texto.Titulo = _localizer.GetString("TituloPlastico");
                    texto.IntroduccionTitulo = _localizer.GetString("IntroPlasticoTitulo");
                    texto.IntroduccionTexto = _localizer.GetString("IntroPlastico");
                    texto.RecicladoTexto = _localizer.GetString("RecicladoPlastico");
                    texto.CreatividadTexto = _localizer.GetString("CreatividadPlastico");
                    texto.CuriosidadesTexto = _localizer.GetString("CuriosidadesPlastico");
                    break;
                case "papel":
                    texto.Titulo = _localizer.GetString("TituloPapel");
                    texto.IntroduccionTitulo = _localizer.GetString("IntroPapelTitulo");
                    texto.IntroduccionTexto = _localizer.GetString("IntroPapel");
                    texto.RecicladoTexto = _localizer.GetString("RecicladoPapel");
                    texto.CreatividadTexto = _localizer.GetString("CreatividadPapel");
                    texto.CuriosidadesTexto = _localizer.GetString("CuriosidadesPapel");
                    break;
                case "vidrio":
                    texto.Titulo = _localizer.GetString("TituloVidrio");
                    texto.IntroduccionTitulo = _localizer.GetString("IntroVidrioTitulo");
                    texto.IntroduccionTexto = _localizer.GetString("IntroVidrio");
                    texto.RecicladoTexto = _localizer.GetString("RecicladoVidrio");
                    texto.CreatividadTexto = _localizer.GetString("CreatividadVidrio");
                    texto.CuriosidadesTexto = _localizer.GetString("CuriosidadesVidrio");
                    break;
                case "metal":
                    texto.Titulo = _localizer.GetString("TituloMetal");
                    texto.IntroduccionTitulo = _localizer.GetString("IntroMetalTitulo");
                    texto.IntroduccionTexto = _localizer.GetString("IntroMetal");
                    texto.RecicladoTexto = _localizer.GetString("RecicladoMetal");
                    texto.CreatividadTexto = _localizer.GetString("CreatividadMetal");
                    texto.CuriosidadesTexto = _localizer.GetString("CuriosidadesMetal");
                    break;
                case "organico":
                    texto.Titulo = _localizer.GetString("TituloOrganico");
                    texto.IntroduccionTitulo = _localizer.GetString("IntroOrganicoTitulo");
                    texto.IntroduccionTexto = _localizer.GetString("IntroOrganico");
                    texto.RecicladoTexto = _localizer.GetString("RecicladoOrganico");
                    texto.CreatividadTexto = _localizer.GetString("CreatividadOrganico");
                    texto.CuriosidadesTexto = _localizer.GetString("CuriosidadesOrganico");
                    break;
                default:
                    texto = null;
                    break;
            }

            return View(texto);
        }

        public IActionResult VerPdf()
        {
            var fileUrl = Url.Content("~/files/guia.pdf");
            ViewBag.FileUrl = fileUrl;
            return View("GuiaDeReciclaje");
        }
    }
}
