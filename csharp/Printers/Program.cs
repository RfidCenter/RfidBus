using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;

using RfidBus.Primitives.Messages;
using RfidBus.Primitives.Messages.Printers;
using RfidBus.Primitives.Messages.Printers.Elements.BarCodes;
using RfidBus.Primitives.Network;
using RfidBus.Serializers.Ws;

using RfidCenter.Basic.Arguments;
using RfidCenter.Basic.Encode;

using Printers.Properties;

using RfidBus.Primitives.Messages.Printers.Elements;
using RfidCenter.Devices;
using RfidCenter.Printing.Elements;
using Color = System.Windows.Media.Color;
using ImageElement = RfidBus.Primitives.Messages.Printers.Elements.ImageElement;
using TextElement = RfidBus.Primitives.Messages.Printers.Elements.TextElement;
using WriteGiai96LabelElement = RfidBus.Primitives.Messages.Printers.Elements.WriteGiai96LabelElement;
using WriteMultipleBlocksLabelElement = RfidBus.Primitives.Messages.Printers.Elements.WriteMultipleBlocksLabelElement;
using WriteSgtin96LabelElement = RfidBus.Primitives.Messages.Printers.Elements.WriteSgtin96LabelElement;
using WriteSscc96LabelElement = RfidBus.Primitives.Messages.Printers.Elements.WriteSscc96LabelElement;

namespace Printers
{
    // http://rfidcenter.ru/demo/rfidbus/PrinterEmulatorClient.html
    // ws://127.0.0.1:9301/printer/

    internal class Program
    {
        private const int LABEL_WIDTH = 50;
        private const int LABEL_HEIGHT = 50;

        private const ulong COMPANY_PREFIX = 461000232;
        private const uint PRODUCT_ID = 800008;
        private const ulong START_SERIAL = 1;
        private const ulong EXTENSION = 1000;
        private const ulong ASSET = 1000;
        private const int START_BLOCK = 1;

        private const int GIAI_MENU_ITEM_NUMBER = 1;
        private const int SGTIN_MENU_ITEM_NUMBER = 2;
        private const int SSCC_MENU_ITEM_NUMBER = 3;
        private const int WRITE_MULTIPLE_BLOCKS_LABEL_ELEMENT = 4;
        private static RfidBusClient _client;

        private static readonly byte[] _AccessPassword = {255, 255, 255, 255};
        private static readonly byte[] _KillPassword = {255, 255, 255, 255};

        private static readonly Dictionary<int, Guid> _printersList = new Dictionary<int, Guid>();

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

