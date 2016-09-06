using System;
using System.Globalization;
using System.Xml;
using RfidBus.Primitives.Messages;
using RfidBus.Primitives.Messages.Readers;
using RfidBus.Primitives.Network;
using RfidBus.Serializers.Ws;
using RfidCenter.Basic.Arguments;

namespace Readers.MobileReadersInterface
{
    public static class Mobile
    {
        private static RfidBusClient _client;
        private const string MOBILE_COMMAND_SETWINDOWCONTROLS = "SetWindowControls";
        private const string GUI_XML_ELEMENT_BUTTON = "Button";
        private const string GUI_BUTTON_CLOSE = "CloseButton";
        private const string GUI_BUTTON_TEST1 = "TestButton1";
        private const string GUI_BUTTON_TEST2 = "Test1";
        private const string GUI_PARAMETER_ID = "Id";
        private const string GUI_XML_ATTR_NAME = "Name";
        private const string GUI_XML_ATTR_COLUMN = "Column";
        private const string GUI_XML_ATTR_VALUE = "Value";
        private const string GUI_XML_ATTR_TEXT = "Text";
        private const string GUI_XML_ELEMENT_TABLE = "Table";
        private const string GUI_XML_ELEMENT_COLUMNS = "Columns";
        private const string GUI_XML_ELEMENT_COLUMN = "Column";
        private const string GUI_XML_ELEMENT_ROWS = "Rows";
        private const string GUI_XML_ELEMENT_ROW = "Row";
        private const string GUI_XML_ELEMENT_FIELD = "Field";
        private const string GUI_TABLE_LIST = "List";
        private const string GUI_XML_COLUMN_1 = "Column1";
        private const string GUI_XML_COLUMN_2 = "Column2";
        private const string GUI_XML_COLUMN_3 = "Column3";

        private const string MOBILE_COMMAND_GETWORKAREASIZE = "GetWorkareaSize";
        private const string MOBILE_EVENT_BUTTONCLICK = "ButtonClick";
        private const string MOBILE_EVENT_TABLESELECYEDCHANGED = "TableSelectedChanged";

        private static int _mobileWidth;
        private static int _mobileHeight;

        private static void Initialize()
        {
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
                throw new Exception("Invalid login-password.");
            }
            Console.WriteLine("Connection established.");
            _client.ReceivedEvent += RfidBusClientOnReceivedEvent;           
        }

        private static bool CheckMobileGui(ReaderRecord reader)
        {
            var workSizeResponse = _client.SendRequest(new ExecuteSpecialCommand(reader.Id,
                    MOBILE_COMMAND_GETWORKAREASIZE,
                    new ParametersValues()));
            if ((workSizeResponse == null) || (workSizeResponse.Status != ResponseStatus.Ok))
            {               
                return false;
            }

            _mobileWidth = workSizeResponse.Result.GetValue<int>("Width");
            _mobileHeight = workSizeResponse.Result.GetValue<int>("Height");

            _client.SendRequest(new SubscribeToSpecialEvent(reader.Id, MOBILE_EVENT_BUTTONCLICK));
            _client.SendRequest(new SubscribeToSpecialEvent(reader.Id, MOBILE_EVENT_TABLESELECYEDCHANGED));

            return true;
        }
       
        public static void SetDefaultParameters(this XmlElement element,
                                                string id,
                                                int left,
                                                int top,
                                                int width,
                                                int height,
                                                string fontFamily = null,
                                                float? fontSize = null,                                               
                                                bool isEnabled = true,
                                                bool raiseEvents = false)
        {
            element.SetAttribute("Id", id);
            element.SetAttribute("Left", left.ToString(CultureInfo.InvariantCulture));
            element.SetAttribute("Top", top.ToString(CultureInfo.InvariantCulture));
            element.SetAttribute("Width", width.ToString(CultureInfo.InvariantCulture));
            element.SetAttribute("Height", height.ToString(CultureInfo.InvariantCulture));

            if (!string.IsNullOrWhiteSpace(fontFamily) && (fontSize != null))
            {
                var fontNode = element.OwnerDocument.CreateAndAppendElement("Font", element);
                fontNode.SetAttribute("Family", fontFamily);
                fontNode.SetAttribute("Size", fontSize.Value.ToString(CultureInfo.InvariantCulture));
            }
            element.SetAttribute("IsEnabled", isEnabled.ToString());
            element.SetAttribute("RaiseEvents", raiseEvents.ToString());
        }
        
        public static XmlElement CreateAndAppendElement(this XmlDocument doc, string name, XmlElement parent = null)
        {
            var element = doc.CreateElement(name);

            if (parent != null)
                parent.AppendChild(element);
            else
                doc.AppendChild(element);

            return element;
        }

