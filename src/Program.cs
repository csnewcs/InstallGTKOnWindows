using System;
using System.Net;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace src
{
    class Program
    {
        // msys2링크: http://repo.msys2.org/distrib/x86_64/msys2-x86_64-20190524.exe
        // 
        static void Main(string[] args)
        {
            Program pr = new Program();

            WebClient client = new WebClient();
            client.Proxy = null;

            Console.CursorVisible = false;
            Console.WriteLine("MSYS2 다운로드중....");


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
                decimal speed = now - last;
                last = now;
                
                int remainder = (int)((all - last) / speed);
                percentage = e.ProgressPercentage;
                Console.Write($"{percentage}% ({pr.convertUnit(now)} / {pr.convertUnit(all)}), {pr.convertUnit(speed)}, {remainder / 60}분 {remainder % 60}초 남음      ");
                Console.SetCursorPosition(0, top);
                };
            client.DownloadFileAsync(new Uri("http://repo.msys2.org/distrib/x86_64/msys2-x86_64-20190524.exe"), "msys2install.exe");
            while (doing)
            {
                sec = false;
                Thread.Sleep(1000);
            }
            
            ProcessStartInfo startInfo = new ProcessStartInfo("msys2install.exe");
            startInfo.WorkingDirectory = @"C:\";

            Process msys2 = new Process();
            msys2.StartInfo = startInfo;
            msys2.Start();

            Console.WriteLine("창이 뜨면 MSYS2를 설치해주세요");
            msys2.Disposed += new Program().msys2Disposed;
        }
        void msys2Disposed(object sender, EventArgs e)
        {
            Console.WriteLine("MSYS2 설치 완료");
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
