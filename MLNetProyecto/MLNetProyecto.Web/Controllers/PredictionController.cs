using Microsoft.AspNetCore.Mvc;
using MLNetProyecto.Logica;
using X.PagedList.Extensions;


namespace MLNetProyecto.Web.Controllers
{
    public class PredictionController : Controller
    {
        private readonly IMLNetLogica _mlNetLogica;

        public PredictionController(IMLNetLogica mlNetLogica)
        {
            _mlNetLogica = mlNetLogica;
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
