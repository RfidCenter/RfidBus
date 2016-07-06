Чтение данных из банков памяти метки
====================================

Чтение данных из банков памяти транспондера осуществляется через запрос ReadMultipleBlocks.

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

[⬅ К оглавлению](../README.md)
