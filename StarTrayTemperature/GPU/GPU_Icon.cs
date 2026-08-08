using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace StarTrayTemperature
{
    public partial class IconTray : Form
    {
        private List<GPUDeviceState> gpuDevices = new List<GPUDeviceState>();

        private void StartGPUDevices()
        {
            List<HardwareInfo> gpus = FindAllGPUSensors();

            if (gpus.Count == 0)
            {
                showGPU = false;

                foreach (var cpuDevice in cpuDevices)
                {
                    if (cpuDevice.ShowGPUMenuItem != null)
                    {
                        cpuDevice.ShowGPUMenuItem.Enabled = false;
                        cpuDevice.ShowGPUMenuItem.Checked = false;
                        cpuDevice.ShowGPUMenuItem.Text = "Show GPU icon (disabled)";
                    }
                }
                Properties.Settings.Default.showGPU = showGPU;
                Properties.Settings.Default.Save();

                return;
            }

            foreach (var gpuInfo in gpus)
            {
                int deviceIndex = gpuDevices.Count;
                bool showDevice = GetShowGPUSetting(deviceIndex);

                GPUDeviceState device = new GPUDeviceState
                {
                    Info = gpuInfo,
                    CurrentTemp = 0,
                    Visible = showDevice
                };

                LoadSettings_GPU(device, deviceIndex);

                device.IconImage = Image.FromFile(device.IconPath);

                InitializeGPUContextMenu(device, deviceIndex);

                device.NotifyIcon = new NotifyIcon();
                device.NotifyIcon.Text = "GPU " + (deviceIndex + 1) + " Temperature: " + device.CurrentTemp + " C";
                device.NotifyIcon.Icon = CreateGPUIcon(device, device.CurrentTemp);
                device.NotifyIcon.Visible = showDevice;
                device.NotifyIcon.ContextMenu = device.ContextMenu;

                device.Timer = new Timer();
                device.Timer.Interval = 1000;
                device.Timer.Tick += (s, e) => timerGPU_Tick(s, e, device, deviceIndex);
                device.Timer.Start();

                gpuDevices.Add(device);
            }

            GC.Collect();
        }

        private void StopGPUDevices()
        {
            foreach (var device in gpuDevices)
            {
                device.IconImage?.Dispose();
                device.Timer?.Stop();
                device.Timer?.Dispose();
                device.NotifyIcon?.Icon?.Dispose();
                if (device.NotifyIcon?.Icon != null)
                    NativeMethods.DestroyIcon(device.NotifyIcon.Icon.Handle);
                device.NotifyIcon?.ContextMenu?.Dispose();
                device.NotifyIcon?.Dispose();
            }
            gpuDevices.Clear();
            GC.Collect();
        }

        private void StopGPUDevice(int index)
        {
            if (index < 0 || index >= gpuDevices.Count) return;

            var device = gpuDevices[index];
            device.IconImage?.Dispose();
            device.Timer?.Stop();
            device.Timer?.Dispose();
            device.NotifyIcon?.Icon?.Dispose();
            if (device.NotifyIcon?.Icon != null)
                NativeMethods.DestroyIcon(device.NotifyIcon.Icon.Handle);
            device.NotifyIcon?.ContextMenu?.Dispose();
            device.NotifyIcon?.Dispose();

            gpuDevices.RemoveAt(index);
            GC.Collect();
        }

        private List<HardwareInfo> FindAllGPUSensors()
        {
            List<HardwareInfo> gpus = new List<HardwareInfo>();

            for (int i = 0; i < computer.Hardware.Count; i++)
            {
                var hardware = computer.Hardware[i];
                if (hardware.HardwareType == HardwareType.GpuNvidia ||
                    hardware.HardwareType == HardwareType.GpuAmd ||
                    hardware.HardwareType == HardwareType.GpuIntel)
                {
                    hardware.Update();
                    for (int j = 0; j < hardware.Sensors.Length; j++)
                    {
                        var sensor = hardware.Sensors[j];
                        if (sensor != null && sensor.SensorType == SensorType.Temperature)
                        {
                            gpus.Add(new HardwareInfo
                            {
                                HardwareIndex = i,
                                SensorIndex = j,
                                Name = hardware.Name
                            });
                            break;
                        }
                    }
                }
            }

            return gpus;
        }

        private void timerGPU_Tick(object sender, EventArgs e, GPUDeviceState device, int deviceIndex)
        {
            try
            {
                computer.Hardware[device.Info.HardwareIndex].Update();
                int newTemp = Convert.ToInt32(computer.Hardware[device.Info.HardwareIndex].Sensors[device.Info.SensorIndex].Value);

                if (newTemp == 0 && device.CurrentTemp != 0) return;

                device.CurrentTemp = newTemp;

                if (device.ColorMode == "thermal")
                {
                    device.TextColor = GetThermalColor(device.CurrentTemp);
                }

                string temperatureText = "GPU " + (deviceIndex + 1) + " Temperature: " + device.CurrentTemp + " C";

                if (useFahrenheit)
                {
                    int fahrenheit = Convert.ToInt32(device.CurrentTemp * 1.8 + 32);
                    temperatureText = "GPU " + (deviceIndex + 1) + " Temperature: " + fahrenheit + " F";
                    device.CurrentTemp = fahrenheit;
                }

                device.NotifyIcon.Text = temperatureText;

                device.NotifyIcon.Icon?.Dispose();
                NativeMethods.DestroyIcon(device.NotifyIcon.Icon.Handle);
                device.NotifyIcon.Icon = CreateGPUIcon(device, device.CurrentTemp);
            }
            catch { }
        }

        private Icon CreateGPUIcon(GPUDeviceState device, int temperature)
        {
            string temperatureText = temperature.ToString();

            Bitmap bitmap = new Bitmap(iconWidth, iconHeight);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Transparent);

                graphics.DrawImage(device.IconImage, new Rectangle(0, 0, iconWidth, iconHeight));

                int fontSize = 18;
                int moveX = 3;
                int moveY = 0;

                if (temperature >= 100)
                {
                    fontSize = 14;
                    moveX = 2;
                    moveY = 1;
                }

                using (Font font = new Font(customFontFamily, fontSize))
                {
                    using (Brush brush = new SolidBrush(device.TextColor))
                    {
                        if (device.TextColor == Color.Black)
                        {
                            graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                        }

                        SizeF textSize = graphics.MeasureString(temperatureText, font);
                        float x = (bitmap.Width - textSize.Width) / 2 + moveX;
                        float y = (bitmap.Height - textSize.Height) / 2 + moveY;

                        graphics.DrawString(temperatureText, font, brush, new PointF(x, y));
                    }
                }
            }

            Icon icon = Icon.FromHandle(bitmap.GetHicon());
            bitmap.Dispose();

            return icon;
        }

        private bool GetShowGPUSetting(int deviceIndex)
        {
            if (deviceIndex == 0)
                return Properties.Settings.Default.showGPU;

            var setting = Properties.Settings.Default["showGPU_" + deviceIndex];
            if (setting == null)
                return true;
            return (bool)setting;
        }

        private void SetShowGPUSetting(int deviceIndex, bool value)
        {
            if (deviceIndex == 0)
            {
                Properties.Settings.Default.showGPU = value;
            }
            else
            {
                Properties.Settings.Default["showGPU_" + deviceIndex] = value;
            }
            Properties.Settings.Default.Save();
        }
    }
}
