Работа со спец. командами
=========================

Некоторое оборудование поддерживает расширенные функции, работа с которыми в Шине RFID
реализована через специализированные команды. Например, управление звуком, светодиодами,
счётчиком людей, графическими интерфейсами и пр.

Получение списка поддерживаемых считывателем специальных команд реализовано через
запрос GetSpecialCommands.

В примере продемонстрировано получение списка специализированных команд, с последующей
отправкой спец. команды ReinitializeTransponders (повторной инициализации меток)
эмулятору через запрос ExecuteSpecialCommand.

```delphi
rfidbus = New COMObject("AddIn.RfidBus1cClient");
...
readers = rfidbus.GetReaders();
For each reader In readers Do
    Message("Get special commands list from reader " + reader.Id + " (" + reader.Name + ")");
    specialCommands = rfidbus.GetSpecialCommands(reader.Id);
    For each specialCommand In specialCommands Do
        Message("   " + specialCommand.Name + " Detail:" + specialCommand.Description);

        REINITIALIZE_TRANSPONDERS_COMMAND = "ReinitializeTransponders";
        If specialCommand.Name = REINITIALIZE_TRANSPONDERS_COMMAND Then
            Message("Execute special command: " + REINITIALIZE_TRANSPONDERS_COMMAND);

            params = New COMObject("RfidBus.ParametersValues");
            rfidbus.ExecuteSpecialCommand(reader.Id, REINITIALIZE_TRANSPONDERS_COMMAND, params);
        EndIf;
    EndDo;
EndDo;

```

Подписка на на спец. события осуществляется через запрос SubscribeToSpecialEvent.

```delphi
rfidbus = New COMObject("AddIn.RfidBus1cClient");
...
readers = rfidbus.GetReaders();
For each reader In readers Do

Message("Get special events list from " + reader.Id + " (" + reader.Name + ")");
specialEvents = rfidbus.GetSpecialEvents(reader.Id);
For each specialEvent In specialEvents Do
    Message("   Subscribe to special event: " + specialEvent.Name);
    rfidbus.SubscribeToSpecialEvent(reader.Id, specialEvent.Name);
EndDo;

```

Отмена подписки реализуется через запрос UnsubscribeFromSpecialEvent.

```delphi
rfidbus = New COMObject("AddIn.RfidBus1cClient");
...
readers = rfidbus.GetReaders();
For each reader In readers Do

Message("Get special events list from " + reader.Id + " (" + reader.Name + ")");
specialEvents = rfidbus.GetSpecialEvents(reader.Id);
For each specialEvent In specialEvents Do
    Message("   Subscribe to special event: " + specialEvent.Name);
    rfidbus.UnsubscribeToSpecialEvent(reader.Id, specialEvent.Name);
EndDo;
```

[⬅ К оглавлению](../README.md)
