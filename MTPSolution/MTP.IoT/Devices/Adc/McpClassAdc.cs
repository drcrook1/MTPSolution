using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Adc.Provider;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;
using Microsoft.IoT.DeviceHelpers;
using System.Collections.Generic;

namespace MTP.IoT.Devices.Adc
{
    /// <summary>
    /// Driver for MCP ADC controllers, configure for your usage.
    /// Current Support is for maximum of 8 channels.
    /// </summary>
    public sealed class MCP3008 : IAdcControllerProvider, IDisposable
    {
        #region Constants
        //0000_0110
        //private const byte COMMAND_SINGLE = 0x06;
        ////0000_0100
        //private const byte COMMAND_DIFF = 0x04;
        // static private readonly byte[] CONFIG_BUFFER = new byte[3] { 0x06, 0x00, 0x00 }; // 00000110 channel configuration data for the MCP3208
        #endregion // Constants

        #region Member Variables
        private int chipSelectLine = 0;         // The chip select line used on the SPI controller
        private int channelCount = 8;
        private int maxValue = 1023;
        private int minValue = 0;
        private int resolutionInBits = 10;
        private string controllerName = "SPI0"; // The name of the SPI controller to use
        private bool isInitialized;
        private SpiDevice spiDevice;            // The SPI device the display is connected to
        #endregion // Member Variables

        #region Constructors
        public MCP3008()
        {
            // Set Defaults
            ChannelMode = ProviderAdcChannelMode.SingleEnded;
        }
        #endregion // Constructors

        #region Internal Methods
        private async Task EnsureInitializedAsync()
        {
            // If already initialized, done
            if (isInitialized) { return; }

            // Validate
            if (string.IsNullOrWhiteSpace(controllerName)) { throw new MissingIoException(nameof(ControllerName)); }

            // Create SPI initialization settings
            var settings = new SpiConnectionSettings(chipSelectLine);

            // Datasheet specifies maximum SPI clock frequency of 0.5MHz
            settings.ClockFrequency = 500000;

            // The ADC expects idle-low clock polarity so we use Mode0
            settings.Mode = SpiMode.Mode0;

            // Find the selector string for the SPI bus controller
            string spiAqs = SpiDevice.GetDeviceSelector(controllerName);

            // Find the SPI bus controller device with our selector string
            var deviceInfo = (await DeviceInformation.FindAllAsync(spiAqs)).FirstOrDefault();

            // Make sure device was found
            if (deviceInfo == null) { throw new DeviceNotFoundException(controllerName); }

            // Create an SpiDevice with our bus controller and SPI settings
            spiDevice = await SpiDevice.FromIdAsync(deviceInfo.Id, settings);

            // Done initializing
            isInitialized = true;
        }
        #endregion // Internal Methods

        #region Public Methods
        public void AcquireChannel(int channel)
        {
            // Validate
            if ((channel < 0) || (channel > ChannelCount)) throw new ArgumentOutOfRangeException("channel");

            // This devices does not operate in exclusive mode, so we'll just ignore
        }

        public int ReadValue(int channelNumber)
        {
            // Validate
            if ((channelNumber < 0) || (channelNumber > ChannelCount)) throw new ArgumentOutOfRangeException("channelNumber");
            EnsureInitializedAsync().Wait();
            // The code below is based on the MCP3008 spec sheet by Microchip
            // Buffers to hold write and read data
            byte[] writeBuffer = new byte[3] { 0x00, 0x00, 0x00 };
            byte[] readBuffer = new byte[3];
            if (ChannelMode == ProviderAdcChannelMode.Differential)
            {
                writeBuffer[0] = 0x01;
                writeBuffer[1] = (byte)((channelNumber) << 4);
            }
            else
            {
                writeBuffer[0] = 0x01;
                writeBuffer[1] = (byte)((channelNumber + 8) << 4);
            }
            // Write command and read data from the ADC in one line
            spiDevice.TransferFullDuplex(writeBuffer, readBuffer);

            //bit mask result to ditch all of first 
            //byte except value
            //This changes depending on ADC resolution
            int result = readBuffer[1] & 0x03;
            //Shift these bits by a full byte 
            //as they are most significant bits
            result <<= 8;
            //Add the second byte as an int to the result.
            //C# rocks.
            result += readBuffer[2];
            //return the result
            return (int)result;
        }

        public void ReleaseChannel(int channel)
        {
            // Validate
            if ((channel < 0) || (channel > ChannelCount)) throw new ArgumentOutOfRangeException("channel");

            // This devices does not operate in exclusive mode, so we'll just ignore
        }

        public void Dispose()
        {
            if (spiDevice != null)
            {
                spiDevice.Dispose();
                spiDevice = null;
            }
            isInitialized = false;
        }

        public bool IsChannelModeSupported(ProviderAdcChannelMode channelMode)
        {
            // All modes currently supported, but in case another mode is added later.
            switch (channelMode)
            {
                case ProviderAdcChannelMode.Differential:
                case ProviderAdcChannelMode.SingleEnded:
                    return true;
                default:
                    return false;
            }
        }
        #endregion // Public Methods


        #region Public Properties
        /// <summary>
        /// Gets or sets the chip select line to use on the SPIO controller.
        /// </summary>
        /// <value>
        /// The chip select line to use on the SPIO controller. The default is 0.
        /// </value>
        [DefaultValue(0)]
        public int ChipSelectLine
        {
            get
            {
                return chipSelectLine;
            }
            set
            {
                if (isInitialized) { throw new IoChangeException(); }
                chipSelectLine = value;
            }
        }

        public int ChannelCount
        {
            get
            {
                return 8;
            }
            set
            {
                if (isInitialized) { throw new IoChangeException(); }
                channelCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the reading mode of the ADC.
        /// </summary>
        /// <value>
        /// A <see cref="ProviderAdcChannelMode"/> that represents the reading mode of the ADC. 
        /// The default is <see cref="ProviderAdcChannelMode.SingleEnded"/>.
        /// </value>
        /// <remarks>
        /// For more information see 
        /// <see href="http://www.maximintegrated.com/en/app-notes/index.mvp/id/1108">
        /// Understanding Single-Ended, Pseudo-Differential and Fully-Differential ADC Inputs
        /// </see>
        /// </remarks>
        [DefaultValue(ProviderAdcChannelMode.SingleEnded)]
        public ProviderAdcChannelMode ChannelMode { get; set; }

        /// <summary>
        /// Gets or sets the name of the SPIO controller to use.
        /// </summary>
        /// <value>
        /// The name of the SPIO controller to use. The default is "SPI0".
        /// </value>
        [DefaultValue("SPI0")]
        public string ControllerName
        {
            get
            {
                return controllerName;
            }
            set
            {
                if (isInitialized) { throw new IoChangeException(); }
                controllerName = value;
            }
        }

        public int MaxValue
        {
            get
            {
                return maxValue;
            }
        }

        public int MinValue
        {
            get
            {
                return minValue;
            }
        }

        public int ResolutionInBits
        {
            get
            {
                return resolutionInBits;
            }
            set
            {
                if (isInitialized) { throw new IoChangeException(); }
                resolutionInBits = value;
                maxValue = (int)Math.Pow(2, value) - 1;
            }
        }
        #endregion // Public Properties
    }
}

