using System.Drawing;
using System.Windows.Forms;

namespace StarTrayTemperature
{
    public class HardwareInfo
    {
        public int HardwareIndex;
        public int SensorIndex;
        public string Name;
    }

    public class CPUDeviceState
    {
        public HardwareInfo Info;
        public int CurrentTemp;
        public NotifyIcon NotifyIcon;
        public Timer Timer;
        public ContextMenu ContextMenu;
        public MenuItem StartupMenuItem;
        public MenuItem ShowCPUMenuItem;
        public MenuItem ShowGPUMenuItem;
        public MenuItem ChangeScaleMenuItem;
        public MenuItem ToggleMenuItem;
        public string ColorMode;
        public Color TextColor;
        public string IconPath;
        public Image IconImage;
        public bool Visible;
    }

    public class GPUDeviceState
    {
        public HardwareInfo Info;
        public int CurrentTemp;
        public NotifyIcon NotifyIcon;
        public Timer Timer;
        public ContextMenu ContextMenu;
        public MenuItem StartupMenuItem;
        public MenuItem ShowCPUMenuItem;
        public MenuItem ShowGPUMenuItem;
        public MenuItem ChangeScaleMenuItem;
        public MenuItem ToggleMenuItem;
        public string ColorMode;
        public Color TextColor;
        public string IconPath;
        public Image IconImage;
        public bool Visible;
    }
}
