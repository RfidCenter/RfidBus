using System;
using RfidBus.Primitives;
using RfidBus.Primitives.Messages;
using RfidBus.Primitives.Messages.Readers;
using RfidBus.Primitives.Network;
using RfidBus.Serializers.Ws;
using RfidCenter.Basic.Arguments;
using RfidCenter.Basic.Encode;

namespace Readers.Write
{
    class WriteExample
    {
        private static RfidBusClient _client;
        private const ushort BANK_ADDRESS = 2;
        private static readonly byte[] _accessPassword = {};
        private static readonly byte[] _data = { 255, 255, 255, 255 };

        private const ulong GCP = 461000232;
        private const ulong EXTENSION = 1000;
        private const ulong ASSET = 10;
        private const uint ITEM = 10;
        private const ulong SERIAL = 10;

        private const int GIAI_MENU_ITEM_NUMBER = 1;
        private const int SGTIN_MENU_ITEM_NUMBER = 2;
        private const int SSCC_MENU_ITEM_NUMBER = 3;
        private const int WRITE_MULTIPLE_BLOCKS_LABEL_ELEMENT = 4;

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
            Console.WriteLine();
            Console.WriteLine("Select EPC encoder:");
            Console.WriteLine($" {GIAI_MENU_ITEM_NUMBER} - GS1/UNISCAN (GIAI)");
            Console.WriteLine($" {SGTIN_MENU_ITEM_NUMBER} - SGTIN");
            Console.WriteLine($" {SSCC_MENU_ITEM_NUMBER} - SSCC");
            Console.WriteLine($" {WRITE_MULTIPLE_BLOCKS_LABEL_ELEMENT} - Write multiple blocks label element");
            Console.WriteLine();
            Console.WriteLine("Enter number:");

            int selectedEncoder = 0;
            var isValueInMenuItems = false;
            var isEnteredValueNumber = false;

            while (!isEnteredValueNumber || !isValueInMenuItems)
            {
                isEnteredValueNumber = Int32.TryParse(Console.ReadLine(), out selectedEncoder);
                isValueInMenuItems = selectedEncoder >= GIAI_MENU_ITEM_NUMBER && selectedEncoder <= WRITE_MULTIPLE_BLOCKS_LABEL_ELEMENT;
            }

            foreach (var readerRecord in readersResult.Readers)
            {
                if (readerRecord.Mode == ReaderMode.StandBy)
                {
                    
                    GetTranspondersResponse response = _client.SendRequest(new GetTransponders(readerRecord.Id));
                    foreach (var transponder in response.Transponders)
                    {
                        switch (selectedEncoder)
                        {
                            case GIAI_MENU_ITEM_NUMBER:
                                Console.Write($"WriteEpcGiai96 (GS1) to transponders {transponder.IdAsString}... ");
                                WriteEpcGiai96(readerRecord.Id, transponder, GCP, ASSET);
                                break;

                            case SGTIN_MENU_ITEM_NUMBER:
                                Console.Write($"WriteEpcSgtin96 to transponder {transponder.IdAsString}... ");
                                WriteEpcSgtin96(readerRecord.Id, transponder, GCP, ITEM, SERIAL);
                                break;

                            case SSCC_MENU_ITEM_NUMBER:
                                Console.Write($"WriteEpcSscc96 to transponder {transponder.IdAsString}... ");
                                WriteEpcSscc96(readerRecord.Id, transponder, GCP, EXTENSION);
                                break;

                            case WRITE_MULTIPLE_BLOCKS_LABEL_ELEMENT:
                                Console.Write($"WriteMultipleBlocks to transponder {transponder.IdAsString}... ");
                                WriteMultipleBlocks(readerRecord.Id, transponder, TransponderBank.Epc, _data,
                                    BANK_ADDRESS, _accessPassword);
                                break;
                        }
                        Console.WriteLine("OK");
                    }
                }
            }

            WaitForKey("Press ENTER to exit.", ConsoleKey.Enter);
            _client.Close();
        }

        private static void WriteMultipleBlocks(Guid readerId,
            Transponder transponder,
            TransponderBank transponderBank,
            byte[] data,
            ushort bankAddress = 0,
            byte[] accessPassword = null)
        {
            var result = _client.SendRequest(new RfidBus.Primitives.Messages.Readers.WriteMultipleBlocks(readerId, transponder, transponderBank, data, bankAddress, accessPassword));
            if (result.Status != ResponseStatus.Ok)
            {
                throw new Exception($"WriteMultipleBlocks error: {result.Details}");
            }
        }

        private static void WriteEpcSgtin96(Guid readerId,
                Transponder transponder,
                ulong gcp,
                uint item,
                ulong serial,
                EpcFilter filter = EpcFilter.AllOthers,
                byte partition = 3)
        {
            var result = _client.SendRequest(new WriteEpcSgtin96(readerId, transponder, gcp, item, serial, filter, partition));
            if (result.Status != ResponseStatus.Ok)
            {
                throw new Exception($"WriteEpcSgtin96 error: {result.Details}");
            }           
        }

        private static void WriteEpcGiai96(Guid readerId,
                Transponder transponder,
                ulong gcp,
                ulong asset,
                EpcFilter filter = EpcFilter.AllOthers,
                byte partition = 3)
        {
            var result = _client.SendRequest(new WriteEpcGiai96(readerId, transponder, gcp, asset, filter, partition));
            if (result.Status != ResponseStatus.Ok)
            {
                throw new Exception($"WriteEpcGiai96 (GS1) error: {result.Details}");
            }
        }

        private static void WriteEpcSscc96(Guid readerId,
                Transponder transponder,
                ulong gcp,
                ulong extension,
                EpcFilter filter = EpcFilter.AllOthers,
                byte partition = 3)
        {
            var result = _client.SendRequest(new WriteEpcSscc96(readerId, transponder, gcp, extension, filter, partition));
            if (result.Status != ResponseStatus.Ok)
            {
                throw new Exception($"WriteEpcSscc96 error: {result.Details}");
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
