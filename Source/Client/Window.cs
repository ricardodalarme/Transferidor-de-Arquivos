using Client.Network;
using System.Windows.Forms;

namespace Client
{
    public partial class Window : Form
    {
        // Usado para acessar os dados da janela
        public static Window Form;

        public Window()
        {
            InitializeComponent();
        }

        private void butDownload_Click(object sender, System.EventArgs e)
        {
            // Apenas se estiver selecionando um arquivo
            if (lstFiles.SelectedItem == null)
            {
                MessageBox.Show("Selecione algum arquivo.");
                return;
            }

            // Verifica com o servidor quem tem o arquivo para baixá-lo
            Send.RequestOwner((string)lstFiles.SelectedItem);
            lstFiles.Focus();
        }

        private void Window_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Fecha a aplicação
            Program.Exit();
        }
    }
}
