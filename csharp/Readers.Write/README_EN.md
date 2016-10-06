[![на русском](http://rfidcenter.ru/img/flag-ru.svg) РУССКАЯ ВЕРСИЯ](README.md)

* [Data recording into the tag's memory banks](#WriteMultipleBlocks)
* [EPC memory bank encoding by GS1/UNISCAN data format](#WriteEpc)

<a name="WriteMultipleBlocks"></a>Data recording into the tag's memory banks
====================================

The WriteMultipleBlocks request is used for the data recording into the tag's memory banks.

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

<a name="WriteEpc"></a>EPC memory bank encoding by GS1/UNISCAN data format
=================================================

Such EPC special formats as SGTIN, GIAI etc. were implemented in RFID Bus to programming the EPC memory bank.
GIAI-96 format data recording:

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

[⬅ Back to content](../README_EN.md)

