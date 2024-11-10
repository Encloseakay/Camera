using Camera.Camera;
using System;
using System.Windows.Forms;

namespace WebcamApp
{
    public partial class MainForm : Form
    {
        private new AutoAdaptWindowsSize AutoSize;
        private WebcamHandler webcamHandler;

        public MainForm()
        {
            InitializeComponent();
            webcamHandler = new WebcamHandler(pictureBox1);
        }
        private void btnStartCamera_Click_1(object sender, EventArgs e)
        {
            string[] cameraNames = webcamHandler.GetCameraNames();
            if (cameraNames.Length > 0)
            {
                webcamHandler.StartCamera(0); 
                MessageBox.Show("已启动摄像头: " + cameraNames[0]);
            }
            else
            {
                MessageBox.Show("没有可用的摄像头设备！");
            }
        }

        private void btnStopCamera_Click_1(object sender, EventArgs e)
        {
            webcamHandler.StopCamera();

            MessageBox.Show("已关闭摄像头                     ");
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            AutoSize = new AutoAdaptWindowsSize(this);
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (AutoSize != null) 
            {
                AutoSize.FormSizeChanged();
            }
            groupBox1.Width = this.Width;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            webcamHandler.StopCamera();
        }
        private void btnTakeSnapshot_Click(object sender, EventArgs e)
        {
            webcamHandler.TakeSnapshot();
        }

        private void btnStartRecording_Click(object sender, EventArgs e)
        {
            webcamHandler.StartRecording();
        }

        private void btnStopRecording_Click(object sender, EventArgs e)
        {
            webcamHandler.StopRecording();
        }

        private void customButton_Click(object sender, EventArgs e)
        {
            webcamHandler.StopCamera();

            MessageBox.Show("已关闭摄像头                     ");
        }
    }
}
 