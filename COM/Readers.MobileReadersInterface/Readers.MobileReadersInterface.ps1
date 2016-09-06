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

        
        $rfidbus.SubscribeToSpecialEvent($reader.Id, "TriggerPressed");
		$rfidbus.SubscribeToSpecialEvent($reader.Id, "TableSelectedChanged");

        $paramValues = New-Object -ComObject RfidBus.ParametersValues
        $workareaSize = $rfidbus.ExecuteSpecialCommand($reader.Id, "GetWorkareaSize", $paramValues)

        $guiWidth = $workareaSize.GetValue("Width");
        $guiHeight = $workareaSize.GetValue("Height");

        Write-Host "$($reader.Id) ($($reader.Name)) GUI support. Sending GUI: Width $guiWidth; Lenght $guiHeight"

        $mtclGui = @"
<Window>
	<Label	Left='5' Top='5' Width='$($guiWidth - 10)' Height='40'
			Text=' Hello world!' BackColor='#000000' ForeColor='#ffffff'>
		<Font Family='Arial' Size='22' Style='Bold' />
	</Label>
 	<Table	Id='TidList'
			Left='5' Top='50' 
			Width='$($guiWidth - 10)' Height='$($guiHeight - 100)'  
			RaiseEvents='True'>
		<Columns>
			<Column Name='Tid' Header='TID' />
		</Columns>
		<Rows>
			<Row Id='1'>
				<Field Column='Tid' Value='Row 1' />
			</Row>
			<Row Id='2'>
				<Field Column='Tid' Value='Row 2' />
			</Row>
			<Row Id='3'>
				<Field Column='Tid' Value='Row 3' />
			</Row>
		</Rows>
	</Table>
 	<Button Id='CloseButton' Text='Tap To Close Program' StandartCommand='CloseApplication'
			Left='5' Top='$($guiHeight - 40)'  
			Width='$($guiWidth - 10)' Height='20' 
			/>
</Window>
"@

		$prmsValues = New-Object -ComObject RfidBus.ParametersValues
		$prmsValues.SetValue("UiXml", $mtclGui);
		
		$rfidbus.ExecuteSpecialCommand($reader, "SetWindowControls", $prmsValues);
						
    }
}

Register-ObjectEvent -InputObject $rfidbus -EventName "ReaderSpecialEvent" –SourceIdentifier "ReaderSpecialEvent" -Action {

    $reader   = $Args[0];
    $eventName  = $Args[1];
    $parameters = $Args[2];

    Write-Host("Event: $eventName; Reader: $($reader.Id); Object Id: $($parameters.GetValue('Id')); Value: $($parameters.GetValue('Value'))")
}

Write-Host "Waiting events.. Timeout 10 sec."
Wait-Event ReaderSpecialEvent -TimeOut 10

Unregister-Event -SourceIdentifier "ReaderSpecialEvent"
$rfidbus.Close()