Работа с Шиной RFID через COM объекты. Реализация на скриптах PowerShell.
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


<a name="GetStarted"></a>Начало работы
-------------

COM/ActiveX компонент для работы с [Шиной RFID](http://rfidcenter.ru/product/rfidbus)
устанавливается из дистрибутива [Менеджера Шины RFID](http://rfidcenter.ru/files/RfidBusManagerSetup.exe)


Для демонстрационных целей интеграционных возможностей Шины RFID развёрнут демо-сервер:
* Хост: demo.rfidbus.rfidcenter.ru
* Порт: 80
* Логин: demo
* Пароль: demo


<a name="Connect"></a>
Подключение к Шине RFID
-----------------------

За управление подключением и передачей команд Шине RFID отвечает COM-объект
RfidBus.RfidBusComClient. При создании экземпляра класса необходимо передать параметры
подключения: хост, порт, логин и пароль.

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
$rfidbus.AllowReconnect = True;

$connectResult = $rfidbus.Connect("demo.rfidbus.rfidcenter.ru", 80, "demo", "demo")

if ($connectResult -ne "True")
{
    throw "Invalid login/password."   
}
Write-Host "Connect was successfuly esteblished."
```

Свойство AllowReconnect включает встроенный механизм восстановления связи с Шиной RFID,
с последующей автоматической авторизацией по токену. При авторизации по токену происходит
подключения к ранее созданной сессии пользователя на Шине RFID, поэтому все подписки на
события и состояния устройств сохраняются.

<a name="GetReaders"></a>
Получение списка считывателей
-----------------------
Получить список доступных в Шине RFID считывателей можно через запрос загруженных считывателей GetReaders.

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
 ...
Write-Host "Getting readers..."
$readers = $rfidbus.GetReaders()
foreach ($reader in $readers) 
{
   Write-Host "  $($reader.Name)" 
}
```

Пример на GitHub: [Readers.ObserveTags](Readers.ObserveTags)


<a name="GetPrinters"></a>
Получение списка принтеров
-----------------------

Получить список доступных в Шине RFID принтеров можно через запрос загруженных принтеров GetPrinters.

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
...
$printers = $rfidbus.GetPrinters()

foreach ($printer in $printers)
{
    Write-Host "Name: $($printer.Name); ID: $($printer.Id)"
}
```

Пример на GitHub: [Printers](Printers)


