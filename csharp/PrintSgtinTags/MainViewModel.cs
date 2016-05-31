using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using PrintSgtinTags.Properties;

using RfidBus.Primitives.Messages;
using RfidBus.Primitives.Messages.Printers;
using RfidBus.Primitives.Messages.Printers.Elements;
using RfidBus.Primitives.Network;
using RfidBus.Serializers.Ws;

using RfidCenter.Basic;
using RfidCenter.Basic.Arguments;
using RfidCenter.Basic.Encode;

using Code128BarCodeElement = RfidBus.Primitives.Messages.Printers.Elements.BarCodes.Code128BarCodeElement;

namespace PrintSgtinTags
{
    internal class MainViewModel : ViewModelBase
    {
        private RfidBusClient _client;
        private ulong _companyPrefix = Settings.Default.CompanyPrefix;
        private ICommand _connectCommand;
        private bool _isWriteEpcData = Settings.Default.IsWriteEpcData;
        private ObservableCollection<PrinterRecord> _loadedPrinters = new ObservableCollection<PrinterRecord>();
        private ICommand _printCommand;
        private uint _productId = Settings.Default.ProductId;
        private PrinterRecord _selectedPrinter;
        private ulong _startSerial = Settings.Default.StartSerial;
        private int _tagsCount = Settings.Default.TagsCount;

        public bool IsWriteEpcData
        {
            get { return this._isWriteEpcData; }

            set
            {
                if (value != this._isWriteEpcData)
                {
                    this._isWriteEpcData = value;
                    this.RaisePropertyChanged("IsWriteEpcData");
                }
            }
        }

        /// <summary>
        ///     Загруженные принтеры
        /// </summary>
        public ObservableCollection<PrinterRecord> LoadedPrinters
        {
            get { return this._loadedPrinters; }
            set
            {
                if (value != this._loadedPrinters)
                {
                    this._loadedPrinters = value;
                    this.RaisePropertyChanged("LoadedPrinters");
                }
            }
        }

        /// <summary>
        ///     Выбранный принтер
        /// </summary>
        public PrinterRecord SelectedPrinter
        {
            get { return this._selectedPrinter; }
            set
            {
                if (value != this._selectedPrinter)
                {
                    this._selectedPrinter = value;
                    this.RaisePropertyChanged("SelectedPrinter");
                }
            }
        }

        /// <summary>
        ///     Префикс компании
        /// </summary>
        public ulong CompanyPrefix
        {
            get { return this._companyPrefix; }

            set
            {
                if (value != this._companyPrefix)
                {
                    this._companyPrefix = value;
                    this.RaisePropertyChanged("CompanyPrefix");
                }
            }
        }

        /// <summary>
        ///     Идентификатор продукта
        /// </summary>
        public uint ProductId
        {
            get { return this._productId; }
            set
            {
                if (value != this._productId)
                {
                    this._productId = value;
                    this.RaisePropertyChanged("ProductId");
                }
            }
        }

        /// <summary>
        ///     Серийный номер
        /// </summary>
        public ulong StartSerial
        {
            get { return this._startSerial; }
            set
            {
                if (value != this._startSerial)
                {
                    this._startSerial = value;
                    this.RaisePropertyChanged("StartSerial");
                }
            }
        }

        /// <summary>
        ///     Количество меток для печати
        /// </summary>
        public int TagsCount
        {
            get { return this._tagsCount; }
            set
            {
                if (value != this._tagsCount)
                {
                    this._tagsCount = value;
                    this.RaisePropertyChanged("TagsCount");
                }
            }
        }

        /// <summary>
        ///     Команда печати
        /// </summary>
        public ICommand PrintCommand
        {
            get
            {
                return this._printCommand ??
                       (this._printCommand = new RelayCommand(execute => this.Print(), predicate => this.IsPrintAvailable()));
            }
        }

        /// <summary>
        ///     Команда подключения к шине RFID
        /// </summary>
        public ICommand ConnectCommand
        {
            get
            {
                return this._connectCommand ??
                       (this._connectCommand = new RelayCommand(execute => this.Connect(), predicate => this.IsConnectAvailable()));
            }
        }

        ~MainViewModel()
        {
            if ((this._client != null) && this._client.IsConnected)
            {
                this._client.Close();
            }
        }

