using EI.SI;                            // É aqui que estão as classes para criar e ler os "pacotes" de dados
using System;                           // Biblioteca base do C# para tipos fundamentais.
using System.Drawing;                   // Permite manipular elementos gráficos e cores (como Color.Gray).
using System.Linq;                      // Fornece métodos de extensão para coleções e arrays (como o SequenceEqual).
using System.Net;                       // Necessário para lidar com Endereços IP (IPAddress) e pontos de ligação (IPEndPoint).
using System.Net.Sockets;               // Para poder usar o TcpClient, o TcpListener nem o NetworkStream
using System.Security.Cryptography;     // Contém os algoritmos de segurança (RSA, AES, PBKDF2 e RNG).
using System.Text;                      // Permite manipular codificações de texto (como Encoding.UTF8).
using System.Threading;                 // Permite criar e controlar execuções paralelas em Threads.
using System.Windows.Forms;             // Infraestrutura base das janelas, caixas de texto e botões do Windows.
using System.Data.SqlClient;            // Permite conectar, ler e escrever numa base de dados Microsoft SQL Server.

namespace SecureChatTS
{
    public partial class Form1 : Form
    {
        private const int PORT = 20000;                                                     // Define o número da porta fixa (20000) onde o servidor estará à espera de algo;
                                                                                            // Se mudares para 25000: O cliente tentará ligar-se a uma porta errada e receberá uma SocketException (Conexão recusada) a menos que mudes também no servidor.


        NetworkStream networkStream;                                                        // Declara o fluxo que será usado para enviar e receber bytes
                                                                                            // Se removeres: O programa não consegue extrair o canal de dados da ligação TCP.


        ProtocolSI protocolSI;                                                              // Serve para empacotar e interpretar as mensagens seguindo o protocolo definido
                                                                                            // Se removeres: O cliente deixa de ter o buffer global e as funções para decifrar os pacotes do servidor.


        TcpClient client;                                                                   // objeto que vai iniciar e gerir a ligação
                                                                                            // Se removeres: Não consegues disparar o método .Connect() para iniciar o aperto de mão com o servidor.


        private const int SALTSIZE = 8;                                                     // Define o tamanho em bytes do Salt (vetor de inicialização aleatório para a password).
                                                                                            // Se mudares para 4: O Salt fica muito curto e vulnerável a ataques de dicionário.


        private const int NUMBER_OF_ITERATIONS = 1000;                                      // Número de vezes que o algoritmo PBKDF2 vai aplicar a função de hash na password.
                                                                                            // Se mudares para 1: O hash gera-se instantaneamente, mas fica extremamente fácil de quebrar por força bruta.


        private bool autenticado = false;                                                   // Variável de estado para controlar se o utilizador já validou as suas credenciais na base de dados.
                                                                                            // Se mudares para "true" por padrão: O utilizador consegue enviar mensagens para o servidor sem fazer login, embora não tenha a chave AES configurada adequadamente.


        private string usernameAtual = "";                                                  // Guarda em memória RAM o nome do utilizador que efetuou o login com sucesso.
                                                                                            // Se removeres: O programa não sabe quem anexar no cabeçalho "Username:Mensagem" antes de enviar dados.


        
        
        private string connectionString =                                                   // String de ligação que dita onde está guardada a base de dados SQL Server LocalDB.
            @"Data Source=(LocalDB)\MSSQLLocalDB;
            Initial Catalog=SecureChatDB;
            Integrated Security=True";                                                      // Se trocar uma letra aqui: O programa rebenta com uma SqlException assim que abres a aplicação devido à falha de conexão.

