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
        private void ApplyCPUTheme(string theme, CPUDeviceState device, int deviceIndex)
        {
            if (device.ColorMode != theme)
            {
                device.ColorMode = theme;

                switch (theme)
                {
                    case "light":
                        device.TextColor = Color.FromArgb(255, 255, 255);
                        device.IconPath = Path.Combine(Application.StartupPath, "Resources", "cpuicon.ico");
                        break;
                    case "dark":
                        device.TextColor = Color.FromArgb(0, 0, 0);
                        device.IconPath = Path.Combine(Application.StartupPath, "Resources", "cpuicon_dark.ico");
                        break;
                    case "blue11":
                        device.TextColor = Color.FromArgb(151, 234, 255);
                        device.IconPath = Path.Combine(Application.StartupPath, "Resources", "cpuicon_blue11.ico");
                        break;
                    case "green":
                        device.TextColor = Color.FromArgb(189, 255, 71);
                        device.IconPath = Path.Combine(Application.StartupPath, "Resources", "cpuicon_green.ico");
                        break;
                    case "red":
                        device.TextColor = Color.FromArgb(255, 161, 150);
                        device.IconPath = Path.Combine(Application.StartupPath, "Resources", "cpuicon_red.ico");
                        break;
                    case "blue":
                        device.TextColor = Color.FromArgb(130, 228, 255);
                        device.IconPath = Path.Combine(Application.StartupPath, "Resources", "cpuicon_blue.ico");
                        break;
                    case "thermal":
                        device.TextColor = GetThermalColor(device.CurrentTemp);
                        device.IconPath = Path.Combine(Application.StartupPath, "Resources", "cpuicon.ico");
                        break;
                }

                device.NotifyIcon.Icon?.Dispose();
                device.IconImage = Image.FromFile(device.IconPath);
                device.NotifyIcon.Icon = CreateCPUIcon(device, device.CurrentTemp);
                SaveSettings_CPU(device, deviceIndex);
            }
        }

        private void SaveSettings_CPU(CPUDeviceState device, int deviceIndex)
        {
            string suffix = deviceIndex == 0 ? "" : "_" + deviceIndex;

            Properties.Settings.Default["ColorMode_CPU" + suffix] = device.ColorMode;
            Properties.Settings.Default["TextColor_CPU" + suffix] = device.TextColor;
            Properties.Settings.Default["IconPath_CPU" + suffix] = device.IconPath;
            Properties.Settings.Default.Save();
        }

        private void LoadSettings_CPU(CPUDeviceState device, int deviceIndex)
        {
            string suffix = deviceIndex == 0 ? "" : "_" + deviceIndex;

            string colorMode = GetSettingValue("ColorMode_CPU" + suffix, "thermal");
            Color textColor = GetSettingColor("TextColor_CPU" + suffix, GetThermalColor(0));
            string iconPath = GetSettingValue("IconPath_CPU" + suffix, "");

            device.ColorMode = colorMode;
            device.TextColor = textColor;
            device.IconPath = iconPath;

            // First launch
            if (string.IsNullOrEmpty(device.IconPath))
            {
                device.ColorMode = "thermal";
                device.TextColor = GetThermalColor(device.CurrentTemp);
                device.IconPath = Path.Combine(Application.StartupPath, "Resources", "cpuicon.ico");

                SaveSettings_CPU(device, deviceIndex);
            }
        }

        private string GetSettingValue(string key, string defaultValue)
        {
            var val = Properties.Settings.Default[key];
            if (val == null || string.IsNullOrEmpty(val.ToString()))
                return defaultValue;
            return val.ToString();
        }

        private Color GetSettingColor(string key, Color defaultValue)
        {
            var val = Properties.Settings.Default[key];
            if (val == null)
                return defaultValue;
            try
            {
                return (Color)val;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
