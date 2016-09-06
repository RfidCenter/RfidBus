Работа с GPIO
=============

* [Управление исполнительными механизмами](#SetReaderGpoStates)
* [Запрос состояния исполнительных механизмов](#GetReaderGpiStates)
* [События от исполнительных механизмов](#ReaderGpiStatesChangedEvent)


<a name="SetReaderGpoStates"></a>Управление исполнительными механизмами
--------------------------------------
Управление состоянием GPO реализовано запросом SetGpoStates, в котором передаётся массив с состояниями портов.

```delphi
rfidbus = New COMObject("AddIn.RfidBus1cClient");
reader = rfidbus.GetReaders().GetValue(0);
...
gpoPorts = New COMSafeArray("VT_VARIANT", 4);
For i = 0 To 3 Do
    gpoPort = New COMObject("RfidBus.ReaderGpoStateRecord");
    gpoPort.State = True;
    gpoPort.Port = i + 1;
    gpoPorts.SetValue(i, gpoPort);
EndDo;
	
rfidbus.SetGpoStates(reader, gpoPort);
```

<a name="GetReaderGpiStates"></name>Запрос состояния исполнительных механизмов
------------------------------------------

Получение текущего состояния GPI реализовано в запросе GetGpiStates.

```delphi
AttachAddIn("AddIn.RfidBus1cClient");
rfidbus = New COMObject("AddIn.RfidBus1cClient");
reader = rfidbus.GetReaders().GetValue(0);
...
gpiStates = rfidbus.GetGpiStates(reader.Id);

For each gpiPort In gpiStates Do
    Message("Port: " + gpiPort.Port + "; State: " + gpiPort.State);
EndDo
```

<a name="ReaderGpiStatesChangedEvent"></a>События от исполнительных механизмов
------------------------------------

Для получения событий изменения GPI необходимо произвести подписку на базовые события
считывателя, после чего в обработчике событий появится возможность фиксировать изменения
GPI (ReaderGpiStatesChangedEvent).

```delphi
AttachAddIn("AddIn.RfidBus1cClient");
rfidbus = New COMObject("AddIn.RfidBus1cClient");
reader = rfidbus.GetReaders().GetValue(0);
rfidbus.SubscribeToReader(reader.Id);
...

&AtClient
Procedure ExternalEvent(source, event, data)
	Сообщить(event);
	If source = "RfidBus1cClient" Then
		eventDetails = rfidbus.GetEventDetails(data);
		If event = "ReaderGpiStatesChangedEvent" Then
			AddToLog("Event: ReaderGpiStatesChangedEvent",
					eventDetails.Reader,
					"Port: " + eventDetails.Port +
							"; State: " + eventDetails.State);
		EndIf
	EndIf
EndProcedure
```

В эмуляторе состояния портов GPI соответствуют состоянию портов GPO. Таким образом, чтобы
получить ReaderGpiStatesChangedEvent на эмуляторе необходимо изменить состояние его GPO.

[⬅ К оглавлению](../README.md)
