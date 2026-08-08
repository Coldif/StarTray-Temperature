using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StarTrayTemperature
{
    public partial class IconTray : Form
    {
        private void ApplyGPUTheme(string theme, GPUDeviceState device, int deviceIndex)
        {
            if (device.ColorMode != theme)
            {
                device.ColorMode = theme;

                switch (theme)
                {
                    case "light":
                        device.TextColor = Color.FromArgb(255, 255, 255);
                        device.IconPath = Path.Combine(Application.StartupPath, "Resources", "gpuicon.ico");
                        break;
                    case "dark":
                        device.TextColor = Color.FromArgb(0, 0, 0);
                        device.IconPath = Path.Combine(Application.StartupPath, "Resources", "gpuicon_dark.ico");
                        break;
                    case "blue11":
                        device.TextColor = Color.FromArgb(151, 234, 255);
                        device.IconPath = Path.Combine(Application.StartupPath, "Resources", "gpuicon_blue11.ico");
                        break;
                    case "green":
                        device.TextColor = Color.FromArgb(189, 255, 71);
                        device.IconPath = Path.Combine(Application.StartupPath, "Resources", "gpuicon_green.ico");
                        break;
                    case "red":
                        device.TextColor = Color.FromArgb(255, 161, 150);
                        device.IconPath = Path.Combine(Application.StartupPath, "Resources", "gpuicon_red.ico");
                        break;
                    case "blue":
                        device.TextColor = Color.FromArgb(130, 228, 255);
                        device.IconPath = Path.Combine(Application.StartupPath, "Resources", "gpuicon_blue.ico");
                        break;
                    case "thermal":
                        device.TextColor = GetThermalColor(device.CurrentTemp);
                        device.IconPath = Path.Combine(Application.StartupPath, "Resources", "gpuicon.ico");
                        break;
                }

                device.NotifyIcon.Icon?.Dispose();
                device.IconImage = Image.FromFile(device.IconPath);
                device.NotifyIcon.Icon = CreateGPUIcon(device, device.CurrentTemp);
                SaveSettings_GPU(device, deviceIndex);
            }
        }

        private void SaveSettings_GPU(GPUDeviceState device, int deviceIndex)
        {
            string suffix = deviceIndex == 0 ? "" : "_" + deviceIndex;

            Properties.Settings.Default["ColorMode_GPU" + suffix] = device.ColorMode;
            Properties.Settings.Default["TextColor_GPU" + suffix] = device.TextColor;
            Properties.Settings.Default["IconPath_GPU" + suffix] = device.IconPath;
            Properties.Settings.Default.Save();
        }

        private void LoadSettings_GPU(GPUDeviceState device, int deviceIndex)
        {
            string suffix = deviceIndex == 0 ? "" : "_" + deviceIndex;

            string colorMode = GetSettingValue("ColorMode_GPU" + suffix, "thermal");
            Color textColor = GetSettingColor("TextColor_GPU" + suffix, GetThermalColor(0));
            string iconPath = GetSettingValue("IconPath_GPU" + suffix, "");

            device.ColorMode = colorMode;
            device.TextColor = textColor;
            device.IconPath = iconPath;

            // First launch
            if (string.IsNullOrEmpty(device.IconPath))
            {
                device.ColorMode = "thermal";
                device.TextColor = GetThermalColor(device.CurrentTemp);
                device.IconPath = Path.Combine(Application.StartupPath, "Resources", "gpuicon.ico");

                SaveSettings_GPU(device, deviceIndex);
            }
        }
    }
}
