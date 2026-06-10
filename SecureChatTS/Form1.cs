// É aqui que estão as classes para criar e ler os "pacotes" de dados
using EI.SI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
// Necessário para lidar com Endereços IP (IPAddress) e pontos de ligação (IPEndPoint).

using System.Net.Sockets;
// Para poder usar o TcpClient, o TcpListener nem o NetworkStream

using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;


namespace SecureChatTS
{
    public partial class Form1 : Form
    {
        // Define o número da porta fixa (20000) onde o servidor estará à espera de algo;
        private const int PORT = 20000;
        // Declara o fluxo que será usado para enviar e receber bytes
        NetworkStream networkStream;
        // Serve para empacotar e interpretar as mensagens seguindo o protocolo definido
        ProtocolSI protocolSI;
        //objeto que vai iniciar e gerir a ligação
        TcpClient client;

        private const int SALTSIZE = 8;
        private const int NUMBER_OF_ITERATIONS = 1000;

        private bool autenticado = false;

        private string usernameAtual = "";


        private RSACryptoServiceProvider rsa;
        private Aes aes;
        private string publicKey;
        private string privateKey;


        

        

        public Form1()
        {
            InitializeComponent();

            

            // CRIAR A COMUNICAÇÃO COM O SERVIDOR

            // Define o destino: o próprio PC (127.0.0.1) na porta 20000.
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);

            // Cria o objeto que vai realizar a ligação
            client = new TcpClient();

            // Estabelece a ligação real com o servidor.
            client.Connect(endPoint);

            // Abre o "tubo" de comunicação para enviar e receber bytes.
            networkStream = client.GetStream();

            // Inicializa o formatador de mensagens obrigatório do projeto.
            protocolSI = new ProtocolSI();


            rsa = new RSACryptoServiceProvider(2048);

            publicKey = rsa.ToXmlString(false);
            privateKey = rsa.ToXmlString(true);

            ProtocolSI protocoloRSA = new ProtocolSI();

            byte[] packetRSA =
                protocoloRSA.Make(
                    ProtocolSICmdType.USER_OPTION_1,
                    publicKey
                );

            networkStream.Write(packetRSA, 0, packetRSA.Length);

            // Cria um "segundo processo" que corre o método 'LerMensagens'
            Thread t = new Thread(LerMensagens);
            t.IsBackground = true; // Se fechares a janela, a thread morre também
            t.Start();
        }


        private static byte[] GenerateSalt(int size)
        {
            //Gera o numero encriptado aleatorio.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return buff;
        }

        private static byte[] GenerateSaltedHash(string plainText, byte[] salt)
        {
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(plainText, salt, NUMBER_OF_ITERATIONS);
            return rfc2898.GetBytes(32);
        }

