namespace WinClickWpf.Models
{
    public class SystemSpecs
    {
        public string CpuName { get; set; } = "Загрузка...";
        public string CpuCores { get; set; } = "";
        public string GpuName { get; set; } = "Загрузка...";
        public string RamTotal { get; set; } = "Загрузка...";
        public string StorageInfo { get; set; } = "Загрузка...";
        public string Motherboard { get; set; } = "Загрузка...";
        public string OsName { get; set; } = "Windows";
        public string OsBuild { get; set; } = "";
        public string OsArchitecture { get; set; } = "64-bit";
        public string PcName { get; set; } = Environment.MachineName;
        public string UserName { get; set; } = Environment.UserName;
        public string Uptime { get; set; } = "";
    }
}
