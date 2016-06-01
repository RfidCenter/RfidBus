using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using RfidBus.Primitives.Messages;
using RfidBus.Primitives.Messages.Readers;
using RfidBus.Primitives.Network;
using RfidBus.Serializers.Ws;

using RfidCenter.Basic.Arguments;
using RfidCenter.Devices;

namespace ChangeGpoStates
{
    internal class Program
    {
        //интервал времени, через который производить смену состояния портов GPO (в миллисекундах)
        private const int TIME_INTERVAL_CHANGES = 1000;
        private static CancellationTokenSource _cancelSource = null;

        private RfidBusClient _client = null;

        private static void Main(string[] args)
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
            config.SetValue(ConfigConstants.PARAMETER_HOST, "demo.rfidbus.rfidcenter.ru");
            config.SetValue(ConfigConstants.PARAMETER_PORT, 80);

            this._client = new RfidBusClient(protocol, config)
                           {
                               AllowReconnect = true,
                               RequestTimeOut = TimeSpan.FromSeconds(30),
                           };
            this._client.Connect();

            if (!this._client.Authorize("demo", "demo"))
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

                _cancelSource = new CancellationTokenSource();
                this.ChanchingReadersGpoWork(readersResult.Readers, _cancelSource.Token);

                WaitForKey("Press ESC to stop.", ConsoleKey.Escape);
                _cancelSource.Cancel();
            }
        }

        private async void ChanchingReadersGpoWork(IEnumerable<ReaderRecord> readers, CancellationToken token)
        {
            await Task.Run(() =>
                           {
                               var currentState = true;
                               while (true)
                               {
                                   if (token.IsCancellationRequested)
                                   {
                                       return;
                                   }

                                   currentState = !currentState;
                                   var records = readers as ReaderRecord[] ?? readers.ToArray();
                                   foreach (var reader in records)
                                   {
                                       this.SetReaderGpoStates(reader, currentState);
                                   }

                                   Thread.Sleep(TIME_INTERVAL_CHANGES);
                               }
                           },
                           token);
        }

        private void SetReaderGpoStates(ReaderRecord reader, bool state)
        {
            var result = this._client.SendRequest(new SetReaderGpoStates(reader.Id,
                                                                         new[]
                                                                         {
                                                                             new GpoStateRecord(1, state),
                                                                             new GpoStateRecord(2, state),
                                                                             new GpoStateRecord(3, state),
                                                                             new GpoStateRecord(4, state),
                                                                         }));

            Console.WriteLine("Reader: {0} ({1}). GPO ports: 1, 2, 3, 4, value: {2}, status: {3}",
                              reader.Name,
                              reader.Id,
                              state,
                              result.Status);
        }

        private void RfidBusClientOnReceivedEvent(object sender, ReceivedEventEventArgs args)
        {
            if (args.EventMessage is TransponderFoundEvent)
            {
                var msg = (TransponderFoundEvent) args.EventMessage;

                Console.WriteLine("> Reader '{0}' found {1} transponder(s):",
                                  msg.ReaderRecord.Name,
                                  msg.Transponders.Length);
                foreach (var transponder in msg.Transponders)
                {
                    Console.WriteLine(" * ID: '{0}', Antenna: {1}, Type: {2}",
                                      transponder.IdAsString,
                                      transponder.Antenna,
                                      transponder.Type);
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
            while (Console.ReadKey(true).Key != key)
            {
            }
        }
    }
}
