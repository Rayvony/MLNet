using Microsoft.AspNetCore.Mvc;
using MLNetMVC.Data.EF;
using MLNetMVC.Logica;
using static System.Formats.Asn1.AsnWriter;

namespace MLNetMVC.Web.Controllers
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
        public ActionResult AgregarElemento(string ImagePath, float Score, string PredictedLabelValue)
        {
            if (ModelState.IsValid) {
                

                _mlNetLogica.AgregarElemento(ImagePath,Score,PredictedLabelValue);

                return RedirectToAction("AnalizarImagen");
            }
            return RedirectToAction("AnalizarImagen");
        }

        [HttpPost]
        public ActionResult CorregirElementoView(string ImagePath)
        {
            ViewBag.ImagePath = ImagePath;

            return View();
        }

        [HttpPost]
        public ActionResult CorregirElemento(string ImagePath,string label)
        {
            _mlNetLogica.CorregirElemento(ImagePath, label);

            return RedirectToAction("AnalizarImagen");
        }
    }
}
