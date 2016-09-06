Чтение данных из банков памяти метки
====================================

Чтение данных из банков памяти транспондера осуществляется через запрос ReadMultipleBlocks.

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
  ...
$readers = $rfidbus.GetReaders()
foreach ($reader in $readers) 
{
   if ($reader.Mode -ne "StandBy")
   {
        throw "It is impossible to read data when reader $($reader.Name) mode set as '$($reader.Mode)'. It needs 'StandBy' mode."
   }
    
   $transponders = $rfidbus.GetTransponders($reader.Id);
   Write-Host "    Reader $($reader.Name) found $($transponders.Count) transponder(s). Reader ID: $($reader.Id)" 
   foreach ($transponder in $transponders)
   {
        Write-Host "    Transponder ID: $($transponder.Id)"
        $epcBank = [RfidBus.Com.Primitives.Transponders.TransponderBank]::Epc
        $userBank = [RfidBus.Com.Primitives.Transponders.TransponderBank]::UserMemory

        $accessPassword = [Byte[]](0,0,0,0)
        $bankAddress = 2
        $count = 6

        $readResult = $rfidbus.ReadMultipleBlocks($reader.Id, $transponder, $epcBank, $bankAddress, $count, $accessPassword)
        Write-Host "        EPC: $($readResult)"

        $bankAddress = 0
        $count = 4

        $readResult = $rfidbus.ReadMultipleBlocks($reader.Id, $transponder, $userBank, $bankAddress, $count, $accessPassword)
        Write-Host "        Reserved: $($readResult)"       
   }
}

```

[⬅ К оглавлению](../README.md)
