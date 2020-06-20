using Client.Network;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Client
{
    static class Program
    {
        // Usado para identificar se o programa está funcionando
        public static bool Working = true;

        // Lista de arquivos
        public static Dictionary<string, FileData> Files = new Dictionary<string, FileData>();

        [STAThread]
        static void Main()
        {
            // Inicia o dispositivo de rede
            Socket.Init();

            // Tenta se conectar ao servidor
            if (!Socket.TryConnect())
            {
                MessageBox.Show("O servidor não está disponível.");
                return;
            }

            // Envia para o servidor todos os arquivos que possui
            Send.Join();

            // Abre a janela principal do programa 
            Application.EnableVisualStyles();
            Window.Form = new Window();
            Window.Form.Show();

            // Manuseia todos os dados recebidos enquanto o programa estive funcionando
            while (Working)
            {
                Socket.HandleData();
                Application.DoEvents(); // para que o programa consiga executar adequadamente a janela
            }
        }

        public static void Exit()
        {
            // Termina a conexão com o servidor
            Socket.Device.Disconnect(string.Empty);

            // Espera até que o cliente seja desconectado
            int waitTimer = Environment.TickCount;
            while (Socket.IsConnected && Environment.TickCount < waitTimer + 1000) ;

            // Fecha apliacação
            Working = false;
            Application.Exit();
        }
    }
}