[![in english](http://rfidcenter.ru/img/flag-uk.svg) ENGLISH VERSION](README_EN.md)

Работа с принтером
==================

* [Получение списка принтеров](#GetPrinters)
* [Программирование банков памяти метки](#WriteMultipleBlocksLabelElement)
* [Программирование EPC форматами данных GS1/UNISCAN](#Gs1)
* [Управление доступом, блокировка метки](#Access)
* [Графическая персонализация](#Graphic)


<a name="GetPrinters"></a>
Получение списка принтеров
--------------------------
Получить список доступных в Шине RFID принтеров можно через запрос загруженных принтеров GetPrinters.

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
Программирование банков памяти метки
------------------------------------

Запись произвольных данных в банки памяти метки реализована через элемент WriteMultipleBlocksLabelElement.

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
Программирование EPC форматами данных GS1/UNISCAN
-------------------------------------------------

В Шине RFID реализованы запросы для программирования банка EPC специализированными форматами, такими как SGTIN, GIAI и др.
Пример записи средствами принтера данных  в память метки в формате GIAI-96:

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
Управление доступом, блокировка метки
--------------------------

За установку паролей доступа и уничтожения метки отвечают классы SetAccessPasswordElement и SetKillPasswordElement соответственно.
Для блокировки определённых банков памяти меток используется класс LockTransponderBankElement, а при необходимости заблокировать все банки памяти метки - LockTransponderElement.


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
Графическая персонализация
--------------------------

При печати на метках имеется возможность добавлять такие элементы как текст, изображения, различные геометрические фигуры, штрихкоды.
Пример добавления текста на метку:

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

Пример добавления изображения на метку:

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

В следующем примере рассматривается добавление на метку линии, с указанием начальных и конечных координат, толщины и цвета. Добавление других геометрических фигур производится аналогичным способом, различия заключаются только в параметрах фигур, например, для описания окружности (CircleElement) следует указать координаты центра и радиус.

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

Пример добавления штрихкода на метку. В данном случае применён стандарт Code 128.

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

[⬅ К оглавлению](../README.md)
