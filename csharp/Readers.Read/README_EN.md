[![на русском](http://rfidcenter.ru/img/flag-ru.svg) РУССКАЯ ВЕРСИЯ](README.md)

Tag Memory Reading
====================================

ReadMultipleBlocks request is used to read tag memory banks of the transponder.

```cs
RfidBusClient client;
  ...
ReaderRecord readerRecord;
Transponder transponder
ushort bankAddress = 2;
int blocksCount = 2;
byte[] accessPassword = {};
  ...
ReadMultipleBlocksResponse result = client.SendRequest(
        new ReadMultipleBlocks(readerRecord.Id,
            transponder,
            TransponderBank.Epc,
            bankAddress,
            blocksCount,
            accessPassword));
    if (result.Status != ResponseStatus.Ok)
    {
        throw new Exception($"ReadMultipleBlocks error: {result.Details}");
    }
byte[] data = result.Data;
```

[⬅ Back to contents](../README_EN.md)