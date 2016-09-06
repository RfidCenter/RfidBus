cls
Write-Host "Special commands example"
Write-Host ""
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient

$connectResult = $rfidbus.Connect("demo.rfidbus.rfidcenter.ru", 80, "demo", "demo")

if ($connectResult -ne "True")
{
    throw "Invalid login/password."   
}

Write-Host "Connection esteblished."

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

    $specialEvents = $rfidbus.GetSpecialEvents($reader.Id)
    foreach ($specialEvent in $specialEvents)
    {
        Write-Host "        Special event: '$($specialEvent.Name)'; description: $($specialEvent.Description)"
        $rfidbus.SubscribeToSpecialEvent($reader.Id, $specialEvent.Name)
        Write-Host "        Subscribe OK"
    }
}

Register-ObjectEvent -InputObject $rfidbus -EventName "ReaderSpecialEvent" –SourceIdentifier "ReaderSpecialEvent" -Action{
     foreach($params in $Args)
     {
        Write-Host "$($params)"
     }
}

Write-Host "Waiting events.. Timeout 10 sec."
Wait-Event ReaderSpecialEvent -TimeOut 10

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