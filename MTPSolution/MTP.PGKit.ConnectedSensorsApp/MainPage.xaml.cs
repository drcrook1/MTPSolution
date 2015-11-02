using Microsoft.IoT.DeviceCore.Adc;
using Microsoft.IoT.DeviceCore.Sensors;
using Microsoft.IoT.Devices.Sensors;
using MTP.IoT.Devices.Adc;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Adc.Provider;
using Windows.Devices.Gpio;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Azure.Devices.Client;
using System.Text;
using MTP.DeviceCore.TelemetryObj;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MTP.PGKit.ConnectedSensorsApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Variables required for project
        private GpioController gpioController;
        private AdcProviderManager adcManager;
        //List used to average values, as you will get weird sensory input occasionally.
        //This helps smooth the output.
        private List<double> lightVals;
        private List<double> tempVals;
        DeviceClient deviceClient;
        private string Location = "MIC Miami";
        private string DeviceId = "PGKit1";

            public MainPage()
            {
                this.InitializeComponent();
                //When page is loaded, we will do things as we do use the UI in this project.
                this.Loaded += MainPageLoaded;
            }

            private async void MainPageLoaded(object sender, RoutedEventArgs e)
            {
                //Initialize objects
                gpioController = GpioController.GetDefault();
                IAdcControllerProvider MCP3008_SPI0 = new McpClassAdc();
                adcManager = new AdcProviderManager();
                lightVals = new List<double>();
                deviceClient = DeviceClient
                                .CreateFromConnectionString("HostName=TelemetryHub.azure-devices.net;DeviceId=PGKit1;SharedAccessKey=TuU5wrAKXt6+LtxkRUXplGB9yR/x2xCEyX7pSkERtcM=", 
                                TransportType.Http1);

                //Insert 10 dummy values to initialize.
                for (int i = 0; i < 30; i++)
                {
                    lightVals.Add(0);
                }

                tempVals = new List<double>();
                for (int i = 0; i < 30; i++)
                {
                    tempVals.Add(0);
                }
                //Initialize MCP3008 device.
                //Remember, 8 channels, 10 bit resolution
                //We wired up to SPI0 of pi with Chip select 0.
                ((McpClassAdc)MCP3008_SPI0).ChannelCount = 8;
                ((McpClassAdc)MCP3008_SPI0).ChannelMode = ProviderAdcChannelMode.SingleEnded;
                ((McpClassAdc)MCP3008_SPI0).ChipSelectLine = 0;
                ((McpClassAdc)MCP3008_SPI0).ControllerName = "SPI0";
                ((McpClassAdc)MCP3008_SPI0).ResolutionInBits = 10;
                #region ADC Provider Stuff
                //Add ADC Provider to list of providers
                adcManager.Providers.Add(MCP3008_SPI0);
                //Get all ADC Controllers.
                var adcControllers = await adcManager.GetControllersAsync();
                //This is just how its done. 
                #endregion ADC Provider Stuff
                var lightSensor = new AnalogSensor()
                {
                    //Notice access via controller index.
                    //You will need to keep tabs on your 
                    //ADC providers locations in the list
                    //Channel 7 as this is where we wired the photo resistor to.
                    AdcChannel = adcControllers[0].OpenChannel(7),
                    //every 500 milliseconds, grab read the value.
                    ReportInterval = 250
                };
                //Attach a function to the event we fire every 500 milliseconds.
                lightSensor.ReadingChanged += LightSensor_ReadingChanged;

                #region Temp Sensor Cheat Codes
                var tempSensor = new AnalogSensor()
                {
                    AdcChannel = adcControllers[0].OpenChannel(6),
                    ReportInterval = 1000
                };
                tempSensor.ReadingChanged += TempSensor_ReadingChanged;
                #endregion Temp Sensor Cheat Codes
            }

        private async void TempSensor_ReadingChanged(IAnalogSensor sender, AnalogSensorReadingChangedEventArgs args)
        {
            var reading = args.Reading.Value;
            tempVals.Add(((9 / 5) * (reading / 10)) + 32);
            tempVals.RemoveAt(0);
            double temp = tempVals.Average();
            #region comment this out if running headless.
            await Dispatcher.RunIdleAsync((s) =>
            {
                // Value
                TemperatureProgress.Text = "Temperature: " + temp.ToString() + "deg F";

                // Color
                if (temp < 40)
                {
                    TemperatureProgress.Foreground = new SolidColorBrush(Colors.LightBlue);
                }
                else if (temp < 80)
                {
                    TemperatureProgress.Foreground = new SolidColorBrush(Colors.Yellow);
                }
                else
                {
                    TemperatureProgress.Foreground = new SolidColorBrush(Colors.Red);
                }
            });
            #endregion comment this out if running headless.
            TemperatureEvent te = new TemperatureEvent();
            te.Location = this.Location;
            te.DeviceId = this.DeviceId;
            te.Temperature = (float)temp;
            te.CollectionTime = DateTime.UtcNow;
            var text = te.ToString();
            var msg = new Message(Encoding.UTF8.GetBytes(text));
            try {
                await deviceClient.SendEventAsync(msg);
            }
            catch(Exception e) { }

        }

            /// <summary>
            /// Event Handler for Light Sensor Reading Changed.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="args"></param>
            private async void LightSensor_ReadingChanged(IAnalogSensor sender, AnalogSensorReadingChangedEventArgs args)
            {
                //Read the ratio, which is readvalue/maxvalue
                var reading = args.Reading.Ratio;
                //Add reading to end of list
                lightVals.Add(reading * 100);
                //remove first reading.  Constant size of list is 10.
                lightVals.RemoveAt(0);
                //Smooth the reading out by averaging all 10
                //this is average light value across 5 seconds
                //Note: You occasionally get bad values, hence the smoothing. 
                var avg = lightVals.Average();
                // Update UI
                #region comment this out if running headless.
                await Dispatcher.RunIdleAsync((s) =>
                {
                    // Value
                    LightProgress.Text = "Light: " + avg.ToString() + "%";

                    // Color
                    if (avg < 25)
                    {
                        LightProgress.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    }
                    else if (avg < 75)
                    {
                        LightProgress.Foreground = new SolidColorBrush(Colors.Yellow);
                    }
                    else
                    {
                        LightProgress.Foreground = new SolidColorBrush(Colors.Green);
                    }
                });
                #endregion comment this out if running headless.
            }
        }
    }
