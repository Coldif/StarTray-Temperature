using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StarTrayTemperature
{
    public partial class IconTray : Form
    {
        private bool showCPU = true;
        private bool showGPU = true;

        private void LoadGlobalSettings()
        {
            useFahrenheit = Properties.Settings.Default.UseFahrenheit;
            showCPU = Properties.Settings.Default.showCPU;
            showGPU = Properties.Settings.Default.showGPU;
        }

        private void ToggleGPU(object sender, EventArgs e)
        {
            bool anyCPUVisible = false;
            foreach (var cpu in cpuDevices)
            {
                if (cpu.Visible) { anyCPUVisible = true; break; }
            }

            if (!anyCPUVisible && showGPU) return;

            showGPU = !showGPU;

            if (!showGPU)
            {
                StopGPUDevices();
            }
            else
            {
                StartGPUDevices();
            }

            foreach (var cpuDevice in cpuDevices)
            {
                if (cpuDevice.ShowGPUMenuItem != null)
                {
                    cpuDevice.ShowGPUMenuItem.Checked = showGPU;
                }
            }

            foreach (var gpuDevice in gpuDevices)
            {
                if (gpuDevice.ToggleMenuItem != null)
                {
                    gpuDevice.ToggleMenuItem.Checked = gpuDevice.Visible;
                }
            }

            GC.Collect();

            Properties.Settings.Default.showGPU = showGPU;
            Properties.Settings.Default.Save();
        }

        private void ToggleCPU(object sender, EventArgs e)
        {
            bool anyGPUVisible = false;
            foreach (var gpu in gpuDevices)
            {
                if (gpu.Visible) { anyGPUVisible = true; break; }
            }

            if (!anyGPUVisible && showCPU) return;

            showCPU = !showCPU;

            if (!showCPU)
            {
                StopCPUDevices();
            }
            else
            {
                StartCPUDevices();
            }

            foreach (var gpuDevice in gpuDevices)
            {
                if (gpuDevice.ShowCPUMenuItem != null)
                {
                    gpuDevice.ShowCPUMenuItem.Checked = showCPU;
                }
            }

            foreach (var cpuDevice in cpuDevices)
            {
                if (cpuDevice.ToggleMenuItem != null)
                {
                    cpuDevice.ToggleMenuItem.Checked = cpuDevice.Visible;
                }
            }

            GC.Collect();

            Properties.Settings.Default.showCPU = showCPU;
            Properties.Settings.Default.Save();
        }

        private void RunOnStartup_Click(object sender, EventArgs e)
        {
            foreach (var cpu in cpuDevices)
            {
                if (cpu.StartupMenuItem != null)
                    cpu.StartupMenuItem.Checked = !cpu.StartupMenuItem.Checked;
            }
            foreach (var gpu in gpuDevices)
            {
                if (gpu.StartupMenuItem != null)
                    gpu.StartupMenuItem.Checked = !gpu.StartupMenuItem.Checked;
            }

            if (!IsTaskScheduled())
            {
                CreateTask();
            }
            else
            {
                RemoveTask();
            }
        }

        private bool IsTaskScheduled()
        {
            Task task = taskService.GetTask(TaskName);
            return task != null;
        }

        private void CreateTask()
        {
            TaskDefinition taskDefinition = taskService.NewTask();
            taskDefinition.RegistrationInfo.Description = "Start StarTray on system startup.";
            taskDefinition.Triggers.Add(new LogonTrigger());
            taskDefinition.Actions.Add(new ExecAction(Application.ExecutablePath));
            taskDefinition.Principal.RunLevel = TaskRunLevel.Highest;
            taskDefinition.Settings.DisallowStartIfOnBatteries = false;
            taskDefinition.Settings.StopIfGoingOnBatteries = false;
            taskDefinition.Settings.RunOnlyIfIdle = false;
            taskDefinition.Settings.IdleSettings.StopOnIdleEnd = false;
            taskDefinition.Settings.RunOnlyIfNetworkAvailable = false;
            taskDefinition.Settings.ExecutionTimeLimit = TimeSpan.Zero;
            taskDefinition.Settings.StartWhenAvailable = true;

            taskService.RootFolder.RegisterTaskDefinition(TaskName, taskDefinition);
        }

        private void RemoveTask()
        {
            if (taskService == null)
            {
                taskService = new TaskService();
            }

            taskService.RootFolder.DeleteTask(TaskName, false);
        }

        private void ChangeScale_Click(object sender, EventArgs e)
        {
            useFahrenheit = !useFahrenheit;

            foreach (var cpu in cpuDevices)
            {
                if (cpu.ChangeScaleMenuItem != null)
                {
                    cpu.ChangeScaleMenuItem.Text = useFahrenheit ? "Change to Celsius" : "Change to Fahrenheit";
                }
            }

            foreach (var gpu in gpuDevices)
            {
                if (gpu.ChangeScaleMenuItem != null)
                {
                    gpu.ChangeScaleMenuItem.Text = useFahrenheit ? "Change to Celsius" : "Change to Fahrenheit";
                }
            }

            Properties.Settings.Default.UseFahrenheit = useFahrenheit;
            Properties.Settings.Default.Save();
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Close();
        }
    }
}
