using Microsoft.AspNetCore.Components;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;

[Route("/")]
public class IndexApp : ShinyAppBase
{
    // inputs are here...
    Subject<int> mainSlider = new Subject<int>();

    public IndexApp(WebSocket webSocket) :
        base(webSocket)
    {
        // TODO - so far we've put InitSubscriptions() here (not in OnInitComplete)
        // ... because Stuart has forgotten how observables work and needs initial values
        // ... to come through the pipes...
        InitSubscriptions();
    }

    private void InitSubscriptions()
    {
        var printer = new RenderPrintObserver("sliderText", this);
        mainSlider.Subscribe(i => printer.OnNext(i));

        // the plot observables here are a bit of a mess...
        // I'm sure someone more Rx aware than me could assist...
        // there are also other client data things to consider here
        // - e.g. `clientdata_output_mainPlot_visible`
        var mainPlot = new RenderPlotObserver("mainPlot", this);
        var mainPlotHeight = GetClientData(".clientdata_output_mainPlot_height");
        var mainPlotWidth = GetClientData(".clientdata_output_mainPlot_width");
        var plotDetails = Observable.CombineLatest(mainSlider, mainPlotHeight, mainPlotWidth,
            (slider, h, w) => new { Slider = slider, H = h.GetInt32(), W = w.GetInt32() });

        var testData = Enumerable.Range(0, 100).Select(i => Random.Shared.NextDouble()).ToArray();
        plotDetails.Subscribe(i =>
        {
            var plotter = new BucketColumnPlotter();
            var simplePlot = plotter.Plot(i.Slider, i.H, i.W, testData);
            mainPlot.OnNext(simplePlot);
        });
    }
}
