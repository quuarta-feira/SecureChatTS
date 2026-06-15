using System;                       // Importa as bibliotecas necessárias para o funcionamento do servidor
using System.Collections.Generic;   // Listas, para guardar os clientes ligados
using System.Net;                   // Para trabalhar com IPs e portas
using System.Net.Sockets;           // Para criar o servidor que aceita ligações
using System.Text;                  // Para converter texto em bytes e vice-versa
using System.IO;                    // Para trabalhar com ficheiros
using System.Threading;             // Para trabalhar com threads (cada cliente corre numa thread à parte, uma thread é como um mini-programa dentro do programa principal)
using System.Security.Cryptography; // Para trabalhar com criptografia
using EI.SI;                        // Biblioteca personalizada (presumivelmente para funções específicas)

namespace Servidor                  // Esta é a pasta onde fica guardada o código
{
    internal class Program          // Esta é a classe principal do programa, onde tudo começa
    {
        public static List<ClientHandler> clientesLigados = new List<ClientHandler>();      // Guarda todos os clientes para podermos enviar mensagens a todos os clientes ligados.
                                                                                            //Se removeres esta lista: O servidor deixa de saber quem está ligado.

        public static int totalMensagens = 0;                                               // Guarda o número total de mensagens processadas pelo servidor. Começa em 0 porque ainda ninguém enviou mensagens.
                                                                                            // Se começares em 100:  A primeira mensagem mostrará 101 mensagens processadas.
                                                                                            // Se removeres esta variável: O contador de mensagens deixa de existir.
                                                                                            // Se colocares um valor negativo: O contador começa em números negativos.
        
        public static Aes aesGlobal = Aes.Create();                                         // Cria uma instância AES. AES é o algoritmo utilizado para cifrar e decifrar mensagens.
                                                                                            // Quando esta linha executa: É criada uma chave AES aleatória.É criado um IV (Initialization Vector) aleatório. A chave é usada para transformar texto normal em cifrado
                                                                                            // em texto cifrado.
                                                                                            // Se removeres esta linha: O servidor deixa de conseguir cifrar mensagens. Os métodos EncryptString e DecryptString vão gerar erro.
                                                                                            //Se criares um novo AES para cada cliente: Cada cliente terá uma chave diferente. Neste programa todos os clientes usam a mesma chave AES.

        private const int PORT = 20000;                                                     // Define a porta TCP utilizada pelo servidor.  Pensa numa porta como o número da porta de uma casa.  O IP é a morada. A porta é a porta da casa. Os clientes têm de saber esta porta para conseguirem entrar no servidor."const" significa que este valor nunca pode mudar enquanto o programa está a correr.
                                                                                            // Se mudares para 30000: O servidor passa a ouvir na porta 30000.s clientes também terão de usar 30000. 
                                                                                            // Se mudares para 80: Pode falhar por falta de permissões.
                                                                                            // Se dois programas usarem a mesma porta: Um deles não consegue arrancar.
                                                                                            // Se removeres esta constante: Terás de escrever o número da porta manualmente em todos os locais onde ela for usada.

                                                                                            // conta os clientes que entram
        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, PORT);                      // Define que o servidor aceita ligações em qualquer IP do PC na porta 20000.
                                                                                            // Se mudares "IPAddress.Any" para "IPAddress.Loopback": O servidor só aceita ligações vindas do próprio computador (localhost). Clientes externos não entram.
                                                                                            // Se removeres esta linha: Não tens uma configuração de rede para passar ao Listener, gerando erro de compilação.


            TcpListener listener = new TcpListener(endPoint);                               // Cria o "ouvinte" que monitoriza os pedidos de entrada na rede.
                                                                                            // Se removeres esta linha: O objeto responsável por gerir o socket de escuta TCP não existirá.
                                                                                            // Se passares um endPoint nulo: O programa rebenta com uma NullReferenceException ao tentar iniciar.


            listener.Start();                                                               // Inicia o serviço e começa a aceitar comunicações.
                                                                                            // Se removeres esta linha: A porta 20000 continua fechada no sistema operacional. Qualquer cliente que tente ligar-se receberá um erro de "Conexão Recusada".

            Console.WriteLine("Servidor Pronto");                                           // Mensagem para indicar que o servidro ligou bem

