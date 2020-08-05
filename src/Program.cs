using System;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace InstallGTKOnWindows
{
    class Program
    {
        // msys2링크: https://repo.msys2.org/distrib/x86_64/msys2-x86_64-20200720.exe
        // 
        static void Main(string[] args)
        {
            Program pr = new Program();

            WebClient client = new WebClient();
            client.Proxy = null;

            Console.CursorVisible = false;
            Console.WriteLine("MSYS2 설치기 다운로드중....");


            bool doing = true;
            int top = Console.CursorTop;
            long last = 0;
            bool sec = false;
            long all = 10;
            int percentage = 0;

 
            client.DownloadFileCompleted += (sender, e) => {doing = false;};
            client.DownloadProgressChanged += (sender, e) => {
                if (sec) 
                {
                    return;
                }
                else
                {
                    sec = true;
                }
                all = e.TotalBytesToReceive;
                long now = e.BytesReceived;
                decimal speed = (now - last) * 2;
                last = now;
                
                int remainder = (int)((all - last) / speed);
                percentage = e.ProgressPercentage;
                Console.Write($"{percentage}% ({pr.convertUnit(now)} / {pr.convertUnit(all)}), {pr.convertUnit(speed)}/s, {remainder / 60}분 {remainder % 60}초 남음      ");
                Console.SetCursorPosition(0, top);
                };
            client.DownloadFileAsync(new Uri("https://repo.msys2.org/distrib/x86_64/msys2-x86_64-20200720.exe"), "msys2install.exe");
            while (doing)
            {
                sec = false;
                Thread.Sleep(500);
            }
            Console.WriteLine($"100% ({pr.convertUnit(all)} / {pr.convertUnit(all)}), 0 BYTE/s, 0분 0초 남음                   ");

            Console.CursorVisible = true;
            Thread.Sleep(50);
            
            Process.Start("msys2install.exe");
            Console.WriteLine("창이 뜨면 MSYS2를 설치해주세요");
            Process[] processArray = Process.GetProcessesByName("msys2install");
            while (processArray.Length <= 0) 
            {
                processArray = Process.GetProcessesByName("msys2install");
                Thread.Sleep(100);
            }
            GC.Collect();
            while (processArray.Length > 0) 
            {
                processArray = Process.GetProcessesByName("msys2install");
                Thread.Sleep(100);
            }
            GC.Collect();
            Console.WriteLine("MSYS2 설치 완료");

            Process.Start("C:/msys64/msys2.exe", "pacman -Sy");
            while (processArray.Length <= 0) 
            {
                processArray = Process.GetProcessesByName("mintty");
                Thread.Sleep(100);
            }
            GC.Collect();
            while (processArray.Length > 0) 
            {
                processArray = Process.GetProcessesByName("mintty");
                Thread.Sleep(100);
            }
            GC.Collect();

            Process.Start("C:/msys64/msys2.exe", "pacman -S mingw-w64-x86_64-gtk3 mingw-w64-x86_64-glade");
            while (processArray.Length <= 0) 
            {
                processArray = Process.GetProcessesByName("mintty");
                Thread.Sleep(100);
            }
            GC.Collect();
            while (processArray.Length > 0) 
            {
                processArray = Process.GetProcessesByName("mintty");
                Thread.Sleep(100);
            }
            GC.Collect();
            Console.WriteLine("GTK 설치 완료");
            string paths = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);

            if (paths.Contains("C:\\msys64\\mingw64\\bin"))
            {
                if (!paths.Contains("C:\\msys32\\mingw32\\bin"))
                {
                    Environment.SetEnvironmentVariable("Path", paths + ";C:\\msys32\\mingw32\\bin", EnvironmentVariableTarget.User);
                }
            }
            else if (paths.Contains("C:\\msys32\\mingw32\\bin"))
            {
                if (!paths.Contains("C:\\msys64\\mingw32\\bin"))
                {
                    Environment.SetEnvironmentVariable("Path", paths + ";C:\\msys64\\mingw64\\bin", EnvironmentVariableTarget.User);
                }
            }
            else
            {
                Environment.SetEnvironmentVariable("Path", paths + ";C:\\msys64\\mingw64\\bin;C:\\msys32\\mingw32\\bin", EnvironmentVariableTarget.User);
            }
            Console.WriteLine("환경 변수 설정 완료\n설치를 끝내려면 아무 키나 누르세요...");
            Console.ReadKey();

        }
        string convertUnit(decimal speed)
        {
            string unit = "BYTE";
            while (speed > 1000)
            {
                if (unit == "BYTE")
                {
                    unit = "KiB";
                    speed /= 1024;
                    speed = Math.Truncate(speed * 100) / 100;
                }
                else if (unit == "KiB")
                {
                    unit = "MiB";
                    speed /= 1024;
                    speed = Math.Truncate(speed * 100) / 100;
                }
                else
                {
                    unit = "GiB";
                    speed /= 1024;
                    speed = Math.Truncate(speed * 100) / 100;
                    break;
                }
            }
            return $"{speed} {unit}";
        }
    }
}