                                                                                            // Declarações dos provedores de criptografia assimétrica (RSA) e simétrica (AES).
        private RSACryptoServiceProvider rsa;                                               // Se removeres o "rsa": Não consegues assinar mensagens nem decifrar a chave do servidor.                                             
        private Aes aes;                                                                    // Se removeres o "aes": Não consegues decifrar o feed de mensagens em tempo real vindo do chat global.
        private string publicKey;                                                           // Guarda a chave pública em formato XML para enviar ao servidor durante o aperto de mão.
                                                                                            // Se removeres: O servidor nunca recebe a tua chave pública e não consegue cifrar a chave AES do chat para ti, deixando-te de fora da conversa.
        private string privateKey;                                                          // Guarda a chave privada em formato XML para uso local na decifração da chave AES e na assinatura de mensagens.
                                                                                            // Se removeres: Não consegues decifrar a chave AES enviada pelo servidor nem assinar digitalmente as tuas mensagens, o que faz com que o servidor rejeite os teus envios.

        
        public Form1()                                                                      // Construtor principal do formulário (executa ao arrancar a aplicação).
        {
            InitializeComponent();                                                          // Configura os botões e caixas de texto desenhados no ecrã.

            
            CriarBaseDados();                                                               // Garante que a infraestrutura local de dados está pronta antes do utilizador interagir.
            CriarTabelaUsers();                                                     

            
            
            
            IPEndPoint endPoint = new IPEndPoint(                                           // CRIAR A COMUNICAÇÃO COM O SERVIDOR
                                          IPAddress.Loopback,                               // Define o destino: o próprio PC (127.0.0.1) na porta 20000.
                                          PORT                                              // Se mudares para um IP remoto (ex: "192.168.1.50"): O cliente tentará conectar-se a esse PC na rede local.
                                      );

            client = new TcpClient();                                                       // Cria o objeto que vai realizar a ligação


            client.Connect(endPoint);                                                       // Estabelece a ligação real com o servidor.
                                                                                            // Se o servidor estiver desligado neste momento: O programa falha imediatamente e fecha com uma SocketException.


            networkStream = client.GetStream();                                             // Abre o "tubo" de comunicação para enviar e receber bytes.


            protocolSI = new ProtocolSI();                                                  // Inicializa o formatador de mensagens obrigatório do projeto.


            rsa = new RSACryptoServiceProvider(2048);                                       // Gera um par de chaves RSA novo (Pública e Privada) de 2048 bits para este cliente específico.
                                                                                            // Se mudares para 512: O RSA fica fraco e inseguro para os padrões atuais.


            publicKey = rsa.ToXmlString(false);                                             // Exporta as chaves geradas em formato XML. 'false' exporta apenas a pública, 'true' inclui a chave privada.
            privateKey = rsa.ToXmlString(true);

            ProtocolSI protocoloRSA = new ProtocolSI();                                     // Cria um formatador local isolado apenas para tratar do envio da chave pública.


            
            byte[] packetRSA =                                                              // Monta o pacote de dados do tipo USER_OPTION_1 que carrega a chave pública textual.
                protocoloRSA.Make(                                                          // Se usasses o mesmo protocoloSI global para isto: O buffer seria corrompido pela Thread de escuta se recebesses uma mensagem do servidor enquanto estavas a montar este pacote.
                    ProtocolSICmdType.USER_OPTION_1,                                        // O tipo de comando específico para o envio da chave pública.
                    publicKey                                                               // A string da chave pública em formato XML é a "data" do pacote. O servidor vai ler esta string para obter a tua chave pública e cifrar a chave AES do chat para ti.
                );

            networkStream.Write(packetRSA, 0, packetRSA.Length);                            // Envia a chave pública para o servidor imediatamente após conectar.
                                                                                            // Se removeres esta linha: O servidor nunca saberá a tua chave pública e não conseguirá enviar-te a chave AES do chat de forma segura.


            Thread t = new Thread(LerMensagens);                                            // Cria um "segundo processo" que corre o método 'LerMensagens'
                                                                                            // Se removeres a criação desta Thread: O programa nunca vai escutar as mensagens recebidas de outros utilizadores.


            t.IsBackground = true;                                                          // Se fechares a janela, a thread morre também.
                                                                                            // Se mudares para "false": Ao fechares a janela do programa, o processo continua oculto em background no Gestor de Tarefas do Windows a consumir memória.

            t.Start();                                                                      // Inicializa a thread de escuta.
        }

        
        private void CriarBaseDados()                                                       // Método responsável por garantir a existência da Base de Dados no SQL Server Express.
        {
            
            string masterConnection =                                                       // Liga-se à tabela de sistema 'master' para poder criar novas bases de dados.
                @"Data Source=(LocalDB)\MSSQLLocalDB;                                       
                Initial Catalog=master;
                Integrated Security=True";                                                  // Se alterar uma letra aqui: O programa falha com uma SqlException assim que tenta criar a base de dados porque não consegue conectar ao SQL Server.

            
            using (SqlConnection conn = new SqlConnection(masterConnection))                // O bloco 'using' garante que a ligação à base de dados é fechada e destruída mesmo que ocorra um erro.
            {
                conn.Open();                                                                // Abre a ligação com o SQL Server para executar comandos administrativos (como criar uma base de dados).

                
                string sql =                                                                // Comando SQL que verifica se a base de dados existe, se não existir, cria-a.
                @"IF DB_ID('SecureChatDB') IS NULL
                CREATE DATABASE SecureChatDB";

                SqlCommand cmd = new SqlCommand(sql, conn);                                 // Prepara o comando SQL para ser executado no servidor.
                cmd.ExecuteNonQuery();                                                      // Executa o comando no servidor SQL.
            }
        }

        
        private void CriarTabelaUsers()                                                     // Método que cria a estrutura da tabela onde os dados dos utilizadores ficam registados.
        {
            using (SqlConnection conn = new SqlConnection(connectionString))                // Liga-se à base de dados específica do projeto para criar a tabela 'Users' se ela ainda não existir.
            {
                conn.Open();                                                                // Abre a ligação com a base de dados para executar comandos de manipulação de dados (DML).

                
                
                string sql =                                                                // Cria a tabela 'Users' com campos binários adequados para armazenar hashes e salts com segurança.
                @"IF NOT EXISTS
                (
                    SELECT *
                    FROM sys.tables
                    WHERE name = 'Users'
                )
                CREATE TABLE Users
                (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Username NVARCHAR(50) NOT NULL,
                    SaltedPasswordHash VARBINARY(MAX) NOT NULL,
                    Salt VARBINARY(MAX) NOT NULL
                )";                                                                         // Se mudar o tipo de SaltedPasswordHash para NVARCHAR: Os bytes brutos do hash seriam corrompidos ao tentar converter em string comum.

                SqlCommand cmd = new SqlCommand(sql, conn);                                 // Prepara o comando SQL para criar a tabela.
                cmd.ExecuteNonQuery();                                                      // Executa o comando no servidor SQL.
            }   
        }

        
        
