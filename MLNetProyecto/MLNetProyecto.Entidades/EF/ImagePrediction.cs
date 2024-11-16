using System;
using System.Collections.Generic;

namespace MLNetProyecto.Entidades.EF;

public partial class ImagePrediction
{
    public int Id { get; set; }

    public int? ImageDataId { get; set; }

    public string? Score { get; set; }

    public string? PredictedLabelValue { get; set; }

    public bool? IsCorrect { get; set; }

    public virtual ImageDatum? ImageData { get; set; }
}
