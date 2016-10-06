[![на русском](http://rfidcenter.ru/img/flag-ru.svg) РУССКАЯ ВЕРСИЯ](README.md)

How to work with printers
==================

* [Get A List Of Printers](#GetPrinters)
* [Data recording into the tag's memory banks](#WriteMultipleBlocksLabelElement)
* [EPC memory bank encoding by GS1/UNISCAN data format](#Gs1)
* [Access controll, lock of a tag](#Access)
* [Graphical personalization](#Graphic)

<a name="GetPrinters"></a>
Get A List Of Printers
--------------------------

One can get the list of printers available to RFID Bus by using GetPrinters request.

```cs
RfidBusClient client;
 ...
GetPrintersResponse printersResult = client.SendRequest(new GetPrinters());

Console.WriteLine("Printers list");
foreach (var printerRecord in printersResult.Printers)
{
    Console.WriteLine(" Name: '{0}'; description: '{1}'; model name: '{2}'; model Id: {3}; model description: '{4}'",
            printerRecord.Name,
            printerRecord.Description,
            printerRecord.ModelDescription.Name,
            printerRecord.ModelDescription.Id,
            printerRecord.ModelDescription.Description);
}
```

<a name="WriteMultipleBlocksLabelElement"></a>
Data recording into the tag's memory banks
------------------------------------

One can record data into different memory blocks of the tag by using the WriteMultipleBlocksLabelElement element.

```cs
RfidBusClient client;
PrinterRecord printerRecord;
int startingBlock = 1,
int? writeCount,
var data = new byte[] { 255, 255, 255, 255, 255 };
var label = new PrintLabel
{
    Width = 50,
    Height = 50
};
 ...
var writeEpcElement = new WriteMultipleBlocksLabelElement(data,
        startingBlock,
        writeCount,
        memoryBank: TagMemoryBankEnum.Epc);
label.Elements.Add(writeEpcElement);

var printResult = client.SendRequest(
        new EnqueuePrintLabelTask(printerRecord.Id, label));
if (printResult.Status != ResponseStatus.Ok)
{
   Throw new Exception($"Unable to add a task to the print queue. Reason:" +
           "{printResult.Status}");
}
Console.WriteLine("Label was enqueued to printing");
```

<a name="Gs1"></a>
EPC memory bank encoding by GS1/UNISCAN data format
-------------------------------------------------

Such EPC special formats as SGTIN, GIAI etc. were implemented in RFID Bus for a programming of EPC memory banks. An example of data recording into the tag's memory by the printer at GIAI-96 format:

```cs
const ulong COMPANY_PREFIX = 461000232;
const ulong ASSET = 1000;
RfidBusClient client;
PrinterRecord printerRecord;
 ...
var label = new PrintLabel
{
    Width = 50,
    Height = 50
};

var writeEpcElement = new WriteGiai96LabelElement(
        COMPANY_PREFIX, 
        ASSET,
        EpcFilter.AllOthers, 
        partition);
label.Elements.Add(writeEpcElement);
client.SendRequest(new EnqueuePrintLabelTask(printerRecord.Id, label));
```

<a name="Access"></a>
Access controll, lock of a tag
--------------------------

SetAccessPasswordElement and SetKillPasswordElement classes control the management of access password management and tag kill command respectively. LockTransponderBankElement class is used to lock the specified memory bank and LockTransponderElement is used to lock all memory banks.

```cs
RfidBusClient client;
PrinterRecord printerRecord;
byte[] accessPassword = { 255, 255, 255, 255 };
byte[] killPassword = { 100, 100, 100, 100 };
 ...
var label = new PrintLabel
{
    Width = 50,
    Height = 50
};

var accessPasswordElement = new SetAccessPasswordElement(_AccessPassword);
label.Elements.Add(accessPasswordElement);
var killPasswordElement = new SetKillPasswordElement(_KillPassword);
label.Elements.Add(killPasswordElement);
var lockTransponderElement = new LockTransponderBankElement(TransponderBank.Reserved,
        TransponderBankLockType.Locked,
        _AccessPassword);
label.Elements.Add(lockTransponderElement);

client.SendRequest(new EnqueuePrintLabelTask(printerRecord.Id, label));
```

<a name="Graphic"></a>
Graphical personalization
--------------------------

It is possible to print different graphical elements such as text, images, different geometrical objects, barcodes etc. on the tag's surface. An example of text printing on the tag's surface:

```cs
RfidBusClient client;
PrinterRecord printerRecord;
struct TextParams
{
    public const double X = 5.0;
    public const double Y = 20.0;
    public const double WIDTH = 45.0;
    public const double HEIGHT = 25.0;
    public const double FONT_SIZE = 4;
    public const string FONT_FAMILY = "Arial";
}
 ...
var label = new PrintLabel
{
    Width = 50,
    Height = 50
};
var element = new TextElement(TextParams.X,
        TextParams.Y,
        TextParams.WIDTH,
        TextParams.HEIGHT,
        TextParams.FONT_FAMILY,
        TextParams.FONT_SIZE,
        "Text example");
label.Elements.Add(element);
client.SendRequest(new EnqueuePrintLabelTask(printerRecord.Id, label));
```

An example of image printing on the tag's surface:

```cs
const double IMAGE_POS_X = 1.0;
const double IMAGE_POS_Y = 1.0;
RfidBusClient client;
PrinterRecord printerRecord;

byte[] ImageToByte(Image img)
{
    byte[] byteArray = new byte[0];
    using (MemoryStream stream = new MemoryStream())
    {
         img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
         stream.Close();
         byteArray = stream.ToArray();
     }
     return byteArray;
}
 ...
var label = new PrintLabel
{
    Width = 50,
    Height = 50
};
var logo = new Bitmap(@"C:\Logo.bmp");
var element = new ImageElement(ImageToByte(logo),
        IMAGE_POS_X,
        IMAGE_POS_Y,
        logo.Width,
        logo.Height);
label.Elements.Add(element);
client.SendRequest(new EnqueuePrintLabelTask(printerRecord.Id, label));
```

In the following example, the printing of a line on the tag's surface by specifying coordinates of start and end position and the thickness and the color of this line is considered. An addition of other geometrical objects is performed in a similar way except the description of some different parameters of the figures e.g. for a circle (CircleElement), it is necessary to enter coordinates of the center and the radius value.

```cs
RfidBusClient client;
PrinterRecord printerRecord;
struct LineParams
{
    public const double START_X = 1.0;
    public const double START_Y = 1.0;
    public const double END_X = 5.0;
    public const double END_Y = 20.0;
    public const double THIKNESS = 5.0;
    public static Color color = Color.FromRgb(150, 150, 150);
}
 ...
var label = new PrintLabel
{
    Width = 50,
    Height = 50
};
var element = new LineElement(LineParams.START_X,
        LineParams.START_Y,
        LineParams.END_X,
        LineParams.END_Y,
        LineParams.THIKNESS,
        LineParams.Color);
label.Elements.Add(element);
client.SendRequest(new EnqueuePrintLabelTask(printerRecord.Id, label));
```

An example of barcode printing. The Code 128 is applied in this case.

```cs
const ulong COMPANY_PREFIX = 461000232;
const ulong EXTENSION = 1000;
RfidBusClient client;
PrinterRecord printerRecord;
private struct BarcoreParams
{
    public const double X = 5.0;
    public const double Y = 35.0;
    public const double WIDTH = 40.0;
    public const double HEIGHT = 25.0;
    public const double FONT_SIZE = 2;
}
 ...
var label = new PrintLabel
{
    Width = 50,
    Height = 50
};

var barcode = new Code128BarCodeElement(BarcoreParams.X,
        BarcoreParams.Y,
        BarcoreParams.WIDTH,
        BarcoreParams.HEIGHT,
        codeFontSize: BarcoreParams.FONT_SIZE)
{
    Value = COMPANY_PREFIX.ToString(CultureInfo.InvariantCulture) +
        EXTENSION.ToString(CultureInfo.InvariantCulture)
};
label.Elements.Add(barcode);
client.SendRequest(new EnqueuePrintLabelTask(printerRecord.Id, label));
```

[⬅ Back to contents](../README_EN.md)