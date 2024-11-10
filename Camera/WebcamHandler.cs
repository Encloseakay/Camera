using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Accord.Video.FFMPEG;
using AForge.Video;
using AForge.Video.DirectShow;

public class WebcamHandler
{
    private FilterInfoCollection videoDevices;
    private VideoCaptureDevice videoSource;
    private PictureBox pictureBox;
    private VideoFileWriter videoWriter;
    private bool isRecording;

    public WebcamHandler(PictureBox pictureBox)
    {
        this.pictureBox = pictureBox;
        InitializeVideoDevices();
    }
    private void InitializeVideoDevices()
    {
        try
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
                MessageBox.Show("未找到任何摄像头设备！");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"初始化摄像头时出错: {ex.Message}");
        }
    }
    public void StartCamera(int deviceIndex = 0)
    {
        if (videoDevices == null || videoDevices.Count == 0)
        {
            MessageBox.Show("未找到可用的摄像头设备！");
            return;
        }

        try
        {
            videoSource = new VideoCaptureDevice(videoDevices[deviceIndex].MonikerString);
            videoSource.NewFrame += Video_NewFrame;
            videoSource.Start();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"启动摄像头时出错: {ex.Message}");
        }
    }
    public void StopCamera()
    {
        if (videoSource?.IsRunning == true)
        {
            videoSource.SignalToStop();
            videoSource.WaitForStop();
            videoSource.NewFrame -= Video_NewFrame;
        }
        pictureBox.Image?.Dispose();
        pictureBox.Image = null;
    }
    public void StartRecording()
    {
        if (pictureBox.Image == null)
        {
            MessageBox.Show("没有可用的视频帧来录制！");
            return;
        }

        using (SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "AVI Video|*.avi",
            Title = "保存视频",
            FileName = "video.avi"
        })
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    videoWriter = new VideoFileWriter();
                    int width = pictureBox.Width % 2 == 0 ? pictureBox.Width : pictureBox.Width + 1;
                    int height = pictureBox.Height % 2 == 0 ? pictureBox.Height : pictureBox.Height + 1;

                    videoWriter.Open(saveFileDialog.FileName, width, height, 30, VideoCodec.MPEG4);

                    if (!videoWriter.IsOpen)
                    {
                        throw new Exception("视频文件打开失败，可能是文件路径或参数配置问题。");
                    }

                    isRecording = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"录制视频时出错: {ex.Message}");
                }
            }
        }
    }
    public void StopRecording()
    {
        if (isRecording)
        {
            isRecording = false;
            videoWriter?.Close();
            videoWriter?.Dispose();
            MessageBox.Show("视频已保存！");
        }
    }

    private readonly object frameLock = new object();

    private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
    {
        lock (frameLock) 
        {
            try
            {
                using (Bitmap frame = (Bitmap)eventArgs.Frame.Clone())
                {
                    if (pictureBox.InvokeRequired)
                    {
                        pictureBox.Invoke(new Action(() =>
                        {
                            pictureBox.Image?.Dispose(); 
                            pictureBox.Image = new Bitmap(frame); 
                        }));
                    }
                    else
                    {
                        pictureBox.Image?.Dispose(); 
                        pictureBox.Image = new Bitmap(frame); 
                    }
                    if (isRecording && videoWriter != null)
                    {
                        videoWriter.WriteVideoFrame(frame);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"处理视频帧时出错: {ex.Message}");
            }
        }
    }
    public void TakeSnapshot()
    {
        if (pictureBox.Image == null)
        {
            MessageBox.Show("没有可用的图像来拍照！");
            return;
        }

        try
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JPEG Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp",
                Title = "保存照片",
                FileName = "snapshot.png"
            })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ImageFormat format = ImageFormat.Png; 
                    string extension = Path.GetExtension(saveFileDialog.FileName)?.ToLower();

                    switch (extension)
                    {
                        case ".jpg":
                        case ".jpeg":
                            format = ImageFormat.Jpeg;
                            break;
                        case ".bmp":
                            format = ImageFormat.Bmp;
                            break;
                    }

                    using (Bitmap bitmap = new Bitmap(pictureBox.Image))
                    {
                        bitmap.Save(saveFileDialog.FileName, format);
                    }

                    MessageBox.Show($"照片已保存到: {saveFileDialog.FileName}");
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"保存照片时出错: {ex.Message}");
        }
    }
    public string[] GetCameraNames()
    {
        if (videoDevices == null) return Array.Empty<string>();

        string[] names = new string[videoDevices.Count];
        for (int i = 0; i < videoDevices.Count; i++)
        {
            names[i] = videoDevices[i].Name;
        }
        return names;
    }
}
