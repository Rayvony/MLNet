using Microsoft.ML.Data;

namespace MLNetProyecto.Entidades.VM
{
    public class ImageDatumVM
    {
        [LoadColumn(0)]
        public string? ImagePath;

        [LoadColumn(1)]
        public string? Label;

        [LoadColumn(2)]
        public string? FileName;
    }
}
