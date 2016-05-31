using System;

using RfidBus.Primitives.Messages;
using RfidBus.Primitives.Messages.Readers;
using RfidBus.Primitives.Network;
using RfidBus.Serializers.Ws;

using RfidCenter.Basic.Arguments;

namespace RfidBusClientExample
{
    internal class Program
    {
        private RfidBusClient _client = null;

        private static void Main()
        {
            var program = new Program();
            try
            {
                program.Initialize();
                program.DoWork();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception {0}: '{1}'.", ex.GetType(), ex.Message);
            }
            finally
            {
                program.Finish();

                WaitForKey("Press ENTER to exit.", ConsoleKey.Enter);
            }
        }

        private void Initialize()
        {
            Console.WriteLine("Establishing connection to RFID Bus...");
            var protocol = new WsCommunicationDescription();
            var config = new ParametersValues(protocol.GetClientConfiguration());
            config.SetValue(ConfigConstants.PARAMETER_HOST, "127.0.0.1");
            config.SetValue(ConfigConstants.PARAMETER_PORT, 8080);

            this._client = new RfidBusClient(protocol, config)
                           {
                               AllowReconnect = true,
                               RequestTimeOut = TimeSpan.FromSeconds(30),
                           };
            this._client.Connect();

            if (!this._client.Authorize("admin", "admin"))
            {
                throw new Exception("Invalid login-password.");
            }
            Console.WriteLine("Connection established.");
        }

        private void DoWork()
        {
            if ((this._client != null) && this._client.IsConnected)
            {
                this._client.ReceivedEvent += this.RfidBusClientOnReceivedEvent;

                Console.WriteLine("Getting readers ...");
                var readersResult = this._client.SendRequest(new GetReaders());
                if (readersResult.Status != ResponseStatus.Ok)
                {
                    throw new Exception(string.Format("Can't get info about connected readers. Reason: {0}.",
                                                      readersResult.Status));
                }

                foreach (var record in readersResult.Readers)
                {
                    Console.WriteLine(" * processing reader: {0}", record.Name);

                    this._client.SendRequest(new SubscribeToReader(record.Id));
                    this._client.SendRequest(new StartReading(record.Id));
                }

                WaitForKey("Press ESC to stop.", ConsoleKey.Escape);
            }
        }

        private void RfidBusClientOnReceivedEvent(object sender, ReceivedEventEventArgs args)
        {
            if (args.EventMessage is TransponderFoundEvent)
            {
                var msg = (TransponderFoundEvent) args.EventMessage;

                Console.WriteLine("> Reader '{0}' found {1} transponder(s):", msg.ReaderRecord.Name, msg.Transponders.Length);
                foreach (var transponder in msg.Transponders)
                {
                    Console.WriteLine(" * ID: '{0}', Antenna: {1}, Type: {2}", transponder.IdAsString, transponder.Antenna, transponder.Type);
                }
            }
        }

        private void Finish()
        {
            if ((this._client == null) || !this._client.IsConnected)
            {
                return;
            }

            Console.WriteLine("Closing connection to RfidBus.");
            this._client.Close();
            this._client = null;
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
