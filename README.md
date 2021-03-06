# Transferidor de Arquivos
O projeto se trata de um transferidor de arquivos Peer-to-Peer utilizando o protocolo UDP para comunicação, contendo há a existência de um servidor responsável por listar os arquivos disponíveis e indiciar o proprietário de determinado arquivo quando requisitado.

Ele foi desenvolvido na linguagem C# utilizando o framework [Lidgren.Network](https://github.com/lidgren/lidgren-network-gen3) para o controle da rede e é destinado apenas para Windows. 

## Funcionamento 
| Transmissão             | Descrição                                                                                                   | Pacote                     |
|-------------------------|-------------------------------------------------------------------------------------------------------------|----------------------------|
| Cliente -> Servidor     | Envia o nome de todos os arquivos que estão no diretório em que o programa está sendo executado | ClientPackets.Join         |
| Servidor -> Cliente     | Envia todos os arquivos disponíveis para serem baixados                                                     | ServerPackets.Files        |
| Cliente -> Servidor     | Requisita o proprietário de um determinado arquivo                                                          | ClientPackets.RequestOwner |
| Servidor -> Cliente     | Envia o endereço de rede do proprietário do arquivo                                                         | ServerPackets.FileOwner    |
| Cliente -> Proprietário | Requisita o download do arquivo                                                                             | PeerPackets.DownloadFile   |
| Proprietário -> Cliente | Envia algumas informações úteis do arquivo                                                                  | PeerPackets.FileData       |
| Proprietário -> Cliente | Envia diversos fragmentos do arquivo                                                                        | PeerPackets.File           |

## Compilando
O projeto já vem compilado na pasta */Build*, entretanto, caso haja necessidade, pode-se compilar o projeto normalmente utilizando o Visual Studio.

## Executando
1. Certifique-se que você possui o .NET Framework 4.8 instalado em sua máquina e a DLL Lidgren.Network no mesmo diretório do programa.
2. Execute primeiramente o servidor.
3. Então, execute um ou mais clientes.
4. Selecione um arquivo e faça download (:
