[![на русском](http://rfidcenter.ru/img/flag-ru.svg) РУССКАЯ ВЕРСИЯ](README_EN.md)

How to work with ACS
==============

* [Get A List Of ACS Controllers](#GetAcsControllers)
* [Add An Access Key](#AddAllowedKeys)
* [Get A List Of Access Keys](#GetAllowedKeys)
* [Delete An Access Key](#RemoveAllowedKeys)
* [ACS Controller Events](#Events)

<a name="GetAcsControllers"></a>
Get A List Of ACS Controllers
----------------------------------

One can get the list of ACS controllers available to RFID Bus by using the GetAcsControllers request of ACS controllers had been loaded.

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
Add An Access Key
-------------------------

One can add access keys to the access key list of ACS controller by using the AddAllowedKeys request.

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
Get A List Of Access Keys
------------------------

One can get the list of registered access keys of ACS controller by using the GetAllowedKeys request.

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
Delete An Access Key
-----------------------

One can delete the access key of ACS controller by using the RemoveAllowedKeys request.

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
ACS Controller Events
------------------------

The SubscribeToAcsControllerEvents request is used to subscribe to the ACS controller events, after its execution such events as entrance event, access error event etc. will be generated.

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

[⬅ Back to contents](../README_EN.md)