[![на русском](http://rfidcenter.ru/img/flag-ru.svg) РУССКАЯ ВЕРСИЯ](README.md)

How to work with GPIO
=============

* [External devices management](#SetReaderGpoStates)
* [Request of a state of an external device](#GetReaderGpiStates)
* [Events of external devices](#ReaderGpiStatesChangedEvent)

<a name="SetReaderGpoStates"></a>External devices management
--------------------------------------

One can manage the state of GPO by using the SetReaderGpoStates request, that sends an array with port states.

```cs
RfidBusClient client;
ReaderRecord reader;
bool state = true;
 ...
reader.IsActive = true;
var result = client.SendRequest(new SetReaderGpoStates(reader.Id,
         new GpoStateRecord[]
         {
             new GpoStateRecord(1, state),
             new GpoStateRecord(2, state),
             new GpoStateRecord(3, state),
             new GpoStateRecord(4, state),
         }));

Console.WriteLine($"Reader: {reader.Name} ({reader.Id}). GPO ports:" +
        " 1, 2, 3, 4, value: {state}, status: {result.Status}");
```

<a name="GetReaderGpiStates"></name>Request of a state of an external device
------------------------------------------

One can receive a current state of GPI by using GetReaderGpiStates request.

```cs
RfidBusClient client;
ReaderRecord reader;
 ...
GetReaderGpiStatesResponse response = client.SendRequest(new
        GetReaderGpiStates(reader.Id));
Console.WriteLine($"Reader: {reader.Name} ({reader.Id}). GPI " +
        "reading status: {response.Status}");
 ...
foreach (var gpiState in response.GpiStates)
{
    if (gpiState != null) {
        Console.WriteLine($"    Port: {gpiState.Port}\tstate:{gpiState.State}");
    }
}
```

<a name="ReaderGpiStatesChangedEvent"></a>Events of external devices
------------------------------------

To receive events of GPI status changes it is necessary to subscribe to basic reader events after that event handler will be able to capture GPI status changes (ReaderGpiStatesChangedEvent).

```cs
RfidBusClient client;
...
client.ReceivedEvent += RfidBusClientOnReceivedEvent;

var readersResult = client.SendRequest(new GetReaders());
if (readersResult.Status != ResponseStatus.Ok)
{
    throw new Exception(string.Format("Can't get info about connected"+
           "readers. Reason: {0}.", readersResult.Status));
}

foreach (var reader in readersResult.Readers)
{
   client.SendRequest(new SubscribeToReader(reader.Id));
}

 ...
void RfidBusClientOnReceivedEvent(object sender, ReceivedEventEventArgs args)
{
    if (args.EventMessage is ReaderGpiStatesChangedEvent)
    {
        var message = (ReaderGpiStatesChangedEvent)args.EventMessage;
        Console.WriteLine(
                $"> Reader '{message.ReaderRecord.Name}' Port:" +
                 "{message.GpiState.Port}; State: {message.GpiState.State}");
    }
}
```

In emulator, the GPI port statuses correspond to GPO port statuses, thus it is necessary to change GPO port status to handle the event ReaderGpiStatesChangedEvent in emulator

[⬅ Back to contents](../README_EN.md)