        private static byte[] GenerateSalt(int size)                                        // Função criptográfica que gera bytes completamente imprevisíveis utilizando criptografia do Sistema Operativo.
        {                                                                                   // Se removeres: Não consegues gerar entropia de segurança para aplicar técnicas contra tabelas Rainbow (Rainbow Tables).
            
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();                  // Gera o numero encriptado aleatorio.
            byte[] buff = new byte[size];                                                   // Cria um array de bytes vazio do tamanho definido para o Salt.
            rng.GetBytes(buff);                                                             // Alimenta o array vazio com os bytes aleatórios gerados.
            return buff;                                                                    // Retorna o array de bytes preenchido com o Salt aleatório.
        }

        
        
        private static byte[] GenerateSaltedHash(string plainText, byte[] salt)             // Aplica o algoritmo de derivação PBKDF2 (SHA-1 com iterações por padrão nesta versão da classe).
        {                                                                                   // Se removeres: Guardarias as passwords em texto limpo na base de dados, expondo os dados em caso de vazamento.
            Rfc2898DeriveBytes rfc2898 =                                                    // Cria uma instância do PBKDF2 usando a password em texto limpo, o Salt gerado e o número de iterações definido.
                new Rfc2898DeriveBytes(                                                     // O PBKDF2 é um algoritmo de fortalecimento de senhas que aplica uma função de hash repetidamente para tornar a quebra por força bruta mais difícil.
                    plainText,                                                              // A password original em texto limpo que o utilizador digitou.
                    salt,                                                                   // O Salt aleatório exclusivo para esta password, que impede ataques de dicionário e Rainbow Tables.
                    NUMBER_OF_ITERATIONS                                                    // O número de vezes que a função de hash é aplicada, aumentando a dificuldade de quebra por força bruta.
                );
            return rfc2898.GetBytes(32);                                                    // Extrai uma chave derivada forte de 256 bits (32 bytes).
                                                                                            // Se mudares para 16: O hash gerado tem apenas 128 bits, o que é menos seguro contra ataques de colisão e força bruta. O recomendado atualmente é usar pelo menos 256 bits (32 bytes).
        }                                                                                   // Se mudares para 64: O hash gerado tem 512 bits, o que é mais seguro, mas também consome mais espaço de armazenamento e tempo de processamento. 256 bits (32 bytes) é um bom equilíbrio para senhas.
                                                                                            // Se mudar para um numero aleatorio: O hash gerado teria um tamanho imprevisível, o que pode causar problemas de armazenamento e comparação na base de dados.

