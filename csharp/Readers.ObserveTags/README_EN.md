[![на русском](http://rfidcenter.ru/img/flag-ru.svg) РУССКАЯ ВЕРСИЯ](README.md)

Inventorisation
==============

It is necessary to set up the reader to the reading mode and to subscribe to basic reader events to perform an inventorisation (searching of tags located at the reading area). After that, in an event handler, there will be an opportunity to fix the detection (TransponderFoundEvent) and the loss (TransponderLostEvent) of the tag and GPI status changing.

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
{¬
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

[⬅ Back to the Contents](../README_EN.md)