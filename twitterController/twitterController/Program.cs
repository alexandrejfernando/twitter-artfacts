using System;
using System.Windows.Forms;
using Twittosfera.Controllers;

namespace Twittosfera
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FormController formController = new FormController(new frmTwitterController());
            formController.WindowState = FormWindowState.Maximized;
            formController.Executar();

            tw
        }
    }
}
