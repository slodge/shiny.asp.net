using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

public class ShinyMessageWrapper
{
    public string method { get; set; }
}

public class ShinyMessageWrapper<T> : ShinyMessageWrapper
{
    public T data { get; set; }
}


public class ShinyTagMessage : Dictionary<string, JsonElement>
{

}

public class ShinyUpdateMessage : Dictionary<string, JsonElement>
{

}

public class ShinyInitMessage : Dictionary<string, JsonElement>
{
}

public class ShinyAppBase
{
    WebSocket _webSocket;

    public ShinyAppBase(WebSocket webSocket)
    {
        _webSocket = webSocket;
    }

    public async Task Handle(ShinyInitMessage init)
    {
        foreach (var kvp in init)
        {
            await HandleUpdate(kvp.Key, kvp.Value, isInitial: true);
        }

        await OnInitComplete();
    }

    protected virtual async Task OnInitComplete()
    {
        // nothing to do here...
    }

    // need something better here... some kind of observable dictionary of observables but this will do for demo
    private readonly Dictionary<string, Subject<JsonElement>> _clientData = new Dictionary<string, Subject<JsonElement>>();

    protected IObservable<JsonElement> GetClientData(string key)
    {
        if (!_clientData.TryGetValue(key, out var subject))
        {
            subject = new Subject<JsonElement>();
            _clientData[key] = subject;
        }

        return subject.DistinctUntilChanged();
    }

    public async Task HandleUpdate(string key, JsonElement value, bool isInitial = false)
    {
        // could do better regex here... but is OK!
        var split = key.Split(":", 2);
        var name = split[0];
        var inputHandler =
            split.Length == 2 ?
            split[1] :
            string.Empty;

        if (name.StartsWith(".clientdata"))
        {
            if (!_clientData.TryGetValue(name, out var subject))
            {
                subject = new Subject<JsonElement>();
                _clientData[name] = subject;
            }
            subject.OnNext(value);
            return;
        }

        var fields = this.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var the_field = fields.FirstOrDefault(t => t.Name == name); // kvp.Key);

        if (the_field == null)
        {
            // logging/error would be nice, eh?
            return;
        }

        var the_instance = the_field.GetValue(this);
        if (the_instance == null)
        {
            // logging/error would be nice, eh?
            return;
        }

        ShinyInputHandlers.Known.TryGetValue(inputHandler, out var handler);
        if (handler == null)
        {
            // logging/error would be nice, eh?
            return;
        }

        await handler.Handle(value, the_instance);
    }

    public async Task Handle(ShinyUpdateMessage update)
    {
        foreach (var kvp in update)
        {
            await HandleUpdate(kvp.Key, kvp.Value);
        }
    }

    public async Task Handle(ShinyTagMessage tag)
    {
        // not sure what would be here...
    }


