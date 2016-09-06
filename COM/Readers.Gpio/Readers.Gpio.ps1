cls
Write-Host "GPIO example"
Write-Host ""
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient

$connectResult = $rfidbus.Connect("demo.rfidbus.rfidcenter.ru", 80, "demo", "demo")

if ($connectResult -ne "True")
{
    throw "Invalid login/password."   
}

Write-Host "Connection esteblished."

Register-ObjectEvent -InputObject $rfidbus -EventName "ReaderGpiStatesChangedEvent" –SourceIdentifier "ReaderGpiStatesChangedEvent" -Action {

    $reader = $Args[0];
    $port   = $Args[1];
    $state  = $Args[2];

    Write-Host "ReaderGpiStatesChangedEvent: Reader: $($reader.Id); Port: $port; State: $state"
}

$readers = $rfidbus.GetReaders()
foreach ($reader in $readers) 
{
    $rfidbus.SubscribeToReader($reader.Id)

    if ($reader.IsOpen -eq $true)
    {            
        [RfidBus.Com.Primitives.Readers.ReaderGpoStateRecord[]]$gpoStateRecords = @()

        for ($i=1; $i -le 4; $i++)
        {
            $newStateValue = $true
            $gpoStateRecord = New-Object -ComObject RfidBus.ReaderGpoStateRecord
            $gpoStateRecord.Port = $i
            $gpoStateRecord.State = $newStateValue
            $gpoStateRecords += $gpoStateRecord
            Write-Host "        Port $($i) was set to $($newStateValue)"
        }

        $rfidbus.SetGpoStates($reader.Id, $gpoStateRecords)


        for ($i=1; $i -le 4; $i++)
        {
            $newStateValue = $false
            $gpoStateRecord = New-Object -ComObject RfidBus.ReaderGpoStateRecord
            $gpoStateRecord.Port = $i
            $gpoStateRecord.State = $newStateValue
            $gpoStateRecords += $gpoStateRecord
            Write-Host "        Port $($i) was set to $($newStateValue)"
        }

        $rfidbus.SetGpoStates($reader.Id, $gpoStateRecords)


        Write-Host "      Read GPI states of reader $($reader.Name)"
            
        $gpiStatesRecords = $rfidbus.GetGpiStates($reader.Id)
           
        foreach ($gpiStatesRecord in $gpiStatesRecords)
        {
            Write-Host "        Port: $($gpiStatesRecord.Port) state: $($gpiStatesRecord.State)"
        }                 
    }
}

Unregister-Event -SourceIdentifier "ReaderGpiStatesChangedEvent"