            int clientCounter = 0;                                                          // Cria uma variável local para contar quantos clientes já se conectaram desde que o servidor ligou.
                                                                                            // Se começares em 5: O primeiro cliente a entrar vai aparecer na consola como "Cliente 6".
                                                                                            // Se removeres esta linha: Perdes o controlo numérico individual dos clientes na consola e no ficheiro de logs.


            while (true)                                                                    // Ciclo infinito para que o servidor nunca pare de aceitar novos clientes.
            {                                                                               // Se mudares para "while(false)": O código lá dentro nunca executa e o programa termina imediatamente após iniciar.
                                                                                            // Se removeres o ciclo: O servidor aceita exatamente um cliente e, logo a seguir, o método Main chega ao fim e o servidor desliga-se sozinho.

                TcpClient client = listener.AcceptTcpClient();                              // O servidor "congela" nesta linha e fica à espera; só avança quando um cliente bater à porta.
                                                                                            // Se removeres esta linha: O ciclo while(true) vai rodar à velocidade máxima do processador sem fazer nada, disparando o uso do CPU para os 100%.
                                                                                            // Se o cliente desligar antes de completar a ligação: O método pode lançar uma SocketException que travará o servidor se não for tratada.


                clientCounter++;                                                            // Incrementa o número do cliente e mostra na consola.
                                                                                            // Se removeres esta linha: Todos os clientes que entrarem vão ser identificados como "Cliente 0".
                                                                                            // Se colocares "clientCounter--" em vez de "++": O contador vai subtrair a cada entrada (Cliente -1, Cliente -2, etc).

                Console.WriteLine("Cliente {0} Ligado", clientCounter);

                // ALTERACAO TESTE
                File.AppendAllText(
                    "chatlog.txt",
                    "Conversação entre clientes" + Environment.NewLine
                );
                //

                // Se removeres esta linha: O histórico de acessos deixa de ser guardado no disco.
                File.AppendAllText(                                                         // Grava de forma persistente a entrada do cliente no ficheiro chatlog.txt sem apagar o conteúdo antigo.
                    "chatlog.txt",                                                          // Se o ficheiro chatlog.txt não existir: O C# cria o ficheiro automaticamente neste exato momento.
                    "[" + DateTime.Now.ToString("HH:mm:ss") + "] Cliente "
                    + clientCounter + " ligou-se."                                          // Se o ficheiro estiver aberto e bloqueado por outro programa (como o Excel): Esta linha lança uma IOException e o servidor vai abaixo.
                    + Environment.NewLine
                );

                ClientHandler clientHandler = new ClientHandler(client, clientCounter);     //Tratar o cliente que se ligou Instancia o gestor do cliente, passando o socket de rede individual e o ID gerado.
                                                                                            // Se removeres esta linha: O servidor aceita a ligação mas não cria a lógica para conversar com ele.
                                                                                            // Se passares o ID fixo como "1": Todos os clientes partilharão internamente o mesmo identificador.

                clientesLigados.Add(clientHandler);                                         // ADICIONA À LISTA: Guardamos este cliente na nossa lista global
                                                                                            // Se removeres esta linha: O cliente consegue falar com o servidor, mas quando enviar uma mensagem, o servidor não conseguirá reencaminhá-la para ele nem para os outros utilizadores.

                clientHandler.Handle();                                                     // Dispara o método interno que vai criar a thread dedicada para este cliente.
                                                                                            // Se removeres esta linha: O cliente fica conectado mas o servidor nunca vai ler o que ele escreve na rede.

            }
        }


    }
                                                                                            
    class ClientHandler                                                                     //Deixar os clientes ligados ao servidor
    {
        private TcpClient client;                                                           // Guarda a ligação do cliente específico.
                                                                                            // Se removeres: Ficas sem acesso ao socket físico do cliente, impossibilitando fechar a ligação ou obter o canal de comunicação.
        
        private int clientID;                                                               // Guarda o ID deste cliente.
                                                                                            // Se removeres: Não consegues identificar visualmente nas mensagens da consola quem enviou o quê.

        private NetworkStream networkStream;                                                // É o "tubo" de comunicação por onde entram e saem os bytes deste cliente específico.
                                                                                            // Se removeres: Não tens como usar as funções .Read() e .Write() para enviar ou receber dados de rede.

        private string publicKeyCliente;                                                    // Guarda a chave pública RSA em formato XML enviada por este cliente específico.
                                                                                            // Se removeres: O servidor não conseguirá verificar a assinatura digital das mensagens enviadas por este utilizador.

        private Aes aes;                                                                    // Variável declarada para uso local de criptografia AES (embora o código utilize o Program.aesGlobal).
                                                                                            // Se removeres: Não afeta o comportamento atual do código porque não está a ser lida.

        
        
        public ClientHandler(TcpClient client, int clientID)                                // Construtor: recebe o cliente e o ID vindos do servidor principal.
        {                                                                                   // Se removeres este construtor: A classe deixa de conseguir inicializar os campos obrigatórios (client e ID) no momento em que o cliente liga.
            this.client     = client;
            this.clientID   = clientID;

            this.networkStream = this.client.GetStream();                                   // Extrai o fluxo de rede (Stream) do cliente para permitir operações de leitura e escrita.
                                                                                            // Se a ligação "client" cair antes desta linha: Lança uma InvalidOperationException e desliga o servidor.
        }

        
        
        
        public void Handle()                                                                // Inicia a tarefa em paralelo para não bloquear o servidor
        {
            Thread thread = new Thread(threadHandler);                                      // Se correres o "threadHandler()" diretamente aqui sem criar a Thread: O servidor principal congela no primeiro cliente e mais ninguém consegue ligar-se.
            thread.Start();                                                                 // Se removeres o "thread.Start()": A thread é configurada na memória, mas nunca começa a correr. O cliente nunca será processado.
        }

        
        
        
        public void EnviarMensagem(byte[] pacote)                                           // Permite ao servidor enviar uma mensagem diretamente para este cliente
        {                                                                                   // Se o array "pacote" for nulo: O método lança uma exceção que será capturada pelo catch vazio.
            try                                                                             // Se removeres o bloco try/catch: Caso o cliente tenha desligado o PC repentinamente, o .Write() gera um erro de rede e o servidor inteiro vai abaixo.
            {
                networkStream.Write(pacote, 0, pacote.Length);                              // Escreve os bytes diretamente no tubo de rede do cliente, enviando-os para o ecrã dele.
            }
            catch { }                                                                       // O catch está vazio por design para que se um cliente cair, o servidor ignore o erro e continue a enviar para os outros.
        }

        
        private void threadHandler()                                                        // Este método corre numa thread à parte para não travar o resto do servidor
        {

            ProtocolSI protocolSI = new ProtocolSI();                                       // Cria a ferramenta que traduz os bytes da rede em mensagens (tipo DATA ou EOT)
                                                                                            // Se removeres: Não tens o interpretador de protocolo e o servidor não saberá processar os cabeçalhos das mensagens estruturadas.


            while (protocolSI.GetCmdType() != ProtocolSICmdType.EOT)                        // Repete tudo o que está aqui dentro enquanto o cliente não mandar o sinal de "Sair" (EOT)
            {                                                                               // Se mudares a condição para "while(true)": Mesmo que o cliente envie um sinal de saída, o servidor continuará a tentar ler dados fantasmas infinitamente.
                                                                                            // Se removeres este ciclo: O servidor processa apenas o primeiríssimo comando enviado pelo cliente (por exemplo, a chave pública) e encerra a comunicação logo de seguida.

                
               
                
                int bytesRead = networkStream.Read(                                         // O programa para aqui e fica à espera que o cliente envie bytes; quando chegam, guarda-os no buffer
                                    protocolSI.Buffer,                                      // Se removeres esta linha: O ciclo roda em loop infinito vazio sem esperar por dados, consumindo 100% do CPU e gerando erros ao tentar ler pacotes vazios.
                                    0,                                                      // Se "bytesRead" retornar 0: Significa que o cliente desligou a aplicação abruptamente sem enviar o sinal de EOT adequado.
                                    protocolSI.Buffer.Length
                                );
                byte[] ack;                                                                 // declara uma variável local chamada ack que é capaz de armazenar um array (vetor) de bytes.

                
                
                switch (protocolSI.GetCmdType())                                            // Verifica qual foi o tipo de pacote que a ferramenta acabou de ler do buffer
                {                                                                           // Se removeres este bloco switch: O servidor recebe os bytes da rede mas não toma nenhuma ação com eles, tornando o chat inútil.
                    
                    case ProtocolSICmdType.USER_OPTION_1:                                   // Caso o cliente envie a sua chave pública (Passo inicial do aperto de mão criptográfico).

                        
                        publicKeyCliente = protocolSI.GetStringFromData();                  // Extrai o texto limpo do pacote recebido (que é a chave pública RSA em formato XML).
                        Console.WriteLine("Chave pública recebida.");

                        
                        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();      // Instancia o motor criptográfico RSA para carregar a chave recebida.
                        
                        rsa.FromXmlString(publicKeyCliente);                                // Importa a chave pública do cliente. Se a string não for um XML RSA válido: Esta linha quebra com uma CryptographicException.

                        
                        
                        string dadosAES =                                                   // Se removeres o separador '|': O método Split no lado do cliente não saberá onde termina a chave e onde começa o IV, quebrando a decifragem.
                            Convert.ToBase64String(Program.aesGlobal.Key) + "|" +           // Junta a Chave AES e o IV global numa única string separados por um pipe '|'.
                            Convert.ToBase64String(Program.aesGlobal.IV);

                        
                        byte[] chaveCifrada =                                               // Cifra os dados do AES usando o RSA com a chave pública do cliente (garante que só aquele cliente consegue ler esta chave AES).
                            rsa.Encrypt(Encoding.UTF8.GetBytes(dadosAES), false);

                        
                        string chaveBase64 =                                                // Transforma o resultado cifrado em Base64 para poder ser transmitido como string de forma segura.
                            Convert.ToBase64String(chaveCifrada);

                        
                        byte[] packet =                                                     // Cria um pacote do tipo USER_OPTION_2 contendo a chave AES cifrada.
                            protocolSI.Make(
                                ProtocolSICmdType.USER_OPTION_2,
                                chaveBase64                                                 // Se alterares a chaveBase64 para a string "Teste": O cliente vai tentar decifrar "Teste" como se fosse a chave AES cifrada, falhando miseravelmente com um erro de Padding inválido.
                            );

                        
                        networkStream.Write(packet, 0, packet.Length);                      // Envia o pacote de volta para o cliente através do fluxo de rede.

                        Console.WriteLine("Chave AES enviada.");
                        break;                                                              // Sai do switch e volta para o topo do while esperar mais dados.

                    case ProtocolSICmdType.DATA:                                            // Se o pacote for uma mensagem de texto normal:
                                                                                            // Transforma os bytes em letras e escreve o texto na janela do servidor

                        
                        
                        string textoRecebido = DecryptString(                               // Obtém a string cifrada de dentro do pacote e passa-a à função DecryptString para obter o texto original em formato "Mensagem|Assinatura".
                                                  protocolSI.GetStringFromData()            // Se a mensagem não vier cifrada com o AES correto: O DecryptString falha estrepitosamente com erro de Padding inválido.
                                               );

                        
                        string[] partes = textoRecebido.Split(new char[] { '|' }, 2);       // Divide a string em duas partes usando o caractere '|'. O '2' garante que só divide no primeiro pipe encontrado.
                                                                                            // Se o cliente não incluir o caractere '|' na mensagem: O array "partes" terá apenas 1 elemento e a linha seguinte (partes[1]) vai estourar com IndexOutOfRangeException.

                        string mensagem = partes[0];
                        string assinatura = partes[1];

                        
                        bool assinaturaValida =                                             // Invoca a validação RSA para conferir se a assinatura confere com o texto da mensagem enviado.
                            VerificarAssinatura(                                            // A função VerificarAssinatura retorna true se a assinatura for válida e false se for inválida (se a mensagem tiver sido alterada ou se não tiver sido assinada com a chave privada correspondente à chave pública que o servidor tem).
                                mensagem,                                                   // Se a mensagem tiver sido alterada no caminho: A assinatura não bate e a função retorna false.
                                assinatura                                                  // Se a assinatura não tiver sido gerada com a chave privada correspondente à chave pública do cliente: A função retorna false.
                            );

                        //ALTERACAO TESTE
                        byte[] resultadoVerificacao =
                            protocolSI.Make(
                                ProtocolSICmdType.USER_OPTION_5,
                                assinaturaValida.ToString()
                            );

                        foreach (ClientHandler outroCliente in Program.clientesLigados)
                        {
                            outroCliente.EnviarMensagem(resultadoVerificacao);
                        }

                        //

                        if (!assinaturaValida)                                              // Se alguém alterou a mensagem no caminho ou se a chave não bater:
                        {
                            Console.WriteLine("Assinatura inválida!");
                            break;                                                          // Abandona o processamento desta mensagem inválida imediatamente, protegendo o chat contra falsificações.
                        }

                        //ALTERACAO TESTE

                        byte[] packetHash =
                        protocolSI.Make(
                            ProtocolSICmdType.USER_OPTION_4,
                            assinaturaValida.ToString()
                        );

                        networkStream.Write(
                            packetHash,
                            0,
                            packetHash.Length
                        );

                        //
                        Console.WriteLine("Assinatura válida.");

                        
                        
                        File.AppendAllText(                                                 // Escreve a mensagem validada no histórico chatlog.txt.
                             "chatlog.txt",                                                 // Significa que mesmo que a mensagem seja legítima, se o ficheiro estiver bloqueado por outro programa ou o disco cheio: Esta linha lança uma IOException e o servidor vai abaixo.
                             "[" + DateTime.Now.ToString("HH:mm:ss") + "] "                 // O timestamp é importante para manter a ordem cronológica das mensagens no histórico, especialmente se o servidor ficar offline por um tempo e depois voltar.
                             + mensagem                                                     // Esta variável contém o texto original da mensagem, sem a assinatura, para que o histórico fique limpo e legível. Se colocares "textoRecebido" aqui: O histórico vai ficar poluído com as assinaturas junto das mensagens.
                             + Environment.NewLine                                          // Garante que cada mensagem nova no histórico começa numa linha nova. Se removeres: Todas as mensagens vão se amontoar na mesma linha, tornando o histórico ilegível.
                         );                                                                 // Se o disco estiver cheio: Lança uma IOException e desliga o servidor.

                        Console.WriteLine("Cliente " + clientID + " : " + mensagem);        // Mostra na consola do servidor a mensagem decifrada com a identificação do ID do remetente.


                        
                        Program.totalMensagens++;                                           // Incrementa a contagem de mensagens do programa principal.

                        Console.WriteLine(                                                  // Mostra o número total de mensagens processadas até agora. Se removeres esta linha: O servidor não mostra mais o contador de mensagens na consola, mas ele continua a funcionar normalmente.
                            "Mensagens processadas: "
                            + Program.totalMensagens
                        );

                        
                        string mensagemCifrada = EncryptString(mensagem);                   // Cifra novamente a mensagem utilizando o AES global antes de reencaminhar para todos (garante privacidade na rede).

                        
                        byte[] pacoteParaTodos =                                            // Empacota a string cifrada de volta no formato ProtocolSI do tipo DATA.
                            protocolSI.Make(ProtocolSICmdType.DATA, mensagemCifrada);


                        
                        
                        
                        foreach (ClientHandler outroCliente in Program.clientesLigados)     // Percorremos a lista e "empurramos" a mensagem para cada um
                        {                                                                   // Se removeres este foreach: O chat passa a ser privado (o servidor lê mas ninguém recebe as mensagens de ninguém).
                            outroCliente.EnviarMensagem(pacoteParaTodos);                   // Se a lista "clientesLigados" for modificada por outra thread (ex: um cliente a desligar-se) enquanto este ciclo roda: O programa lança uma InvalidOperationException.
                        }

                        
                        ack = protocolSI.Make(ProtocolSICmdType.ACK);                       // Cria um pacote de resposta que apenas diz "OK, recebi" (ACK) ACK é um tipo de pacote muito comum em protocolos de rede para confirmar que uma mensagem chegou bem.
                        
                        networkStream.Write(ack, 0, ack.Length);                            // Atira esse pacote de "OK" de volta pelo tubo para o cliente
                        break;

                    
                    case ProtocolSICmdType.USER_OPTION_3:                                   // Caso o cliente solicite o histórico de conversas do servidor (Pedido de histórico).

                        string path = "chatlog.txt";

                        
                        if (!File.Exists(path))                                             // Se o ficheiro de log ainda não existir no disco:
                            File.WriteAllText(path, "");                                    // Cria um ficheiro em branco para evitar erros de "Ficheiro Não Encontrado" logo à frente.

                        
                        string historico = File.ReadAllText(path);                          // Lê todo o conteúdo textual do histórico guardado.

                        int chunkSize = 1024;                                               // Define o tamanho máximo (1024 caracteres) de cada pedaço de texto enviado, evitando estouro de buffer na rede.
                                                                                            // Se mudares para um valor gigante (ex: 999999): O pacote pode exceder o limite de transporte do protocolo de rede e falhar.


                        
                       
                        for (int i = 0; i < historico.Length; i += chunkSize)               // Ciclo que fatia o histórico em blocos de 1024 em 1024 caracteres.
                        {                                                                   // Se removeres este ciclo: Terias de enviar o histórico inteiro de uma vez, arriscando truncar os dados se o ficheiro for muito pesado.
                            
                            string chunk = historico.Substring(                             // Corta com precisão matemática a string do histórico do índice atual até ao tamanho do chunk (ou o que sobrar da string).
                                i,                                                          // O i serve como ponto de partida para o corte, e a função Substring vai extrair a parte da string que começa no índice i.
                                Math.Min(chunkSize, historico.Length - i)                   // O segundo parâmetro do Substring é o comprimento do pedaço a cortar. Usamos Math.Min para garantir que não tentamos cortar além do final da string, o que causaria um erro. Se o que sobra da string for menor que chunkSize, cortamos só o que resta.
                            );

                            
                            string chunkCifrado = EncryptString(chunk);                     // Cifra o pedaço cortado com o AES.

                            
                            byte[] packetHistorico = protocolSI.Make(                       // Transforma o pedaço cifrado num pacote ProtocolSI de dados.
                                ProtocolSICmdType.DATA,
                                chunkCifrado
                            );

                            
                            networkStream.Write(                                            // Transmite este pedaço específico para o cliente
                                packetHistorico, 
                                0, 
                                packetHistorico.Length
                            );
                        }

                        
                        
                        byte[] eof = protocolSI.Make(ProtocolSICmdType.EOF);                // Após enviar todos os pedaços, cria um pacote EOF (End of File) para avisar o cliente de que o histórico terminou. EOF Significa "End of File" e é um sinal comum em protocolos para indicar que não há mais dados a serem enviados.
                        networkStream.Write(eof, 0, eof.Length);                            // Se removeres estas duas linhas: O cliente fica eternamente à espera, bloqueado, sem saber se o histórico já veio todo ou não.

                        break;


                    case ProtocolSICmdType.EOT:                                             // Se o pacote disser "Vou desligar": EOT significa "End of Transmission" e é o sinal formal de que o cliente quer encerrar a comunicação de forma limpa.
                                                                                        
                        Console.WriteLine("Fim da comunicação do cliente {0}", clientID);   // Escreve no servidor que aquele cliente específico se vai embora

                        
                        ack = protocolSI.Make(ProtocolSICmdType.ACK);                       // Cria o último pacote de "OK" para confirmar o fecho da ligação
                        
                        networkStream.Write(ack, 0, ack.Length);                            // Envia o último "OK" de volta para o cliente

                        
                        File.AppendAllText(                                                 // Grava no log do ficheiro que o utilizador saiu voluntariamente.
                            "chatlog.txt",                                                  // chatlog.txt é o mesmo ficheiro onde guardamos as mensagens, criando um histórico completo de quem entrou, quem falou e quem saiu.
                            "[" + DateTime.Now.ToString("HH:mm:ss") + "] Cliente "          // Aqui aparece no servidor a mensagem de que o cliente se desligou, com o timestamp para manter a ordem cronológica dos eventos.
                            + clientID + " desligou-se."                                
                            + Environment.NewLine
                        );
                        
                        
                        Program.clientesLigados.Remove(this);                               // Remove da lista para o servidor não tentar enviar mensagens a quem já saiu
                        break;                                                              // Se removeres esta linha: Da próxima vez que alguém enviar uma mensagem, o servidor vai tentar mandar para esta instância morta, disparando uma exceção no método EnviarMensagem.
                }
            }
        }

        
        
        private string EncryptString(string texto)                                          // Método auxiliar que cifra strings usando criptografia simétrica AES.
        {                                                                                   // Se passares uma string vazia (""): O método executa normalmente e devolve o bloco correspondente ao padding cifrado.
                                                                                            // Se removeres esta função: O servidor perde a capacidade de cifrar o tráfego de saída, impedindo o envio seguro de mensagens e históricos.
            
            ICryptoTransform encryptor = Program.aesGlobal.CreateEncryptor();               // Cria o objeto transformador responsável pelo algoritmo de cifragem baseado na chave e IV do aesGlobal.

            
            byte[] dados = Encoding.UTF8.GetBytes(texto);                                   // Converte a string de texto legível num array de bytes no formato UTF8.

            
            byte[] cifrado = encryptor.TransformFinalBlock(dados, 0, dados.Length);         // Cifra os bytes processando o bloco final (aplica o preenchimento/padding obrigatório do AES)

            
            return Convert.ToBase64String(cifrado);                                         // Converte os bytes cifrados resultantes numa string textual Base64 legível para transporte estável na rede.
        }

        
        
        private bool VerificarAssinatura(string mensagem, string assinaturaBase64)          // Método auxiliar que valida a assinatura assimétrica RSA SHA256 vinda do cliente.
        {                                                                                   // Se a assinaturaBase64 vier corrompida (caracteres inválidos fora do padrão Base64): Lança uma FormatException nesta linha.
                                                                                            // Se removeres este método: O servidor perde o pilar do "Não-repúdio" e da "Autenticidade", aceitando qualquer mensagem adulterada por terceiros na rede.
            
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();                  // Instancia o provedor de serviços criptográficos RSA.

            
            rsa.FromXmlString(publicKeyCliente);                                            // Alimenta o motor RSA com a chave pública do cliente guardada previamente no handshake.

            
            byte[] assinatura = Convert.FromBase64String(assinaturaBase64);                 // Converte a assinatura recebida em formato de texto Base64 de volta para o array de bytes binário original.

            
            byte[] dados = Encoding.UTF8.GetBytes(mensagem);                                // Converte o texto da mensagem recebida em bytes para podermos recalcular o hash localmente.

            
            
            return rsa.VerifyData(                                                          // Verifica matematicamente os dados contra a assinatura utilizando o identificador oficial do algoritmo SHA256.
                dados,
                CryptoConfig.MapNameToOID("SHA256"),
                assinatura
            );                                                                              // Retorna 'true' se a assinatura foi gerada pela chave privada correspondente a esta chave pública sobre esta exata mensagem. Caso contrário, retorna 'false'.
        }

        
        
        private string DecryptString(string texto)                                          // Método auxiliar que decifra strings recebidas usando criptografia simétrica AES.
        {                                                                                   // Se a string recebida em "texto" não for um Base64 válido: Dispara imediatamente uma FormatException.
                                                                                            // Se removeres esta função: O servidor fica totalmente incapaz de ler qualquer mensagem de chat ou comando enviado pelos clientes que usam proteção.
            
            ICryptoTransform decryptor = Program.aesGlobal.CreateDecryptor();               // Cria o objeto transformador responsável pelo algoritmo de decifragem baseado na chave e IV do aesGlobal.

            
            byte[] dados = Convert.FromBase64String(texto);                                 // Reconverte a string Base64 recebida da rede para o array de bytes cifrados originais.

            
            byte[] decifrado = decryptor.TransformFinalBlock(dados, 0, dados.Length);       // Executa o reverso do algoritmo, decifrando os blocos e removendo o padding automático
                                                                                            // Se a chave AES global do servidor for diferente da chave que o cliente usou para cifrar: Esta linha arrebenta com erro "Bad Data" ou "Padding is invalid".

            
            return Encoding.UTF8.GetString(decifrado);                                      // Converte os bytes decifrados limpos de volta para a string de texto legível original em UTF8.
        }
    }
}



