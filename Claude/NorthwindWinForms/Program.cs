using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NorthwindWinForms
{
    internal static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // フェーズ0: DB接続疎通確認用の Form0 を起動する。
            // （フェーズ2以降で new Form1() に戻す）
            Application.Run(new Form0());
        }
    }
}
