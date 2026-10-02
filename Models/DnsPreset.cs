namespace WinClickWpf.Models
{
    public class DnsPreset
    {
        public string Name { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string PrimaryDns { get; set; } = string.Empty;
        public string SecondaryDns { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconSvgPath { get; set; } = string.Empty;

        public bool IsDhcp => PrimaryDns.Equals("DHCP", System.StringComparison.OrdinalIgnoreCase);

        public string DisplayText => IsDhcp ? $"{Name} (Авто)" : $"{Name} ({PrimaryDns})";

        public DnsPreset() { }

        public DnsPreset(string name, string provider, string primary, string secondary, string desc, string svgPath)
        {
            Name = name;
            Provider = provider;
            PrimaryDns = primary;
            SecondaryDns = secondary;
            Description = desc;
            IconSvgPath = svgPath;
        }
    }
}
