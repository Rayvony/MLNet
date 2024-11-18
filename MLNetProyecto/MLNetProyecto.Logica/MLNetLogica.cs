using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using MLNetProyecto.Entidades.EF;
using MLNetProyecto.Entidades.VM;

namespace MLNetProyecto.Logica
{
    public interface IMLNetLogica
    {
        ITransformer GenerateModel();
        Task<ImagePredictionVM> ClassifySingleImage(IFormFile imageFile, ITransformer model);
        void DisplayResults(IEnumerable<ImagePredictionVM> imagePredictionData);
        Task AgregarElementoAsync(string FileName, string ImagePath, float Score, string PredictedLabelValue);
        Task CorregirElementoAsync(string FileName, string ImagePath, string label);
        Task<List<ImagePrediction>> MostrarResultados();
    }

    public class MLNetLogica : IMLNetLogica
    {
        private MlnetProyectoContext _context;
        private MLContext _mlContext;


        public MLNetLogica(MlnetProyectoContext context, MLContext mlContext)
        {
            _context = context;
            _mlContext = mlContext;
        }

        static string _assetsPath = Path.Combine(Environment.CurrentDirectory, "assets");
        static string _imagesFolder = Path.Combine(_assetsPath, "images");
        static string _trainTagsTsv = Path.Combine(_imagesFolder, "tags.tsv");
        static string _testTagsTsv = Path.Combine(_imagesFolder, "test-tags.tsv");
        static string _predictSingleImage = Path.Combine(_imagesFolder, "toaster3.jpg");
        static string _inceptionTensorFlowModel = Path.Combine(_assetsPath, "inception", "tensorflow_inception_graph.pb");

        //creo el contexto para trabajar con ML
        public ITransformer GenerateModel()
        {

            IEstimator<ITransformer> pipeline = _mlContext.Transforms.LoadImages(outputColumnName: "input", imageFolder: _imagesFolder, inputColumnName: nameof(ImageDatumVM.ImagePath))
                        // The image transforms transform the images into the model's expected format.
                        .Append(_mlContext.Transforms.ResizeImages(outputColumnName: "input", imageWidth: InceptionSettings.ImageWidth, imageHeight: InceptionSettings.ImageHeight, inputColumnName: "input"))
                        .Append(_mlContext.Transforms.ExtractPixels(outputColumnName: "input", interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean))
                        .Append(_mlContext.Model.LoadTensorFlowModel(_inceptionTensorFlowModel).
            ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true))
                        .Append(_mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: "Label"))
                        .Append(_mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: "softmax2_pre_activation"))
                        .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
        .AppendCacheCheckpoint(_mlContext);

            //esto es codigo original
            IDataView trainingData = _mlContext.Data.LoadFromTextFile<ImageDatumVM>(path: _trainTagsTsv, hasHeader: false);

            ITransformer model = pipeline.Fit(trainingData);

            IDataView testData = _mlContext.Data.LoadFromTextFile<ImageDatumVM>(path: _testTagsTsv, hasHeader: false);
            IDataView predictions = model.Transform(testData);

            return model;
        }

        public void DisplayResults(IEnumerable<ImagePredictionVM> imagePredictionData)
        {
            foreach (ImagePredictionVM prediction in imagePredictionData)
            {
                Console.WriteLine($"Image: {Path.GetFileName(prediction.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score?.Max()} ");
            }
        }

        public async Task<ImagePredictionVM> ClassifySingleImage(IFormFile imageFile, ITransformer model)
        {
            var filePath = Path.Combine(_imagesFolder, imageFile.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            var imageData = new ImageDatumVM()
            {
                ImagePath = filePath,
            };

            string trainFile = _trainTagsTsv;

            string nuevaLinea = imageFile.FileName;

            using (StreamWriter sw = new StreamWriter(trainFile, true))
            {
                sw.WriteLine();
                sw.Write(nuevaLinea);
            }

            var predictor = _mlContext.Model.CreatePredictionEngine<ImageDatumVM, ImagePredictionVM>(model);
            var prediction = predictor.Predict(imageData);

            byte[] imageBytes = File.ReadAllBytes(imageData.ImagePath);
            prediction.ImagePath = Convert.ToBase64String(imageBytes);
            prediction.FileName = imageFile.FileName;

            var imagen = new ImageDatum
            {
                ImagePath = prediction.ImagePath,
                Label = string.Join(", ", prediction.PredictedLabelValue),
                FileName = prediction.FileName
            };

            var prediccion = new ImagePrediction
            {
                ImageData = imagen,
                Score = prediction.Score?.Max().ToString(),
                PredictedLabelValue = string.Join(", ", prediction.PredictedLabelValue)
            };

            await _context.ImageData.AddAsync(imagen);
            await _context.ImagePredictions.AddAsync(prediccion);
            await _context.SaveChangesAsync();

            return prediction;
        }

        public async Task AgregarElementoAsync(string FileName, string ImagePath, float Score, string PredictedLabelValue)
        {
            var imagen = await _context.ImageData.FirstOrDefaultAsync(i => i.FileName == FileName);

            if (imagen == null)
                throw new Exception("la imagen no existe");

            var prediccion = await _context.ImagePredictions.FirstOrDefaultAsync(p => p.ImageDataId == imagen.Id);

            if (prediccion == null)
                throw new Exception("la prediccion no existe");

            prediccion.IsCorrect = true;
            _context.ImagePredictions.Update(prediccion);
            _context.SaveChanges();

            string trainFile = _trainTagsTsv;

            string nuevaLinea = "\t" + PredictedLabelValue;

            using (StreamWriter sw = new StreamWriter(trainFile, true))
            {
                sw.Write(nuevaLinea);
            }
        }

        public async Task CorregirElementoAsync(string FileName, string ImagePath, string label)
        {
            var imagen = await _context.ImageData.AsNoTracking().FirstOrDefaultAsync(i => i.FileName == FileName);

            if (imagen == null)
                throw new Exception("la imagen no existe");

            var prediccion = await _context.ImagePredictions.FirstOrDefaultAsync(p => p.ImageDataId == imagen.Id);

            if (prediccion == null)
                throw new Exception("la prediccion no existe");

            imagen.Label = label;
            _context.ImageData.Update(imagen);

            prediccion.IsCorrect = false;
            _context.ImagePredictions.Update(prediccion);

            _context.SaveChanges();

            string filePath = _trainTagsTsv;

            string nuevaLinea = "\t" + label;

            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.Write(nuevaLinea);
            }
        }

        public async Task<List<ImagePrediction>> MostrarResultados()
        {

            var imagePredictions = await _context.ImagePredictions.Include(ip => ip.ImageData).ToListAsync();

            return imagePredictions;
        }

        struct InceptionSettings
        {
            public const int ImageHeight = 224;
            public const int ImageWidth = 224;
            public const float Mean = 117;
            public const float Scale = 1;
            public const bool ChannelsLast = true;
        }
    }
}
