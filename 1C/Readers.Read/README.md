Чтение данных из банков памяти метки
====================================

Чтение данных из банков памяти транспондера осуществляется через запрос ReadMultipleBlocks.

```delphi
rfidbus = New COMObject("AddIn.RfidBus1cClient");
reader = rfidbus.GetReaders().GetValue(0);
...
tags = rfidbus.GetTransponders(reader.Id);
For each tag In tags Do
    accessPassword = New Array(4);
    accessPassword[0] = 0;
    accessPassword[1] = 0;
    accessPassword[2] = 0;
    accessPassword[3] = 0;

    TransponderBank_Reserved   = 0;
    TransponderBank_Epc        = 1;
    TransponderBank_Tid        = 2;
    TransponderBank_UserMemory = 3;

    startingBlock = 2; // 0 -- R/O CRC; 1 -- PC; 2+ -- EPC
    countReadBlock = 6;

    data = rfidbus.ReadMultipleBlocks(reader.Id, tag, TransponderBank_Epc, startingBlock, countReadBlock, accessPassword);
    Message("Tag: " + tag.IdAsString + "; Data: " + ByteArrayToHexString(data, 2) + "; startingBlock: 2; countReadBlock: 6");
EndDo;
```

[⬅ К оглавлению](../README.md)
