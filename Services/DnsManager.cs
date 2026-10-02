using System.Diagnostics;
using WinClickWpf.Models;

namespace WinClickWpf.Services
{
    public static class DnsManager
    {
        public static List<DnsPreset> Presets { get; } = new()
        {
            new DnsPreset(
                "Cloudflare", 
                "Cloudflare Inc.", 
                "1.1.1.1", 
                "1.0.0.1", 
                "Самый быстрый DNS в мире с защитой приватности без логов", 
                "M7 2v11h3v9l7-12h-4l4-8z"),
                
            new DnsPreset(
                "Google Public DNS", 
                "Google LLC", 
                "8.8.8.8", 
                "8.8.4.4", 
                "Высокая стабильность, скорость и глобальная доступность", 
                "M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-1 17.93c-3.95-.49-7-3.85-7-7.93 0-.62.08-1.21.21-1.79L9 15v1c0 1.1.9 2 2 2v1.93zm6.9-2.54c-.26-.81-1-1.39-1.9-1.39h-1v-3c0-.55-.45-1-1-1H8v-2h2c.55 0 1-.45 1-1V7h2c1.1 0 2-.9 2-2v-.41c2.93 1.19 5 4.06 5 7.41 0 2.08-.8 3.97-2.1 5.39z"),
                
            new DnsPreset(
                "AdGuard DNS", 
                "AdGuard Software", 
                "94.140.14.14", 
                "94.140.15.15", 
                "Блокировка рекламы, фишинга и трекеров на уровне всей системы", 
                "M12 2L4 5v6.09c0 5.05 3.41 9.76 8 10.91 4.59-1.15 8-5.86 8-10.91V5l-8-3zm6 9.09c0 4-2.55 7.7-6 8.83-3.45-1.13-6-4.82-6-8.83v-4.7l6-2.25 6 2.25v4.7z"),
                
            new DnsPreset(
                "Comss.one DNS", 
                "Comss.ru", 
                "92.223.65.71", 
                "92.38.151.151", 
                "Разблокировка зарубежных ресурсов, сайтов и сервисов в РФ", 
                "M12 2.5s-4 4.5-4 9.5c0 2.5 1 4.5 2 5.5v3.5l2-1 2 1V17.5c1-1 2-3 2-5.5 0-5-4-9.5-4-9.5zm0 8a2 2 0 110-4 2 2 0 010 4z"),
                
            new DnsPreset(
                "Quad9", 
                "Quad9 Foundation", 
                "9.9.9.9", 
                "149.112.112.112", 
                "Безопасность с блокировкой вредоносных доменов и фишинга", 
                "M18 8h-1V6c0-2.76-2.24-5-5-5S7 3.24 7 6v2H6c-1.1 0-2 .9-2 2v10c0 1.1.9 2 2 2h12c1.1 0 2-.9 2-2V10c0-1.1-.9-2-2-2zm-6 9c-1.1 0-2-.9-2-2s.9-2 2-2 2 .9 2 2-.9 2-2 2zm3.1-9H8.9V6c0-1.71 1.39-3.1 3.1-3.1 1.71 0 3.1 1.39 3.1 3.1v2z"),
                
            new DnsPreset(
                "OpenDNS", 
                "Cisco Systems", 
                "208.67.222.222", 
                "208.67.220.220", 
                "Надежный быстрый DNS от Cisco с защитой от сбоев", 
                "M2 4h20v4H2V4zm0 6h20v4H2v-4zm0 6h20v4H2v-4zM4 6h2V5H4v1zm0 6h2v-1H4v1zm0 6h2v-1H4v1z"),
                
            new DnsPreset(
                "Автоматический (DHCP)", 
                "Ваш роутер / Провайдер", 
                "DHCP", 
                "DHCP", 
                "Сброс на стандартные DNS-серверы вашего провайдера", 
                "M17.65 6.35C16.2 4.9 14.21 4 12 4c-4.42 0-7.99 3.58-7.99 8s3.57 8 7.99 8c3.73 0 6.84-2.55 7.73-6h-2.08c-.82 2.33-3.04 4-5.65 4-3.31 0-6-2.69-6-6s2.69-6 6-6c1.66 0 3.14.69 4.22 1.78L13 11h7V4l-2.35 2.35z")
        };

        public static DnsPreset SelectedPreset { get; set; } = Presets[0]; // Default: Cloudflare

        public static void ApplyDns(DnsPreset preset, Action<string>? logCallback = null)
        {
            try
            {
                if (preset.IsDhcp)
                {
                    logCallback?.Invoke("Сброс DNS на автоматический (DHCP) для всех адаптеров...");
                    ProcessRunner.RunPowerShell(@"
$adapters = Get-NetAdapter | Where-Object { $_.Status -eq 'Up' -and $_.Virtual -ne $true }
foreach ($a in $adapters) {
    Set-DnsClientServerAddress -InterfaceIndex $a.InterfaceIndex -ResetServerAddresses -ErrorAction SilentlyContinue
}
ipconfig /flushdns
");
                }
                else
                {
                    logCallback?.Invoke($"Установка {preset.Name} ({preset.PrimaryDns}, {preset.SecondaryDns}) на сетевые адаптеры...");
                    ProcessRunner.RunPowerShell($@"
$adapters = Get-NetAdapter | Where-Object {{ $_.Status -eq 'Up' -and $_.Virtual -ne $true }}
foreach ($a in $adapters) {{
    Set-DnsClientServerAddress -InterfaceIndex $a.InterfaceIndex -ServerAddresses ('{preset.PrimaryDns}', '{preset.SecondaryDns}') -ErrorAction SilentlyContinue
}}
ipconfig /flushdns
");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ApplyDns error: {ex.Message}");
            }
        }
    }
}
