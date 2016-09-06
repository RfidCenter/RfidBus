cls
Write-Host "Mobile readers interface example"

$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient

$connectResult = $rfidbus.Connect("127.1", 7266, "admin", "admin")

if ($connectResult -ne "True")
{
    throw "Invalid login/password."   
}

$readers = $rfidbus.GetReaders()
foreach ($reader in $readers) 
{
    if (!$reader.IsOpen) { continue }

    $specialCommands = $rfidbus.GetSpecialCommands($reader.Id);
    foreach($specialCommand in $specialCommands) 
    {
        if ($specialCommand.Name -ne "GetWorkareaSize") { continue; }

        
        $rfidbus.SubscribeToSpecialEvent($reader.Id, "ImageMouseDown");

        $paramValues = New-Object -ComObject RfidBus.ParametersValues
        $workareaSize = $rfidbus.ExecuteSpecialCommand($reader.Id, "GetWorkareaSize", $paramValues)

        $guiWidth = $workareaSize.GetValue("Width");
        $guiHeight = $workareaSize.GetValue("Height");

        Write-Host "$($reader.Id) ($($reader.Name)) GUI support. Sending GUI: Width $guiWidth; Lenght $guiHeight"

		$mtclGui = @"
<Window>
	<Image Id='ImageGui' Height='150' Width='150' 
			Picture='iVBORw0KGgoAAAANSUhEUgAAAJYAAACWAQMAAAAGz+OhAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAGUExURQAAAP///6XZn90AAAAJcEhZcwAADsMAAA7DAcdvqGQAAAE1SURBVEjH7dUxisMwEAXQES5c6gIBX2RZX2uLQHSQHEZH0RFUuhD2SpoUmZkPFptmCXGXh4Ol8f8yHeYq72Q7WUvkjQVy2nYi0rZVW5XlaouyVG1WFqpNwJwystaWIhZTrXS7ndrWbQW2CMuDVrd2oecJsl2BFW2RXBGDYbtba8/4v4Zn4Oq955a7/W326F2O5gBlCGUNGspuBBlHXUCd2eXyuINkOwi7WsRf3+u8+tjzFVZjW8+htNTDJC300AlrofPKCodY2MYFEJa4PNZu0iKwwKUV9ij3mXFpPbBZWBk0PkCmEdu1tYOrDUba1D+d6j5vjeYXzNvn0gLsG9iXtrq3H7DfbM0lbaX+jmbOY4be5WgOYIbiYP5QdjPIOOoC6kxbzKItgg6irh79+/A5616yo/wCUck9kPmYmcQAAAAASUVORK5CYII='
			RaiseEvents='True'
			/>
 </Window>
"@

		$prmsValues = New-Object -ComObject RfidBus.ParametersValues
		$prmsValues.SetValue("UiXml", $mtclGui);
		
		$rfidbus.ExecuteSpecialCommand($reader, "SetWindowControls", $prmsValues);
						
    }
}

Register-ObjectEvent -InputObject $rfidbus -EventName "ReaderSpecialEvent" –SourceIdentifier "ReaderSpecialEvent" -Action {

    $reader     = $Args[0];
    $eventName  = $Args[1];
    $parameters = $Args[2];

    [int]$x =  $parameters.GetValue('X')
    [int]$y = $parameters.GetValue('Y')

    Write-Host "Event: $eventName; Reader: $($reader.Id); Object Id: $($parameters.GetValue('Id')); X: $x; Y: $y"

    if ($x -gt 7 -and $x -lt 64 -and $y -gt 7 -and $y -lt 64)
    {
		Write-Host "Raster button 1 was pressed";
	}
		
    if ($x -gt 87 -and $x -lt 144 -and $y -gt 87 -and $y -lt 144) 
    {
		Write-Host "Raster button 2 was pressed";
    }
}

Write-Host "Waiting events.. Timeout 10 sec."
Wait-Event ReaderSpecialEvent -TimeOut 10

Unregister-Event -SourceIdentifier "ReaderSpecialEvent"
$rfidbus.Close()