        private string EncryptString(string texto)                                          // Cifra texto legível usando a chave simétrica AES recebida pelo servidor.
        {
            
            ICryptoTransform encryptor = aes.CreateEncryptor();                             // Cria o motor de cifragem com base na configuração atual do objeto aes.
                                                                                            // Se o objeto 'aes' for nulo (antes de receber do servidor): Esta linha lança uma NullReferenceException.

            byte[] dados = Encoding.UTF8.GetBytes(texto);                                   // Converte o texto legível em bytes usando UTF-8 para que possa ser processado pelo motor de cifragem.
            byte[] cifrado =                                                                // Aplica a cifra AES sobre os bytes do texto, gerando um array de bytes cifrado.
                encryptor.TransformFinalBlock(                                              // O método TransformFinalBlock é usado para cifrar um bloco de dados completo. Ele processa os bytes de entrada e retorna os bytes cifrados.
                    dados,                                                                  // O array de bytes do texto original que queremos cifrar.
                    0,                                                                      // O índice de início no array de bytes (0 para começar do início).
                    dados.Length                                                            // O número de bytes a processar (o comprimento total do array de bytes do texto).
                );  

            return Convert.ToBase64String(cifrado);                                         // Retorna em Base64 para evitar caracteres de rede ilegais.
        }

        
        private string DecryptString(string texto)                                          // Decifra texto criptografado vindo da rede usando a chave simétrica AES do chat.
        {
            ICryptoTransform decryptor = aes.CreateDecryptor();                             // Cria o motor de decifração com base na configuração atual do objeto aes.
                                                                                            // Se o objeto 'aes' for nulo (antes de receber do servidor): Esta linha lança uma NullReferenceException.

            byte[] dados = Convert.FromBase64String(texto);                                 // Converte o texto cifrado em Base64 de volta para bytes para que possa ser processado pelo motor de decifração.
            byte[] decifrado =                                                              // Aplica a decifra AES sobre os bytes cifrados, gerando um array de bytes legível.
                decryptor.TransformFinalBlock(                                              // O método TransformFinalBlock é usado para decifrar um bloco de dados completo. Ele processa os bytes de entrada e retorna os bytes decifrados.
                    dados, 
                    0, 
                    dados.Length
                );

            return Encoding.UTF8.GetString(decifrado);                                      // Converte os bytes decifrados de volta para uma string legível usando UTF-8.
        }

        
        private void LerMensagens()                                                         // Loop infinito executado numa Thread paralela encarregue de escutar o servidor.
        {
            while (true)                                                                    // Fica bloqueado a ler os dados de rede entrantes e armazena-os no buffer do protocolo.
            {                                                                               // Se for removido: O programa não escuta as mensagens do servidor e a janela do chat fica muda mesmo que outros utilizadores estejam a enviar mensagens para o chat global.
                try                                                                         // O bloco try-catch é necessário para capturar erros de rede, como a queda abrupta do servidor ou perda de conexão, sem que o programa trave completamente.
                {
                    
                    
                    networkStream.Read(                                                     // Fica bloqueado a ler os dados de rede entrantes e armazena-os no buffer do protocolo.
                        protocolSI.Buffer,                                                  // Se a conexão com o servidor cair abruptamente: Esta linha lança uma exceção e cai no bloco catch abaixo.
                        0, 
                        protocolSI.Buffer.Length
                    );

                    
                    if (protocolSI.GetCmdType() == ProtocolSICmdType.USER_OPTION_2)         // SE FOR O PACOTE COM A CHAVE AES (Passo 2 do aperto de mão):
                    {
                        string chaveRecebida = protocolSI.GetStringFromData();              // Extrai a string da chave AES cifrada que o servidor enviou usando a tua chave pública RSA.

                        
                        byte[] chaveCifrada = Convert.FromBase64String(chaveRecebida);      // Converte o texto recebido de volta para bytes criptografados.

                        
                        byte[] dadosAES = rsa.Decrypt(chaveCifrada, false);                 // Decifra os dados usando a tua chave PRIVADA RSA (só tu consegues abrir, porque o servidor cifrou com a pública que lhe deste).

                        string textoAES = Encoding.UTF8.GetString(dadosAES);                // Converte os bytes decifrados de volta para uma string legível. O formato esperado é "Chave_Base64|IV_Base64".

                        
                        string[] partesAES = textoAES.Split('|');                           // Divide a string que contém "Chave_Base64|IV_Base64"

                        
                        aes = Aes.Create();                                                 // Instancia o algoritmo e carrega as chaves secretas enviadas pelo servidor.
                        aes.Key = Convert.FromBase64String(partesAES[0]);                   // Configura a chave AES do chat convertendo a parte da chave de Base64 para bytes.
                                                                                            // Se mudares para partesAES[1]: O programa tenta usar o IV como chave e a decifração falha, resultando em mensagens ilegíveis ou erros de preenchimento (PaddingException).
                        aes.IV  = Convert.FromBase64String(partesAES[1]);                   // Configura o vetor de inicialização (IV) do AES convertendo a parte do IV de Base64 para bytes.
                                                                                            // Se mudares para partesAES[0]: O programa tenta usar a chave como IV e a decifração falha, resultando em mensagens ilegíveis ou erros de preenchimento (PaddingException).

                        continue;                                                           // Ignora o resto do código do while e volta ao topo à espera de mensagens de texto reais.
                    }

                    
                    if (protocolSI.GetCmdType() == ProtocolSICmdType.DATA)                  // SE FOR UM PACOTE DE TEXTO (DATA):
                    {
                        string recebido = protocolSI.GetStringFromData();                   // Extrai a string cifrada da mensagem recebida do servidor.

                        string msgRecebida = DecryptString(recebido);                       // Decifra a payload do pacote utilizando o motor AES configurado.


                        string hora = DateTime.Now.ToString("HH:mm:ss");                    // Obtém a hora atual no formato HH:mm:ss


                        
                        
                        richTextBox1.Invoke(new MethodInvoker(delegate                      // Como esta thread é "esquema à parte", temos de usar Invoke para escrever na RichTextBox
                        {                                                                   // Se tentares mudar o texto da richTextBox1 diretamente sem Invoke: O Windows Forms dispara um erro fatal de Cross-thread (operação inválida entre threads).
                            
                            richTextBox1.SelectionColor = Color.Gray;                       // Define a cor cinza para a hora e escreve
                            richTextBox1.AppendText("[" + hora + "] ");

                            
                            if (msgRecebida.Contains(":"))                                  // Se a mensagem contiver "Cliente X:", vamos destacar o nome
                            {
                                string[] partes = msgRecebida.Split(new[] { ':' }, 2);      // Divide a mensagem em duas partes: o nome do remetente (antes dos dois pontos) e o conteúdo da mensagem (depois dos dois pontos). O '2' limita a divisão para evitar quebrar mensagens que contenham ":" no texto.
                                                                                            // Se mudar para 6 ou outro número maior: Mensagens com mais de 6 ":" seriam divididas em mais partes do que o esperado, o que pode causar erros ao tentar acessar partes[1] para o conteúdo da mensagem.

                                
                                richTextBox1.SelectionFont =                                // Define cor preta e negrito para o nome do remetente
                                    new Font(
                                        richTextBox1.Font, 
                                        FontStyle.Bold
                                    );

                                richTextBox1.SelectionColor = Color.Black;                  // Define cor preta para o nome do remetente (corrigido o bug visual do código que estava a meter branco no fundo branco) e escreve o nome seguido de ": "
                                richTextBox1.AppendText(partes[0].Trim() + ": ");           // Trim() remove espaços em branco extras no nome do remetente, caso existam.

                                
                                richTextBox1.SelectionFont =                                // Define cor preta (corrigido o bug visual do código que estava a meter branco no fundo branco) e fonte normal para a mensagem.
                                    new Font(
                                        richTextBox1.Font, 
                                        FontStyle.Regular
                                    );
                                richTextBox1.SelectionColor = Color.Black;                  // Define cor preta para o conteúdo da mensagem e escreve a parte da mensagem depois dos ": "
                                richTextBox1.AppendText(partes[1] + Environment.NewLine);   // Environment.NewLine é a forma correta de adicionar uma nova linha independente do sistema operativo (Windows, Linux, etc).
                                                                                            // Se mudar para partes[8] ou outro número maior: O programa lança um erro de IndexOutOfRangeException para mensagens que não tenham tantas partes, o que pode ocorrer se a mensagem original contiver ":" no texto, quebrando a lógica de divisão.
                            }
                            else                                                            // Se a mensagem não tiver o formato "Cliente X: Mensagem", escreve a mensagem inteira em preto sem tentar destacar o remetente.
                            {
                                
                                richTextBox1.SelectionColor = Color.Black;                  // Caso a mensagem não tenha o formato padrão, escreve normal
                                richTextBox1.AppendText(msgRecebida + Environment.NewLine);
                            }

                            
                            richTextBox1.ScrollToCaret();                                   // Faz scroll automático para o fim
                        }));
                    }
                }
                catch (Exception ex)                                                        // Captura qualquer exceção que ocorra durante a leitura de mensagens (como perda de conexão) e exibe uma caixa de erro.
                {
                    
                    MessageBox.Show(ex.Message);                                            // Mostra uma caixa de erro no ecrã se algo falhar e força a quebra do loop da Thread.
                    break;
                }
            }
        }

        
        
