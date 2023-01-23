using ScottPlot;
using System.Drawing;
using System.Reactive.Linq;

public class DateChartPlotter
{
    public DateChartPlotter() { }

    public SimplePlot Plot(int height, int width, DateTime[] array)
    {
        var start = array[0];
        var end = array[1];

        if (height <= 0) height = 400;
        if (width <= 0) width = 600;

        var plt = new ScottPlot.Plot(width, height);

        OHLC[] ohlcs = DataGen.RandomStockPrices(null, (int)Math.Floor((end-start).TotalDays + 1));
        for (var i = 0; i<ohlcs.Length; i++)
        {
            ohlcs[i].DateTime = start.AddDays(i);
        }

        var candlePlot = plt.AddCandlesticks(ohlcs);

        var bol = candlePlot.GetBollingerBands(20);
        plt.AddScatterLines(bol.xs, bol.sma, Color.Blue);
        plt.AddScatterLines(bol.xs, bol.lower, Color.Blue, lineStyle: LineStyle.Dash);
        plt.AddScatterLines(bol.xs, bol.upper, Color.Blue, lineStyle: LineStyle.Dash);
        plt.XAxis.DateTimeFormat(true);

        return plt.ToSimplePlot(height, width);
    }
}
