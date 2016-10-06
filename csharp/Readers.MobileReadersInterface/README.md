[![in english](http://rfidcenter.ru/img/flag-uk.svg) ENGLISH VERSION](README_EN.md)

Работа с Mobile Thin Client (MTCL)
==================================

MTCL для Шины RFID реализует механизмы для построения тонких клиентов для мобильных RFID
считывателей используя команды и события Шины RFID.

Создание графического интерфейса (GUI) мобильного ТСД реализовано через отправку команды
содержащей описание элементов управления в формате XML. Интерактивность графического
интерфейса достигается через события пользовательского взаимодействия и запросов состояния
именованных элементов управления.

Программирование интерфейса
---------------------------

Для отправки интерфейса на мобильный RFID считыватель необходимо выполнить специальную
команду `SetWindowControls`, параметр `UiXml` которой должен содержать XML описание
окна.

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

Со списком доступных элементов интерфейса можно ознакомиться в документации на MTCL: http://rfidcenter.ru/product/rfidbus/support
