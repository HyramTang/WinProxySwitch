using System;
using System.Runtime.InteropServices;

namespace WinProxySwitch
{
    class Program
    {
        /*
         * 1.打开程序显示Proxy状态
         * 2.如果【开启】则显示开启的状态和Proxy的信息，并显示按键菜单
         * 3.[1].关闭 [2].更改Proxy
         * 4.选择更改，提示输入新Proxy地址
         * 
         * 2.如果【未开启】则判断是否有设置Proxy地址
         * 3.如果没有设置Proxy地址则直接显示按键操作菜单
         * 4.[1].开启 [2].添加Proxy
         * 
         * 3.如果有则显示：[1].开启 [2].更改Proxy
         */

        [DllImport(@"wininet", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "InternetSetOption", CallingConvention = CallingConvention.StdCall)]
        public static extern bool InternetSetOption(int hInternet, int dmOption, IntPtr lpBuffer, int dwBufferLength);

        static void Main(string[] args)
        {
            //打开注册表键 
            Microsoft.Win32.RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true);

            //判断代理状态
            if (rk.GetValue("ProxyEnable").Equals(1))
            {
                Console.WriteLine(string.Format("代理处于[开启]状态，代理地址为：{0}\t\n", rk.GetValue("ProxyServer")));
                Console.WriteLine("请按数字键选择：\t\n\t\n[1].关闭\t\n\t\n[2].更改Proxy");
                Console.WriteLine("\n");

                if (Console.ReadKey().KeyChar.Equals('1'))
                {
                    rk.SetValue("ProxyEnable", 0);
                    Console.WriteLine("开启成功！");
                }
                else
                {
                    rk.SetValue("ProxyServer", Console.ReadLine());
                }
            }
            else
            {
                var proxy = rk.GetValue("ProxyServer").ToString();
                Console.WriteLine(string.Format("代理处于[关闭]状态，代理地址为：{0}\t\n", proxy));

                if (string.IsNullOrEmpty(proxy))
                {
                    Console.Write("请添加Proxy：");
                    rk.SetValue("ProxyServer", Console.ReadLine());
                }
                else
                {
                    Console.WriteLine("请按数字键选择：\t\n\t\n[1].开启\t\n\t\n[2].修改Proxy");
                    if (Console.ReadKey().KeyChar.Equals('1'))
                    {
                        rk.SetValue("ProxyEnable", 1);
                    }
                    else
                    {
                        Console.Write("请输入Proxy：");
                        rk.SetValue("ProxyServer", Console.ReadLine());
                    }
                }
            }

            rk.Flush();
            rk.Close();
            InternetSetOption(0, 39, IntPtr.Zero, 0);
            Console.ReadKey();


            //设置代理可用 
            //rk.SetValue("ProxyEnable", 1);
            //设置代理IP和端口 
            //rk.SetValue("ProxyServer", ConfigurationManager.ConnectionStrings["ProxyServer"].ConnectionString);











            //if (NetWork(ConfigurationManager.ConnectionStrings["LinkName"].ConnectionString, "启用"))
            //{
            //    txtMessage.Text += ConfigurationManager.ConnectionStrings["LinkName"].ConnectionString + "--启用成功！" + "\r\n";
            //}


            //txtMessage.Text += "IE自动脚本配置成功！" + "\r\n";



            //txtMessage.Text += "IE代理服务器配置成功！" + "\r\n";
            //rk.Flush();
            //rk.Close();
            //InternetSetOption(0, 39, IntPtr.Zero, 0);
        }
    }
}
