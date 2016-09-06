Работа с принтером
==================

* [Получение списка принтеров](#GetPrinters)
* [Программирование банков памяти метки](#WriteMultipleBlocksLabelElement)
* [Программирование EPC форматами данных GS1/UNISCAN](#Gs1)
* [Управление доступом, блокировка метки](#Access)
* [Графическая персонализация](#Graphic)


<a name="GetPrinters"></a>
Получение списка принтеров
--------------------------
Получить список доступных в Шине RFID принтеров можно через запрос загруженных принтеров GetPrinters.

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
...
$printers = $rfidbus.GetPrinters()
foreach ($printer in $printers)
{
    Write-Host "$($printerNumber). Name: $($printer.Name); model: $($printer.ModelDescription.Name); id: $($printer.Id)"
}
```

<a name="WriteMultipleBlocksLabelElement"></a>
Программирование банков памяти метки
------------------------------------

Запись произвольных данных в банки памяти метки реализована через элемент WriteMultipleBlocksLabelElement.

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
...
$printers = $rfidbus.GetPrinters()
$selectedPrinter = $printers[0]

$label = New-Object -ComObject RfidBus.PrintLabel
$label.Width = 50
$label.Height = 50

$dataElement = New-Object -ComObject RfidBus.WriteMultipleBlocksLabelElement
$dataElement.Data = [Byte[]](255, 255, 255, 255, 255, 255)
$dataElement.StartingBlock = 1
$dataElement.WriteCount = $dataElement.Data.Length
$dataElement.MemoryBank = [RfidBus.Com.Primitives.Printers.Elements.PrintTagMemoryBankEnum]::Epc
$label.Elements.Add($dataElement)

$rfidbus.EnqueuePrintLabelTask($selectedPrinter, $label)
```

<a name="Gs1"></a>
Программирование EPC форматами данных GS1/UNISCAN
-------------------------------------------------

В Шине RFID реализованы запросы для программирования банка EPC специализированными форматами, такими как SGTIN, GIAI и др.
Пример записи средствами принтера данных  в память метки в формате GIAI-96:

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
...
$printers = $rfidbus.GetPrinters()
$selectedPrinter = $printers[0]
...
$label = New-Object -ComObject RfidBus.PrintLabel
$label.Width = 50
$label.Height = 50

$dataElement = New-Object -ComObject RfidBus.WriteEpcGiai96LabelElement
$dataElement.Gcp = 461000232
$dataElement.Asset = 2
$dataElement.Filter = 0
$dataElement.Partition = 3
$label.Elements.Add($dataElement)

$printerTaskId = $rfidbus.EnqueuePrintLabelTask($selectedPrinter, $label)
```

<a name="Access"></a>
Управление доступом, блокировка метки
--------------------------

За установку паролей доступа и уничтожения метки отвечают элементы SetAccessPasswordElement и SetKillPasswordElement соответственно.
Для блокировки определённых банков памяти меток используется элемент LockTransponderBankElement, а при необходимости заблокировать все банки памяти метки - LockTransponderElement.

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
...
$printers = $rfidbus.GetPrinters()
$selectedPrinter = $printers[0]

$label = New-Object -ComObject RfidBus.PrintLabel
$label.Width = 50
$label.Height = 50

$setAccessPasswordElement = New-Object -ComObject RfidBus.SetAccessPasswordElement
$setAccessPasswordElement.NewAccessPassword = [Byte[]](255, 255, 255, 255)
$label.Elements.Add($setAccessPasswordElement)

$setKillPasswordElement = New-Object -ComObject RfidBus.SetKillPasswordElement
$setKillPasswordElement.KillPassword = [Byte[]](255, 255, 255, 255)
$label.Elements.Add($setKillPasswordElement)

# Блокировка определённого банка памяти метки
$lockTransponderBankElement = New-Object -ComObject RfidBus.LockTransponderBankElement
$lockTransponderBankElement.LockType = [RfidBus.Com.Primitives.Transponders.TransponderBankLockType]::Locked
$lockTransponderBankElement.MemoryBank = [RfidBus.Com.Primitives.Transponders.TransponderBank]::Reserved
$lockTransponderBankElement.AccessPassword = [Byte[]](255, 255, 255, 255)
$label.Elements.Add($lockTransponderBankElement)

# Блокировка всей метки
$lockTransponderElement = New-Object -ComObject RfidBus.LockTransponderElement
$lockTransponderElement.LockType = [RfidBus.Com.Primitives.Transponders.TransponderBankLockType]::Locked
$lockTransponderElement.AccessPassword = [Byte[]](255, 255, 255, 255)
$label.Elements.Add($lockTransponderElement)

$rfidbus.EnqueuePrintLabelTask($selectedPrinter, $label)
```

<a name="Graphic"></a>
Графическая персонализация
--------------------------

При печати на метках имеется возможность добавлять такие элементы как текст, изображения, различные геометрические фигуры, штрихкоды.
Пример добавления текста на метку:

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
...
$printers = $rfidbus.GetPrinters()
$selectedPrinter = $printers[0]

$label = New-Object -ComObject RfidBus.PrintLabel
$label.Width = 50
$label.Height = 50

$textElement = New-Object -ComObject RfidBus.TextElement
$textElement.Text =  "Text example"
$textElement.X = 5
$textElement.Y = 20
$textElement.Width = 45
$textElement.Height = 25
$textElement.FontFamily = "Arial"
$textElement.FontSize = 4
$label.Elements.Add($textElement)

$rfidbus.EnqueuePrintLabelTask($selectedPrinter, $label)
```

Пример добавления изображения на метку:

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
...
$printers = $rfidbus.GetPrinters()
$selectedPrinter = $printers[0]

$label = New-Object -ComObject RfidBus.PrintLabel
$label.Width = 50
$label.Height = 50

[byte[]]$image = Get-Content $PSScriptRoot\Resources\Logo.bmp -Encoding byte

$imageElement = New-Object -ComObject RfidBus.ImageElement
$imageElement.X = 1
$imageElement.Y = 1
$imageElement.Height = 16
$imageElement.Width = 16
$imageElement.ImageData = $image
$label.Elements.Add($imageElement)

$rfidbus.EnqueuePrintLabelTask($selectedPrinter, $label)
```

В следующем примере рассматривается добавление на метку линии, с указанием начальных и конечных координат, толщины и цвета. Добавление других геометрических фигур производится аналогичным способом, различия заключаются только в параметрах фигур, например, для описания окружности (CircleElement) следует указать координаты центра и радиус.

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
...
$printers = $rfidbus.GetPrinters()
$selectedPrinter = $printers[0]

$label = New-Object -ComObject RfidBus.PrintLabel
$label.Width = 50
$label.Height = 50

$lineElement = New-Object -ComObject RfidBus.LineElement
$lineElement.X1 = 1
$lineElement.Y1 = 18
$lineElement.X2 = 50
$lineElement.Y2 = 18
$label.Elements.Add($lineElement)

$rfidbus.EnqueuePrintLabelTask($selectedPrinter, $label)
```

Пример добавления штрихкода на метку. В данном случае применён стандарт Code 128.

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
...
$printers = $rfidbus.GetPrinters()
$selectedPrinter = $printers[0]

$label = New-Object -ComObject RfidBus.PrintLabel
$label.Width = 50
$label.Height = 50

$barcodeElement = New-Object -ComObject RfidBus.Code128BarCodeElement
$barcodeElement.X = 5
$barcodeElement.Y = 35
$barcodeElement.Width = 40
$barcodeElement.Height = 25
$barcodeElement.CodeFontSize = 2
$barcodeElement.ShowCodeText = $true
$barcodeElement.Value = "4610002321000"
$label.Elements.Add($barcodeElement)

$rfidbus.EnqueuePrintLabelTask($selectedPrinter, $label)
```

[⬅ К оглавлению](../README.md)
