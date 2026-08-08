using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StarTrayTemperature
{
    public partial class IconTray : Form
    {
        private void InitializeCPUContextMenu(CPUDeviceState device, int deviceIndex)
        {
            device.ContextMenu = new ContextMenu();

            string headerLabel = cpuDevices.Count > 1
                ? AppLabel + " (CPU " + (deviceIndex + 1) + ")"
                : AppLabel + " (CPU)";

            // -- Header --
            device.ContextMenu.MenuItems.Add(new MenuItem(headerLabel) { Enabled = false });
            device.ContextMenu.MenuItems.Add("-");

            // ------------ Themes ------------
            MenuItem colorModes = new MenuItem("CPU theme");

            MenuItem lightMode = new MenuItem("Light Theme");
            lightMode.Click += (s, e) => ApplyCPUTheme("light", device, deviceIndex);
            colorModes.MenuItems.Add(lightMode);

            MenuItem darkMode = new MenuItem("Dark Theme");
            darkMode.Click += (s, e) => ApplyCPUTheme("dark", device, deviceIndex);
            colorModes.MenuItems.Add(darkMode);

            MenuItem blue11Mode = new MenuItem("Blue11 Theme");
            blue11Mode.Click += (s, e) => ApplyCPUTheme("blue11", device, deviceIndex);
            colorModes.MenuItems.Add(blue11Mode);

            colorModes.MenuItems.Add("-");

            MenuItem greenMode = new MenuItem("Green Theme");
            greenMode.Click += (s, e) => ApplyCPUTheme("green", device, deviceIndex);
            colorModes.MenuItems.Add(greenMode);

            MenuItem redMode = new MenuItem("Red Theme");
            redMode.Click += (s, e) => ApplyCPUTheme("red", device, deviceIndex);
            colorModes.MenuItems.Add(redMode);

            MenuItem blueMode = new MenuItem("Blue Theme");
            blueMode.Click += (s, e) => ApplyCPUTheme("blue", device, deviceIndex);
            colorModes.MenuItems.Add(blueMode);

            colorModes.MenuItems.Add("-");

            MenuItem thermalMode = new MenuItem("Thermal Theme");
            thermalMode.Click += (s, e) => ApplyCPUTheme("thermal", device, deviceIndex);
            colorModes.MenuItems.Add(thermalMode);

            device.ContextMenu.MenuItems.Add(colorModes);

            // ------------ Global Options ------------
            MenuItem globalOptions = new MenuItem("Options");

            // -- Startup --
            device.StartupMenuItem = new MenuItem("Run on Startup");
            device.StartupMenuItem.Checked = IsTaskScheduled();
            device.StartupMenuItem.Click += RunOnStartup_Click;
            globalOptions.MenuItems.Add(device.StartupMenuItem);

            // -- Show this CPU --
            device.ToggleMenuItem = new MenuItem("Show CPU " + (deviceIndex + 1) + " icon");
            device.ToggleMenuItem.Checked = device.Visible;
            device.ToggleMenuItem.Click += (s, e) => ToggleSpecificCPU(deviceIndex);
            globalOptions.MenuItems.Add(device.ToggleMenuItem);

            // -- Show GPU --
            device.ShowGPUMenuItem = new MenuItem("Show GPU icon");
            device.ShowGPUMenuItem.Checked = showGPU;
            device.ShowGPUMenuItem.Click += ToggleGPU;
            globalOptions.MenuItems.Add(device.ShowGPUMenuItem);

            // -- Change Scale --
            device.ChangeScaleMenuItem = new MenuItem("Change to Fahrenheit");
            if (useFahrenheit)
            {
                device.ChangeScaleMenuItem.Text = "Change to Celsius";
            }
            device.ChangeScaleMenuItem.Click += ChangeScale_Click;
            globalOptions.MenuItems.Add(device.ChangeScaleMenuItem);

            device.ContextMenu.MenuItems.Add(globalOptions);

            // -------------- More info --------------
            MenuItem information = new MenuItem("Info");

            information.MenuItems.Add(new MenuItem("Processor:") { Enabled = false });
            information.MenuItems.Add(new MenuItem(device.Info.Name) { Enabled = false });
            information.MenuItems.Add("-");
            information.MenuItems.Add(new MenuItem(AppLabel + " " + VersionLabel + " " + CopyrightLabel) { Enabled = false });

            device.ContextMenu.MenuItems.Add(information);

            // ------------ Exit ------------
            device.ContextMenu.MenuItems.Add("-");
            MenuItem exitMenuItem = new MenuItem("Exit");
            exitMenuItem.Click += ExitMenuItem_Click;
            device.ContextMenu.MenuItems.Add(exitMenuItem);
        }

        static string GetCpuName()
        {
            string cpuName = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Name from Win32_Processor");

            foreach (ManagementObject obj in searcher.Get())
            {
                cpuName = obj["Name"].ToString();
            }

            return cpuName;
        }

        static List<string> GetAllCpuNames()
        {
            List<string> cpuNames = new List<string>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Name from Win32_Processor");

            foreach (ManagementObject obj in searcher.Get())
            {
                cpuNames.Add(obj["Name"].ToString());
            }

            return cpuNames;
        }

        private void ToggleSpecificCPU(int deviceIndex)
        {
            if (deviceIndex < 0 || deviceIndex >= cpuDevices.Count) return;

            var device = cpuDevices[deviceIndex];
            device.Visible = !device.Visible;
            device.NotifyIcon.Visible = device.Visible;

            SetShowCPUSetting(deviceIndex, device.Visible);

            if (device.ToggleMenuItem != null)
                device.ToggleMenuItem.Checked = device.Visible;

            GC.Collect();
        }
    }
}
