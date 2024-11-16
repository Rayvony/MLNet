using System;
using System.Collections.Generic;

namespace MLNetProyecto.Entidades.EF;

public partial class ImageDatum
{
    public int Id { get; set; }

    public string? ImagePath { get; set; }

    public string? Label { get; set; }

    public string? FileName { get; set; }

    public virtual ICollection<ImagePrediction> ImagePredictions { get; set; } = new List<ImagePrediction>();
}
