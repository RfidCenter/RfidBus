* [Программирование банков памяти метки](#WriteMultipleBlocks)
* [Программирование EPC форматами данных GS1/UNISCAN](#WriteEpc)

<a name="WriteMultipleBlocks"></a>Программирование банков памяти метки
====================================
Запись данных в банки памяти транспондера осуществляется через запрос WriteMultipleBlocks.

```cs
RfidBusClient client;
 ...
ReaderRecord readerRecord;
const ushort BANK_ADDRESS = 2;
Transponder transponder;
byte[] data = { 255, 255, 255, 255 };
byte[] accessPassword = {};
 ...
var result = client.SendRequest(new WriteMultipleBlocks(readerRecord.Id,
        transponder,
        TransponderBank.Epc,
        data,
        BANK_ADDRESS,
        accessPassword));
if (result.Status != ResponseStatus.Ok)
{
    throw new Exception($"WriteMultipleBlocks error:{result.Details}");
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


