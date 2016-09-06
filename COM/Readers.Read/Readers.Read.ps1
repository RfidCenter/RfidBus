cls
Write-Host "Read data example"
Write-Host ""

function Get-HexString 
{
    $([String]::Join(" ", ($args[0] | % { "{0:X2}" -f $_})))
}

$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient

$connectResult = $rfidbus.Connect("demo.rfidbus.rfidcenter.ru", 80, "demo", "demo")

if ($connectResult -ne "True")
{
    throw "Can't connect to RFID Bus."
}

Write-Host "Connection esteblished."

$tidBank = [RfidBus.Com.Primitives.Transponders.TransponderBank]::Tid
$epcBank = [RfidBus.Com.Primitives.Transponders.TransponderBank]::Epc
$userBank = [RfidBus.Com.Primitives.Transponders.TransponderBank]::UserMemory

$accessPassword = [Byte[]](0,0,0,0)

$readers = $rfidbus.GetReaders()
foreach ($reader in $readers) 
{
   if ($reader.Mode -ne "StandBy")
   {
		continue       
   }
    
   $transponders = $rfidbus.GetTransponders($reader.Id);
   Write-Host "`n Reader $($reader.Name) found $($transponders.Count) transponder(s). Reader ID: $($reader.Id)" 
   foreach ($transponder in $transponders)
   {
        Write-Host "`t Transponder ID: $(Get-HexString $transponder.Id)"
       
        $bankAddress = 0
        $count = 8
        $readResult = $rfidbus.ReadMultipleBlocks($reader.Id, $transponder, $epcBank, $bankAddress, $count, $accessPassword)
        Write-Host "`t`t EPC: $(Get-HexString $readResult)"

        $bankAddress = 0
        $count = 4
        $readResult = $rfidbus.ReadMultipleBlocks($reader.Id, $transponder, $tidBank, $bankAddress, $count, $accessPassword)
        Write-Host "`t`t TID: $(Get-HexString $readResult)"

        $bankAddress = 0
        $count = 4
        $readResult = $rfidbus.ReadMultipleBlocks($reader.Id, $transponder, $userBank, $bankAddress, $count, $accessPassword)
        Write-Host "`t`t Reserved: $(Get-HexString $readResult)"

   }
}

