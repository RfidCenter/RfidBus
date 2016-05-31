I. Представленные примеры

* ComAsClassExample - пример использования COM/Active-X объекта для работы с Шиной RFID в качестве обычного класса .NET.
* ObserveRfidTids - пример использования класса RfidBusClient для работы с Шиной RFID.
* ChangeGpoStates - пример использования класса RfidBusClient для управления состояниями GPO через Шину RFID
* PrintSgtinTags - пример использования класса RfidBusClient для управления печатью меток через Шину RFID 

II. Зависимости

1. ComAsClassExample

Проект ComAsClassExample зависит от сборок RfidBus.Com.Primitives.dll и RfidBus.Com.dll, которые можно найти по месту установки Менеджера Шины RFID (обычно: «c:\Program Files (x86)\RfidCenter\RfidbusManager\»).

2. ObserveRfidTids

Проект ObserveRfidTids зависит от сборок RfidCenter.Basic.dll, RfidCenter.Devices.dll, RfidBus.Primitives.dll, расположенных по месту установки Менеджера Шины RFID, и RfidBus.Serializers.Ws.dll, расположенной в поддиректории «serializers» в корневой директории Менеджера Шины RFID.

3. ChangeGpoStates

Проект ChangeGpoStates зависит от сборок RfidCenter.Basic.dll, RfidCenter.Devices.dll, RfidBus.Primitives.dll, расположенных по месту установки Менеджера Шины RFID, и RfidBus.Serializers.Ws.dll, расположенной в поддиректории «serializers» в корневой директории Шины RFID.

4. PrintSgtinTags

Проект PrintSgtinTags зависит от сборок RfidCenter.Basic.dll, RfidCenter.Printing, RfidBus.Primitives.dll, DevExpress.Docs.v14.1, расположенных по месту установки Менеджера Шины RFID, и RfidBus.Serializers.Ws.dll, расположенной в поддиректории «serializers» в корневой директории Шины RFID.