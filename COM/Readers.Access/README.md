Управление доступом
===================

* [Access Password](#AccessPassword)
* [Kill Password](#KillPassword)
* [Блокировка банка памяти](#LockTransponder)
* [Деактивация метки](#KillTransponder)

<a name="AccessPassword"></a>Access Password
===============
Установка пароля на доступ к изменению данных (Access Password) реализована в запросе SetAccessPassword

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
 ...
$readers = $rfidbus.GetReaders()
$reader = $readers[0]
$transponders = $rfidbus.GetTransponders($reader.Id);
$transponder = $transponders[0]
$oldAccessPassword = [Byte[]](0,0,0,0)       
$newAccessPassword = [Byte[]](100, 100, 100, 100)
$rfidbus.SetAccessPassword($reader.Id, $transponder, $newAccessPassword, $oldAccessPassword)
```

<a name="KillPassword"></a>Kill Password
=============
Установка 32-битного пароля для метки, после введения которого метка навсегда прекратит обмен
информацией со считывателями, реализована в запросе SetKillPassword.

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
 ...
$readers = $rfidbus.GetReaders()
$reader = $readers[0]
$transponders = $rfidbus.GetTransponders($reader.Id);
$transponder = $transponders[0]
$accessPassword = [Byte[]](0,0,0,0)       
$newKillPassword = [Byte[]](100, 100, 100, 100)
$rfidbus.SetKillPassword($reader.Id, $transponder, $newKillPassword, $accessPassword)
```

<a name="LockTransponder"></a>Блокировка банка памяти
=======================

Блокировка банка памяти реализована в запросе LockTransponderBank. Для банков памяти Epc, Reserved,
User Memory доступные следующие типы блокировки:
* Блокировка (Locked)
* Разблокировка (Unlocked)
* Постоянная блокировка (PermanentLocked)
* Постоянная разблокировка (PermanentUnlocked)

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
 ...
$readers = $rfidbus.GetReaders()
$reader = $readers[0]
$transponders = $rfidbus.GetTransponders($reader.Id);
$transponder = $transponders[0]
$accessPassword = [Byte[]](0,0,0,0) 
$epcBank = [RfidBus.Com.Primitives.Transponders.TransponderBank]::Epc
$lockTypeLocked = [RfidBus.Com.Primitives.Transponders.TransponderBankLockType]::Locked
$rfidbus.LockTransponderBank($reader.Id, $transponder, $epcBank, $lockTypeLocked, $accessPassword)
```

<a name="KillTransponder"></a>Деактивация метки
=================
Посылка KILL-команды метке реализована в запросе KillTransponder — после его выполнения метка навсегда прекратит
обмен информацией со считывателями.

```powershell
$rfidbus = New-Object -ComObject RfidBus.RfidBusComClient
 ...
$readers = $rfidbus.GetReaders()
$reader = $readers[0]
$transponders = $rfidbus.GetTransponders($reader.Id);
$transponder = $transponders[0]

$rfidbus.KillTransponder($reader.Id, $transponder, $newKillPassword)
```

[⬅ К оглавлению](../README.md)
