Работа с Mobile Thin Client (MTCL)
==================================

MTCL для Шины RFID реализует механизмы для построения тонких клиентов для мобильных RFID
считывателей используя команды и события Шины RFID.

Создание графического интерфейса (GUI) мобильного ТСД реализовано через отправку команды
содержащей описание элементов управления в формате XML. Интерактивность графического
интерфейса достигается через события пользовательского взаимодействия и запросов состояния
именованных элементов управления.

Получение размеров экрана
=========================

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
$reader = $rfidbus.GetReaders()[0]
...
$paramValues = New-Object -ComObject RfidBus.ParametersValues
$workareaSize = $rfidbus.ExecuteSpecialCommand($reader.Id, "GetWorkareaSize", $paramValues)

$guiWidth = $workareaSize.GetValue("Width");
$guiHeight = $workareaSize.GetValue("Height");
```

Отправка интерфейса
===================

Для отправки интерфейса на мобильный RFID считыватель необходимо выполнить специальную
команду `SetWindowControls`, параметр `UiXml` которой должен содержать XML описание
окна. При взаимодействии с элементами интерфейса имеющими идентификатор (Id) и помеченными
как активные (RaiseEvents = True) будет полождаться внешнее событие на Шине RFID.

```powershell
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
```

Взаимодействия с GUI
====================

Обработка событий GUI мобильного считывателя осуществляется через события. Для этого нужно
подписаться на соотвестствующие события Шины RFID. Например, TableSelectedChanged:

```powershell
$rfidbus.SubscribeToSpecialEvent($reader.Id, "TableSelectedChanged");
```

Подобная модель позволяет обработать взаимодействия с таблицами, кнопками и иными элементами, GUI,
обновить значения отдельных графических элементов, или полностью весь интерфейс.

```powershell
Register-ObjectEvent -InputObject $rfidbus -EventName "ReaderSpecialEvent" –SourceIdentifier "ReaderSpecialEvent" -Action {

    $readerId   = $Args[0];
    $eventName  = $Args[1];
    $parameters = $Args[2];

    Write-Host("Event: $eventName; Reader: $readerId; Object Id: $($parameters.GetValue('Id')); Value: $($parameters.GetValue('Value'))")
}
```

Растовые интерфейсы
===================

В случаях, когда возможностей элементной базы недостаточно, возможна реализация интерфейсов на
основе изображений.

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
$reader = $rfidbus.GetReaders()[0]
...
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

Register-ObjectEvent -InputObject $rfidbus -EventName "ReaderSpecialEvent" –SourceIdentifier "ReaderSpecialEvent" -Action {

    $readerId   = $Args[0];
    $eventName  = $Args[1];
    $parameters = $Args[2];

    [int]$x =  $parameters.GetValue('X')
    [int]$y = $parameters.GetValue('Y')

    Write-Host "Event: $eventName; Reader: $readerId; Object Id: $($parameters.GetValue('Id')); X: $x; Y: $y"

    if ($x -gt 7 -and $x -lt 64 -and $y -gt 7 -and $y -lt 64)
    {
		Write-Host "Raster button 1 was pressed";
	}

    if ($x -gt 87 -and $x -lt 144 -and $y -gt 87 -and $y -lt 144)
    {
		Write-Host "Raster button 2 was pressed";
    }
}
```
Пример на GitHub: [Readers.MobileReadersRasterInterface](../Readers.MobileReadersRasterInterface)

[⬅ К оглавлению](../README.md)