        private static void Main(string[] args)
        {
            Initialize();

            var printersResult = _client.SendRequest(new GetPrinters());
            var printerNumber = 0;

            Console.WriteLine("Printers' list");
            foreach (var printerRecord in printersResult.Printers)
            {
                if (printerRecord.IsActive)
                {
                    printerNumber++;
                    _printersList.Add(printerNumber, printerRecord.Id);

                    Console.WriteLine($" {printerNumber}. Name: '{printerRecord.Name}'; model name: " +
                                      $"'{printerRecord.ModelDescription.Name}'; Id: {printerRecord.Id}");
                }
            }

            Console.WriteLine();
            Console.WriteLine("Enter printer number:");
            var selectedPrinterNumber = 0;

            var isEnteredValueNumber = false;
            var isPrintersExists = false;
            while (!isEnteredValueNumber || !isPrintersExists)
            {
                isEnteredValueNumber = int.TryParse(Console.ReadLine(), out selectedPrinterNumber);
                isPrintersExists = _printersList.ContainsKey(selectedPrinterNumber);
            }

            var selectedPrinterId = _printersList[selectedPrinterNumber];
            Console.WriteLine();
            Console.WriteLine($"Selected printer number = {selectedPrinterNumber}; GUID: {selectedPrinterId}");

            var label = new PrintLabel
                        {
                            Width = LABEL_WIDTH,
                            Height = LABEL_HEIGHT
                        };

            Console.WriteLine();
            Console.WriteLine("Select EPC encoder:");
            Console.WriteLine();
            Console.WriteLine($" {GIAI_MENU_ITEM_NUMBER} - GIAI");
            Console.WriteLine($" {SGTIN_MENU_ITEM_NUMBER} - SGTIN");
            Console.WriteLine($" {SSCC_MENU_ITEM_NUMBER} - SSCC");
            Console.WriteLine($" {WRITE_MULTIPLE_BLOCKS_LABEL_ELEMENT} - Write custom data");
            Console.WriteLine();
            Console.WriteLine("Enter number:");

            var selectedEncoder = 0;
            var isValueInMenuItems = false;

            while (!isEnteredValueNumber || !isValueInMenuItems)
            {
                isEnteredValueNumber = int.TryParse(Console.ReadLine(), out selectedEncoder);
                isValueInMenuItems = (selectedEncoder >= GIAI_MENU_ITEM_NUMBER) &&
                                     (selectedEncoder <= WRITE_MULTIPLE_BLOCKS_LABEL_ELEMENT);
            }

            switch (selectedEncoder)
            {
                case GIAI_MENU_ITEM_NUMBER:
                    AddGiaiEpcElement(label, COMPANY_PREFIX, ASSET);

                    AddTextElement(label,
                                   TextParams.X,
                                   TextParams.Y,
                                   TextParams.WIDTH,
                                   TextParams.HEIGHT,
                                   TextParams.FONT_FAMILY,
                                   TextParams.FONT_SIZE,
                                   "EpcGiai96Encoder");

                    AddBarcode(label,
                               BarcoreParams.X,
                               BarcoreParams.Y,
                               BarcoreParams.WIDTH,
                               BarcoreParams.HEIGHT,
                               BarcoreParams.FONT_SIZE,
                               COMPANY_PREFIX.ToString(CultureInfo.InvariantCulture) +
                               ASSET.ToString(CultureInfo.InvariantCulture));
                    break;

                case SGTIN_MENU_ITEM_NUMBER:
                    AddSgtinEpcElement(label, COMPANY_PREFIX, PRODUCT_ID, START_SERIAL);

                    AddTextElement(label,
                                   TextParams.X,
                                   TextParams.Y,
                                   TextParams.WIDTH,
                                   TextParams.HEIGHT,
                                   TextParams.FONT_FAMILY,
                                   TextParams.FONT_SIZE,
                                   "EpcSgtin96Encoder");

                    AddBarcode(label,
                               BarcoreParams.X,
                               BarcoreParams.Y,
                               BarcoreParams.WIDTH,
                               BarcoreParams.HEIGHT,
                               BarcoreParams.FONT_SIZE,
                               COMPANY_PREFIX.ToString(CultureInfo.InvariantCulture) +
                               PRODUCT_ID.ToString(CultureInfo.InvariantCulture));
                    break;

                case SSCC_MENU_ITEM_NUMBER:
                    AddSsccEpcElement(label, COMPANY_PREFIX, EXTENSION);

                    AddTextElement(label,
                                   TextParams.X,
                                   TextParams.Y,
                                   TextParams.WIDTH,
                                   TextParams.HEIGHT,
                                   TextParams.FONT_FAMILY,
                                   TextParams.FONT_SIZE,
                                   "EpcSscc96Encoder");

                    AddBarcode(label,
                               BarcoreParams.X,
                               BarcoreParams.Y,
                               BarcoreParams.WIDTH,
                               BarcoreParams.HEIGHT,
                               BarcoreParams.FONT_SIZE,
                               COMPANY_PREFIX.ToString(CultureInfo.InvariantCulture) +
                               EXTENSION.ToString(CultureInfo.InvariantCulture));
                    break;

                case WRITE_MULTIPLE_BLOCKS_LABEL_ELEMENT:
                    AddTextElement(label,
                                   TextParams.X,
                                   TextParams.Y,
                                   TextParams.WIDTH,
                                   TextParams.HEIGHT,
                                   TextParams.FONT_FAMILY,
                                   TextParams.FONT_SIZE,
                                   "Write custom data");

                    var data = new byte[] {255, 255, 255, 255, 255, 255};
                    AddEpcElement(label, data, START_BLOCK, data.Length, TransponderBank.Epc);

                    AddBarcode(label,
                               BarcoreParams.X,
                               BarcoreParams.Y,
                               BarcoreParams.WIDTH,
                               BarcoreParams.HEIGHT,
                               BarcoreParams.FONT_SIZE,
                               COMPANY_PREFIX.ToString(CultureInfo.InvariantCulture) +
                               EXTENSION.ToString(CultureInfo.InvariantCulture));
                    break;
            }

            var logo = new Bitmap(Resources.Logo);

            AddLineElement(label,
                           ImageParams.X,
                           ImageParams.Y + logo.Height + 1,
                           label.Width,
                           ImageParams.Y + logo.Height + 1,
                           color: Color.FromRgb(150, 150, 150));
            AddImageElement(label, ImageToByte(logo), ImageParams.X, ImageParams.Y, logo.Width, logo.Height);

            var accessPasswordElement = new SetAccessPasswordElement(_AccessPassword);
            label.Elements.Add(accessPasswordElement);

            var killPasswordElement = new SetKillPasswordElement(_KillPassword);
            label.Elements.Add(killPasswordElement);

            var lockTransponderElement = new LockTransponderBankElement(TransponderBank.Reserved,
                                                                        TransponderBankLockType.Locked,
                                                                        _AccessPassword);
            label.Elements.Add(lockTransponderElement);

            var printResult = _client.SendRequest(new EnqueuePrintLabelTask(selectedPrinterId, label));
            Console.WriteLine("Label was enqueued to printing");

            if (printResult.Status != ResponseStatus.Ok)
            {
                Console.WriteLine($"Unable to add a task to the print queue. Reason: {printResult.Status}");
            }

            WaitForKey("Press ENTER to exit.", ConsoleKey.Enter);
            _client.Close();
        }

