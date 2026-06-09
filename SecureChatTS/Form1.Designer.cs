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
            this.bt_Enviar = new System.Windows.Forms.Button();
            this.bt_Sair = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.buttonRegistar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxMensagem
            // 
            this.textBoxMensagem.BackColor = System.Drawing.Color.Silver;
            this.textBoxMensagem.Location = new System.Drawing.Point(12, 513);
            this.textBoxMensagem.Multiline = true;
            this.textBoxMensagem.Name = "textBoxMensagem";
            this.textBoxMensagem.Size = new System.Drawing.Size(940, 100);
            this.textBoxMensagem.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(8, 477);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(252, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "Mensagem para o servidor:";
            // 
            // bt_Enviar
            // 
            this.bt_Enviar.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.bt_Enviar.BackColor = System.Drawing.Color.White;
            this.bt_Enviar.ForeColor = System.Drawing.Color.Black;
            this.bt_Enviar.Location = new System.Drawing.Point(968, 513);
            this.bt_Enviar.Name = "bt_Enviar";
            this.bt_Enviar.Size = new System.Drawing.Size(95, 100);
            this.bt_Enviar.TabIndex = 2;
            this.bt_Enviar.Text = "Enviar";
            this.bt_Enviar.UseVisualStyleBackColor = false;
            this.bt_Enviar.Click += new System.EventHandler(this.bt_Enviar_Click);
            // 
            // bt_Sair
            // 
            this.bt_Sair.BackColor = System.Drawing.Color.White;
            this.bt_Sair.ForeColor = System.Drawing.Color.Black;
            this.bt_Sair.Location = new System.Drawing.Point(968, 12);
            this.bt_Sair.Name = "bt_Sair";
            this.bt_Sair.Size = new System.Drawing.Size(95, 37);
            this.bt_Sair.TabIndex = 3;
            this.bt_Sair.Text = "Sair";
            this.bt_Sair.UseVisualStyleBackColor = false;
            this.bt_Sair.Click += new System.EventHandler(this.bt_Sair_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.richTextBox1.ForeColor = System.Drawing.Color.White;
            this.richTextBox1.Location = new System.Drawing.Point(12, 240);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1051, 218);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(12, 208);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(171, 22);
            this.label4.TabIndex = 9;
            this.label4.Text = "Histórico do Chat:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(26, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 22);
            this.label2.TabIndex = 10;
            this.label2.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(29, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 22);
            this.label3.TabIndex = 11;
            this.label3.Text = "Password:";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(138, 44);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(238, 26);
            this.textBoxUsername.TabIndex = 13;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(138, 104);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(238, 26);
            this.textBoxPassword.TabIndex = 14;
            // 
            // buttonLogin
            // 
            this.buttonLogin.BackColor = System.Drawing.Color.White;
            this.buttonLogin.ForeColor = System.Drawing.Color.Black;
            this.buttonLogin.Location = new System.Drawing.Point(305, 147);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(71, 29);
            this.buttonLogin.TabIndex = 15;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = false;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // buttonRegistar
            // 
            this.buttonRegistar.BackColor = System.Drawing.Color.White;
            this.buttonRegistar.ForeColor = System.Drawing.Color.Black;
            this.buttonRegistar.Location = new System.Drawing.Point(192, 147);
            this.buttonRegistar.Name = "buttonRegistar";
            this.buttonRegistar.Size = new System.Drawing.Size(97, 29);
            this.buttonRegistar.TabIndex = 16;
            this.buttonRegistar.Text = "Registar";
            this.buttonRegistar.UseVisualStyleBackColor = false;
            this.buttonRegistar.Click += new System.EventHandler(this.buttonRegistar_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1075, 625);
            this.Controls.Add(this.buttonRegistar);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxUsername);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.bt_Sair);
            this.Controls.Add(this.bt_Enviar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxMensagem);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMensagem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button bt_Enviar;
        private System.Windows.Forms.Button bt_Sair;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.Button buttonRegistar;
    }
}

