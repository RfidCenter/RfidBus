Управление доступом
===================

* [Access Password](#AccessPassword)
* [Kill Password](#KillPassword)
* [Блокировка банка памяти](#LockTransponder)
* [Деактивация метки](#KillTransponder)

<a name="AccessPassword"></a>Access Password
===============
Установка пароля на доступ к изменению данных (Access Password) реализована в запросе SetAccessPassword

```cs
RfidBusClient client;
 ...
Transponder transponder;
byte[] accessPassword = { 0x11, 0x22, 0x33, 0x44 };
ReaderRecord readerRecord;
 ...
var result = client.SendRequest(new SetAccessPassword(readerRecord.Id,
        transponder,
        accessPassword));
if (result.Status != ResponseStatus.Ok)
{
    throw new Exception($"SetAccessPassword error: {result.Details}");
}
```

<a name="KillPassword"></a>Kill Password
=============
Установка 32-битного пароля для метки, после введения которого метка навсегда прекратит обмен информацией со считывателями, реализована в запросе SetKillPassword.

```cs
RfidBusClient client;
 ...
Transponder transponder;
byte[] killPassword = { 100, 100, 100, 100 };
ReaderRecord readerRecord;
 ...
var result = client.SendRequest(new SetKillPassword(readerRecord.Id,
        transponder,
        killPassword));
if (result.Status != ResponseStatus.Ok)
{
    throw new Exception($"SetKillPassword error: {result.Details}");
}
```

<a name="LockTransponder"></a>Блокировка банка памяти
=======================

Блокировка банка памяти реализована в запросе LockTransponderBank. Для банков памяти Epc, Reserved, User Memory доступные следующие типы блокировки:
* Блокировка (Locked)
* Разблокировка (Unlocked)
* Постоянная блокировка (PermanentLocked)
* Постоянная разблокировка (PermanentUnlocked)

```cs
RfidBusClient client;
 ...
Transponder transponder;
byte[] password = {};
ReaderRecord readerRecord;
 ...
var result = client.SendRequest(new LockTransponderBank(readerRecord.Id,
        transponder,
        TransponderBank.Epc,
        TransponderBankLockType.Locked,
        password));
if (result.Status != ResponseStatus.Ok)
{
    throw new Exception($"LockTransponderBank error:{result.Details}");
}
```

<a name="KillTransponder"></a>Деактивация метки
=================
Посылка KILL-команды метке реализована в запросе KillTransponder — после его выполнения метка навсегда прекратит обмен информацией со считывателями.

```cs
RfidBusClient client;
 ...
Transponder transponder;
byte[] killPassword= {};
ReaderRecord readerRecord;
 ...
var result = client.SendRequest(new KillTransponder(readerRecord.Id,
        transponder,
        killPassword));
if (result.Status != ResponseStatus.Ok)
{
    throw new Exception($"KillTransponder error: {result.Details}");
}
```

[⬅ К оглавлению](../README.md)
