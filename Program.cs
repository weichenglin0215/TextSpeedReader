using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Web;

using System.IO;
using System.Text;
using System.Text.RegularExpressions;


namespace TextSpeedReader
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            JTextFileLib.Instance();
            Application.Run(new FormTextSpeedReader());
        }
    }
}
