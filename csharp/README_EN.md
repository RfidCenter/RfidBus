[![на русском](http://rfidcenter.ru/img/flag-ru.svg) РУССКАЯ ВЕРСИЯ](README.md)

How to work with RFID Bus via .NET SDK
==========================================

* [Getting Started](#GetStarted)
* [Connetion to RFID Bus](#Connect)

**How work with readers**

* [Get A List Of Readers](#GetReaders)
* [Tag Search](Readers.ObserveTags/README_EN.md)
* [Tag Memory Reading](Readers.Read/README_EN.md)
* [Tag Programming](Readers.Write/README_EN.md)
* [Access Control](Readers.Access/README_EN.md)
* [Special Commands](Readers.SpecialCommands/README_EN.md)
* [GPIO](Readers.Gpio/README_EN.md)
* [Mobile Reader Interface](Readers.MobileReadersInterface)
* [Image Based Mobile Reader GUI](Readers.MobileReadersRasterInterface)

**How to work with printers**

* [Get A List Of Printers](#GetPrinters)
* [Tag Printing](Printers/README_EN.md)

**How to work with ACS**

* [Get A List Of ACS Controllers](#GetAcs)
* [Get A List Of ACS Controllers](AcsControllers/README_EN.md#GetAcsControllers)
* [Add An Access Key](AcsControllers/README_EN.md#AddAllowedKeys)
* [Get A List Of Access Keys](AcsControllers/README_EN.md#GetAllowedKeys)
* [Delete An Access Key](AcsControllers/README_EN.md#RemoveAllowedKeys)
* [ACS Controller Events](AcsControllers/README_EN.md#Events)

<a name="GetStarted"></a>Getting Started
-------------

All necessary assemblies for [RFID Bus](http://rfidcenter.ru/en/product/rfidbus) are located in the [RFID Bus Manager](http://rfidcenter.ru/files/RfidBusManagerSetup.exe) installation folder (common directory: «С:\Program Files (x86)\RfidCenter\RfidbusManager\»):
* RfidCenter.Basic.dll
* RfidCenter.Devices.dll
* RfidBus.Primitives.dll
* serializers/RfidBus.Serializers.Ws.dll

For a demonstration of integration ability of RFID Bus one can use a special demo server:
* Host: demo.rfidbus.rfidcenter.ru
* Port: 80
* Login: demo
* Password: demo

<a name="Connect"></a>
Connetion to RFID Bus
-----------------------

The RfidBusClient class is responsible for a connection control and for a command transmission with RFID Bus. It is necessary to send connection parameters and an instance of the description of the used protocol for the creation of an instance of a class. After each successful connection, it is necessary to perform authorization.

```cs

RfidBusClient client;
 ...
var protocol = new WsCommunicationDescription();
var config = new ParametersValues(protocol.GetClientConfiguration());
config.SetValue(ConfigConstants.PARAMETER_HOST, "demo.rfidbus.rfidcenter.ru");
config.SetValue(ConfigConstants.PARAMETER_PORT, 80);

client = new RfidBusClient(protocol, config)
{
    AllowReconnect = true
};
client.Connect();

if (!client.Authorize("demo", "demo"))
{
    throw new Exception("Invalid login-password.");
}

Console.WriteLine("Connection established.");
```

AllowReconnect property turns on the built-in mechanism of restoration of the connection with RFID Bus with the subsequent automatic token based authorization. Due to the token based authorization, all subscriptions to events and devices statuses are saved when the connection to the previously created RFID Bus user session takes place.

One can follow the RFID Bus connection status by subscribing to the Disconnected and Reconnected events.

```cs
RfidBusClient client;
 ...
client = new RfidBusClient(protocol, config)
    {
        AllowReconnect = true,
        ReconnectRetries = 10,
        ReconnectInterval = TimeSpan.FromSeconds(30)
    };

client.Disconnected += RfidBusClientOnDisconnectedEvent;
 ...
void RfidBusClientOnDisconnectedEvent(object sender,
        ExceptionEventArgs args)
{
    Console.WriteLine($"Connection was lost! Details:"+
            "{args.Exception.Message}");
}

```

It is possible to perform a token based authorization when the disconnection happens. At this case, the reader's statuses and its event subscription statuses are saved.

```cs
RfidBusClient client;
 ...
if (!client.Authorize(95735487))
{
    throw new Exception("Invalid token.");
}
Console.WriteLine("Connection established.");

```

In a case of successful authorization, one can send requests to the server by using SendRequest/SendRequestAsync methods.

```cs

RfidBusClient client;
  ...
client.SendRequestAsync(new GetReaders());
client.ReceivedResponse += RfidBusClientOnReceivedResponseEvent;
 ...
void RfidBusClientOnReceivedResponseEvent(object sender,
      ReceivedResponseEventArgs e)
{
      MessageBaseResponse response = args.Response;
      Console.WriteLine($"Status: {response.Status}; details: "+
                "{response.Details}");
}

```

<a name="GetReaders"></a>
Get A List Of Readers
-----------------------

One can get the list of readers available to RFID Bus by using the GetReaders request of readers had been loaded.

```cs
RfidBusClient client;
 ...
var readersResult = client.SendRequest(new GetReaders());
if (readersResult.Status != ResponseStatus.Ok)
{
    throw new Exception(string.Format("Can't get info about connected" +
            " readers. Reason: {0}.", readersResult.Status));
}

foreach (ReaderRecord reader in readersResult.Readers)
{
    Console.WriteLine(" * processing reader: {0}", reader.Name);
}

```

Example on GitHub: [Readers.ObserveTags](Readers.ObserveTags)

<a name="GetPrinters"></a>
Get A List Of Printers
-----------------------

One can get the list of printers available to RFID Bus by using the GetPrinters request of printers had been loaded.

```cs
RfidBusClient client;
...
GetPrintersResponse printersResult = client.SendRequest(new GetPrinters());

Console.WriteLine("Printers list");
foreach (var printerRecord in printersResult.Printers)
{
    Console.WriteLine(" Name: '{0}'; description: '{1}'; model name: '{2}'; model Id: {3}; model description: '{4}'",
            printerRecord.Name,
            printerRecord.Description,
            printerRecord.ModelDescription.Name,
            printerRecord.ModelDescription.Id,
            printerRecord.ModelDescription.Description);
}

```

Example on GitHub: [Printers](Printers)

<a name="GetAcs"></a>
Get A List Of ACS Controllers
-----------------------

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

Example on GitHub: [AcsControllers](AcsControllers)