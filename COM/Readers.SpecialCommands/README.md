Работа со спец. командами
=========================

Некоторое оборудование поддерживает расширенные функции, работа с которыми в Шине RFID
реализована через специализированные команды. Например, управление звуком,
светодиодами, счётчиком людей, графическими интерфейсами и пр.

Получение списка поддерживаемых считывателем специальных команд реализовано через запрос
GetSpecialCommands.

В примере продемонстрировано получение списка специализированных команд, с последующей
отправкой спец. команды ReinitializeTransponders (повторной инициализации меток)
эмулятору через запрос ExecuteSpecialCommand.

```cs
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
...
$readers = $rfidbus.GetReaders()
foreach ($reader in $readers) 
{
    Write-Host "    Reader: $($reader.Name)"

    $specialCommands = $rfidbus.GetSpecialCommands($reader.Id)
    foreach ($specialCommand in $specialCommands)
    {
        Write-Host "        Special command: '$($specialCommand.Name)'; description: $($specialCommand.Description)"
    
        $reinitCommand = "ReinitializeTransponders"

        if ($specialCommand.Name -eq $reinitCommand -And $reader.IsOpen)
        {
            Write-Host "        Executing '$($reinitCommand)' command..."
            $rfidbus.ExecuteSpecialCommand($reader.Id, $reinitCommand, $null)
        }
    }
}
```

Подписка на на спец. события осуществляется через запрос ReaderSpecialEvent. Ниже,
как и в предыдущем примере, производится перебор всех подключенных считывателей, и
у каждого из них запрашивается список специальных событий. Далее  на каждое из них
производится подписка.

```cs
RfidBusClient client;
...
$specialEvents = $rfidbus.GetSpecialEvents($reader.Id)
foreach ($specialEvent in $specialEvents)
{
    Write-Host "        Special event: '$($specialEvent.Name)'; description: $($specialEvent.Description)"
    $rfidbus.SubscribeToSpecialEvent($reader.Id, $specialEvent.Name)
    Write-Host "        Subscribe OK"
}
...
Register-ObjectEvent -InputObject $rfidbus -EventName "ReaderSpecialEvent" –SourceIdentifier "ReaderSpecialEvent" -Action{
     foreach($parameterValue in $Args)
     {
        Write-Host "$($parameterValue)"
     }
}
```

Отмена подписки реализована через запрос UnsubscribeFromSpecialEvent.

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
 ...
Unregister-Event -SourceIdentifier "ReaderSpecialEvent"

foreach ($reader in $readers) 
{
    $specialCommands = $rfidbus.GetSpecialCommands($reader.Id)
    foreach ($specialCommand in $specialCommands)
    {
        Write-Host "Unsubscribe from special command $($specialCommand.Name); Reader $($reader.Name);"
        $rfidbus.UnsubscribeFromSpecialEvent($reader.Id, $specialCommand.Name)
    }
}
```

[⬅ К оглавлению](../README.md)
