using System;
using System.Linq;
using RfidBus.Primitives;
using RfidBus.Primitives.Messages;
using RfidBus.Primitives.Messages.Readers;
using RfidBus.Primitives.Network;
using RfidBus.Serializers.Ws;
using RfidCenter.Basic.Arguments;
using RfidCenter.Devices;

namespace Readers.Read
{
    class ReadExample
    {
        private static RfidBusClient _client;
        private static ushort _bankAddress = 2;
        private static int _blocksCount = 2;
        private static readonly byte[] _accessPassword = { 0, 0, 0, 0 };

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

            foreach (var readerRecord in readersResult.Readers)
            {               
                if (readerRecord.Mode == ReaderMode.StandBy)
                {
                    GetTranspondersResponse response = _client.SendRequest(new GetTransponders(readerRecord.Id));
                    Console.WriteLine("> Reader '{0}' found {1} transponder(s):", readerRecord.Name, response.Transponders.Length);
                    foreach (var transponder in response.Transponders)
                    {                       
                        var readResult = ReadMultipleBlocks(readerRecord.Id, transponder, TransponderBank.Epc, _bankAddress,
                                _blocksCount, _accessPassword);
                        var temp = readResult.Data.Aggregate("", (current, element) => current + (element + " "));
                        Console.WriteLine($"     Read bank result: {temp}");                        
                    }
                }
            }
           
            WaitForKey("Press ENTER to exit.", ConsoleKey.Enter);
            _client.Close();
        }

        private static ReadMultipleBlocksResponse ReadMultipleBlocks(Guid readerId,
                Transponder transponder,
                TransponderBank transponderBank,
                ushort bankAddress,
                int count,
                byte[] accessPassword)
        {
            var result = _client.SendRequest(new RfidBus.Primitives.Messages.Readers.ReadMultipleBlocks(readerId, transponder, transponderBank, bankAddress, count, accessPassword));
            if (result.Status != ResponseStatus.Ok)
            {
                throw new Exception($"ReadMultipleBlocks error: {result.Details}");
            }           
            return result;
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
