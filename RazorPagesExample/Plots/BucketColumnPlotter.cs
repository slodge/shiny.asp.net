using System.Reactive.Linq;

public class BucketColumnPlotter
{
    public BucketColumnPlotter() { }

    public SimplePlot Plot(int bins, int height, int width, double[] array)
    {
        if (height <= 0) height = 400;
        if (width <= 0) width = 600;

        var (values, positions) = CreateValuesAndPositions(bins, array);

        var plt = new ScottPlot.Plot(width, height);
        var bar = plt.AddBar(values, positions);
        bar.BarWidth = (positions[1] - positions[0]) * .8;
        plt.SetAxisLimits(yMin: 0);

        return plt.ToSimplePlot(height, width);
    }

    private static (double[] values, double[] positions) CreateValuesAndPositions(int bins, double[] array)
    {
        var min = array.Min();
        var max = array.Max();

        var step = (max - min) / bins;

        var bucketDividers = Enumerable.Range(0, bins).Select(i =>
                new
                {
                    Key = i,
                    Min = min + i * step
                }
            ).ToArray();
        var bucketCounts = Enumerable.Repeat(0, bins).ToList();

        foreach (var d in array)
        {
            // faster code is available...
            var position = bucketDividers.Last(x => d >= x.Min).Key;
            bucketCounts[position] = bucketCounts[position] + 1;
        }

        // return the sampled data
        return (
            bucketCounts.ConvertAll(i => (double)i).ToArray(),
            bucketDividers.Select(b => b.Min).ToArray()
        );
    }
}
