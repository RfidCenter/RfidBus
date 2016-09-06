cls
Write-Host "Observe tags example"
Write-Host ""
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient

$connectResult = $rfidbus.Connect("demo.rfidbus.rfidcenter.ru", 80, "demo", "demo")

if ($connectResult -ne "True")
{
    throw "Invalid login/password."   
}

Write-Host "Connection esteblished."

Write-Host "Getting readers..."
$readers = $rfidbus.GetReaders()
foreach ($reader in $readers) 
{
   Write-Host "    $($reader.Name)" 
   $rfidbus.StartReading($reader)
   $rfidbus.SubscribeToReader($reader)
}

Register-ObjectEvent -InputObject $rfidbus -EventName "TransponderFoundEvent" –SourceIdentifier "TransponderFoundEvent" -Action {
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