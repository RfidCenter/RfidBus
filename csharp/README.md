[![in english](http://rfidcenter.ru/img/flag-uk.svg) ENGLISH VERSION](README_EN.md)

Работа с Шиной RFID через .NET SDK
==========================================

* [Начало работы](#GetStarted)
* [Подключение к Шине RFID](#Connect)

**Работа со считывателями**

* [Получение списка считывателей](#GetReaders)
* [Поиск меток](Readers.ObserveTags/README.md)
* [Чтение данных из меток](Readers.Read/README.md)
* [Программирование меток](Readers.Write/README.md)
* [Управление доступом](Readers.Access/README.md)
* [Специализированные команды](Readers.SpecialCommands/README.md)
* [GPIO](Readers.Gpio/README.md)
* [Интерфейс мобильных считывателей](Readers.MobileReadersInterface)
* [Интерфейс мобильных считывателей на основе изображения](Readers.MobileReadersRasterInterface)

**Работа с принтерами**

* [Получение списка принтеров](#GetPrinters)
* [Печать меток](Printers/README.md)

**Работа со СКУД**
* [Получение списка СКУД контроллеров](#GetAcs)
* [Получение списка СКУД контроллеров](AcsControllers/README.md#GetAcsControllers)
* [Добавление ключей доступа](AcsControllers/README.md#AddAllowedKeys)
* [Получение ключей доступа](AcsControllers/README.md#GetAllowedKeys)
* [Удаление ключей доступа](AcsControllers/README.md#RemoveAllowedKeys)
* [События контроллера СКУД](AcsControllers/README.md#Events)

<a name="GetStarted"></a>Начало работы
-------------

Все необходимые для работы с [Шиной RFID](http://rfidcenter.ru/product/rfidbus) сборки находятся по месту установки [Менеджера Шины RFID](http://rfidcenter.ru/files/RfidBusManagerSetup.exe)
(обычно «С:\Program Files (x86)\RfidCenter\RfidbusManager\»):
* RfidCenter.Basic.dll
* RfidCenter.Devices.dll
* RfidBus.Primitives.dll
* serializers/RfidBus.Serializers.Ws.dll

Для демонстрационных целей интеграционных возможностей Шины RFID развёрнут демо-сервер:
* Хост: demo.rfidbus.rfidcenter.ru
* Порт: 80
* Логин: demo
* Пароль: demo


<a name="Connect"></a>
Подключение к Шине RFID
-----------------------

За управление подключением и передачей команд Шине RFID отвечает класс RfidBusClient.
При создании экземпляра класса необходимо передать экземпляр описания используемого
протокола и параметры подключения по этому протоколу. После каждого успешного
подключения, необходимо производить авторизацию.

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

Свойство AllowReconnect включает встроенный механизм восстановления связи с Шиной RFID,
с последующей автоматической авторизацией по токену. При авторизации по токену происходит
подключения к ранее созданной сессии пользователя на Шине RFID, поэтому все подписки на
события и состояния устройств сохраняются.

Отслеживать состояние связи с Шиной RFID можно подписавшись на события Disconnected и Reconnected.

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

При разрыве соединения возможна авторизация по токену. В этом случае сохраняются состояния работы считывателей и подписки на их события.

```cs
RfidBusClient client;
 ...
if (!client.Authorize(95735487))
{
    throw new Exception("Invalid token.");
}
Console.WriteLine("Connection established.");
```

В случае успешной авторизации можно отправлять запросы серверу через методы SendRequest/SendRequestAsync.

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
Получение списка считывателей
-----------------------
Получить список доступных в Шине RFID считывателей можно через запрос загруженных считывателей GetReaders.

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

Пример на GitHub: [Readers.ObserveTags](Readers.ObserveTags)


<a name="GetPrinters"></a>
Получение списка принтеров
-----------------------

Получить список доступных в Шине RFID принтеров можно через запрос загруженных принтеров GetPrinters.

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

Пример на GitHub: [Printers](Printers)


<a name="GetAcs"></a>
Получение списка СКУД контроллеров
-----------------------

Получить список доступных в Шине RFID контроллеров СКУД принтеров можно через запрос загруженных контроллеров СКУД GetAcsControllers.

```cs
RfidBusClient client;
 ...
GetAcsControllersResponse acsControllers = client.SendRequest(new GetAcsControllers());

foreach (var controller in acsControllers.Controllers)
{
    Console.WriteLine(" * processing Asc controller: {0}", controller.Name);
}
```

Пример на GitHub: [AcsControllers](AcsControllers)