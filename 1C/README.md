Работа с Шиной RFID из 1C
========================

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

**Работа с принтерами**

* [Получение списка принтеров](#GetPrinters)
* [Печать меток](Printers/README.md)


<a name="GetStarted"></a>Начало работы
-------------

Внешняя компонета 1С для работы с [Шиной RFID](http://rfidcenter.ru/product/rfidbus) устанавливается из
дистрибутива [Менеджера Шины RFID](http://rfidcenter.ru/files/RfidBusManagerSetup.exe)

Для демонстрационных целей интеграционных возможностей Шины RFID развёрнут демо-сервер:
* Хост: demo.rfidbus.rfidcenter.ru
* Порт: 80
* Логин: demo
* Пароль: demo



<a name="Connect"></a>
Подключение к Шине RFID
-----------------------

За управление подключением и передачей команд Шине RFID отвечает COM-объект AddIn.RfidBus1cClient.
При создании экземпляра класса необходимо передать параметры подключения: хост, порт, логин и пароль.
Для регистрации внешних событий необходимо подключить внешнюю компоненту.

```bsl
Var rfidbus;
AttachAddIn("AddIn.RfidBus1cClient");
rfidbus = New COMObject("AddIn.RfidBus1cClient");
rfidbus.AutoReconnect = True;
If rfidbus.Connect(rfidbusHost, rfidbusPort, rfidbusLogin, rfidbusPassword) Then
	Message("Соединение установлено");
EndIf
```

Свойство AllowReconnect включает встроенный механизм восстановления связи с Шиной RFID,
с последующей автоматической авторизацией по токену. При авторизации по токену происходит
подключения к ранее созданной сессии пользователя на Шине RFID, поэтому все подписки на
события и состояния устройств сохраняются.

<a name="GetReaders"></a>
Получение списка считывателей
-----------------------
Получить список доступных в Шине RFID считывателей можно через запрос загруженных считывателей GetReaders.

```bsl
rfidbus = New COMObject("AddIn.RfidBus1cClient");
...
readers = rfidbus.GetReaders();
For each reader In readers Do
	Message("Name: " + reader.Name + " GUID: " + reader.Id);
EndDo;
```

Пример на GitHub: [Readers.ObserveTags](Readers.ObserveTags)


<a name="GetPrinters"></a>
Получение списка принтеров
-----------------------

Получить список доступных в Шине RFID принтеров можно через запрос загруженных принтеров GetPrinters.

```bsl
rfidbus = New COMObject("AddIn.RfidBus1cClient");
...
For each printer In printers Do
	Message("Name: " + printer.Name + " GUID: " + printer.Id);
EndDo;
```

Пример на GitHub: [Printers](Printers)