    public async Task RunApp()
    {
        // this buffer might not be big enough :)
        var buffer = new byte[1024 * 4];

        var receiveResult = await _webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            var data = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, receiveResult.Count));
            Debug.WriteLine("Received ", data);

            var decode1 = System.Text.Json.JsonSerializer.Deserialize<ShinyMessageWrapper>(data);
            switch (decode1?.method ?? "unknown")
            {
                case "init":
                    var init_msg = System.Text.Json.JsonSerializer.Deserialize<ShinyMessageWrapper<ShinyInitMessage>>(data);
                    await Handle(init_msg?.data);
                    break;
                case "update":
                    var update_msg = System.Text.Json.JsonSerializer.Deserialize<ShinyMessageWrapper<ShinyUpdateMessage>>(data);
                    await Handle(update_msg?.data);
                    break;
                case "tag":
                    var tag_msg = System.Text.Json.JsonSerializer.Deserialize<ShinyMessageWrapper<ShinyTagMessage>>(data);
                    await Handle(tag_msg?.data);
                    break;

                default:
                    Debug.WriteLine("Unknown message method received: ", data);
                    break;
            }

            receiveResult = await _webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await _webSocket.CloseAsync(
            receiveResult.CloseStatus.Value,
            receiveResult.CloseStatusDescription,
            CancellationToken.None);
    }

    public void SendOutput(ShinyServerUpdateMessage update)
    {
        // feels bad that these sends aren't awaited and protected...
        // maybe that's what needs to happen with some kind of Rx scheduler

        //var json = System.Text.Json.JsonSerializer.Serialize(update);
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(update);
        var binary = System.Text.Encoding.UTF8.GetBytes(json);
        
        // unawaited ... oh my!
        _webSocket.SendAsync(binary, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}

public interface IShinyInputHandler
{
    string Name { get; }

    Task<bool> Handle(JsonElement value, object target);
}

public static class ShinyInputHandlers
{
    public readonly static Dictionary<string, IShinyInputHandler> Known = new Dictionary<string, IShinyInputHandler>();

    static ShinyInputHandlers()
    {
        Add(new ShinyDefaultInputHandler());
        Add(new ShinyActionInputHandler());
        Add(new ShinyDateInputHandler());
    }

    private static void Add(IShinyInputHandler toAdd)
    {
        Known[toAdd.Name] = toAdd;
    }
}

public class ShinyDefaultInputHandler : IShinyInputHandler
{
    public string Name => string.Empty;

    public async Task<bool> Handle(JsonElement value, object target)
    {

        var intTypedTarget = target as ISubject<int>;
        if (intTypedTarget != null)
        {
            if (value.ValueKind != JsonValueKind.Number)
            {
                // Log message...
                return false;
            }

            var typedValue = value.GetInt32();
            intTypedTarget.OnNext(typedValue);
            return true;
        }

        var doubleTypedTarget = target as ISubject<double>;
        if (doubleTypedTarget != null)
        {
            if (value.ValueKind != JsonValueKind.Number)
            {
                // Log message...
                return false;
            }

            var typedValue = value.GetDouble();
            doubleTypedTarget.OnNext(typedValue);
            return true;
        }

        var stringTypedTarget = target as ISubject<string>;
        if (stringTypedTarget != null)
        {
            var typedValue = value.GetString();
            stringTypedTarget.OnNext(typedValue);
            return true;
        }

        // Log message and error...
        return false;
    }
}


public class ShinyDateInputHandler : IShinyInputHandler
{
    public string Name => "shiny.date";

    public async Task<bool> Handle(JsonElement value, object target)
    {
        var values = new List<DateTime>();

        if (value.ValueKind == JsonValueKind.Array)
        {
            foreach (var element in value.EnumerateArray())
            {
                if (element.ValueKind != JsonValueKind.String)
                {
                    // todo - logging would be nice...
                    return false;
                }

                values.Add(element.GetDateTime());
            }
        } 
        else if (value.ValueKind == JsonValueKind.String)
        {
            values.Add(value.GetDateTime());
        }
        else
        {
            // todo - logging and errors and...
            return false;
        }

        var datetimeTypedTarget = target as ISubject<DateTime>;
        if (datetimeTypedTarget != null)
        {
            datetimeTypedTarget.OnNext(values.Single());
            return true;
        }

        var datetimeArrayTypedTarget = target as ISubject<DateTime[]>;
        if (datetimeArrayTypedTarget != null)
        {
            datetimeArrayTypedTarget.OnNext(values.ToArray());
            return true;
        }

        // Log message and error...
        return false;
    }
}


public class ShinyActionInputHandler : IShinyInputHandler
{
    public string Name => "shiny.action";

    public async Task<bool> Handle(JsonElement value, object target)
    {
        if (value.ValueKind != JsonValueKind.Number)
        {
            // Log message...
            return false;
        }

        var intTypedTarget = target as ISubject<int>;
        if (intTypedTarget != null)
        {
            var typedValue = value.GetInt32();
            intTypedTarget.OnNext(typedValue);
            return true;
        }

        var doubleTypedTarget = target as ISubject<double>;
        if (doubleTypedTarget != null)
        {
            var typedValue = value.GetDouble();
            doubleTypedTarget.OnNext(typedValue);
            return true;
        }

        // Log message and error...
        return false;
    }
}

public class ShinyServerUpdateMessage
{
    public Dictionary<string, object> errors = new Dictionary<string, object>();
    public Dictionary<string, object> values = new Dictionary<string, object>();
    public List<string> inputMessage = new List<string>();
}

public class CaseSensitiveIgnoreTrailingSlashComparer : IEqualityComparer<string>
{
    public bool Equals(string? x, string? y)
    {
        return x?.TrimEnd('/') == y?.TrimEnd('/');
    }

    public int GetHashCode([DisallowNull] string obj)
    {
        return obj?.TrimEnd('/').GetHashCode() ?? 0;
    }
}

public static class ShinyAppLookup
{
    public static Dictionary<string, ConstructorInfo> Constructors = new Dictionary<string, ConstructorInfo>(new CaseSensitiveIgnoreTrailingSlashComparer());

    public static void AddAssembly(Assembly assembly)
    {
        var types =
            assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ShinyAppBase)))
            .Select(t =>
            {
                var attr = t.GetCustomAttribute<RouteAttribute>();
                if (attr == null) return null;
                return new
                {
                    Route = attr.Template,
                    Type = t,
                    Constructor = t.GetConstructor(new[] { typeof(WebSocket) })
                };
            })
            .Where(t => t != null)
            .Where(t => !string.IsNullOrWhiteSpace(t.Route))
            .Where(t => t.Constructor != null);

        foreach (var t in types)
        {
            Constructors[t.Route] = t.Constructor;
        }
    }
}
