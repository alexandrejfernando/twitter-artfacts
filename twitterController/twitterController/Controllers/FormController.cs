using System;
using System.Windows.Forms;

namespace Twittosfera.Controllers
{
    public class FormController
    {
        private Form _form;

        public FormController(Form form)
        {
            _form = form;
        }

        public void Executar()
        {

            Application.Run(_form);

        }
        public FormWindowState WindowState
        {
            get { return _form.WindowState; }
            set { _form.WindowState = value; }
        }
    }
}