        private static string CreateMobileInteface()
        {
            var xml = new XmlDocument();
            var root = xml.CreateAndAppendElement("Window");

            var closeButton = xml.CreateAndAppendElement(GUI_XML_ELEMENT_BUTTON, root);
            closeButton.SetDefaultParameters(GUI_BUTTON_CLOSE, _mobileWidth - 20, 0, 20, 20);
            closeButton.SetAttribute(GUI_XML_ATTR_TEXT, "X");
            closeButton.SetAttribute("StandartCommand", "CloseApplication");

            var testButton1 = xml.CreateAndAppendElement(GUI_XML_ELEMENT_BUTTON, root);
            testButton1.SetDefaultParameters(GUI_BUTTON_TEST1, 5, 25, _mobileWidth - 10, 20, raiseEvents:true);
            testButton1.SetAttribute(GUI_XML_ATTR_TEXT, "Test 1");

            var testButton2 = xml.CreateAndAppendElement(GUI_XML_ELEMENT_BUTTON, root);
            testButton2.SetDefaultParameters(GUI_BUTTON_TEST2, 5, 50, _mobileWidth - 10, 20, raiseEvents: true);
            testButton2.SetAttribute(GUI_XML_ATTR_TEXT, "Test 2");

            var table = xml.CreateAndAppendElement(GUI_XML_ELEMENT_TABLE, root);
            table.SetDefaultParameters(GUI_TABLE_LIST, 5, 75, _mobileWidth - 10, _mobileHeight - 80, raiseEvents: true);

            var columns = xml.CreateAndAppendElement(GUI_XML_ELEMENT_COLUMNS, table);
            var artColumn = xml.CreateAndAppendElement(GUI_XML_ELEMENT_COLUMN, columns);
            artColumn.SetAttribute(GUI_XML_ATTR_NAME, GUI_XML_COLUMN_1);
            artColumn.SetAttribute("Header", "Column 1");

            var foundColumn = xml.CreateAndAppendElement(GUI_XML_ELEMENT_COLUMN, columns);
            foundColumn.SetAttribute(GUI_XML_ATTR_NAME, GUI_XML_COLUMN_2);
            foundColumn.SetAttribute("Width", "30");
            foundColumn.SetAttribute("Header", "Column 2");

            var countColumn = xml.CreateAndAppendElement(GUI_XML_ELEMENT_COLUMN, columns);
            countColumn.SetAttribute(GUI_XML_ATTR_NAME, GUI_XML_COLUMN_3);
            countColumn.SetAttribute("Width", "30");
            countColumn.SetAttribute("Header", "Column 3");

            var rows = xml.CreateAndAppendElement(GUI_XML_ELEMENT_ROWS, table);

            for (int i = 1; i <= 10; i++)
            {
                var row = xml.CreateAndAppendElement(GUI_XML_ELEMENT_ROW, rows);
                row.SetAttribute("Id", i.ToString(CultureInfo.InvariantCulture));

                var field1 = xml.CreateAndAppendElement(GUI_XML_ELEMENT_FIELD, row);
                field1.SetAttribute(GUI_XML_ATTR_COLUMN, GUI_XML_COLUMN_1);
                field1.SetAttribute(GUI_XML_ATTR_VALUE, $"Value 1.{i}");

                var field2 = xml.CreateAndAppendElement(GUI_XML_ELEMENT_FIELD, row);
                field2.SetAttribute(GUI_XML_ATTR_COLUMN, GUI_XML_COLUMN_2);
                field2.SetAttribute(GUI_XML_ATTR_VALUE, $"Value 2.{i}");

                var field3 = xml.CreateAndAppendElement(GUI_XML_ELEMENT_FIELD, row);
                field3.SetAttribute(GUI_XML_ATTR_COLUMN, GUI_XML_COLUMN_3);
                field3.SetAttribute(GUI_XML_ATTR_VALUE, $"Value 3.{i}");
            }
            return xml.InnerXml;
        }

        static void Main(string[] args)
        {
            Initialize();

            var readersResult = _client.SendRequest(new GetReaders());

            foreach (var readerRecord in readersResult.Readers)
            {
                Console.WriteLine($"Reader: {readerRecord.Name}");

                if (!CheckMobileGui(readerRecord))
                {
                    continue;
                }
                
                var parameters = new ParametersValues();
                parameters.SetValue("UiXml", CreateMobileInteface());
                ExecuteSpecialCommand(readerRecord, MOBILE_COMMAND_SETWINDOWCONTROLS, parameters);
            }
    
            WaitForKey("Press ENTER to exit.", ConsoleKey.Enter);
            _client.Close();
        }

        private static void RfidBusClientOnReceivedEvent(object sender, ReceivedEventEventArgs e)
        {
            var message = e.EventMessage as ReaderSpecialEvent;
            if (message != null)
            {
                Console.WriteLine($"Special event. Reader: {message.Reader.Name}; eventName: {message.EventName}");

                switch (message.EventName)
                {
                    case MOBILE_EVENT_BUTTONCLICK:
                        var button = message.Parameters.GetValue<string>(GUI_PARAMETER_ID);
                        switch (button)
                        {
                            case GUI_BUTTON_TEST1:
                                Console.WriteLine("Test 1 button was pressed");
                                break;

                            case GUI_BUTTON_TEST2:
                                Console.WriteLine("Test 2 button was pressed");
                                break;
                        }
                        break;

                    case MOBILE_EVENT_TABLESELECYEDCHANGED:
                        var selectedRowId = message.Parameters.GetValue<string>("Value");
                        Console.WriteLine($"    Selected row ID: {selectedRowId}");                       
                        break;
                }              
            }
        }

        private static void ExecuteSpecialCommand(ReaderRecord reader, string command, ParametersValues parameters)
        {
            var result = _client.SendRequest(new ExecuteSpecialCommand(reader.Id, command, parameters));
            Console.WriteLine($" Special command '{command}' was executed on reader '{reader.Name}'. Result: {result.Status}");
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