        /// <summary>
        ///     Предикат доступности кнопки подключения к шине RFID
        /// </summary>
        /// <returns></returns>
        private bool IsConnectAvailable()
        {
            return (this._client == null) || !this._client.IsConnected;
        }

        /// <summary>
        ///     Предикат доступности кнопки печати меток
        /// </summary>
        /// <returns></returns>
        private bool IsPrintAvailable()
        {
            return (this._client != null) && this._client.IsConnected && (this.SelectedPrinter != null) && (this.CompanyPrefix > 0) && (this.ProductId > 0) && (this.StartSerial > 0) && (this.TagsCount > 0);
        }

        /// <summary>
        ///     Метод выполняет соединения с шиной RFID
        /// </summary>
        private async void Connect()
        {
            try
            {
                var pbCommunication = new WsCommunicationDescription();
                var config = new ParametersValues(pbCommunication.GetClientConfiguration());
                config.SetValue(ConfigConstants.PARAMETER_HOST, Settings.Default.BusHost);
                config.SetValue(ConfigConstants.PARAMETER_PORT, Settings.Default.BusPort);

                this._client = new RfidBusClient(pbCommunication, config);

                if (!this._client.Authorize(Settings.Default.BusLogin, Settings.Default.BusPassword))
                {
                    throw new BaseException(RfidErrorCode.InvalidLoginAndPassword);
                }

                this._client.ReceivedEvent += RfidBusReceivedEvent;
                var result = await this._client.SendRequestAsync(new GetPrinters());
                if (result.Status != ResponseStatus.Ok)
                {
                    throw new BaseException(string.Format("Ошибка авторизации. Код статуса: {0}", result.Status));
                }

                this.LoadedPrinters.Clear();

                if (result.Printers.Length == 0)
                {
                    return;
                }

                foreach (var printer in result.Printers)
                {
                    this.LoadedPrinters.Add(printer);
                }

                this.SelectedPrinter = this.LoadedPrinters.First();
            }

            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Не удалось установить соединение с шиной RFID. {0}", ex.Message));
            }
        }

        private static void RfidBusReceivedEvent(object sender, ReceivedEventEventArgs args)
        {
            var message = args.EventMessage as PrintTaskStateChanged;
            if (message == null)
            {
                return;
            }
            switch (message.State)
            {
                case PrintOperationState.Canceled:

                    MessageBox.Show(string.Format("Печать метки была отменена."));
                    break;

                case PrintOperationState.Failed:

                    MessageBox.Show(string.Format("Не удалось произвести печать метки. {0}", message.FailReason));
                    break;
            }
        }

        /// <summary>
        ///     Метод выполняет печать информации на метке
        /// </summary>
        private async void Print()
        {
            if ((this._client == null) || !this._client.IsConnected)
            {
                MessageBox.Show("Нет установленного соединения с шиной RFID");
                return;
            }
            try
            {
                //кодировщик
                var encoder = new EpcSgtin96Encoder(this.CompanyPrefix, this.ProductId, this.StartSerial);

                //баркод
                var barcode = new Code128BarCodeElement(5, 35, 40, 25, codeFontSize: 2)
                              {
                                  Value = this.CompanyPrefix.ToString(CultureInfo.InvariantCulture) + this.ProductId
                              };

                for (var i = 0; i < this.TagsCount; i++)
                {
                    var label = new PrintLabel
                                {
                                    Width = 50,
                                    Height = 50
                                };

                    //текстовый элемент epc
                    var epcElement = new TextElement(5, 20, 45, 25, "Arial", 4, BaseTools.GetStringFromBinary(encoder.Epc));

                    label.Elements.Add(epcElement);
                    label.Elements.Add(barcode);

                    if (this.IsWriteEpcData)
                    {
                        //элемент записи rfid данных
                        var writeEpcElement = new WriteDataLabelElement(encoder.Epc, retries: 1);
                        label.Elements.Add(writeEpcElement);
                    }

                    var printResult =
                        await this._client.SendRequestAsync(new EnqueuePrintLabelTask(this.SelectedPrinter.Id, label));

                    if (printResult.Status != ResponseStatus.Ok)
                    {
                        MessageBox.Show(string.Format("Не удалось добавить задачу в очередь на печать. Код статуса: {0}", printResult.Status));
                    }

                    encoder++;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Не удалось добавить задачу на печать. Описание: {0}", ex.Message));
            }
        }
    }
}
