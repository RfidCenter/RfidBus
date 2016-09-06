using System;
using RfidBus.Primitives.Messages.Readers;
using RfidBus.Primitives.Network;
using RfidBus.Serializers.Ws;
using RfidCenter.Basic.Arguments;
using RfidCenter.Devices;

namespace Readers.Gpio
{
    class GpioExample
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
            _client.ReceivedEvent += RfidBusClientOnReceivedEvent;

            foreach (var readerRecord in readersResult.Readers)
            {
                SetReaderGpoStates(readerRecord, true);

                _client.SendRequest(new SubscribeToReader(readerRecord.Id));

                var response = GetReaderGpiStates(readerRecord);
                foreach (var gpiState in response.GpiStates)
                {
                    if (gpiState != null)
                    {
                        Console.WriteLine($"    Port: {gpiState.Port} state: {gpiState.State}");
                    }
                }
            }

            WaitForKey("Press ENTER to exit.", ConsoleKey.Enter);
            _client.Close();
        }

        private static void RfidBusClientOnReceivedEvent(object sender, ReceivedEventEventArgs args)
        {
            if (args.EventMessage is ReaderGpiStatesChangedEvent)
            {
                var message = (ReaderGpiStatesChangedEvent)args.EventMessage;
                Console.WriteLine($"> GPO was changed. Reader '{message.ReaderRecord.Name}'; Port: {message.GpiState.Port}; State: {message.GpiState.State}");              
            }
            
        }

        private static void SetReaderGpoStates(ReaderRecord reader, bool state)
        {
            reader.IsActive = true;
            var result = _client.SendRequest(new SetReaderGpoStates(reader.Id,
                                                                         new GpoStateRecord[]
                                                                         {
                                                                             new GpoStateRecord(1, state),
                                                                             new GpoStateRecord(2, state),
                                                                             new GpoStateRecord(3, state),
                                                                             new GpoStateRecord(4, state)
                                                                         }));

            Console.WriteLine($"Reader: {reader.Name} ({reader.Id}). GPO ports: 1, 2, 3, 4, value: {state}, status: {result.Status}");
        }

        private static GetReaderGpiStatesResponse GetReaderGpiStates(ReaderRecord reader)
        {
            var result = _client.SendRequest(new GetReaderGpiStates(reader.Id));
            Console.WriteLine($"Reader: {reader.Name} ({reader.Id}). GPI reading status: {result.Status}");
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
