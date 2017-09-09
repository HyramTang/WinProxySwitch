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
            if (rk.GetValue("ProxyEnable").Equals(1))//开启状态
            {
                Console.WriteLine(string.Format("代理处于[开启]状态，代理地址为：{0}\n", rk.GetValue("ProxyServer")));
                Console.WriteLine("请按数字键选择：\n\n[1].关闭\n\n[2].修改代理\n");

                if (Console.ReadKey(true).KeyChar.Equals('1'))
                {
                    rk.SetValue("ProxyEnable", 0);
                    Console.WriteLine("代理已[关闭]!");
                }
                else
                {
                    Console.Write("请输入代理地址：");
                    rk.SetValue("ProxyServer", Console.ReadLine());
                    Console.WriteLine(string.Format("\n代理已[开启]，代理已修改为:{0}", rk.GetValue("ProxyServer")));
                }
            }
            else//关闭状态
            {
                var proxy = rk.GetValue("ProxyServer").ToString();
                Console.WriteLine(string.Format("代理处于[关闭]状态，代理地址为：{0}\n", proxy));

                if (string.IsNullOrEmpty(proxy))
                {
                    Console.Write("请输入代理地址：");
                    rk.SetValue("ProxyServer", Console.ReadLine());
                    rk.SetValue("ProxyEnable", 1);
                    Console.WriteLine(string.Format("\n代理已[开启]，代理已修改为:{0}", rk.GetValue("ProxyServer")));
                }
                else
                {
                    Console.WriteLine("请按数字键选择：\n\n[1].开启\n\n[2].修改代理\n");
                    if (Console.ReadKey(true).KeyChar.Equals('1'))
                    {
                        rk.SetValue("ProxyEnable", 1);
                        Console.WriteLine("代理已[开启]!");
                    }
                    else
                    {
                        Console.Write("请输入代理地址：");
                        rk.SetValue("ProxyServer", Console.ReadLine());
                        rk.SetValue("ProxyEnable", 1);
                        Console.WriteLine(string.Format("\n代理已[开启]，代理已修改为:{0}", rk.GetValue("ProxyServer")));
                    }
                }
            }

            //刷新注册表
            rk.Flush();
            rk.Close();
            //刷新IE设置
            InternetSetOption(0, 39, IntPtr.Zero, 0);

            Console.WriteLine("\n请按任意键退出程序！");
            Console.ReadKey(true);

            return;
        }
    }
}
