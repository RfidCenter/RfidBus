Работа со спец. командами
=========================

Некоторое оборудование поддерживает расширенные функции, работа с которыми в Шине RFID реализована через специализированные команды. Например, управление звуком, светодиодами, счётчиком людей, графическими интерфейсами и пр.

Получение списка поддерживаемых считывателем специальных команд реализовано через запрос GetSpecialCommands.

В примере продемонстрировано получение списка специализированных команд, с последующей отправкой спец. команды ReinitializeTransponders (повторной инициализации меток) эмулятору через запрос ExecuteSpecialCommand.

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

Подписка на на спец. события осуществляется через запрос SubscribeToSpecialEvent. Ниже, как и в предыдущем примере, производится перебор всех подключенных считывателей, и у каждого из них запрашивается список специальных событий. Далее  на каждое из них производится подписка.

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
    var message = e.EventMessage as OnReaderSpecialEvent;
    if (message != null)
    {
        Console.WriteLine($"Special event. Reader: {message.Reader.Name};" +
                "eventName: {message.EventName}");
    }
}
```

Отмена подписки реализована через запрос UnsubscribeFromSpecialEvent.

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

[⬅ К оглавлению](../README.md)
