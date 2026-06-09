using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using EI.SI;
using System.Runtime.Remoting;
using System.Threading;
using System.IO;
using System.Security.Cryptography;

namespace Servidor
{
    internal class Program
    {
        // Guarda todos os clientes para podermos enviar mensagens a todos
        public static List<ClientHandler> clientesLigados = new List<ClientHandler>();

        public static int totalMensagens = 0;

        public static Aes aesGlobal = Aes.Create();

        // Define a porta fixa onde o servidor vai "abrir a porta"
        private const int PORT = 20000;

       

        //contar os clientes que entram
        static void Main(string[] args)
        {
            // Define que o servidor aceita ligações em qualquer IP do PC na porta 20000.
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, PORT);
            // Cria o "ouvinte" que monitoriza os pedidos de entrada na rede.
            TcpListener listener = new TcpListener(endPoint);

            // Inicia o serviço e começa a aceitar comunicações.
            listener.Start();
            Console.WriteLine("Servidor Pronto");

            int clientCounter = 0;

            // Ciclo infinito para que o servidor nunca pare de aceitar novos clientes.
            while (true)
            {
                // O servidor "congela" nesta linha e fica à espera; só avança quando um cliente bater à porta.
                TcpClient client = listener.AcceptTcpClient();
                // Incrementa o número do cliente e mostra na consola.
                clientCounter++;
                Console.WriteLine("Cliente {0} Ligado", clientCounter);
                //Tratar o cliente que se ligou
                ClientHandler clientHandler = new ClientHandler(client, clientCounter);

                // ADICIONA À LISTA: Guardamos este cliente na nossa lista global
                clientesLigados.Add(clientHandler);

                clientHandler.Handle();
            }
        }


    }
    //Deixar os clientes ligados ao servidor
    class ClientHandler
    {
        private TcpClient client; // Guarda a ligação do cliente específico.
        private int clientID;     // Guarda o ID deste cliente.

        private NetworkStream networkStream;

        private string publicKeyCliente;

        private Aes aes;


        // Construtor: recebe o cliente e o ID vindos do servidor principal.
        public ClientHandler(TcpClient client, int clientID)
        {
            this.client = client;
            this.clientID = clientID;

            this.networkStream = this.client.GetStream();
        }

        // Inicia a tarefa em paralelo para não bloquear o servidor.
        public void Handle()
        {
            Thread thread = new Thread(threadHandler);
            thread.Start();
        }

        // Permite ao servidor enviar uma mensagem diretamente para este cliente
        public void EnviarMensagem(byte[] pacote)
        {
            try
            {
                networkStream.Write(pacote, 0, pacote.Length);
            }
            catch { }
        }

        // Este método corre numa thread à parte para não travar o resto do servidor
        private void threadHandler()
        {
            
            // Cria a ferramenta que traduz os bytes da rede em mensagens (tipo DATA ou EOT)
            ProtocolSI protocolSI = new ProtocolSI();

            // Repete tudo o que está aqui dentro enquanto o cliente não mandar o sinal de "Sair" (EOT)
            while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)
            {
                // O programa para aqui e fica à espera que o cliente envie bytes; quando chegam, guarda-os no buffer
                int bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                byte[] ack;

                // Verifica qual foi o tipo de pacote que a ferramenta acabou de ler do buffer
                switch (protocolSI.GetCmdType())
                {
                    case ProtocolSICmdType.USER_OPTION_1:

                        publicKeyCliente = protocolSI.GetStringFromData();
                        Console.WriteLine("Chave pública recebida.");

                        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                        rsa.FromXmlString(publicKeyCliente);

                        string dadosAES =
                            Convert.ToBase64String(Program.aesGlobal.Key) + "|" +
                            Convert.ToBase64String(Program.aesGlobal.IV);

                        byte[] chaveCifrada =
                            rsa.Encrypt(Encoding.UTF8.GetBytes(dadosAES), false);

                        string chaveBase64 =
                            Convert.ToBase64String(chaveCifrada);

                        byte[] packet =
                            protocolSI.Make(
                                ProtocolSICmdType.USER_OPTION_2,
                                chaveBase64
                            );

                        networkStream.Write(packet, 0, packet.Length);

                        Console.WriteLine("Chave AES enviada.");
                        break;

                    case ProtocolSICmdType.DATA: // Se o pacote for uma mensagem de texto normal:
                                                 // Transforma os bytes em letras e escreve o texto na janela do servidor

                        string textoRecebido = DecryptString( protocolSI.GetStringFromData());

                        string[] partes = textoRecebido.Split( new char[] { '|' }, 2);

                        string mensagem = partes[0];
                        string assinatura = partes[1];

                        bool assinaturaValida =
                            VerificarAssinatura(
                                mensagem,
                                assinatura
                            );

                        if (!assinaturaValida)
                        {
                            Console.WriteLine(
                                "Assinatura inválida!"
                            );
                            break;
                        }

                        Console.WriteLine(
                            "Assinatura válida."
                        );

                        string msgTexto = protocolSI.GetStringFromData();

                        

                        File.AppendAllText(
                            "chatlog.txt",
                            "[" + DateTime.Now.ToString("HH:mm:ss") + "] "
                            + msgTexto
                            + Environment.NewLine
                        );

                        File.AppendAllText(
                            "chatlog.txt",
                            "[" + DateTime.Now.ToString("HH:mm:ss") + "] Cliente "
                            + clientID + " ligou-se."
                            + Environment.NewLine
                        );

                        File.AppendAllText(
                            "chatlog.txt",
                            "[" + DateTime.Now.ToString("HH:mm:ss") + "] Cliente "
                            + clientID + " desligou-se."
                            + Environment.NewLine
                        );


                        Console.WriteLine("Cliente " + clientID + " : " + mensagem);

                        Program.totalMensagens++;

                        Console.WriteLine(
                            "Mensagens processadas: "
                            + Program.totalMensagens
                        );
                        string mensagemCifrada = EncryptString(mensagem);

                        byte[] pacoteParaTodos =
                            protocolSI.Make(ProtocolSICmdType.DATA,mensagemCifrada);

                      
                        // Percorremos a lista e "empurramos" a mensagem para cada um
                        foreach (ClientHandler outroCliente in Program.clientesLigados)
                        {
                            outroCliente.EnviarMensagem(pacoteParaTodos);
                        }

                        // Cria um pacote de resposta que apenas diz "OK, recebi" (ACK)
                        ack = protocolSI.Make(ProtocolSICmdType.ACK);
                        // Atira esse pacote de "OK" de volta pelo tubo para o cliente
                        networkStream.Write(ack, 0, ack.Length);
                        break;

                    case ProtocolSICmdType.EOT: // Se o pacote disser "Vou desligar":
                                                // Escreve no servidor que aquele cliente específico se vai embora
                        Console.WriteLine("Fim da comunicação do cliente {0}", clientID);

                        // Cria o último pacote de "OK" para confirmar o fecho da ligação
                        ack = protocolSI.Make(ProtocolSICmdType.ACK);
                        // Envia o último "OK" de volta para o cliente
                        networkStream.Write(ack, 0, ack.Length);
                        // Remove da lista para o servidor não tentar enviar mensagens a quem já saiu
                        Program.clientesLigados.Remove(this);
                        break;
                }
            }
        }

        private bool VerificarAssinatura( string mensagem, string assinaturaBase64)
        {
            RSACryptoServiceProvider rsa =
                new RSACryptoServiceProvider();

            rsa.FromXmlString(publicKeyCliente);

            byte[] assinatura =
                Convert.FromBase64String(
                    assinaturaBase64
                );

            byte[] dados =
                Encoding.UTF8.GetBytes(mensagem);

            return rsa.VerifyData(
                dados,
                CryptoConfig.MapNameToOID("SHA256"),
                assinatura
            );
        }

        private string DecryptString(string texto)
        {
            ICryptoTransform decryptor =
                Program.aesGlobal.CreateDecryptor();

            byte[] dados =
                Convert.FromBase64String(texto);

            byte[] decifrado =
                decryptor.TransformFinalBlock(
                    dados,
                    0,
                    dados.Length
                );

            return Encoding.UTF8.GetString(decifrado);
        }

        private string EncryptString(string texto)
        {
            ICryptoTransform encryptor =
                Program.aesGlobal.CreateEncryptor();

            byte[] dados =
                Encoding.UTF8.GetBytes(texto);

            byte[] cifrado =
                encryptor.TransformFinalBlock(
                    dados,
                    0,
                    dados.Length
                );

            return Convert.ToBase64String(cifrado);
        }
    }
}



