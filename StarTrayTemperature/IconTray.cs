using Microsoft.Win32;
using LibreHardwareMonitor.Hardware;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;
using System.Collections.Generic;

namespace StarTrayTemperature
{
    public partial class IconTray : Form
    {
        private string AppLabel = "StarTray";
        private string VersionLabel = "v1.2";
        private string CopyrightLabel = "© justinnas";


        private string resourcesFolder = Path.Combine(Application.StartupPath, "Resources");

        // --==+

        private Computer computer;

        // -+

        private bool useFahrenheit = false;

        // +=-

        private TaskService taskService = new TaskService();
        private const string TaskName = "StarTray_RunOnStartup";

        // --==+

        private int iconWidth = 32;
        private int iconHeight = 32;
        private FontFamily customFontFamily = FontFamily.GenericSansSerif;


        public IconTray()
        {
            InitializeComponent();
            LoadGlobalSettings();

            computer = new Computer {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
            };

            computer.Open();

            // Initialize Fonts
            PrivateFontCollection fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile(Path.Combine(resourcesFolder, "font.ttf"));
            customFontFamily = fontCollection.Families[0];

            // Initialize CPU icons (multiple supported)
            if (showCPU)
            {
                StartCPUDevices();
            }

            // Initialize GPU icons (multiple supported)
            if (showGPU)
            {
                StartGPUDevices();
            }

            // Start CPU icon if both of the icons are somehow turned off
            bool anyCPUCPUVisible = false;
            foreach (var cpu in cpuDevices)
            {
                if (cpu.Visible) { anyCPUCPUVisible = true; break; }
            }
            bool anyGPUVisible = false;
            foreach (var gpu in gpuDevices)
            {
                if (gpu.Visible) { anyGPUVisible = true; break; }
            }

            if (!anyCPUCPUVisible && !anyGPUVisible)
            {
                showCPU = true;
                StartCPUDevices();
            }

            this.Hide();
            this.Visible = false;
        }

        private bool IsWindowsThemeLight()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        object registryValueObject = key.GetValue("SystemUsesLightTheme");
                        if (registryValueObject != null)
                        {
                            int registryValue = (int)registryValueObject;
                            return registryValue == 1;
                        }
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        private static class NativeMethods // Used for clearing up GDI's and User's icon handles
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern bool DestroyIcon(IntPtr handle);
        }

        private static Color GetThermalColor(int temperature)
        {
            if (temperature < 40)
                return Color.FromArgb(100, 200, 255);
            else if (temperature < 60)
                return Color.FromArgb(100, 255, 100);
            else if (temperature < 70)
                return Color.FromArgb(255, 255, 80);
            else if (temperature < 80)
                return Color.FromArgb(255, 170, 50);
            else
                return Color.FromArgb(255, 80, 80);
        }
    }
}
