using Microsoft.AspNetCore.Components;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;

[Route("/DateChart")]
public class DateChartApp : ShinyAppBase
{
    // inputs are here...
    Subject<DateTime[]> mainDateRange = new Subject<DateTime[]>();

    public DateChartApp(WebSocket webSocket) :
        base(webSocket)
    {
        // TODO - so far we've put InitSubscriptions() here (not in OnInitComplete)
        // ... because Stuart has forgotten how observables work and needs initial values
        // ... to come through the pipes...
        InitSubscriptions();
    }

    private void InitSubscriptions()
    {
        var printer = new RenderPrintObserver("dateText", this);
        mainDateRange.Subscribe(dates =>
        {
            var text = string.Join(" -> ", dates.Select(d => d.ToString()));
            printer.OnNext(text);
        });

        // the plot observables here are a bit of a mess...
        // I'm sure someone more Rx aware than me could assist...
        // there are also other client data things to consider here
        // - e.g. `clientdata_output_mainPlot_visible`
        var datePlot = new RenderPlotObserver("datePlot", this);
        var datePlotHeight = GetClientData(".clientdata_output_datePlot_height");
        var datePlotWidth = GetClientData(".clientdata_output_datePlot_width");
        var plotDetails = Observable.CombineLatest(mainDateRange, datePlotHeight, datePlotWidth,
            (dateRange, h, w) => new { DateRange = dateRange, H = h.GetInt32(), W = w.GetInt32() });

        plotDetails.Subscribe(i =>
        {
            var plotter = new DateChartPlotter();
            var simplePlot = plotter.Plot( i.H, i.W, i.DateRange);
            datePlot.OnNext(simplePlot);
        });

    }
}
