using System;
using System.Linq;
using RfidBus.Primitives;
using RfidBus.Primitives.Messages;
using RfidBus.Primitives.Messages.Acs;
using RfidBus.Primitives.Network;
using RfidBus.Serializers.Ws;
using RfidCenter.Basic;
using RfidCenter.Basic.Arguments;

namespace AcsControllers
{
    public static class SphinxClient
    {
        
        private static RfidBusClient _client;
        private static bool _isAuthorize;

        public static void Initialize()
        {
            Console.WriteLine("Establishing connection to RFID Bus...");

            var protocol = new WsCommunicationDescription();
            var config = new ParametersValues(protocol.GetClientConfiguration());

            config.SetValue(ConfigConstants.PARAMETER_HOST, "127.0.0.1");
            config.SetValue(ConfigConstants.PARAMETER_PORT, 7266);

            _client = new RfidBusClient(protocol, config)
                      {
                          AllowReconnect = true
                      };

            _client.Connect();

            if (!_client.Authorize("admin", "admin"))
            {
                throw new Exception("Invalid login-password");
            }
            _isAuthorize = true;

            Console.WriteLine("Conenction established");
        }

        public static void GetKeys()
        {
            if (_client != null && _client.IsConnected && _isAuthorize)
            {
                
                Console.WriteLine("Getting readers ...");
                var acsControllers = GetAcsControllersList();
                if (acsControllers.Status != ResponseStatus.Ok)
                {
                    throw new Exception($"Can't get info about connected Asc controller. Reason: {acsControllers.Status}.");
                }

                Console.WriteLine("Get keys ...");

                foreach (var controller in acsControllers.Controllers)
                {
                    Console.WriteLine(" * processing Asc controller: {0}", controller.Name);

                    var getKeysResult = _client.SendRequest(new GetAllowedKeys(controller.Id));
                    if (getKeysResult.AllowedKeys != null)
                    {

                        foreach (var key in getKeysResult.AllowedKeys)
                        {
                            AddKeys(new []{key.IdAsString});
                            //RemoveKeys(new []{key.IdAsString});
                        }
                    }
                }
                
            }
        }

        public static void AddKeys(string[] keys)
        {
            if (keys.Length != 0)
            {
                if (_client != null && _client.IsConnected && _isAuthorize)
                {
                    _client.ReceivedEvent += RfidBusClientOnReceivedEvent;

                    Console.WriteLine("Getting readers ...");
                    var acsControllers = GetAcsControllersList();
                    if (acsControllers.Status != ResponseStatus.Ok)
                    {
                        throw new Exception($"Can't get info about connected Asc controller. Reason: {acsControllers.Status}.");
                    }

                    Console.WriteLine("Adding keys ...");

                    foreach (var controller in acsControllers.Controllers)
                    {
                        Console.WriteLine(" * processing Asc controller: {0}", controller.Name);
                        _client.SendRequest(new SubscribeToAcsControllerEvents(controller.Id));
                        _client.SendRequest(new AddAllowedKeys(controller.Id, GenerateKeys(keys)));
                    }
                    
                }
            }
        }

        public static void RemoveKeys(string[] keys)
        {
            if (_client != null && _client.IsConnected && _isAuthorize)
            {
                _client.ReceivedEvent += RfidBusClientOnReceivedEvent;

                Console.WriteLine("Getting readers ...");
                var acsControllers = GetAcsControllersList();
                if (acsControllers.Status != ResponseStatus.Ok)
                {
                    throw new Exception($"Can't get info about connected Asc controller. Reason: {acsControllers.Status}.");
                }

                Console.WriteLine("Deleting keys ...");

                foreach (var controller in acsControllers.Controllers)
                {
                    Console.WriteLine(" * processing Asc controller: {0}", controller.Name);
                    _client.SendRequest(new SubscribeToAcsControllerEvents(controller.Id));
                    _client.SendRequest(new RemoveAllowedKeys(controller.Id, GenerateKeys(keys)));
                }
                
            }
        }

        private static GetAcsControllersResponse GetAcsControllersList()
        {
            return _client.SendRequest(new GetAcsControllers());
        }

        private static void RfidBusClientOnReceivedEvent(object sender, ReceivedEventEventArgs receivedEventEventArgs)
        {
            if (receivedEventEventArgs.EventMessage is AcsControllerObjectPassEvent)
            {
                var passEvent = (AcsControllerObjectPassEvent) receivedEventEventArgs.EventMessage;
                Console.WriteLine(($"ACS controller {passEvent.Controller.Name} generate event Object Pass Event for transponder {passEvent.Transponder.IdAsString}"));
            }
            if (receivedEventEventArgs.EventMessage is AcsControllerUpdatingKeysEvent)
            {
                var updatingKeyEvent = (AcsControllerUpdatingKeysEvent) receivedEventEventArgs.EventMessage;
                Console.WriteLine(($"ACS controller {updatingKeyEvent.Controller.Name} generate event Updating Key Event"));
            }
            if (receivedEventEventArgs.EventMessage is AcsControllerOpenDoorPassEvent)
            {
                var openDoorEvent = (AcsControllerOpenDoorPassEvent) receivedEventEventArgs.EventMessage;
                Console.WriteLine(($"ACS controller {openDoorEvent.Controller.Name} generate event Open Door Pass Event"));
            }
            if (receivedEventEventArgs.EventMessage is AcsControllerBreakingPassEvent)
            {
                var breakingPassEvent = (AcsControllerBreakingPassEvent) receivedEventEventArgs.EventMessage;
                Console.WriteLine(($"ACS controller {breakingPassEvent.Controller.Name} generate event Breaking Pass Event"));
            }
            if (receivedEventEventArgs.EventMessage is AcsControllerObjectPassDeniedEvent)
            {
                var passDeniedEvent = (AcsControllerObjectPassDeniedEvent) receivedEventEventArgs.EventMessage;
                Console.WriteLine(($"ACS controller {passDeniedEvent.Controller.Name} generate event Object Pass Denied Event for transponder {passDeniedEvent.Transponder.IdAsString}"));
            }
        }

        private static void WaitForKey(string message, ConsoleKey key)
        {
            Console.WriteLine();
            Console.WriteLine(message);
            while (Console.ReadKey(true).Key != key)
            {
            }
        }

        private static Transponder[] GenerateKeys(string[] keys)
        {
            var transponsders = (from key in keys
                                 where !string.IsNullOrEmpty(key)
                                 select new Transponder {Id = BaseTools.GetBinaryFromString(key)}).ToList();

            return transponsders.ToArray();
        }
    }
}