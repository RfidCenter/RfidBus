* [Программирование банков памяти метки](#WriteMultipleBlocks)
* [Программирование EPC форматами данных GS1/UNISCAN](#WriteEpc)

<a name="WriteMultipleBlocks"></a>Программирование банков памяти метки
====================================
Запись данных в банки памяти транспондера осуществляется через запрос WriteMultipleBlocks.

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
 ...
$readers = $rfidbus.GetReaders()
foreach ($reader in $readers) 
{
   if ($reader.Mode -eq "StandBy")
   {            
       $transponders = $rfidbus.GetTransponders($reader.Id);
       Write-Host "    Reader $($reader.Name) found $($transponders.Count) transponder(s). Reader ID: $($reader.Id)"
          foreach ($transponder in $transponders)
       {
            Write-Host "    Transponder ID: $($transponder.Id)"
            $epcBank = [RfidBus.Com.Primitives.Transponders.TransponderBank]::Epc
            $data = [Byte[]](255, 255, 255, 255)
            $bankAddress = 2
            $accessPassword = [Byte[]](0,0,0,0)
            
            $rfidbus.WriteMultipleBlocks($reader.Id, $transponder, $epcBank, $data, $bankAddress, $accessPassword)
            Write-Host "        Data: '$($data)' was written to EPC"
       }
   }
}
```

<a name="WriteEpc"></a>Программирование EPC форматами данных GS1/UNISCAN
=================================================
В Шине RFID реализованы запросы для программирования банка EPC специализированными форматами, такими как SGTIN, GIAI и др.
Запись в память метки SGTIN-96

```cs
RfidBusClient client;
 ...
const ulong GCP = 461000232;
const uint ITEM = 10;
const ulong SERIAL = 10;
Transponder transponder;
ReaderRecord readerRecord;
  ...
var result = client.SendRequest(new WriteEpcSgtin96(readerRecord.Id,
        transponder,
        GCP,
        ITEM,
        SERIAL));
if (result.Status != ResponseStatus.Ok)
{
    throw new Exception($"WriteEpcSgtin96 error: {result.Details}");
}
```

[⬅ К оглавлению](../README.md)


