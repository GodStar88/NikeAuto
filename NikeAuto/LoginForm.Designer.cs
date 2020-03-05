namespace NikeAuto
{
    partial class LoginForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.input_id = new System.Windows.Forms.TextBox();
            this.input_password = new System.Windows.Forms.TextBox();
            this.label_id = new System.Windows.Forms.Label();
            this.label_password = new System.Windows.Forms.Label();
            this.button_ok = new System.Windows.Forms.Button();
            this.button_join = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // input_id
            // 
            this.input_id.Location = new System.Drawing.Point(94, 31);
            this.input_id.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.input_id.Name = "input_id";
            this.input_id.Size = new System.Drawing.Size(112, 20);
            this.input_id.TabIndex = 0;
            // 
            // input_password
            // 
            this.input_password.Location = new System.Drawing.Point(94, 63);
            this.input_password.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.input_password.Name = "input_password";
            this.input_password.PasswordChar = '*';
            this.input_password.Size = new System.Drawing.Size(112, 20);
            this.input_password.TabIndex = 1;
            // 
            // label_id
            // 
            this.label_id.AutoSize = true;
            this.label_id.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_id.Location = new System.Drawing.Point(11, 32);
            this.label_id.Name = "label_id";
            this.label_id.Size = new System.Drawing.Size(53, 16);
            this.label_id.TabIndex = 2;
            this.label_id.Text = "User ID";
            // 
            // label_password
            // 
            this.label_password.AutoSize = true;
            this.label_password.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_password.Location = new System.Drawing.Point(12, 64);
            this.label_password.Name = "label_password";
            this.label_password.Size = new System.Drawing.Size(68, 16);
            this.label_password.TabIndex = 3;
            this.label_password.Text = "Password";
            // 
            // button_ok
            // 
            this.button_ok.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_ok.Location = new System.Drawing.Point(218, 30);
            this.button_ok.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(71, 25);
            this.button_ok.TabIndex = 4;
            this.button_ok.Text = "Login";
            this.button_ok.UseVisualStyleBackColor = true;
            this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
            // 
            // button_join
            // 
            this.button_join.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_join.Location = new System.Drawing.Point(218, 61);
            this.button_join.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_join.Name = "button_join";
            this.button_join.Size = new System.Drawing.Size(71, 24);
            this.button_join.TabIndex = 5;
            this.button_join.Text = "Cancel";
            this.button_join.UseVisualStyleBackColor = true;
            this.button_join.Click += new System.EventHandler(this.button_join_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 101);
            this.Controls.Add(this.button_join);
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.label_password);
            this.Controls.Add(this.label_id);
            this.Controls.Add(this.input_password);
            this.Controls.Add(this.input_id);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LoginForm_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox input_id;
        private System.Windows.Forms.TextBox input_password;
        private System.Windows.Forms.Label label_id;
        private System.Windows.Forms.Label label_password;
        private System.Windows.Forms.Button button_ok;
        private System.Windows.Forms.Button button_join;
    }
}