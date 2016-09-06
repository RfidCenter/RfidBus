Управление доступом
===================

* [Установка пароля доступа](#AccessPassword)
* [Установка пароля уничтожения метки](#KillPassword)
* [Блокировка банка памяти](#LockTransponder)
* [Деактивация метки](#KillTransponder)

<a name="AccessPassword"></a>Access Password
===============
Установка пароля на доступ к изменению данных (Access Password) реализована в запросе SetAccessPassword

```delphi
rfidbus = New COMObject("AddIn.RfidBus1cClient");
...
reader = rfidbus.GetReaders().GetValue(0);
tag = rfidbus.GetTransponders(reader.Id).GetValue(0);

oldAccessPassword = New Array(4);
oldAccessPassword[0] = 0;
oldAccessPassword[1] = 0;
oldAccessPassword[2] = 0;
oldAccessPassword[3] = 0;

newAccessPassword = New Array(4);
newAccessPassword[0] = 100;
newAccessPassword[1] = 100;
newAccessPassword[2] = 100;
newAccessPassword[3] = 100;

rfidbus.SetAccessPassword(reader.Id, tag, newAccessPassword, oldAccessPassword);
```

<a name="KillPassword"></a>Kill Password
=============
Установка 32-битного пароля для метки, после введения которого метка навсегда прекратит обмен информацией со считывателями, реализована в запросе SetKillPassword.

```delphi
rfidbus = New COMObject("AddIn.RfidBus1cClient");
...
reader = rfidbus.GetReaders().GetValue(0);
tag = rfidbus.GetTransponders(reader.Id).GetValue(0);

accessPassword = New Array(4);
accessPassword[0] = 0;
accessPassword[1] = 0;
accessPassword[2] = 0;
accessPassword[3] = 0;

newKillPassword = New Array(4);
newKillPassword[0] = 100;
newKillPassword[1] = 100;
newKillPassword[2] = 100;
newKillPassword[3] = 100;

rfidbus.SetKillPassword(reader.Id, tag, newKillPassword, accessPassword);
```

<a name="LockTransponder"></a>Блокировка банка памяти
=======================

Блокировка банка памяти реализована в запросе LockTransponderBank. Для банков памяти Epc, Reserved, User Memory доступные следующие типы блокировки:
* Блокировка (Locked)
* Разблокировка (Unlocked)
* Постоянная блокировка (PermanentLocked)
* Постоянная разблокировка (PermanentUnlocked)

```delphi
rfidbus = New COMObject("AddIn.RfidBus1cClient");
...
reader = rfidbus.GetReaders().GetValue(0);
tag = rfidbus.GetTransponders(reader.Id).GetValue(0);

accessPassword = New Array(4);
accessPassword[0] = 0;
accessPassword[1] = 0;
accessPassword[2] = 0;
accessPassword[3] = 0;

TransponderBank_Reserved   = 0;
TransponderBank_Epc        = 1;
TransponderBank_Tid        = 2;
TransponderBank_UserMemory = 3;

TransponderBankLockType_Unlocked          = 0;
TransponderBankLockType_PermanentLocked   = 1;
TransponderBankLockType_Locked            = 2;
TransponderBankLockType_PermanentUnlocked = 3;

rfidbus.LockTransponderBank(
        reader.Id,
        tag,
        TransponderBank_Epc,
        TransponderBankLockType_Locked,
        newAccessPassword
);
```

<a name="KillTransponder"></a>Деактивация метки
=================
Посылка KILL-команды метке реализована в запросе KillTransponder — после его выполнения метка навсегда прекратит обмен информацией со считывателями.

```delphi
rfidbus = New COMObject("AddIn.RfidBus1cClient");
...
reader = rfidbus.GetReaders().GetValue(0);
tag = rfidbus.GetTransponders(reader.Id).GetValue(0);

killPassword = New Array(4);
killPassword[0] = 100;
killPassword[1] = 100;
killPassword[2] = 100;
killPassword[3] = 100;

rfidbus.KillTransponder(reader.Id, tag, killPassword);
```

[⬅ К оглавлению](../README.md)
