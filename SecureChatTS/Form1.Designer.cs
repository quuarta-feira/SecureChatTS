namespace SecureChatTS
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxMensagem = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.enviar_bt_TP = new System.Windows.Forms.Button();
            this.bt_Sair = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.buttonRegistar = new System.Windows.Forms.Button();
            this.label_TestePratico = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.textBox_TestePratico = new System.Windows.Forms.TextBox();
            this.button_TestePratico = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxMensagem
            // 
            this.textBoxMensagem.BackColor = System.Drawing.Color.Silver;
            this.textBoxMensagem.Location = new System.Drawing.Point(8, 333);
            this.textBoxMensagem.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxMensagem.Multiline = true;
            this.textBoxMensagem.Name = "textBoxMensagem";
            this.textBoxMensagem.Size = new System.Drawing.Size(628, 66);
            this.textBoxMensagem.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(5, 310);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(182, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Mensagem para o servidor:";
            // 
            // enviar_bt_TP
            // 
            this.enviar_bt_TP.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.enviar_bt_TP.BackColor = System.Drawing.Color.White;
            this.enviar_bt_TP.ForeColor = System.Drawing.Color.Black;
            this.enviar_bt_TP.Location = new System.Drawing.Point(645, 333);
            this.enviar_bt_TP.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.enviar_bt_TP.Name = "enviar_bt_TP";
            this.enviar_bt_TP.Size = new System.Drawing.Size(63, 65);
            this.enviar_bt_TP.TabIndex = 2;
            this.enviar_bt_TP.Text = "Enviar";
            this.enviar_bt_TP.UseVisualStyleBackColor = false;
            this.enviar_bt_TP.Click += new System.EventHandler(this.bt_Enviar_Click);
            // 
            // bt_Sair
            // 
            this.bt_Sair.BackColor = System.Drawing.Color.White;
            this.bt_Sair.ForeColor = System.Drawing.Color.Black;
            this.bt_Sair.Location = new System.Drawing.Point(645, 8);
            this.bt_Sair.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.bt_Sair.Name = "bt_Sair";
            this.bt_Sair.Size = new System.Drawing.Size(63, 24);
            this.bt_Sair.TabIndex = 3;
            this.bt_Sair.Text = "Sair";
            this.bt_Sair.UseVisualStyleBackColor = false;
            this.bt_Sair.Click += new System.EventHandler(this.bt_Sair_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.richTextBox1.ForeColor = System.Drawing.Color.White;
            this.richTextBox1.Location = new System.Drawing.Point(8, 156);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(702, 143);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(8, 135);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "Histórico do Chat:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(17, 29);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(19, 68);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 15);
            this.label3.TabIndex = 11;
            this.label3.Text = "Password:";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(92, 68);
            this.textBoxPassword.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(160, 20);
            this.textBoxPassword.TabIndex = 14;
            // 
            // buttonLogin
            // 
            this.buttonLogin.BackColor = System.Drawing.Color.White;
            this.buttonLogin.ForeColor = System.Drawing.Color.Black;
            this.buttonLogin.Location = new System.Drawing.Point(203, 96);
            this.buttonLogin.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(47, 19);
            this.buttonLogin.TabIndex = 15;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = false;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // buttonRegistar
            // 
            this.buttonRegistar.BackColor = System.Drawing.Color.White;
            this.buttonRegistar.ForeColor = System.Drawing.Color.Black;
            this.buttonRegistar.Location = new System.Drawing.Point(128, 96);
            this.buttonRegistar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonRegistar.Name = "buttonRegistar";
            this.buttonRegistar.Size = new System.Drawing.Size(65, 19);
            this.buttonRegistar.TabIndex = 16;
            this.buttonRegistar.Text = "Registar";
            this.buttonRegistar.UseVisualStyleBackColor = false;
            this.buttonRegistar.Click += new System.EventHandler(this.buttonRegistar_Click);
            // 
            // label_TestePratico
            // 
            this.label_TestePratico.AutoSize = true;
            this.label_TestePratico.BackColor = System.Drawing.Color.Black;
            this.label_TestePratico.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_TestePratico.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label_TestePratico.Location = new System.Drawing.Point(454, 132);
            this.label_TestePratico.Name = "label_TestePratico";
            this.label_TestePratico.Size = new System.Drawing.Size(126, 18);
            this.label_TestePratico.TabIndex = 17;
            this.label_TestePratico.Text = "Hash Verificada";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(92, 29);
            this.textBoxUsername.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(160, 20);
            this.textBoxUsername.TabIndex = 13;
            // 
            // textBox_TestePratico
            // 
            this.textBox_TestePratico.Location = new System.Drawing.Point(585, 132);
            this.textBox_TestePratico.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_TestePratico.Name = "textBox_TestePratico";
            this.textBox_TestePratico.Size = new System.Drawing.Size(59, 20);
            this.textBox_TestePratico.TabIndex = 18;
            // 
            // button_TestePratico
            // 
            this.button_TestePratico.BackColor = System.Drawing.Color.White;
            this.button_TestePratico.ForeColor = System.Drawing.Color.Black;
            this.button_TestePratico.Location = new System.Drawing.Point(648, 129);
            this.button_TestePratico.Margin = new System.Windows.Forms.Padding(2);
            this.button_TestePratico.Name = "button_TestePratico";
            this.button_TestePratico.Size = new System.Drawing.Size(63, 24);
            this.button_TestePratico.TabIndex = 19;
            this.button_TestePratico.Text = "Verificar";
            this.button_TestePratico.UseVisualStyleBackColor = false;
            this.button_TestePratico.Click += new System.EventHandler(this.button_TestePratico_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(717, 406);
            this.Controls.Add(this.button_TestePratico);
            this.Controls.Add(this.textBox_TestePratico);
            this.Controls.Add(this.label_TestePratico);
            this.Controls.Add(this.buttonRegistar);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxUsername);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.bt_Sair);
            this.Controls.Add(this.enviar_bt_TP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxMensagem);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMensagem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button enviar_bt_TP;
        private System.Windows.Forms.Button bt_Sair;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.Button buttonRegistar;
        private System.Windows.Forms.Label label_TestePratico;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.TextBox textBox_TestePratico;
        private System.Windows.Forms.Button button_TestePratico;
    }
}

