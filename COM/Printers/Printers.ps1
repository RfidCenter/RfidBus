cls
Write-Host "Printers example"

$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient

$connectResult = $rfidbus.Connect("demo.rfidbus.rfidcenter.ru", 80, "demo", "demo")

if ($connectResult -ne "True")
{
    throw "Invalid login/password."   
}

$printers = $rfidbus.GetPrinters()

$printerNumber = 0;
foreach ($printer in $printers)
{
    Write-Host "$($printerNumber). Name: $($printer.Name); model: $($printer.ModelDescription.Name); id: $($printer.Id)"
    $printerNumber++
}

$printerNumber = Read-Host 'Enter printer number'

Write-Host "Printing on selected printer: $($printers[$printerNumber].Name); GUID: $($printers[$printerNumber].Id)"

$selectedPrinter = $printers[$printerNumber]


$label = New-Object -ComObject RfidBus.PrintLabel
$label.Width = 50
$label.Height = 50


$textElement = New-Object -ComObject RfidBus.TextElement
$textElement.Text =  "Write custom data"
$textElement.X = 5
$textElement.Y = 20
$textElement.Width = 45
$textElement.Height = 25
$textElement.FontFamily = "Arial"
$textElement.FontSize = 4
$label.Elements.Add($textElement)


$lineElement = New-Object -ComObject RfidBus.LineElement
$lineElement.X1 = 1
$lineElement.Y1 = 18
$lineElement.X2 = 50
$lineElement.Y2 = 18
$label.Elements.Add($lineElement)


$barcodeElement = New-Object -ComObject RfidBus.Code128BarCodeElement
$barcodeElement.X = 5
$barcodeElement.Y = 35
$barcodeElement.Width = 40
$barcodeElement.Height = 25
$barcodeElement.CodeFontSize = 2
$barcodeElement.ShowCodeText = $true
$barcodeElement.Value = "4610002321000"
$label.Elements.Add($barcodeElement)


[byte[]]$image = Get-Content $PSScriptRoot\Resources\Logo.bmp -Encoding byte

$imageElement = New-Object -ComObject RfidBus.ImageElement
$imageElement.X = 1
$imageElement.Y = 1
$imageElement.Height = 16
$imageElement.Width = 16
$imageElement.ImageData = $image
$label.Elements.Add($imageElement)


$setAccessPasswordElement = New-Object -ComObject RfidBus.SetAccessPasswordElement
$setAccessPasswordElement.NewAccessPassword = [Byte[]](255, 255, 255, 255)
$label.Elements.Add($setAccessPasswordElement)


$setKillPasswordElement = New-Object -ComObject RfidBus.SetKillPasswordElement
$setKillPasswordElement.KillPassword = [Byte[]](255, 255, 255, 255)
$label.Elements.Add($setKillPasswordElement)


$lockTransponderElement = New-Object -ComObject RfidBus.LockTransponderElement
$lockTransponderElement.LockType = [RfidBus.Com.Primitives.Transponders.TransponderBankLockType]::Locked
$lockTransponderElement.AccessPassword = [Byte[]](255, 255, 255, 255)
$label.Elements.Add($lockTransponderElement)



$dataElement = New-Object -ComObject RfidBus.WriteMultipleBlocksLabelElement
$dataElement.Data = [Byte[]](255, 255, 255, 255, 255, 255)
$dataElement.StartingBlock = 2 # 0 -- R/O CRC; 1 -- PC; 2+ -- EPC
$dataElement.WriteCount = $dataElement.Data.Length
$dataElement.MemoryBank = [RfidBus.Com.Primitives.Transponders.TransponderBank]::Epc
$label.Elements.Add($dataElement)


$printerTaskId = $rfidbus.EnqueuePrintLabelTask($selectedPrinter, $label)
Write-Host "Print task id: $printerTaskId"


Write-Host "Print SGTIN-96 label"
$label = New-Object -ComObject RfidBus.PrintLabel
$label.Width = 50
$label.Height = 50

$textElement = New-Object -ComObject RfidBus.TextElement
$textElement.Text =  "SGTIN-96 data"
$textElement.X = 5
$textElement.Y = 20
$textElement.Width = 45
$textElement.Height = 25
$textElement.FontFamily = "Arial"
$textElement.FontSize = 4
$label.Elements.Add($textElement)

$dataElement = New-Object -ComObject RfidBus.WriteEpcSgtin96LabelElement
$dataElement.Gcp = 461000232
$dataElement.Item = 2
$dataElement.Serial = 3 
$dataElement.Filter = 0
$dataElement.Partition = 3
$label.Elements.Add($dataElement)

$printerTaskId = $rfidbus.EnqueuePrintLabelTask($selectedPrinter, $label)
Write-Host "Print task id: $printerTaskId"


Write-Host "Print SSCC-96 label"
$label = New-Object -ComObject RfidBus.PrintLabel
$label.Width = 50
$label.Height = 50

$textElement = New-Object -ComObject RfidBus.TextElement
$textElement.Text =  "SSCC-96 data"
$textElement.X = 5
$textElement.Y = 20
$textElement.Width = 45
$textElement.Height = 25
$textElement.FontFamily = "Arial"
$textElement.FontSize = 4
$label.Elements.Add($textElement)

$dataElement = New-Object -ComObject RfidBus.WriteEpcSscc96LabelElement
$dataElement.Gcp = 461000232
$dataElement.Extension = 2
$dataElement.Filter = 0
$dataElement.Partition = 3
$label.Elements.Add($dataElement)

$printerTaskId = $rfidbus.EnqueuePrintLabelTask($selectedPrinter, $label)
Write-Host "Print task id: $printerTaskId"



Write-Host "Print GIAI-96 label"
$label = New-Object -ComObject RfidBus.PrintLabel
$label.Width = 50
$label.Height = 50

$textElement = New-Object -ComObject RfidBus.TextElement
$textElement.Text =  "GIAI-96 data"
$textElement.X = 5
$textElement.Y = 20
$textElement.Width = 45
$textElement.Height = 25
$textElement.FontFamily = "Arial"
$textElement.FontSize = 4
$label.Elements.Add($textElement)

$dataElement = New-Object -ComObject RfidBus.WriteEpcGiai96LabelElement
$dataElement.Gcp = 461000232
$dataElement.Asset = 2
$dataElement.Filter = 0
$dataElement.Partition = 3
$label.Elements.Add($dataElement)

$printerTaskId = $rfidbus.EnqueuePrintLabelTask($selectedPrinter, $label)
Write-Host "Print task id: $printerTaskId"
