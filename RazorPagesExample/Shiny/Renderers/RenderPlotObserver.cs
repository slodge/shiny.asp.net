public class RenderPlotObserver : IObserver<SimplePlot>
{
    string _outputName;
    ShinyAppBase _appBase;

    public RenderPlotObserver(string outputName, ShinyAppBase appBase)
    {
        _outputName = outputName;
        _appBase = appBase;
    }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(SimplePlot value)
    {
        var msg = new ShinyServerUpdateMessage();
        msg.values[_outputName] = value;
        _appBase.SendOutput(msg);
    }
}

