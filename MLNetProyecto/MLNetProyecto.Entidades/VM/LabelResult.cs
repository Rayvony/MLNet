namespace MLNetProyecto.Entidades
{
    public class LabelAlias
    {
        public string Name { get; set; }
    }

    public class LabelCategory
    {
        public string Name { get; set; }
    }

    public class LabelResult
    {
        public string Name { get; set; }
        public float Confidence { get; set; }
        public List<string> Parents { get; set; }
        public List<LabelAlias> Aliases { get; set; }
        public List<LabelCategory> Categories { get; set; }
    }
}
