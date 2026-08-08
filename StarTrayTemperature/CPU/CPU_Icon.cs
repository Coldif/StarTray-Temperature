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
        private List<CPUDeviceState> cpuDevices = new List<CPUDeviceState>();

        private void StartCPUDevices()
        {
            List<HardwareInfo> cpus = FindAllCPUSensors();

            foreach (var cpuInfo in cpus)
            {
                int deviceIndex = cpuDevices.Count;
                bool showDevice = GetShowCPUSetting(deviceIndex);

                CPUDeviceState device = new CPUDeviceState
                {
                    Info = cpuInfo,
                    CurrentTemp = 0,
                    Visible = showDevice
                };

                LoadSettings_CPU(device, deviceIndex);

                device.IconImage = Image.FromFile(device.IconPath);

                InitializeCPUContextMenu(device, deviceIndex);

                device.NotifyIcon = new NotifyIcon();
                device.NotifyIcon.ContextMenu = device.ContextMenu;
                device.NotifyIcon.Text = "CPU " + (deviceIndex + 1) + " Temperature: " + device.CurrentTemp + " C";
                device.NotifyIcon.Icon = CreateCPUIcon(device, device.CurrentTemp);
                device.NotifyIcon.Visible = showDevice;

                device.Timer = new Timer();
                device.Timer.Interval = 1000;
                device.Timer.Tick += (s, e) => timerCPU_Tick(s, e, device, deviceIndex);
                device.Timer.Start();

                cpuDevices.Add(device);
            }

            GC.Collect();
        }

        private void StopCPUDevices()
        {
            foreach (var device in cpuDevices)
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
            cpuDevices.Clear();
            GC.Collect();
        }

        private void StopCPUDevice(int index)
        {
            if (index < 0 || index >= cpuDevices.Count) return;

            var device = cpuDevices[index];
            device.IconImage?.Dispose();
            device.Timer?.Stop();
            device.Timer?.Dispose();
            device.NotifyIcon?.Icon?.Dispose();
            if (device.NotifyIcon?.Icon != null)
                NativeMethods.DestroyIcon(device.NotifyIcon.Icon.Handle);
            device.NotifyIcon?.ContextMenu?.Dispose();
            device.NotifyIcon?.Dispose();

            cpuDevices.RemoveAt(index);
            GC.Collect();
        }

        private List<HardwareInfo> FindAllCPUSensors()
        {
            List<HardwareInfo> cpus = new List<HardwareInfo>();

            for (int i = 0; i < computer.Hardware.Count; i++)
            {
                var hardware = computer.Hardware[i];
                if (hardware.HardwareType == HardwareType.Cpu)
                {
                    hardware.Update();
                    for (int j = 0; j < hardware.Sensors.Length; j++)
                    {
                        var sensor = hardware.Sensors[j];
                        if (sensor != null && sensor.SensorType == SensorType.Temperature)
                        {
                            cpus.Add(new HardwareInfo
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

            return cpus;
        }

        private void timerCPU_Tick(object sender, EventArgs e, CPUDeviceState device, int deviceIndex)
        {
            try
            {
                computer.Hardware[device.Info.HardwareIndex].Update();
                int newTemp = Convert.ToInt32(computer.Hardware[device.Info.HardwareIndex].Sensors[device.Info.SensorIndex].Value);

                device.CurrentTemp = newTemp;

                if (device.ColorMode == "thermal")
                {
                    device.TextColor = GetThermalColor(device.CurrentTemp);
                }

                string temperatureText = "CPU " + (deviceIndex + 1) + " Temperature: " + device.CurrentTemp + " C";

                if (useFahrenheit)
                {
                    int fahrenheit = Convert.ToInt32(device.CurrentTemp * 1.8 + 32);
                    temperatureText = "CPU " + (deviceIndex + 1) + " Temperature: " + fahrenheit + " F";
                    device.CurrentTemp = fahrenheit;
                }

                device.NotifyIcon.Text = temperatureText;

                device.NotifyIcon.Icon?.Dispose();
                NativeMethods.DestroyIcon(device.NotifyIcon.Icon.Handle);
                device.NotifyIcon.Icon = CreateCPUIcon(device, device.CurrentTemp);
            }
            catch { }
        }

        private Icon CreateCPUIcon(CPUDeviceState device, int temperature)
        {
            string temperatureText = temperature.ToString();

            Bitmap bitmap = new Bitmap(iconWidth, iconHeight);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Transparent);

                graphics.DrawImage(device.IconImage, new Rectangle(0, 0, iconWidth, iconHeight));

                int fontSize = 18;
                int moveX = 1;
                int moveY = 0;

                if (temperature >= 100)
                {
                    fontSize = 14;
                    moveX = 0;
                    moveY = 2;
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

        private bool GetShowCPUSetting(int deviceIndex)
        {
            if (deviceIndex == 0)
                return Properties.Settings.Default.showCPU;

            var setting = Properties.Settings.Default["showCPU_" + deviceIndex];
            if (setting == null)
                return true;
            return (bool)setting;
        }

        private void SetShowCPUSetting(int deviceIndex, bool value)
        {
            if (deviceIndex == 0)
            {
                Properties.Settings.Default.showCPU = value;
            }
            else
            {
                Properties.Settings.Default["showCPU_" + deviceIndex] = value;
            }
            Properties.Settings.Default.Save();
        }
    }
}
