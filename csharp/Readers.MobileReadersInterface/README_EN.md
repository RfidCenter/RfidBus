[![на русском](http://rfidcenter.ru/img/flag-ru.svg) РУССКАЯ ВЕРСИЯ](README.md)

How to work with Mobile Thin Client (MTCL)
==================================

RFID Bus Mobile Thin Client is used to build thin clients for mobile RFID readers by using commands and events of RFID Bus.

Creating of Graphical User Interface (GUI) of handheld RFID reader is realised by  sending a command with the description of controls in XML format. The interactivity of GUI is reached by the combination of events of the user's interactions and requests of the states of called controls.

Interface programming
---------------------------

For a sending of the interface to the handheld RFID reader, it is necessary to execute special command `SetWindowControls`, which parameter `UiXml` has to contain the description of a window in XML format.

```xml
<Window>
    <Label Left="55" Top="30" Width="160" Height="30"
           Text="Hello world!">
        <Font Family="Arial" Size="22" Style="Bold" />
    </Label>
    <Button Id="HeaderButton" RaiseEvents="True" IsEnabled="True"
            Left="40" Top="80" Width="160" Height="54"
            Text="Click to close" StandartCommand="CloseApplication"/>
</Window>
```

Here you can study the list of available elements of the interface in the MTCL documentation: http://rfidcenter.ru/en/product/rfidbus/support