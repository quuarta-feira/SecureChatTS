using System;                       // Provides basic system functions and base classes.
using System.Windows.Forms;         // Provides classes for creating Windows-based applications with a graphical user interface (GUI).

namespace SecureChatTS              // Pasta que contém o código do programa, indicando que este código faz parte do projeto "SecureChatTS".
{
    internal static class Program   // Define a classe "Program" como interna e estática, indicando que ela não pode ser instanciada e é acessível apenas dentro do mesmo assembly.
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]                 // Atributo que indica que o modelo de threading para a aplicação é Single Thread Apartment (STA), necessário para aplicações Windows Forms.
        static void Main()          // O método principal, que é o ponto de entrada da aplicação. Ele é chamado quando a aplicação é iniciada.
        {
            Application.EnableVisualStyles();                       // Habilita os estilos visuais para a aplicação, permitindo que ela tenha uma aparência moderna e consistente com o sistema operacional.
            Application.SetCompatibleTextRenderingDefault(false);   // Define o modo de renderização de texto para a aplicação, garantindo que ela use o modo de renderização padrão do sistema.
            Application.Run(new Form1());                           // Inicia a aplicação e abre a janela principal, que é uma instância da classe "Form1". A aplicação continuará rodando até que a janela seja fechada.
        }
    }
}
