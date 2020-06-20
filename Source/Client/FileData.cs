using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Client
{
    class FileData
    {
        // Dados do arquivo
        public string Name;
        public long Size;
        public IPEndPoint Owner;

        // Usado para escrever o arquivo
        public FileStream Stream;

        // Item disposto na tabela
        public ListViewItem Item;

        // Para garantir a transmissão do arquivo
        public int LastPart = -1;
        public int CreationTime;

        public FileData(string name, long size, IPEndPoint owner)
        {
            // Define os dados
            Name = name;
            Size = size;
            Owner = owner;
            CreationTime = Environment.TickCount;

            // Adiciona os dados do arquivo na lista
            Item = new ListViewItem((Window.Form.lstDownloads.Items.Count + 1).ToString());
            Item.SubItems.Add(Name);
            Item.SubItems.Add(BytesToString(Size));
            Item.SubItems.Add("0%");
            Item.SubItems.Add(Owner.ToString());
            Window.Form.lstDownloads.Items.Add(Item);

            // Diretório do arquivo
            var directory = new FileInfo($"{Application.StartupPath}\\Received\\{Name}");
            directory.Directory.Create(); // cria o diretório caso não existir

            // Abre um fluxo para a escrita do arquivo
            Stream = new FileStream(directory.FullName, FileMode.Create, FileAccess.Write);
        }

        private string BytesToString(long byteCount)
        {
            // Converte a quantidade de bytes para uma string adequada
            string[] prefix = { "B", "KB", "MB", "GB" };
            int place = Convert.ToInt32(Math.Floor(Math.Log(byteCount, 1024)));
            double num = Math.Round(byteCount / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + prefix[place];
        }

        public void Make(int currentPart, byte[] buffer)
        {
            // Verifica se as partes estão sendo recebidas na ordem correta
            if (currentPart - 1 != LastPart || Environment.TickCount >= CreationTime + 5000)
            {
                Item.SubItems[3].Text = "Erro";
                Stream.Dispose();
                return;
            }

            // Escreve o fragmento no arquivo
            Stream.Write(buffer, 0, buffer.Length);
            LastPart++;

            // Demonstra a porcentagem 
            double percentage = Stream.Position * 100.0 / Size;
            Item.SubItems[3].Text = $"{percentage:f2}%";

            // Fecha o fluxo caso tiver terminado de receber o arquivo
            if (percentage == 100) Stream.Dispose();
        }
    }
}