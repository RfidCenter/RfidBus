using System;
using RfidBus.Primitives.Messages;
using RfidBus.Primitives.Messages.Readers;
using RfidBus.Primitives.Network;
using RfidBus.Serializers.Ws;
using RfidCenter.Basic.Arguments;

namespace Readers.SpecialCommands
{
    class SpecialCommands
    {
        private static RfidBusClient _client;
        private const string COMMAND_REINITIALIZE_TAGS = "ReinitializeTransponders";

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
            _client.ReceivedEvent += RfidBusClientOnReceivedEvent;
        }

        static void Main(string[] args)
        {
            Initialize();

            var readersResult = _client.SendRequest(new GetReaders());

            foreach (var readerRecord in readersResult.Readers)
            {               
                Console.WriteLine($"Reader: {readerRecord.Name}");
                var response = GetSpecialCommands(readerRecord);
                foreach (var specialCommand in response.SpecialCommands)
                {
                    Console.WriteLine($"Special command name: {specialCommand.Name}; description: {specialCommand.Description}; parameters: {specialCommand.Parameters}");
                    if (specialCommand.Name == COMMAND_REINITIALIZE_TAGS)
                    {
                        ExecuteSpecialCommand(readerRecord, specialCommand.Name, null);
                    }
                }

                var specialEventsResponse = GetSpecialEvents(readerRecord);
                foreach (var specialEvent in specialEventsResponse.SpecialEvents)
                {
                    Console.Write($" Special event name: {specialEvent.Name} description: {specialEvent.Description}");
                    SubscribeToSpecialEvent(readerRecord, specialEvent.Name);
                    Console.WriteLine("  Subscribe OK");
                }
            }

            WaitForKey("Press ENTER to unsubscribe all.", ConsoleKey.Enter);

            foreach (var readerRecord in readersResult.Readers)
            {
                var specialEventsResponse = GetSpecialEvents(readerRecord);
                foreach (var specialEvent in specialEventsResponse.SpecialEvents)
                {
                    Console.Write($" Special event name: {specialEvent.Name}");
                    UnsubscribeFromSpecialEvent(readerRecord, specialEvent.Name);
                    Console.WriteLine(" Unsubscribe OK");
                }
            }

            WaitForKey("Press ESCAPE to exit.", ConsoleKey.Escape);
            _client.Close();
        }

        private static void RfidBusClientOnReceivedEvent(object sender, ReceivedEventEventArgs e)
        {
            var message = e.EventMessage as OnReaderSpecialEvent;
            if (message != null)
            {
                Console.WriteLine($"Special event. Reader: {message.Reader.Name}; eventName: {message.EventName}");
            }
        }

        private static void SubscribeToSpecialEvent(ReaderRecord reader, string eventName)
        {
            var result = _client.SendRequest(new SubscribeToSpecialEvent(reader.Id, eventName));
            if (result.Status != ResponseStatus.Ok)
            {
                throw new Exception($"SubscribeToSpecialEvent error: {result.Details}");
            }
        }

        private static void UnsubscribeFromSpecialEvent(ReaderRecord reader, string eventName)
        {
            var result = _client.SendRequest(new UnsubscribeFromSpecialEvent(reader.Id, eventName));
            if (result.Status != ResponseStatus.Ok)
            {
                throw new Exception($"UnsubscribeFromSpecialEvent error: {result.Details}");
            }
        }

        private static void ExecuteSpecialCommand(ReaderRecord reader, string command, ParametersValues parameters)
        {
            var result = _client.SendRequest(new ExecuteSpecialCommand(reader.Id, command, parameters));
            Console.WriteLine($" Special command '{command}' was executed on reader '{reader.Name}'. Result: {result.Status}");
        }

        private static GetSpecialEventsResponse GetSpecialEvents(ReaderRecord reader)
        {
            var result = _client.SendRequest(new GetSpecialEvents(reader.Id));
            if (result.Status != ResponseStatus.Ok)
            {
                throw new Exception($"GetSpecialEventsResponse error: {result.Details}");
            }
            return result;
        }

        private static GetSpecialCommandsResponse GetSpecialCommands(ReaderRecord reader)
        {
            var result =  _client.SendRequest(new GetSpecialCommands(reader.Id));
            if (result.Status != ResponseStatus.Ok)
            {
                throw new Exception($"GetSpecialCommandsResponse error: {result.Details}");
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
