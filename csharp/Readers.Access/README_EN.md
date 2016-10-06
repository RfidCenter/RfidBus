[![на русском](http://rfidcenter.ru/img/flag-ru.svg) РУССКАЯ ВЕРСИЯ](README.md)

Access control
===================

* [Access Password](#AccessPassword)
* [Kill Password](#KillPassword)
* [Memory bank blockage](#LockTransponder)
* [Tag deactivation](#KillTransponder)

<a name="AccessPassword"></a>Access Password
===============
SetAccessPassword request sets the password for an access to the data changing (Access Password).

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

SetKillPassword request sets the 32-bit length password for a tag to stop the communication between a tag and a reader forever.

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

<a name="LockTransponder"></a>Memory bank blockage
=======================

One can block the tag's memory bank by using LockTransponderBank request. For managing of EPC, Reserver and User Memory banks one can use following types of locking:
* Locking (Locked)
* Unlocking (Unlocked)
* Permanent locking (PermanentLocked)
* Permanent unlocking (PermanentUnlocked)

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

<a name="KillTransponder"></a>Tag deactivation
=================

KillTransponder request is used to send Kill-commands from a reader to a tag, after executing of this request the tag stops an information exchange with the reader forever.

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

[⬅ Back to contents](../README_EN.md)