        public static byte[] ImageToByte(Image img)
        {
            var byteArray = new byte[0];
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }

        private static void AddLineElement(PrintLabel label, double x1, double y1, double x2, double y2, double thikness = 0.5D, Color color = default(Color))
        {
            var element = new RfidBus.Primitives.Messages.Printers.Elements.LineElement(x1, y1, x2, y2, thikness, color);
            label.Elements.Add(element);
        }

        private static void AddBarcode(PrintLabel label,
                                       double x,
                                       double y,
                                       double width,
                                       double height,
                                       double codeFontSize,
                                       string value)
        {
            var barcode = new Code128BarCodeElement(x, y, width, height, codeFontSize: codeFontSize)
                          {
                              Value = value
                          };
            label.Elements.Add(barcode);
        }

        private static void AddImageElement(PrintLabel label,
                                            byte[] imageData,
                                            double x,
                                            double y,
                                            double width,
                                            double height,
                                            double angle = 0)
        {
            var element = new ImageElement(imageData, x, y, width, height, angle);
            label.Elements.Add(element);
        }

        private static void AddTextElement(PrintLabel label,
                                           double x,
                                           double y,
                                           double width,
                                           double height,
                                           string fontFamily,
                                           double fontSize,
                                           string text)
        {
            var element = new TextElement(x, y, width, height, fontFamily, fontSize, text);
            label.Elements.Add(element);
        }

        private static void AddGiaiEpcElement(PrintLabel label,
                                              ulong companyPrefix,
                                              ulong asset,
                                              byte partition = 0)
        {
            var writeEpcElement = new WriteGiai96LabelElement(companyPrefix, asset, EpcFilter.AllOthers, partition);
            label.Elements.Add(writeEpcElement);
        }

        private static void AddSsccEpcElement(PrintLabel label,
                                              ulong companyPrefix,
                                              ulong extension,
                                              byte partition = 0)
        {
            var writeEpcElement = new WriteSscc96LabelElement(companyPrefix, extension, EpcFilter.AllOthers, partition);
            label.Elements.Add(writeEpcElement);
        }

        private static void AddSgtinEpcElement(PrintLabel label,
                                               ulong companyPrefix,
                                               uint productId,
                                               ulong startSerial,
                                               byte partition = 0)
        {
            var writeEpcElement = new WriteSgtin96LabelElement(companyPrefix, productId, startSerial, EpcFilter.AllOthers, partition);
            label.Elements.Add(writeEpcElement);
        }

        private static void AddEpcElement(PrintLabel label,
                                          byte[] data,
                                          int startingBlock,
                                          int? writeCount,
                                          TransponderBank memoryBank)
        {
            var writeEpcElement = new WriteMultipleBlocksLabelElement(data, startingBlock, writeCount, memoryBank: memoryBank);

            label.Elements.Add(writeEpcElement);
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

        private struct TextParams
        {
            public const double X = 5.0;
            public const double Y = 20.0;
            public const double WIDTH = 45.0;
            public const double HEIGHT = 25.0;
            public const double FONT_SIZE = 4;
            public const string FONT_FAMILY = "Arial";
        }

        private struct BarcoreParams
        {
            public const double X = 5.0;
            public const double Y = 35.0;
            public const double WIDTH = 40.0;
            public const double HEIGHT = 25.0;
            public const double FONT_SIZE = 2;
        }

        private struct ImageParams
        {
            public const double X = 1.0;
            public const double Y = 1.0;
        }
    }
}