        private string AssinarMensagem(string mensagem)                                     // Gera uma assinatura digital RSA SHA256 baseada na mensagem para provar que foste tu quem a escreveu.
        {                                                                                   // Se removeres: O servidor rejeita as tuas mensagens, pois ele exige validação de assinatura no bloco DATA.
            byte[] dados = Encoding.UTF8.GetBytes(mensagem);                                // Converte a mensagem legível em bytes para que possa ser processada pelo algoritmo de assinatura.

            byte[] assinatura = rsa.SignData(dados, CryptoConfig.MapNameToOID("SHA256"));   // Cifra o hash da mensagem usando a tua chave privada. Qualquer um com a pública pode validar que saiu do teu PC.


            return Convert.ToBase64String(assinatura);                                      // Retorna a assinatura em Base64 para evitar caracteres de rede ilegais.
        }

        
        private bool UserExists(string username)                                            // Executa uma consulta escalar rápida para verificar duplicados de utilizadores no registo.
        {
            using (SqlConnection conn = new SqlConnection(connectionString))                // Liga-se à base de dados específica do projeto para verificar se o username já existe.
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM Users WHERE Username = @username";       // Comando SQL que conta quantos registos têm o mesmo username. O uso de COUNT(*) é eficiente para esta verificação.
                SqlCommand cmd = new SqlCommand(sql, conn);                                 // Prepara o comando SQL para ser executado no servidor.

                
                cmd.Parameters.AddWithValue("@username", username);                         // O uso de Parameters evita falhas de segurança por injeção de SQL (SQL Injection).
                                                                                            // Se concatenasses direto: um username com o valor "admin'--" alteraria a lógica do comando SQL.

                int count = (int)cmd.ExecuteScalar();                                       // Executa a consulta e retorna o número de registos encontrados com o mesmo username.
                return count > 0;                                                           // Retorna true se encontrar algum registo.
            }                                                                               // Se alterares o count > 0 para count > 1: O programa só consideraria que o username existe se houver mais de um registo com o mesmo nome, o que permitiria duplicações indesejadas.
        }

