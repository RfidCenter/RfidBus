[![in english](http://rfidcenter.ru/img/flag-uk.svg) ENGLISH VERSION](README_EN.md)

Работа со СКУД
==============

* [Получение списка СКУД контроллеров](#GetAcsControllers)
* [Добавление ключей доступа](#AddAllowedKeys)
* [Получение ключей доступа](#GetAllowedKeys)
* [Удаление ключей доступа](#RemoveAllowedKeys)
* [События контроллера СКУД](#Events)

<a name="GetAcsControllers"></a>
Получение списка СКУД контроллеров
----------------------------------

Получить список доступных в Шине RFID контроллеров СКУД можно через запрос загруженных контроллеров СКУД GetAcsControllers.

```cs
RfidBusClient client;
 ...
GetAcsControllersResponse acsControllers = client.SendRequest(new GetAcsControllers());

foreach (var controller in acsControllers.Controllers)
{
    Console.WriteLine(" * processing Asc controller: {0}", controller.Name);
}
```

<a name="AddAllowedKeys"></a>
Добавление ключей доступа
-------------------------
Добавление списка ключей идентификации в контроллере СКУД реализовано через запрос AddAllowedKeys.

```cs
RfidBusClient client;
 ...
var acsControllers = GetAcsControllersList();

foreach (var controller in acsControllers.Controllers)
{
    client.SendRequest(
            new AddAllowedKeys(
                    controller.Id,
                    new [] {
                            new Transponder {
                                Id = new [] { 0xfa, 0xe4, 0x36, 0x04 }
                            }
                    }
            )
    );
}
````

<a name="GetAllowedKeys"></a>
Получение ключей доступа
------------------------

Получение списка прописанных ключей доступа на контроллере СКУД реализовано через запрос GetAllowedKeys.

```cs
RfidBusClient client;
 ...
var acsControllers = GetAcsControllersList();

foreach (var controller in acsControllers.Controllers)
{
    Transponder getKeysResult[] = client.SendRequest(new GetAllowedKeys(controller.Id))?.AllowedKeys;
}
```

<a name="RemoveAllowedKeys"></a>
Удаление ключей доступа
-----------------------

Удаление ключей идентификации из контроллера СКУД реализовано через запрос RemoveAllowedKeys.

```cs
RfidBusClient client;
...
client.ReceivedEvent += RfidBusClientOnReceivedEvent;
var acsControllers = GetAcsControllersList();

foreach (var controller in acsControllers.Controllers)
{
    client.SendRequest(new SubscribeToAcsControllerEvents(controller.Id));
    client.SendRequest(new RemoveAllowedKeys(
            controller.Id,
            new [] {
                new Transponder {
                    Id = new [] { 0xfa, 0xe4, 0x36, 0x04 }
                }
            }
    ));
}
```

<a name="Events"></a>
События контроллера СКУД
------------------------

Подписка на события контроллера осуществляется через запрос SubscribeToAcsControllerEvents, после выполнения которого будут порождаться события прохода, ошибок доступа и др.

```cs
RfidBusClient client;
client.SendRequest(new SubscribeToAcsControllerEvents(controller.Id));
 ...
private static void RfidBusClientOnReceivedEvent(object sender, ReceivedEventEventArgs receivedEventEventArgs)
{
    if (receivedEventEventArgs.EventMessage is
            AcsControllerObjectPassEvent)
    {
        var passEvent = (AcsControllerObjectPassEvent)
                receivedEventEventArgs.EventMessage;
        Console.WriteLine(($"ACS controller {passEvent.Controller.Name} generate event Object Pass Event for transponder  {passEvent.Transponder.IdAsString} "));
    }
 }
```

[⬅ К оглавлению](../README.md)
