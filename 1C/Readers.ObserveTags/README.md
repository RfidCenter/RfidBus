Инвентаризация
==============

Для инвентаризации (поиска меток, находящихся в зоне действия считывателя) необходимо
произвести подписку на базовые события считывателя и перевести его в режим чтения.
В обработчике событий появится возможность фиксировать нахождение (TransponderFoundEvent)
и потерю (TransponderLostEvent) меток, а также изменения GPI (ReaderGpiStatesChangedEvent).


```bsl
AttachAddIn("AddIn.RfidBus1cClient");
rfidbus = New COMObject("AddIn.RfidBus1cClient");
reader = rfidbus.GetReaders().GetValue(0);
...
rfidbus.SubscribeToReader(reader.Id);
rfidbus.StartReading(reader.Id);
...

&AtClient
Procedure ExternalEvent(source, event, data)
	If source = "RfidBus1cClient" Then
		eventDetails = rfidbus.GetEventDetails(data);
		If event = "TransponderFoundEvent" Then
			For each tag In eventDetails.Transponders Do
				Message("Tag found: " + tag.IdAsString
						+ "Antenna: " + tag.Antenna + "; Tag Type: " + tag.Type);
			EndDo;
		EndIf;
		If event = "TransponderLostEvent" Then
			For each tag In eventDetails.Transponders Do
				Message("Tag lost: " + tag.IdAsString
						+ "Antenna: " + tag.Antenna + "; Tag Type: " + tag.Type);
			EndDo;
		EndIf;
	EndIf
EndProcedure
```

[⬅ К оглавлению](../README.md)