        private void Form1_Load(object sender, EventArgs e) { }                             // Clicado sem querer

        
        private bool VerifyLogin(string username, string password)                          // Executa a autenticação comparando hashes criptográficos guardados.
        {
            SqlConnection conn = null;                                                      // Declarada fora do bloco try para poder fechar a ligação no bloco finally, garantindo que não fica pendurada em caso de erro.
            try                         
            {
                conn = new SqlConnection(connectionString);                                 // Liga-se à base de dados para verificar as credenciais do login.
                conn.Open();                                                                // Se a conexão falhar: O programa lança uma SqlException que é capturada no bloco catch abaixo.

                String sql = "SELECT * FROM Users WHERE Username = @username";              // Comando SQL que seleciona o registo do utilizador com o username fornecido. O uso de parâmetros evita falhas de segurança por injeção de SQL (SQL Injection).
                SqlCommand cmd = new SqlCommand();                                          // Prepara o comando SQL para ser executado no servidor.
                cmd.CommandText = sql;                                                      // Se concatenasses direto: um username com o valor "admin'--" alteraria a lógica do comando SQL, potencialmente expondo dados de outros utilizadores ou permitindo logins não autorizados.

                SqlParameter param = new SqlParameter("@username", username);               // Cria um parâmetro SQL para o username, garantindo que é tratado como um valor literal e não como parte do comando SQL, o que protege contra injeção de SQL.
                cmd.Parameters.Add(param);                                                  // Adiciona o parâmetro ao comando SQL.
                cmd.Connection = conn;                                                      // Associa o comando SQL à conexão aberta para que possa ser executado no servidor.

                SqlDataReader reader = cmd.ExecuteReader();                                 // Executa a leitura dos registos na tabela.


                
                if (!reader.HasRows)                                                        // Se não encontrar nenhuma linha correspondente ao utilizador:
                {
                    return false;
                }

                reader.Read();                                                              // Avança o apontador para a primeira linha encontrada.

                
                byte[] saltedPasswordHashStored = (byte[])reader["SaltedPasswordHash"];     // Faz a extração dos arrays de bytes puros armazenados na base de dados.
                byte[] saltStored = (byte[])reader["Salt"];                                 // Se tentasses ler estes campos como strings: Os dados seriam corrompidos pela conversão de bytes para texto, resultando em falha na comparação de hashes.

                conn.Close();                                                               // Fecha imediatamente o reader e a ligação.

                
                byte[] hash = GenerateSaltedHash(password, saltStored);                     // Recalcula o hash da password que o utilizador digitou agora usando o mesmo SALT que foi guardado quando ele se registou.

                
                return saltedPasswordHashStored.SequenceEqual(hash);                        // Compara bit a bit se o hash gerado agora coincide com o guardado em disco.
            }
            catch (Exception ex)                                                            // Captura qualquer exceção que ocorra durante o processo de verificação (como falhas de conexão ou erros de consulta) e exibe uma caixa de erro.
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        
        private void Register(string username, byte[] saltedPasswordHash, byte[] salt)      // Método responsável por inserir de forma permanente o novo utilizador na BD.
        {
            using (SqlConnection conn = new SqlConnection(connectionString))                // Liga-se à base de dados para inserir o novo registo do utilizador com o username, hash e salt gerados.
            {
                conn.Open();                                                                // Se a conexão falhar: O programa lança uma SqlException que é capturada no bloco catch do método VerifyLogin, pois o registo não foi criado e o utilizador tentará fazer login em seguida.

                string sql =                                                                // Comando SQL que insere um novo registo na tabela 'Users' com os dados do username, hash e salt. O uso de parâmetros evita falhas de segurança por injeção de SQL (SQL Injection).
                    "INSERT INTO Users (Username, SaltedPasswordHash, Salt) " +
                    "VALUES (@username,@hash,@salt)";

                SqlCommand cmd = new SqlCommand(sql, conn);                                 // Prepara o comando SQL para ser executado no servidor.

                cmd.Parameters.AddWithValue("@username", username);                         // O uso de Parameters evita falhas de segurança por injeção de SQL (SQL Injection).
                                                                                            // Se concatenasses direto: um username com o valor "admin'--" alteraria a lógica do comando SQL, potencialmente expondo dados de outros utilizadores ou permitindo logins não autorizados.
                cmd.Parameters.AddWithValue("@hash", saltedPasswordHash);                   // Adiciona o hash gerado como um parâmetro do comando SQL para ser inserido na base de dados.
                                                                                            // Se concatenasses direto: Os bytes do hash seriam corrompidos pela conversão de bytes para texto, resultando em falha na comparação de hashes durante o login.
                cmd.Parameters.AddWithValue("@salt", salt);                                 // Adiciona o salt gerado como um parâmetro do comando SQL para ser inserido na base de dados.
                                                                                            // Se concatenasses direto: Os bytes do salt seriam corrompidos pela conversão de bytes para texto, resultando em falha na comparação de hashes durante o login.

                cmd.ExecuteNonQuery();                                                      // Salva permanentemente as informações nas linhas da tabela.
            }
        }

        
        private void bt_Enviar_Click(object sender, EventArgs e)                            // Evento disparado ao carregar no botão "Enviar Mensagem".
        {
            
            if (!autenticado)                                                               // Bloqueia envios anónimos.
            {
                MessageBox.Show("Tem de fazer login primeiro.");
                return;
            }

            
            string msg = textBoxMensagem.Text;                                              // Guarda o texto da caixa de texto numa variável e limpa a caixa a seguir

            if (string.IsNullOrWhiteSpace(msg)) return;                                     // Se o utilizador só carregar em Espaços ou enviar vazio, sai do método sem gastar dados.


            textBoxMensagem.Clear();                                                        // Limpa o controlo visual para a próxima mensagem.

            ProtocolSI protocoloEnvio = new ProtocolSI();                                   // UM PROTOCOLO NOVO AQUI SÓ PARA ENVIAR (Para não chocar com a Thread)
                                                                                            // Se usasses o mesmo objeto global protocolSI da Thread de escuta: Ocorria corrupção de dados na memória se estivesses a ler e a escrever ao mesmo tempo.


            string mensagemCompleta = usernameAtual + ":" + msg;                            // Formata a string combinando a identidade do utilizador e a mensagem.


            string assinatura = AssinarMensagem(mensagemCompleta);                          // Assina digitalmente a string gerada.


            string pacoteFinal = mensagemCompleta + "|" + assinatura;                       // Monta o payload final: "Utilizador:Mensagem|Assinatura_Base64"


            string mensagemCifrada = EncryptString(pacoteFinal);                            // Aplica a cifra AES sobre todo este bloco de dados.


            
            byte[] packet = protocoloEnvio.Make(                                            // Transforma num pacote de transmissão DATA suportado pelo protocolo.
                ProtocolSICmdType.DATA,
                mensagemCifrada
            );

            networkStream.Write(packet, 0, packet.Length);                                  // Empurra os bytes pelo "tubo" (stream) em direção ao servidor

        }

        
        private void bt_Sair_Click(object sender, EventArgs e)                              // Evento disparado ao carregar no botão "Sair".
        {
            
            ProtocolSI protocoloEnvio = new ProtocolSI();                                   // UM PROTOCOLO NOVO AQUI TAMBÉM

            
            byte[] eot = protocoloEnvio.Make(ProtocolSICmdType.EOT);                        // Cria um pacote especial do tipo EOT (Fim de Transmissão) para avisar que vai sair

            networkStream.Write(eot, 0, eot.Length);                                        // Envia esse aviso de saída para o servidor


            networkStream.Close();                                                          // Fecha o "tubo" de comunicação e a infraestrutura do socket físico.
                                                                                            // Se removeres: A porta fica pendurada aberta no sistema operativo até o programa expirar na rede.

            client.Close();                                                                 // Fecha o socket TCP, liberando os recursos associados e encerrando a conexão com o servidor.
                                                                                            // Se removeres: O programa pode ficar preso em um estado de espera indefinidamente, pois o socket não é fechado corretamente, o que pode causar vazamento de recursos e impedir que o programa termine.

            this.Close();                                                                   // Fecha a janela do formulário (o programa termina)

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) { }               // Clicado sem querer

        
        private void PedirHistorico()                                                       // Método que envia o comando USER_OPTION_3 pedindo os registos antigos gravados no chatlog.txt do servidor.
        {
            ProtocolSI p = new ProtocolSI();                                                // UM PROTOCOLO NOVO AQUI TAMBÉM, para não chocar com a Thread de escuta que tem o seu próprio protocolo global.
            byte[] packet = p.Make(ProtocolSICmdType.USER_OPTION_3);                        // Cria um pacote do tipo USER_OPTION_3, que é o comando definido para solicitar o histórico do chat. O servidor reconhece esse comando e responde enviando os registos antigos em pacotes DATA.

            networkStream.Write(packet, 0, packet.Length);                                  // Pede o histórico pela rede. O servidor responderá fatiando o ficheiro e enviando em pacotes DATA.

        }

        
        private void buttonLogin_Click(object sender, EventArgs e)                          // Evento disparado ao carregar no botão "Login".
        {
            string password = textBoxPassword.Text;                                         // Guarda a password digitada numa variável para processar a autenticação.
            string username = textBoxUsername.Text;                                         // Guarda o username digitado numa variável para processar a autenticação.

            if (VerifyLogin(username, password))                                            // Invoca a verificação da base de dados local.
            {               
                autenticado = true;                                                         // Se a verificação for bem-sucedida, define o estado de autenticado como true para permitir o acesso ao chat e outras funcionalidades restritas.
                usernameAtual = username;                                                   // Armazena o username atual para usar na identificação das mensagens enviadas.

                                                                                            // Desativa os controlos para que o utilizador não faça logins repetidos com a sessão aberta.
                buttonLogin.Enabled = false;                                                // Se não desativasses o botão de login: O utilizador poderia clicar várias vezes, o que não faria sentido e poderia causar confusão ou erros.
                buttonRegistar.Enabled = false;                                             // Se não desativasses o botão de registo: O utilizador poderia clicar para registar um novo utilizador mesmo depois de estar autenticado, o que não faria sentido e poderia causar confusão ou erros.
                textBoxUsername.Enabled = false;                                            // Se não desativasses a caixa de texto do username: O utilizador poderia alterar o username depois de autenticado, o que não faria sentido e poderia causar confusão ou erros.
                textBoxPassword.Enabled = false;                                            // Se não desativasses a caixa de texto da password: O utilizador poderia alterar a password depois de autenticado, o que não faria sentido e poderia causar confusão ou erros.

                MessageBox.Show("Utilizador logado com sucesso");

                PedirHistorico();                                                           // Dispara o download automático do histórico do chat assim que o login é aceite.

            }
            else                                                                            // Se a verificação falhar, exibe uma mensagem de erro informando que as credenciais são inválidas.
            {
                MessageBox.Show("Credenciais inválidas");
            }
        }

        
        private void buttonRegistar_Click(object sender, EventArgs e)                       // Evento disparado ao carregar no botão "Registar".
        {
            string username = textBoxUsername.Text;                                         // Guarda o username digitado numa variável para processar o registo.
            string pass = textBoxPassword.Text;                                             // Guarda a password digitada numa variável para processar o registo.

            
            if (string.IsNullOrWhiteSpace(username))                                        // Validações básicas de segurança para evitar campos em branco.
            {
                MessageBox.Show("Introduza um username.");
                return;
            }

            if (string.IsNullOrWhiteSpace(pass))                                            // Validações básicas de segurança para evitar campos em branco.
            {
                MessageBox.Show("Introduza uma password.");
                return;
            }

            
            if (UserExists(username))                                                       // Verifica se o username já está registado na tabela local para impedir duplicações.
            {
                MessageBox.Show("Esse username já existe.");
                return;
            }

            
            byte[] salt = GenerateSalt(SALTSIZE);                                           // Gera o Salt aleatório exclusivo para esta password e calcula o Hash final.
                                                                                            // Se não gerasses um salt único para cada password: O hash de senhas idênticas seria o mesmo, o que facilitaria ataques de rainbow tables e permitiria que um atacante identificasse usuários com senhas iguais.
            byte[] hash = GenerateSaltedHash(pass, salt);                                   // Gera o hash da password combinada com o salt usando um algoritmo de hash seguro (SHA256). O resultado é um array de bytes que representa o hash final a ser armazenado na base de dados.
                                                                                            // Se não usasses um algoritmo de hash seguro: O hash poderia ser facilmente quebrado por ataques de força bruta, expondo as senhas dos usuários a riscos de segurança.

            Register(username, hash, salt);                                                 // Grava os resultados de forma persistente na base de dados.


            MessageBox.Show("Utilizador registado com sucesso");
       
        }
    }
}