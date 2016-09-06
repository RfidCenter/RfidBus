Работа с GPIO
=============

* [Управление исполнительными механизмами](#SetReaderGpoStates)
* [Запрос состояния исполнительных механизмов](#GetReaderGpiStates)
* [События от исполнительных механизмов](#ReaderGpiStatesChangedEvent)


<a name="SetReaderGpoStates"></a>Управление исполнительными механизмами
--------------------------------------
Управление состоянием GPO реализовано запросом SetReaderGpoStates, в котором передаётся массив с состояниями портов.

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

<a name="GetReaderGpiStates"></name>Запрос состояния исполнительных механизмов
------------------------------------------

Получение текущего состояния GPI реализовано в запросе GetReaderGpiStates.

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

<a name="ReaderGpiStatesChangedEvent"></a>События от исполнительных механизмов
------------------------------------

Для получения событий изменения GPI необходимо произвести подписку на базовые события
считывателя, после чего в обработчике событий появится возможность фиксировать изменения
GPI (ReaderGpiStatesChangedEvent).

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

В эмуляторе состояния портов GPI соответствуют состоянию портов GPO. Таким образом, чтобы
получить ReaderGpiStatesChangedEvent на эмуляторе необходимо изменить состояние его GPO.

[⬅ К оглавлению](../README.md)