        private string EncryptString(string texto)
        {
            ICryptoTransform encryptor =
                aes.CreateEncryptor();

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

        private string DecryptString(string texto)
        {
            ICryptoTransform decryptor =
                aes.CreateDecryptor();

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

        private void LerMensagens()
        {
            while (true)
            {
                try
                {
                    // Fica aqui parado à espera que chegue QUALQUER coisa do servidor
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

                    Console.WriteLine(protocolSI.GetCmdType());

                    if (protocolSI.GetCmdType() ==
                        ProtocolSICmdType.USER_OPTION_2)
                    {
                        string chaveRecebida = protocolSI.GetStringFromData();

                        byte[] chaveCifrada = Convert.FromBase64String(chaveRecebida);

                        string dadosAES = Encoding.UTF8.GetString(
                            rsa.Decrypt(chaveCifrada, false)
                        );

                        string[] partes = dadosAES.Split('|');

                        aes = Aes.Create();
                        aes.Key = Convert.FromBase64String(partes[0]);
                        aes.IV = Convert.FromBase64String(partes[1]);

                        MessageBox.Show("Chave AES recebida!");
                    }

                    // Se o que chegou for uma mensagem (DATA)
                    if (protocolSI.GetCmdType() == ProtocolSICmdType.DATA)
                    {
                        MessageBox.Show("Recebi DATA");
                        string textoRecebido = DecryptString(protocolSI.GetStringFromData());

                        string[] partesAssinatura = textoRecebido.Split( new char[] { '|' },2 );

                        string msgRecebida = partesAssinatura[0];

                        MessageBox.Show(msgRecebida);

                        // Obtém a hora atual no formato HH:mm:ss
                        string hora = DateTime.Now.ToString("HH:mm:ss");

                            // Como esta thread é "esquema à parte", temos de usar Invoke para escrever na RichTextBox
                            richTextBox1.Invoke(new MethodInvoker(delegate
                            {
                                // Define a cor cinza para a hora e escreve
                                richTextBox1.SelectionColor = Color.Gray;
                                richTextBox1.AppendText("[" + hora + "] ");

                                // Se a mensagem contiver "Cliente X:", vamos destacar o nome
                                if (msgRecebida.Contains(":"))
                                {
                                    string[] partes = msgRecebida.Split(new[] { ':' }, 2);


                                    // Define cor preta e negrito para o nome do remetente
                                    richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                                    richTextBox1.SelectionColor = Color.Black;
                                    richTextBox1.AppendText(partes[0].Trim() + ": ");

                                    // Define cor branca e fonte normal para o texto da mensagem
                                    richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
                                    richTextBox1.SelectionColor = Color.White;
                                    richTextBox1.AppendText(partes[1] + Environment.NewLine);
                                }
                                else
                                {
                                    // Caso a mensagem não tenha o formato padrão, escreve normal
                                    richTextBox1.SelectionColor = Color.Black;
                                    richTextBox1.AppendText(msgRecebida + Environment.NewLine);
                                }

                                // Faz scroll automático para o fim
                                richTextBox1.ScrollToCaret();
                            }));
                        }
                    
                }
                catch { break; }
            }


        }
        private string AssinarMensagem(string mensagem)
        {
            byte[] dados =
                Encoding.UTF8.GetBytes(mensagem);

            byte[] assinatura =
                rsa.SignData(
                    dados,
                    CryptoConfig.MapNameToOID("SHA256")
                );

            return Convert.ToBase64String(assinatura);
        }

        private bool UserExists(string username)
        {
            using (SqlConnection conn = new SqlConnection(
                @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Filipa\source\repos\TS\SecureChatTS\SecureChatTS\SecureChatDB.mdf;Integrated Security=True"))
            {
                conn.Open();

                string sql = "SELECT COUNT(*) FROM Users WHERE Username = @username";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@username", username);

                int count = (int)cmd.ExecuteScalar();

                return count > 0;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private bool VerifyLogin(string username, string password)
        {

            SqlConnection conn = null;

            try
            {
                conn = new SqlConnection();

                conn.ConnectionString =
                @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Filipa\source\repos\TS\SecureChatTS\SecureChatTS\SecureChatDB.mdf;Integrated Security=True";

                conn.Open();

                String sql = "SELECT * FROM Users WHERE Username = @username";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;

                SqlParameter param = new SqlParameter("@username", username);

                cmd.Parameters.Add(param);

                cmd.Connection = conn;

                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    return false;
                }

                reader.Read();

                byte[] saltedPasswordHashStored =
                    (byte[])reader["SaltedPasswordHash"];

                byte[] saltStored =
                    (byte[])reader["Salt"];

                conn.Close();

                byte[] hash =
                    GenerateSaltedHash(password, saltStored);

                return saltedPasswordHashStored.SequenceEqual(hash);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void Register(string username, byte[] saltedPasswordHash, byte[] salt)
        {
            using (SqlConnection conn = new SqlConnection(
                @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Filipa\source\repos\TS\SecureChatTS\SecureChatTS\SecureChatDB.mdf;Integrated Security=True"))
            {
                conn.Open();

                string sql =
                    "INSERT INTO Users (Username, SaltedPasswordHash, Salt) " +
                    "VALUES (@username,@hash,@salt)";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@hash", saltedPasswordHash);
                cmd.Parameters.AddWithValue("@salt", salt);

                cmd.ExecuteNonQuery();
            }
        }

        private void bt_Enviar_Click(object sender, EventArgs e)
        {
            if (!autenticado)
            {
                MessageBox.Show("Tem de fazer login primeiro.");
                return;
            }

            // Guarda o texto da caixa de texto numa variável e limpa a caixa a seguir
            string msg = textBoxMensagem.Text;

            if (string.IsNullOrWhiteSpace(msg)) return;

            textBoxMensagem.Clear();

            // UM PROTOCOLO NOVO AQUI SÓ PARA ENVIAR (Para não chocar com a Thread)
            ProtocolSI protocoloEnvio = new ProtocolSI();

            string mensagemCompleta = usernameAtual + ":" + msg;

            string assinatura = AssinarMensagem(mensagemCompleta);

            string pacoteFinal = mensagemCompleta + "|" + assinatura;
            string mensagemCifrada = EncryptString(pacoteFinal);

            byte[] packet = protocoloEnvio.Make(
                ProtocolSICmdType.DATA,
                mensagemCifrada
            );
            // Empurra os bytes pelo "tubo" (stream) em direção ao servidor
            networkStream.Write(packet, 0, packet.Length);
        }

        private void bt_Sair_Click(object sender, EventArgs e)
        {
            // UM PROTOCOLO NOVO AQUI TAMBÉM
            ProtocolSI protocoloEnvio = new ProtocolSI();

            // Cria um pacote especial do tipo EOT (Fim de Transmissão) para avisar que vai sair
            byte[] eot = protocoloEnvio.Make(ProtocolSICmdType.EOT);

            // Envia esse aviso de saída para o servidor
            networkStream.Write(eot, 0, eot.Length);

            // Espera que o servidor responda com um "OK" (ACK) a confirmar que percebeu que vamos sair
     
            // Fecha o "tubo" de comunicação
            networkStream.Close();

            // Fecha a ligação do cliente TCP
            client.Close();

            // Fecha a janela do formulário (o programa termina)
            this.Close();
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void PedirHistorico()
        {
            ProtocolSI p = new ProtocolSI();

            byte[] packet = p.Make(ProtocolSICmdType.USER_OPTION_3);

            networkStream.Write(packet, 0, packet.Length);
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            string password = textBoxPassword.Text;
            string username = textBoxUsername.Text;

            if (VerifyLogin(username, password))
            {
                autenticado = true;
                usernameAtual = username;


                buttonLogin.Enabled = false;
                buttonRegistar.Enabled = false;

                textBoxUsername.Enabled = false;
                textBoxPassword.Enabled = false;

                MessageBox.Show("Utilizador logado com sucesso");

                PedirHistorico();

            }
            else
            {
                MessageBox.Show("Credenciais inválidas");
            }
        }

        private void buttonRegistar_Click(object sender, EventArgs e)
        {
            string username = textBoxUsername.Text;
            string pass = textBoxPassword.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Introduza um username.");
                return;
            }

            if (string.IsNullOrWhiteSpace(pass))
            {
                MessageBox.Show("Introduza uma password.");
                return;
            }

            if (UserExists(username))
            {
                MessageBox.Show("Esse username já existe.");
                return;
            }

            byte[] salt = GenerateSalt(SALTSIZE);
            byte[] hash = GenerateSaltedHash(pass, salt);

            Register(username, hash, salt);

            MessageBox.Show("Utilizador registado com sucesso");
        }
    }
}
