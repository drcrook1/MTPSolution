using Microsoft.IoT.DeviceCore.Adc;
using Microsoft.IoT.Devices.Adc;
using Microsoft.IoT.Devices.Sensors;
using MTP.IoT.Devices.Adc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Adc.Provider;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.IoT.DeviceCore.Sensors;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MTP.PGKitSensorsApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GpioController gpioController;
        private AdcProviderManager adcManager;
        private List<double> lightVals;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPageLoaded;
        }

        private async void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            gpioController = GpioController.GetDefault();
            IAdcControllerProvider MCP3008_SPI0 = new McpClassAdc();
            adcManager = new AdcProviderManager();
            lightVals = new List<double>();
            for (int i = 0; i < 10; i++)
            {
                lightVals.Add(0);
            }
            ((McpClassAdc)MCP3008_SPI0).ChannelCount = 8;
            ((McpClassAdc)MCP3008_SPI0).ChannelMode = ProviderAdcChannelMode.SingleEnded;
            ((McpClassAdc)MCP3008_SPI0).ChipSelectLine = 0;
            ((McpClassAdc)MCP3008_SPI0).ControllerName = "SPI0";
            ((McpClassAdc)MCP3008_SPI0).ResolutionInBits = 10;
            adcManager.Providers.Add(MCP3008_SPI0);
            var adcControllers = await adcManager.GetControllersAsync();

            var lightSensor = new AnalogSensor()
            {
                AdcChannel = adcControllers[0].OpenChannel(7),
                ReportInterval = 500,
            };
            lightSensor.ReadingChanged += LightSensor_ReadingChanged;
        }

        private async void LightSensor_ReadingChanged(IAnalogSensor sender, AnalogSensorReadingChangedEventArgs args)
        {
            // Invert
            var reading = args.Reading.Ratio;
            lightVals.Add(reading);
            lightVals.RemoveAt(0);
            var avg = lightVals.Average();
            // Update UI
            await Dispatcher.RunIdleAsync((s) =>
            {
                // Value
                LightProgress.Text = avg.ToString();

                // Color
                if (avg < .25)
                {
                    LightProgress.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (avg < .75)
                {
                    LightProgress.Foreground = new SolidColorBrush(Colors.Yellow);
                }
                else
                {
                    LightProgress.Foreground = new SolidColorBrush(Colors.Green);
                }
            });

        }
    }
}
