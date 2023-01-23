public class RenderPrintObserver : IObserver<object>
{
    string _outputName;
    ShinyAppBase _appBase;

    public RenderPrintObserver(string outputName, ShinyAppBase appBase)
    {
        _outputName = outputName;
        _appBase = appBase;
    }


    public void OnCompleted()
    {
        // nothing to do
    }

    public void OnError(Exception error)
    {
        // well... this is awkward
    }

    public void OnNext(object value)
    {
        var msg = new ShinyServerUpdateMessage();
        msg.values[_outputName] = value?.ToString();
        _appBase.SendOutput(msg);
    }
}
