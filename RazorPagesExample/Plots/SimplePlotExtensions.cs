using ScottPlot;

public static class SimplePlotExtensions
{
    public static SimplePlot ToSimplePlot(this Plot plt, int height, int width)
    {
        var tempFile = System.IO.Path.GetTempFileName() + ".png";
        try
        {
            plt.SaveFig(tempFile);

            var bytes = File.ReadAllBytes(tempFile);
            var base64 = Convert.ToBase64String(bytes);
            return new SimplePlot
            {
                src = $"data:image/png;base64,{base64}",
                height = height,
                width = width,
                alt = "a plot"
            };
        }
        finally
        {
            System.IO.File.Delete(tempFile);
        }
    }
}
