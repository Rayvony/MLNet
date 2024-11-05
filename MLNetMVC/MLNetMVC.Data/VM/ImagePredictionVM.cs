using MLNetMVC.Data.EF;

namespace MLNetMVC.Data.VM
{
    public class ImagePredictionVM : ImageDatumVM
    {
        public float[]? Score;

        public string? PredictedLabelValue;
    }
}
