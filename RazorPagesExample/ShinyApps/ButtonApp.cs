using Microsoft.AspNetCore.Components;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;

[Route("/Button/")]
public class ButtonApp : ShinyAppBase
{
    // inputs are here...
    Subject<int> mainButton = new Subject<int>();

    public ButtonApp(WebSocket webSocket) :
        base(webSocket)
    {
        // TODO - so far we've put InitSubscriptions() here (not in OnInitComplete)
        // ... because Stuart has forgotten how observables work and needs initial values
        // ... to come through the pipes...
        InitSubscriptions();
    }

    private void InitSubscriptions()
    {
        var printer = new RenderPrintObserver("buttonText", this);
        mainButton.Subscribe(i => printer.OnNext($"Button pressed {i} times"));
    }
}
