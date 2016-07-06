using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Xml;
using Readers.MobileReadersRasterInterface.Properties;
using RfidBus.Primitives.Messages;
using RfidBus.Primitives.Messages.Readers;
using RfidBus.Primitives.Network;
using RfidBus.Serializers.Ws;
using RfidCenter.Basic.Arguments;

namespace Readers.MobileReadersRasterInterface
{
    public static class MobileRasterInterface
    {
        private static RfidBusClient _client;
        private const string MOBILE_COMMAND_SETWINDOWCONTROLS = "SetWindowControls";
        private const string GUI_XML_ELEMENT_BUTTON = "Button";
        private const string GUI_BUTTON_CLOSE = "CloseButton";
        private const string GUI_XML_ATTR_TEXT = "Text";

        private const string GUI_XML_ELEMENT_IMAGE = "Image";
        private const string GUI_IMAGE = "Image1";
        private const string GUI_IMAGE_PICTURE_DATA = "Picture";


        private const string MOBILE_COMMAND_GETWORKAREASIZE = "GetWorkareaSize";
        private const string MOBILE_EVENT_BUTTONCLICK = "ButtonClick";
        private const string MOBILE_EVENT_TABLESELECYEDCHANGED = "TableSelectedChanged";
        private const string MOBILE_EVENT_IMAGEMOUSDOWN = "ImageMouseDown";

        private static int _mobileWidth;
        private static int _mobileHeight;

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
            _client.SendRequest(new SubscribeToSpecialEvent(reader.Id, MOBILE_EVENT_IMAGEMOUSDOWN));

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

        private static string ImageToBase64(this Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }       

        private static string CreateMobileInteface()
        {
            var xml = new XmlDocument();
            var root = xml.CreateAndAppendElement("Window");

            var closeButton = xml.CreateAndAppendElement(GUI_XML_ELEMENT_BUTTON, root);
            closeButton.SetDefaultParameters(GUI_BUTTON_CLOSE, _mobileWidth - 20, 0, 20, 20);
            closeButton.SetAttribute(GUI_XML_ATTR_TEXT, "X");
            closeButton.SetAttribute("StandartCommand", "CloseApplication");

            var image = xml.CreateAndAppendElement(GUI_XML_ELEMENT_IMAGE, root);

            var image1 = new Bitmap(Resources.Image1);
            var base64Data = image1.ImageToBase64(ImageFormat.Bmp);
            image.SetDefaultParameters(GUI_IMAGE, 5, 25, image1.Width, image1.Height, raiseEvents: true);

            image.SetAttribute(GUI_IMAGE_PICTURE_DATA, base64Data);

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
            var message = e.EventMessage as OnReaderSpecialEvent;
            if (message != null)
            {
                Console.WriteLine($"Special event. Reader: {message.Reader.Name}; eventName: {message.EventName}");

                switch (message.EventName)
                {
                    case MOBILE_EVENT_IMAGEMOUSDOWN:
                        foreach (var item in message.Parameters.Items)
                        {
                            Console.WriteLine($"        {item.Name}: {item.Value}"  );                          
                        }
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
