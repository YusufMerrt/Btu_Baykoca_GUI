/* Defines */
//#define PAYLOAD_ON
#define ROCKET_ON

#if !PAYLOAD_ON
#define PAYLOAD_OFF
#endif
#if !ROCKET_ON
#define ROCKET_OFF
#endif

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace BTU_Saturn_GUI
{
    public partial class FormGUI : Form
    {
        /* This variable defines tlm data size */
        public const int MAX_R_TLM_DATA = 38;
        public const int MAX_P_TLM_DATA = 10;
        /* This variable defines graph min to max size */
        public const int GRAPH_SIZE = 60;

        /* Variable definitions */
        /* Serial Data Variables */
        bool processStatusR = false;
        bool processStatusP = false;
        bool getTlmDataR = false;
        bool getTlmDataP = false;
        byte[] receivedDataR = new byte[1];
        byte[] receivedDataP = new byte[1];
        byte[] tlmR = new byte[MAX_R_TLM_DATA];
        byte[] tlmP = new byte[MAX_P_TLM_DATA];
        int dataCounterR = 0;
        int dataCounterP = 0;

        /* Telemetry Variables */
        int packetCounter = 0;
        int year, month, day, hour, minute, second;
        double pressure = 0;
        int state = 0;
        double altitude = 0;
        double speed = 0;
        double acceleration = 0;
        double temperature = 0;
        double rocketXZ = 0;
        double rocketXY = 0;
        double payloadXZ = 0;
        double payloadXY = 0;

        /* Graph Variables */
        long max = 0, min = -GRAPH_SIZE;

        public FormGUI()
        {
            InitializeComponent();
        }

        private void FormGUI_Load(object sender, EventArgs e)
        {
#if ROCKET_OFF
            comboBoxPortRocket.Enabled = false;
            comboBoxBaudRocket.Enabled = false;
            chartAltitude.Visible = false;
            chartSpeed.Visible = false;
            chartAcceleration.Visible = false;
            chartTemperature.Visible = false;
            chartRocketXZ.Visible = false;
            chartRocketXY.Visible = false;
            textBoxPacket.Visible = false;
            textBoxTime.Visible = false;
            textBoxPressure.Visible = false;
            textBoxAltitude.Visible = false;
            textBoxSpeed.Visible = false;
            textBoxAccelaration.Visible = false;
            textBoxTemperature.Visible = false;
            textBoxState.Visible = false;
            textBoxRocketLatitude.Visible = false;
            textBoxRocketLongitude.Visible = false;
#endif
#if PAYLOAD_OFF
            comboBoxPortPayload.Enabled = false;
            comboBoxBaudPayload.Enabled = false;
            chartPayloadXZ.Visible = false;
            chartPayloadXY.Visible = false;
            textBoxPayloadLatitude.Visible = false;
            textBoxPayloadLongitude.Visible = false;
#endif
            /* Get availables port names */
            string[] ports = SerialPort.GetPortNames();

#if ROCKET_ON
            /* Add ports to combo box */
            foreach (string port in ports)
            {
                comboBoxPortRocket.Items.Add(port);
            }

            /* Set default index of combo boxes */
            if (comboBoxPortRocket.Items.Count > 0)
            {
                //comboBoxPortRocket.SelectedIndex = 1;
            }
            comboBoxBaudRocket.SelectedIndex = comboBoxBaudRocket.FindStringExact("115200");

            /* Enable event handler for serial port */
            serialPortRocket.DataReceived += new SerialDataReceivedEventHandler(RocketDataReceived);
#endif
#if PAYLOAD_ON
            /* Add ports to combo box */
            foreach (string port in ports)
            {
                comboBoxPortPayload.Items.Add(port);
            }

            /* Set default index of combo boxes */
            if (comboBoxPortPayload.Items.Count > 0)
            {
                comboBoxPortPayload.SelectedIndex = 1;
            }
            comboBoxBaudPayload.SelectedIndex = comboBoxBaudPayload.FindStringExact("115200");

            /* Enable event handler for serial port */
            serialPortPayload.DataReceived += new SerialDataReceivedEventHandler(PayloadDataReceived);
#endif
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
#if ROCKET_ON
            /* Set port name and baud rate for Payload */
            serialPortRocket.PortName = comboBoxPortRocket.SelectedItem.ToString();
            serialPortRocket.BaudRate = Int32.Parse(comboBoxBaudRocket.SelectedItem.ToString());

            /* Open Serial Port */
            if (serialPortRocket.IsOpen == false)
            {
                try
                {
                    serialPortRocket.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
#endif

#if PAYLOAD_ON
            /* Set port name and baud rate for Rocket */
            serialPortPayload.PortName = comboBoxPortPayload.SelectedItem.ToString();
            serialPortPayload.BaudRate = Int32.Parse(comboBoxBaudPayload.SelectedItem.ToString());

            /* Open Serial Port */
            if (serialPortPayload.IsOpen == false)
            {
                try
                {
                    serialPortPayload.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
#endif
        }

#if ROCKET_ON
        private void RocketDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPortRocket.IsOpen)
            {
                try
                {
                    /* Get Start Command */
                    if (getTlmDataR == false)
                    {
                        serialPortRocket.Read(tlmR, 0, 1);
                        /* 0x24 = $ */
                        if (tlmR[0] == 0x24)
                        {
                            getTlmDataR = true;
                        }
                    }
                    /* Get rest of data */
                    if (getTlmDataR == true)
                    {
                        while (serialPortRocket.BytesToRead > MAX_R_TLM_DATA - 2)
                        {
                            serialPortRocket.Read(tlmR, 1, MAX_R_TLM_DATA - 1);

                            processStatusR = true;
                            getTlmDataR = false;
                        }
                    }
                }
                catch (System.IO.IOException)
                {
                    return;
                }
                catch (System.InvalidOperationException)
                {
                    return;
                }

                /* Precess readed data */
                if (processStatusR == true)
                {
                    /* Note: Use BeginInvoke to not freze winform while closing */
                    this.BeginInvoke(new EventHandler(updateGraphRocket));
                    processStatusR = false;
                }
            }
        }
#endif

#if PAYLOAD_ON
        private void PayloadDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPortRocket.IsOpen)
            {
                try
                {
                    /* Get Start Command */
                    if (getTlmDataP == false)
                    {
                        serialPortRocket.Read(tlmP, 0, 1);
                        /* 0x24 = $ */
                        if (tlmP[0] == 0x24)
                        {
                            getTlmDataP = true;
                        }
                    }
                    /* Get rest of data */
                    if (getTlmDataP == true)
                    {
                        while (serialPortRocket.BytesToRead > MAX_P_TLM_DATA - 2)
                        {
                            serialPortRocket.Read(tlmP, 1, MAX_P_TLM_DATA - 1);

                            processStatusP = true;
                            getTlmDataP = false;
                        }
                    }
                }
                catch (System.IO.IOException)
                {
                    return;
                }
                catch (System.InvalidOperationException)
                {
                    return;
                }

                /* Precess readed data */
                if (processStatusP == true)
                {
                    /* Note: Use BeginInvoke to not freze winform while closing */
                    //this.BeginInvoke(new EventHandler(updatePayloadGraph));
                    processStatusP = false;
                }
            }
        }
#endif

#if ROCKET_ON
        private void updateGraphRocket(object s, EventArgs e)
        {
            /* Decode Data */
            dataCounterR = 8;

            packetCounter = decodeR(16);
            year = decodeR(8);
            month = decodeR(8);
            day = decodeR(8);
            hour = decodeR(8);
            minute = decodeR(8);
            second = decodeR(8);
            state = decodeR(8);

            pressure = decodeR(32);
            altitude = decodeR(32);
            speed = decodeR(32);
            acceleration = decodeR(32);
            temperature = decodeR(32);
            rocketXZ = decodeR(32);
            rocketXY = decodeR(32);

            textBoxPacket.Text = packetCounter.ToString();
            textBoxTime.Text = hour.ToString() + ":" + minute.ToString() + ":" + second.ToString();
            textBoxPressure.Text = (pressure / 100).ToString() + "hPa";
            textBoxAltitude.Text = (altitude / 100).ToString() + "m";
            textBoxSpeed.Text = (speed / 100).ToString() + "m/s";
            textBoxAccelaration.Text = (acceleration / 100).ToString() + "m/s^2";
            textBoxTemperature.Text = (temperature / 100).ToString() + "C°";
            textBoxState.Text = state.ToString();
            textBoxRocketLatitude.Text = rocketXY.ToString();
            textBoxRocketLongitude.Text = rocketXZ.ToString();

            /* Wait 5 sample to stabilize data */
            if (max > 5)
            {
                /* Clear unnecessary points to rescale */
                if (max > 1.5 * GRAPH_SIZE)
                {
                    this.chartAltitude.Series[0].Points.RemoveAt(0);
                }

                /* Plot Altitude Data */
                chartAltitude.ChartAreas[0].AxisX.Minimum = min;
                chartAltitude.ChartAreas[0].AxisX.Maximum = max;
                chartAltitude.ChartAreas[0].AxisX.ScaleView.Zoom(min, max);
                chartAltitude.ChartAreas[0].AxisY.IsStartedFromZero = false;
                chartAltitude.ChartAreas[0].RecalculateAxesScale();
                this.chartAltitude.Series[0].Points.AddXY(max, altitude / 100);

                /* Clear unnecessary points to rescale */
                if (max > 1.5 * GRAPH_SIZE)
                {
                    this.chartSpeed.Series[0].Points.RemoveAt(0);
                }

                /* Plot Speed Data */
                chartSpeed.ChartAreas[0].AxisX.Minimum = min;
                chartSpeed.ChartAreas[0].AxisX.Maximum = max;
                chartSpeed.ChartAreas[0].AxisX.ScaleView.Zoom(min, max);
                chartSpeed.ChartAreas[0].AxisY.IsStartedFromZero = false;
                chartSpeed.ChartAreas[0].RecalculateAxesScale();
                this.chartSpeed.Series[0].Points.AddXY(max, speed / 100);

                /* Clear unnecessary points to rescale */
                if (max > 1.5 * GRAPH_SIZE)
                {
                    this.chartAcceleration.Series[0].Points.RemoveAt(0);
                }
                /* Plot Acceleration Data */
                chartAcceleration.ChartAreas[0].AxisX.Minimum = min;
                chartAcceleration.ChartAreas[0].AxisX.Maximum = max;
                chartAcceleration.ChartAreas[0].AxisX.ScaleView.Zoom(min, max);
                chartAcceleration.ChartAreas[0].AxisY.IsStartedFromZero = false;
                chartAcceleration.ChartAreas[0].RecalculateAxesScale();
                this.chartAcceleration.Series[0].Points.AddXY(max, acceleration / 100);

                /* Clear unnecessary points to rescale */
                if (max > 1.5 * GRAPH_SIZE)
                {
                    this.chartTemperature.Series[0].Points.RemoveAt(0);
                }

                /* Plot Temparature Data */
                chartTemperature.ChartAreas[0].AxisX.Minimum = min;
                chartTemperature.ChartAreas[0].AxisX.Maximum = max;
                chartTemperature.ChartAreas[0].AxisX.ScaleView.Zoom(min, max);
                chartTemperature.ChartAreas[0].AxisY.IsStartedFromZero = false;
                chartTemperature.ChartAreas[0].RecalculateAxesScale();
                this.chartTemperature.Series[0].Points.AddXY(max, temperature / 100);

                /* Clear unnecessary points to rescale */
                /*if (max > 1.5 * GRAPH_SIZE)
                {
                    this.chartRocketXZ.Series[0].Points.RemoveAt(0);
                }*/

                /* Plot Rocket XZ Data */
                chartRocketXZ.ChartAreas[0].AxisX.Minimum = 0;
                chartRocketXZ.ChartAreas[0].AxisX.Maximum = max;
                chartRocketXZ.ChartAreas[0].AxisX.ScaleView.Zoom(min, max);
                chartRocketXZ.ChartAreas[0].AxisY.IsStartedFromZero = false;
                chartRocketXZ.ChartAreas[0].RecalculateAxesScale();
                this.chartRocketXZ.Series[0].Points.AddXY(max, rocketXZ / 100);

                /* Clear unnecessary points to rescale */
                /*if (max > 1.5 * GRAPH_SIZE)
                {
                    this.chartRocketXY.Series[0].Points.RemoveAt(0);
                }*/

                /* Plot Rocket XY Data */
                chartRocketXY.ChartAreas[0].AxisX.Minimum = 0;
                chartRocketXY.ChartAreas[0].AxisX.Maximum = max;
                chartRocketXY.ChartAreas[0].AxisX.ScaleView.Zoom(min, max);
                chartRocketXY.ChartAreas[0].AxisY.IsStartedFromZero = false;
                chartRocketXY.ChartAreas[0].RecalculateAxesScale();
                this.chartRocketXY.Series[0].Points.AddXY(max, rocketXY / 100);
            }

            /* Increase min and max values */
            min++;
            max++;

            /* Redraw all data */
            Invalidate();
        }
#endif

#if PAYLOAD_ON
        private void updateGraphPayload(object s, EventArgs e)
        {
            /* Decode Data */
            dataCounterP = 8;
            payloadXY = decodeP(32);
            payloadXZ = decodeP(32);

            /* Wait 5 sample to stabilize data */
            if (max > 5)
            {
                /* Clear unnecessary points to rescale */
                if (max > 1.5 * GRAPH_SIZE)
                {
                    this.chartPayloadXZ.Series[0].Points.RemoveAt(0);
                }

                /* Plot Payload XZ Data */
                chartPayloadXZ.ChartAreas[0].AxisX.Minimum = min;
                chartPayloadXZ.ChartAreas[0].AxisX.Maximum = max;
                chartPayloadXZ.ChartAreas[0].AxisX.ScaleView.Zoom(min, max);
                chartPayloadXZ.ChartAreas[0].AxisY.IsStartedFromZero = false;
                chartPayloadXZ.ChartAreas[0].RecalculateAxesScale();
                this.chartPayloadXZ.Series[0].Points.AddXY(max, payloadXZ / 100);

                /* Clear unnecessary points to rescale */
                if (max > 1.5 * GRAPH_SIZE)
                {
                    this.chartPayloadXY.Series[0].Points.RemoveAt(0);
                }

                /* Plot Payload XY Data */
                chartPayloadXY.ChartAreas[0].AxisX.Minimum = min;
                chartPayloadXY.ChartAreas[0].AxisX.Maximum = max;
                chartPayloadXY.ChartAreas[0].AxisX.ScaleView.Zoom(min, max);
                chartPayloadXY.ChartAreas[0].AxisY.IsStartedFromZero = false;
                chartPayloadXY.ChartAreas[0].RecalculateAxesScale();
                this.chartPayloadXY.Series[0].Points.AddXY(max, payloadXY / 100);
            }

            /* Increase min and max values */
            min++;
            max++;

            /* Redraw all data */
            Invalidate();
        }
#endif

#if ROCKET_ON
        /* Rocket Decode Function */
        /* Note1: byte[] tlm = new byte[MAX_TLM_DATA] and int dataCounter = 0 must declared globally. */
        /* Note2: dataCounter variable must reset (set to 8 if start command defined) before decoding. */
        private int decodeR(int bitSize)
        {
            int calcNeg = 1;

            for (int i = 0; i < bitSize - 1; i++)
            {
                calcNeg = (calcNeg << 1) | 1;
            }
            int dataIndex;
            int val = 0;
            for (int i = bitSize; i > 0; i--)
            {
                dataIndex = (dataCounterR) / 8;
                val |= ((tlmR[dataIndex] >> (7 - (dataCounterR++ % 8))) & 1) << (i - 1);
            }

            return val;
        }
#endif

        private void checkBoxTimeUpdate_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUpdateTime.Checked)
            {
                buttonUpdateTime.Enabled = true;
            }
            else
            {
                buttonUpdateTime.Enabled = false;
            }
        }

        private void checkBoxRelease_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRelease.Checked)
            {
                buttonRelease.Enabled = true;
            }
            else
            {
                buttonRelease.Enabled = false;
            }
        }

        private void checkBoxReset_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxReset.Checked)
            {
                buttonReset.Enabled = true;
            }
            else
            {
                buttonReset.Enabled = false;
            }
        }

        private void buttonUpdateTime_Click(object sender, EventArgs e)
        {
            buttonUpdateTime.Enabled = false;
            checkBoxUpdateTime.Checked = false;
        }

        private void buttonRelease_Click(object sender, EventArgs e)
        {
            buttonRelease.Enabled = false;
            checkBoxRelease.Checked = false;
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            buttonReset.Enabled = false;
            checkBoxReset.Checked = false;
        }

#if PAYLOAD_ON
        /* PayloadDecode Function */
        /* Note1: byte[] tlm = new byte[MAX_TLM_DATA] and int dataCounter = 0 must declared globally. */
        /* Note2: dataCounter variable must reset (set to 8 if start command defined) before decoding. */
        private int decodeP(int bitSize)
        {
            int calcNeg = 1;

            for (int i = 0; i < bitSize - 1; i++)
            {
                calcNeg = (calcNeg << 1) | 1;
            }
            int dataIndex;
            int val = 0;
            for (int i = bitSize; i > 0; i--)
            {
                dataIndex = (dataCounterP) / 8;
                val |= ((tlmP[dataIndex] >> (7 - (dataCounterP++ % 8))) & 1) << (i - 1);
            }

            return val;
        }
#endif
    }
}