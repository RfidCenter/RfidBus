using System;
using RfidBus.Primitives;
using RfidBus.Primitives.Messages;
using RfidBus.Primitives.Messages.Readers;
using RfidBus.Primitives.Network;
using RfidBus.Serializers.Ws;
using RfidCenter.Basic.Arguments;

namespace Readers.Access
{
    class AccessExample
    {
        private static RfidBusClient _client;

        private static void Initialize()
        {
            var protocol = new WsCommunicationDescription();
            var config = new ParametersValues(protocol.GetClientConfiguration());

            config.SetValue(ConfigConstants.PARAMETER_HOST, "demo.rfidbus.rfidcenter.ru");
            config.SetValue(ConfigConstants.PARAMETER_PORT, 80);

            _client = new RfidBusClient(protocol, config)
            {
                AllowReconnect = true
            };
            _client.Connect();

            if (!_client.Authorize("demo", "demo"))
            {
                throw new Exception("Invalid login-password.");
            }
            Console.WriteLine("Connection established.");
        }

        static void Main(string[] args)
        {
            Initialize();

            var readersResult = _client.SendRequest(new GetReaders());

            var killPassword = new byte[] { 100, 100, 100, 100 };
            var accessPassword = new byte[] { 100, 100, 100, 100 };

            foreach (var readerRecord in readersResult.Readers)
            {
                if (readerRecord.Mode == ReaderMode.StandBy)
                {
                    GetTranspondersResponse response = _client.SendRequest(new GetTransponders(readerRecord.Id));
                    foreach (var transponder in response.Transponders)
                    {
                        try
                        {
                            Console.Write($"Set access password on transponder {transponder.IdAsString}...");
                            SetAccessPassword(readerRecord.Id, transponder, accessPassword);
                            Console.WriteLine("OK");

                            Console.Write($"Set kill password on transponder {transponder.IdAsString}... ");
                            SetKillPassword(readerRecord.Id, transponder, killPassword);
                            Console.WriteLine("OK");
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine($"Set password error: {e.Message}");
                        }

                        try
                        {
                            Console.Write($"Lock transponder bank on transponder {transponder.IdAsString}... ");
                            LockTransponderBank(readerRecord.Id, transponder, TransponderBank.Epc,
                                    TransponderBankLockType.Locked, accessPassword);
                            Console.WriteLine("OK");

                            Console.Write($"Unock transponder bank on transponder {transponder.IdAsString}... ");
                            LockTransponderBank(readerRecord.Id, transponder, TransponderBank.Epc,
                                    TransponderBankLockType.Unlocked, accessPassword);
                            Console.WriteLine("OK");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Lock/Unock transponder error: {e.Message}");
                        }

                        try
                        {
                            Console.Write($"Kill transponder {transponder.IdAsString}... ");
                            KillTransponder(readerRecord.Id, transponder, killPassword);
                            Console.WriteLine("OK");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Kill transponder error: {e.Message}");
                        }
                    }
                }
            }

            WaitForKey("Press ENTER to exit.", ConsoleKey.Enter);
            _client.Close();
        }

        private static void SetAccessPassword(Guid readerId,
                Transponder transponder,
                byte[] accessPassword)
        {
            var result = _client.SendRequest(new SetAccessPassword(readerId, transponder, accessPassword));
            if (result.Status != ResponseStatus.Ok)
            {
                throw new Exception($"SetAccessPassword error: {result.Details}");
            }
        }

        private static void SetKillPassword(Guid readerId,
                Transponder transponder,
                byte[] killPassword)
        {
            var result = _client.SendRequest(new SetKillPassword(readerId, transponder, killPassword));
            if (result.Status != ResponseStatus.Ok)
            {
                throw new Exception($"SetKillPassword error: {result.Details}");
            }
        }

        private static void KillTransponder(Guid readerId,
                Transponder transponder,
                byte[] killPassword)
        {
            var result = _client.SendRequest(new KillTransponder(readerId, transponder, killPassword));
            if (result.Status != ResponseStatus.Ok)
            {
                throw new Exception($"KillTransponder error: {result.Details}");
            }
        }

        private static void LockTransponderBank(Guid readerId,
                Transponder transponder,
                TransponderBank transponderBank,
                TransponderBankLockType lockType,
                byte[] password)
        {
            var result = _client.SendRequest(new LockTransponderBank(readerId, transponder, transponderBank, lockType, password));
            if (result.Status != ResponseStatus.Ok)
            {
                throw new Exception($"LockTransponderBank error: {result.Details}");
            }
        }

        private static void WaitForKey(string message, ConsoleKey key)
        {
            Console.WriteLine();
            Console.WriteLine(message);
            while (Console.ReadKey(true)
                          .Key != key)
            {
            }
        }
    }
}
