Инвентаризация
==============

Для инвентаризации (поиска меток, находящихся в зоне действия считывателя) необходимо
произвести подписку на базовые события считывателя и перевести его в режим чтения.
В обработчике событий появится возможность фиксировать нахождение (TransponderFoundEvent)
и потерю (TransponderLostEvent) меток, а также изменения GPI (ReaderGpiStatesChangedEvent).

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
 ...
Write-Host "Getting readers..."
$readers = $rfidbus.GetReaders()
foreach ($reader in $readers) 
{
   Write-Host "    $($reader.Name)" 
   $rfidbus.StartReading($reader)
   $rfidbus.SubscribeToReader($reader)
}

Register-ObjectEvent -InputObject $rfidbus -EventName "TransponderFoundEvent" –SourceIdentifier "TransponderFoundEvent" -Action{
    foreach($params in $Args)
    {
        if ($params.GetType().Name -eq "ReaderRecord")
        {
            Write-Host "Reader name: $($params.Name)"
        }
        if ($params.GetType().Name -eq "Transponder")
        {
            Write-Host "   Tranponder ID: $($params.IdAsString)"
            Write-Host "   Tranponder type: $($params.Type)"
            Write-Host "   Tranponder antenna: $($params.Antenna)"
        }
    }
}

Write-Host "Waiting events.. Timeout 10 sec."
Wait-Event TransponderFound -TimeOut 10

foreach ($reader in $readers)
{
    Write-Host "$($reader.Name): Stop reading" 
    $rfidbus.StopReading($reader)
}
Unregister-Event -SourceIdentifier "TransponderFoundEvent"
```

[⬅ К оглавлению](../README.md)
