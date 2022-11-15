using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace ArduinoProject
{
    public partial class Arduino : Form
    {
        public Arduino()
        {
            InitializeComponent();
        }
        Dictionary<UInt16, string> StatusCodes;
        private void Arduino_Load(object sender, EventArgs e)
        {
            try
            {
                cmbSerialPort.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                cmbSerialPort.SelectedIndex = 0;
            }catch (Exception )
            {

            }
 
            serialPort1.BaudRate = 9600;
            serialPort1.ReadTimeout = 2000;
            serialPort1.WriteTimeout = 2000;
            batterystatus();
            ShowPowerStatus();
           
        }

        private void btnON_Click(object sender, EventArgs e)
        {
            serialportCommand("on");

        }

        private void btnOff_Click(object sender, EventArgs e)
        {
            serialportCommand("off");
        }

        private void cmbSerialPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.PortName = cmbSerialPort.Text;
        }

        private void serialportCommand(string commandname)
        {
            try
            {
                serialPort1.Open();
                serialPort1.WriteLine(commandname);
                serialPort1.Close();

            }
            catch (Exception )
            {

            }
        }

        private void batterystatus()
        {
            StatusCodes = new Dictionary<ushort, string>();

            StatusCodes.Add(1, "The battery is discharging");

            StatusCodes.Add(2, "The system has access to AC so no battery is being discharged. However, the battery is not necessarily charging");

            StatusCodes.Add(3, "Fully Charged");

            StatusCodes.Add(4, "Low");

            StatusCodes.Add(5, "Critical");

            StatusCodes.Add(6, "Charging");

            StatusCodes.Add(7, "Charging and High");

            StatusCodes.Add(8, "Charging and Low");

            StatusCodes.Add(9, "Undefined");

            StatusCodes.Add(10, "Partially Charged");



            /* Set progress bar values and Properties */

            progressBar1.Maximum = 100;

            progressBar1.Style = ProgressBarStyle.Continuous;





            timer1.Enabled = true;



            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_Battery");

            foreach (ManagementObject mo in mos.Get())

            {

                lblBatteryName.Text = mo["Name"].ToString();

                UInt16 statuscode = (UInt16)mo["BatteryStatus"];

                string statusString = StatusCodes[statuscode];

                lblBatteryStatus.Text = statusString;

            }
        }

        private void timer1_Tick(object sender, EventArgs e)

        {

            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_Battery where Name='" + lblBatteryName.Text + "'");
            ShowPowerStatus();
            foreach (ManagementObject mo in mos.Get())

            {



                UInt16 statuscode = (UInt16)mo["BatteryStatus"];

                string statusString = StatusCodes[statuscode];

                lblBatteryStatus.Text = statusString;



                /* Set Progress bar according to status  */

                if (statuscode == 4)

                {

                    progressBar1.ForeColor = Color.Red;

                    progressBar1.Value = 5;



                }

                else if (statuscode == 3)

                {

                    progressBar1.ForeColor = Color.Blue;

                    progressBar1.Value = 100;

                }

                else if (statuscode == 2)

                {

                    progressBar1.ForeColor = Color.Green;

                    progressBar1.Value = 100;

                }

                else if (statuscode == 5)

                {

                    progressBar1.ForeColor = Color.Red;

                    progressBar1.Value = 1;

                }

                else if (statuscode == 6)

                {

                    progressBar1.ForeColor = Color.Blue;

                    progressBar1.Value = 100;

                }
                this.Refresh();
            }
        }
        private void ShowPowerStatus()
        {
            PowerStatus status = SystemInformation.PowerStatus;
            txtChargeStatus.Text = status.BatteryChargeStatus.ToString();
            if (status.BatteryFullLifetime == -1)
                txtFullLifetime.Text = "Unknown";
            else
                txtFullLifetime.Text = status.BatteryFullLifetime.ToString();
                txtBatere.Text = status.BatteryLifePercent.ToString("P0");
                float battpercent = status.BatteryLifePercent * 100;
                progressBar1.Value =int.Parse( battpercent.ToString());
            if (battpercent == 100)
            {
                bt1.Visible = true;
                bt2.Visible = true;
                bt3.Visible = true;
                bt4.Visible = true;
                bt5.Visible = true;
            }
            else if (battpercent < 100 && battpercent > 80)
            {
                bt1.Visible = true;
                bt2.Visible = true;
                bt3.Visible = true;
                bt4.Visible = true;
                bt5.Visible = true;
            }
            else if (battpercent <= 80 && battpercent > 60)
            {
                bt1.Visible = true;
                bt2.Visible = true;
                bt3.Visible = true;
                bt4.Visible = true;
            }
            else if (battpercent <= 60 && battpercent > 40)
            {
                bt1.Visible = true;
                bt2.Visible = true;
                bt3.Visible = true;
            }
            else if (battpercent <= 40 && battpercent > 20)
            {
                bt1.Visible = true;
                bt2.Visible = true;
                bt1.BackColor = Color.Green;
            }
            else if (battpercent <= 20)
            {
                bt1.Visible = true;
                bt2.Visible = false;
                bt1.BackColor = Color.Red;
            }
            else
            {
                
            }
            try
            {
                if (battpercent >= float.Parse(txtChargeOff.Text.ToString()))
                {
                    serialportCommand("off");
                }
                if (battpercent <= float.Parse(txtChargeON.Text) && txtChargeStatus.Text=="0")
                {
                    serialportCommand("on");
                }
            }
            catch (Exception  )
            {

            }
            
            if (status.BatteryLifeRemaining == -1)
                txtLifeRemaining.Text = "Unknown";
            else
                txtLifeRemaining.Text =
                    status.BatteryLifeRemaining.ToString();
            txtLineStatus.Text = status.PowerLineStatus.ToString();
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void lblBatteryName_Click(object sender, EventArgs e)
        {

        }
    }
}
