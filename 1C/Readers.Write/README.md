* [Программирование банков памяти метки](#WriteMultipleBlocks)
* [Программирование EPC форматами данных GS1/UNISCAN](#WriteEpc)

<a name="WriteMultipleBlocks"></a>Программирование банков памяти метки
====================================
Запись данных в банки памяти транспондера осуществляется через запрос WriteMultipleBlocks.

```bsl
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

    data = New Array(4);
    data[0] = 255;
    data[1] = 255;
    data[2] = 255;
    data[3] = 255;

    TransponderBank_Reserved   = 0;
    TransponderBank_Epc        = 1;
    TransponderBank_Tid        = 2;
    TransponderBank_UserMemory = 3;

    startingBlock = 2; // 0 -- R/O CRC; 1 -- PC; 2+ -- EPC
    countReadBlock = 4;

    rfidbus.WriteMultipleBlocks(reader.Id, tag, TransponderBank_Epc, data, startingBlock, accessPassword);
    Message(
            "Data " + ByteArrayToHexString(data, 2) + " where written on tag " + tag.IdAsString
            + " Reader: " + reader.Id + " (" + reader.Name + ")"
            );
EndDo;
```

<a name="WriteEpc"></a>Программирование EPC форматами данных GS1/UNISCAN
=================================================
В Шине RFID реализованы запросы для программирования банка EPC специализированными форматами, такими как SGTIN, GIAI и др.
Запись в память метки SGTIN-96

```bsl
rfidbus = New COMObject("AddIn.RfidBus1cClient");
reader = rfidbus.GetReaders().GetValue(0);
...
tags = rfidbus.GetTransponders(reader.Id);
For each tag In tags Do
    rfidbus.WriteEpcGiai96(reader.Id, tag,
            461000232,	// GCP
            1,			// Asset
            0, 			// Epc Filter. 0 - All Others; see standard.
            3);			// Partition
    Return;
EndDo;
```

[⬅ К оглавлению](../README.md)
