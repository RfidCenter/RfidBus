Работа с GPIO
=============

* [Управление исполнительными механизмами](#SetReaderGpoStates)
* [Запрос состояния исполнительных механизмов](#GetReaderGpiStates)
* [События от исполнительных механизмов](#ReaderGpiStatesChangedEvent)


<a name="SetReaderGpoStates"></a>Управление исполнительными механизмами
--------------------------------------
Управление состоянием GPO реализовано запросом SetGpoStates, в котором передаётся массив с состояниями портов.

```powershell
$readers = $rfidbus.GetReaders()
foreach ($reader in $readers) 
{
    if ($reader.IsOpen -eq $true)
    {            
        [RfidBus.Com.Primitives.Readers.ReaderGpoStateRecord[]]$gpoStateRecords = @()

        for($i=1
                $i -le 4
                $i++)
        {
            $newStateValue = $true
            $gpoStateRecord = New-Object -ComObject RfidBus.ReaderGpoStateRecord
            $gpoStateRecord.Port = $i
            $gpoStateRecord.State = $newStateValue
            $gpoStateRecords += $gpoStateRecord
            Write-Host "        Port $($i) was set to $($newStateValue)"
        }
        $rfidbus.SetGpoStates($reader.Id, $gpoStateRecords)               
    }
}
```

<a name="GetReaderGpiStates"></name>Запрос состояния исполнительных механизмов
------------------------------------------

Получение текущего состояния GPI реализовано в запросе GetGpiStates.

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
...
$readers = $rfidbus.GetReaders()
foreach ($reader in $readers) 
{
    if ($reader.IsOpen -eq $true)
    {            
        Write-Host "Read GPI states of reader $($reader.Name)"           
        $gpiStatesRecords = $rfidbus.GetGpiStates($reader.Id)
            
        foreach ($gpiStatesRecord in $gpiStatesRecords)
        {
            Write-Host "    Port: $($gpiStatesRecord.Port) state: $($gpiStatesRecord.State)"
        }                 
    }
}
```

<a name="ReaderGpiStatesChangedEvent"></a>События от исполнительных механизмов
------------------------------------

Для получения событий изменения GPI необходимо произвести подписку на базовые события
считывателя, после чего в обработчике событий появится возможность фиксировать изменения
GPI (ReaderGpiStatesChangedEvent).

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
...
$readers = $rfidbus.GetReaders()
$reader = $readers[0]
$rfidbus.SubscribeToReader($reader.Id)
...
Register-ObjectEvent -InputObject $rfidbus -EventName "ReaderGpiStatesChangedEvent" –SourceIdentifier "ReaderGpiStatesChangedEvent" -Action {

    $readerId = $Args[0];
    $port     = $Args[1];
    $state    = $Args[2];

    Write-Host "ReaderGpiStatesChangedEvent: Reader: $readerId; Port: $port; State: $state"
}
```

В эмуляторе состояния портов GPI соответствуют состоянию портов GPO. Таким образом, чтобы
получить ReaderGpiStatesChangedEvent на эмуляторе необходимо изменить состояние его GPO.

[⬅ К оглавлению](../README.md)
