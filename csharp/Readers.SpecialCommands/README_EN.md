[![на русском](http://rfidcenter.ru/img/flag-ru.svg) РУССКАЯ ВЕРСИЯ](README.md)

How to work with Special Commands
=========================

It is possible to operate with different Special Commands of RFID Bus to work with different expanded functions of some RFID devices e.g. sound control, LED control, people counter, graphical interface etc.

GetSpecialCommands request is using to get the list of commands supported by the reader.

In an example, the receiving of the list of special commands with the subsequent sending of the ReinitializeTransponders (reinitialization of transponders) special command to the emulator by using ExecuteSpecialCommand request is considered below.

```cs
const string COMMAND_REINITIALIZE_TAGS = "ReinitializeTransponders";
RfidBusClient client;
...
var readersResult = client.SendRequest(new GetReaders());
foreach (var readerRecord in readersResult.Readers)
{
    Console.WriteLine($"Reader: {readerRecord.Name}");
    var response = GetSpecialCommands(readerRecord);
    foreach (var specialCommand in response.SpecialCommands)
    {
        Console.WriteLine($"Special command name: {specialCommand.Name}; " +
               "description: {specialCommand.Description}; parameters: " +
               "{specialCommand.Parameters}");

        if (specialCommand.Name == COMMAND_REINITIALIZE_TAGS)
        {
            var result = client.SendRequest(
                    new ExecuteSpecialCommand(reader.Id,
                    COMMAND_REINITIALIZE_TAGS,
                    null));
            if (result.Status != ResponseStatus.Ok)
            {
               throw new Exception($"ExecuteSpecialCommand error: " +
                       "{result.Details}");
            }
        }
    }
}
```

The SubscribeToSpecialEvent request is used to the subscription to special events. Here, as well as in the previous example, the search of all connected readers is performed, and the list of special events is requested from each of them. Further, the subscription to each of these readers is performed.

```cs
RfidBusClient client;
...
client.ReceivedEvent += RfidBusClientOnReceivedEvent;
foreach (var readerRecord in readersResult.Readers)
{
    Console.WriteLine($"Reader: {readerRecord.Name}");
    GetSpecialEventsResponse specialEventsResponse  = client.SendRequest(
            new GetSpecialEvents(readerRecord .Id));
    if (specialEventsResponse.Status != ResponseStatus.Ok)
    {
        throw new Exception($"GetSpecialEventsResponse error:" +
                "{specialEventsResponse.Details}");
    }
    
    foreach (var specialEvent in specialEventsResponse.SpecialEvents)
    {
        Console.Write($" Special event name: {specialEvent.Name} " +
                "description: {specialEvent.Description}");
        var result = client.SendRequest(
                new SubscribeToSpecialEvent(readerRecord.Id, specialEvent.Name));
        if (result.Status != ResponseStatus.Ok)
        {
            throw new Exception($"SubscribeToSpecialEvent error: " +
                    "{result.Details}");
        }
    }
}
...
static void RfidBusClientOnReceivedEvent(object sender,
        ReceivedEventEventArgs e)
{
    var message = e.EventMessage as ReaderSpecialEvent;
    if (message != null)
    {
        Console.WriteLine($"Special event. Reader: {message.Reader.Name};" +
                "eventName: {message.EventName}");
    }
}
```

The UnsubscribeFromSpecialEvent request is used for the cancellation of the subscription.

```cs
RfidBusClient client;
 ...
ReaderRecord reader;
 ...
var result = client.SendRequest(
        new UnsubscribeFromSpecialEvent(reader.Id, "ReinitializeTransponders"));
if (result.Status != ResponseStatus.Ok)
{
    throw new Exception($"UnsubscribeFromSpecialEvent error: " +
        "{result.Details}");
}
```

[⬅ Back to contents](../README_EN.md)