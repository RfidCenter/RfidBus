[![in english](http://rfidcenter.ru/img/flag-uk.svg) ENGLISH VERSION](README_EN.md)

Инвентаризация
==============

Для инвентаризации (поиска меток, находящихся в зоне действия считывателя) необходимо
произвести подписку на базовые события считывателя и перевести его в режим чтения.
В обработчике событий появится возможность фиксировать нахождение (TransponderFoundEvent)
и потерю (TransponderLostEvent) меток, а также изменения GPI (ReaderGpiStatesChangedEvent).

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
   client.SendRequest(new StartReading(reader.Id));
}
 ...
void RfidBusClientOnReceivedEvent(object sender,  ReceivedEventEventArgs args)
{
    if (args.EventMessage is TransponderFoundEvent)
    {
        var msg = (TransponderFoundEvent) args.EventMessage;

        Console.WriteLine("> Reader '{0}' found {1} transponder(s):",
                msg.ReaderRecord.Name, msg.Transponders.Length);
        foreach (var transponder in msg.Transponders)
        {
            Console.WriteLine(" * ID: '{0}', Antenna: {1}, Type: {2}",
                    transponder.IdAsString, transponder.Antenna,
                    transponder.Type);
        }
    }
}
```

[⬅ К оглавлению](../README.md)
