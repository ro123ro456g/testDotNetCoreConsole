namespace MaplestoryJanKenPon
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.BtnBack = new System.Windows.Forms.Button();
            this.BtnReset = new System.Windows.Forms.Button();
            this.BtnJan = new System.Windows.Forms.Button();
            this.BtnKen = new System.Windows.Forms.Button();
            this.BtnPon = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CountJan = new System.Windows.Forms.Label();
            this.CountKen = new System.Windows.Forms.Label();
            this.CountPon = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.TipJan = new System.Windows.Forms.Label();
            this.TipKen = new System.Windows.Forms.Label();
            this.TipPon = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.Lazy = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BtnBack
            // 
            this.BtnBack.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.BtnBack.Location = new System.Drawing.Point(12, 12);
            this.BtnBack.Name = "BtnBack";
            this.BtnBack.Size = new System.Drawing.Size(116, 61);
            this.BtnBack.TabIndex = 0;
            this.BtnBack.Text = "靠杯,按錯上一步\r\nback";
            this.BtnBack.UseVisualStyleBackColor = true;
            this.BtnBack.Click += new System.EventHandler(this.BtnBack_Click);
            // 
            // BtnReset
            // 
            this.BtnReset.Font = new System.Drawing.Font("微軟正黑體", 9F);
            this.BtnReset.Location = new System.Drawing.Point(12, 112);
            this.BtnReset.Name = "BtnReset";
            this.BtnReset.Size = new System.Drawing.Size(116, 64);
            this.BtnReset.TabIndex = 1;
            this.BtnReset.Text = "重設\r\nReset";
            this.BtnReset.UseVisualStyleBackColor = true;
            this.BtnReset.Click += new System.EventHandler(this.BtnReset_Click);
            // 
            // BtnJan
            // 
            this.BtnJan.Font = new System.Drawing.Font("微軟正黑體", 9F);
            this.BtnJan.Location = new System.Drawing.Point(574, 46);
            this.BtnJan.Name = "BtnJan";
            this.BtnJan.Size = new System.Drawing.Size(93, 76);
            this.BtnJan.TabIndex = 2;
            this.BtnJan.Text = "剪刀";
            this.BtnJan.UseVisualStyleBackColor = true;
            this.BtnJan.Click += new System.EventHandler(this.BtnJan_Click);
            // 
            // BtnKen
            // 
            this.BtnKen.Font = new System.Drawing.Font("微軟正黑體", 9F);
            this.BtnKen.Location = new System.Drawing.Point(264, 46);
            this.BtnKen.Name = "BtnKen";
            this.BtnKen.Size = new System.Drawing.Size(93, 76);
            this.BtnKen.TabIndex = 3;
            this.BtnKen.Text = "石頭";
            this.BtnKen.UseVisualStyleBackColor = true;
            this.BtnKen.Click += new System.EventHandler(this.BtnKen_Click);
            // 
            // BtnPon
            // 
            this.BtnPon.Font = new System.Drawing.Font("微軟正黑體", 9F);
            this.BtnPon.Location = new System.Drawing.Point(420, 46);
            this.BtnPon.Name = "BtnPon";
            this.BtnPon.Size = new System.Drawing.Size(93, 76);
            this.BtnPon.TabIndex = 4;
            this.BtnPon.Text = "布";
            this.BtnPon.UseVisualStyleBackColor = true;
            this.BtnPon.Click += new System.EventHandler(this.BtnPon_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微軟正黑體", 15F);
            this.label1.Location = new System.Drawing.Point(569, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 25);
            this.label1.TabIndex = 5;
            this.label1.Text = "次數:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微軟正黑體", 15F);
            this.label2.Location = new System.Drawing.Point(259, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 25);
            this.label2.TabIndex = 6;
            this.label2.Text = "次數:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微軟正黑體", 15F);
            this.label3.Location = new System.Drawing.Point(415, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 25);
            this.label3.TabIndex = 7;
            this.label3.Text = "次數:";
            // 
            // CountJan
            // 
            this.CountJan.AutoSize = true;
            this.CountJan.Font = new System.Drawing.Font("微軟正黑體", 15F);
            this.CountJan.Location = new System.Drawing.Point(632, 125);
            this.CountJan.Name = "CountJan";
            this.CountJan.Size = new System.Drawing.Size(24, 25);
            this.CountJan.TabIndex = 8;
            this.CountJan.Text = "0";
            // 
            // CountKen
            // 
            this.CountKen.AutoSize = true;
            this.CountKen.Font = new System.Drawing.Font("微軟正黑體", 15F);
            this.CountKen.Location = new System.Drawing.Point(322, 125);
            this.CountKen.Name = "CountKen";
            this.CountKen.Size = new System.Drawing.Size(24, 25);
            this.CountKen.TabIndex = 9;
            this.CountKen.Text = "0";
            // 
            // CountPon
            // 
            this.CountPon.AutoSize = true;
            this.CountPon.Font = new System.Drawing.Font("微軟正黑體", 15F);
            this.CountPon.Location = new System.Drawing.Point(478, 125);
            this.CountPon.Name = "CountPon";
            this.CountPon.Size = new System.Drawing.Size(24, 25);
            this.CountPon.TabIndex = 10;
            this.CountPon.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微軟正黑體", 9F);
            this.label4.Location = new System.Drawing.Point(147, 160);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(108, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "下次NPC出的機率:";
            // 
            // TipJan
            // 
            this.TipJan.AutoSize = true;
            this.TipJan.Font = new System.Drawing.Font("微軟正黑體", 15F);
            this.TipJan.Location = new System.Drawing.Point(584, 160);
            this.TipJan.Name = "TipJan";
            this.TipJan.Size = new System.Drawing.Size(54, 25);
            this.TipJan.TabIndex = 12;
            this.TipJan.Text = "33%";
            // 
            // TipKen
            // 
            this.TipKen.AutoSize = true;
            this.TipKen.Font = new System.Drawing.Font("微軟正黑體", 15F);
            this.TipKen.Location = new System.Drawing.Point(274, 160);
            this.TipKen.Name = "TipKen";
            this.TipKen.Size = new System.Drawing.Size(54, 25);
            this.TipKen.TabIndex = 13;
            this.TipKen.Text = "33%";
            // 
            // TipPon
            // 
            this.TipPon.AutoSize = true;
            this.TipPon.Font = new System.Drawing.Font("微軟正黑體", 15F);
            this.TipPon.Location = new System.Drawing.Point(430, 160);
            this.TipPon.Name = "TipPon";
            this.TipPon.Size = new System.Drawing.Size(54, 25);
            this.TipPon.TabIndex = 14;
            this.TipPon.Text = "33%";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微軟正黑體", 15F);
            this.label5.Location = new System.Drawing.Point(764, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 25);
            this.label5.TabIndex = 15;
            this.label5.Text = "懶人包:";
            // 
            // Lazy
            // 
            this.Lazy.AutoSize = true;
            this.Lazy.Font = new System.Drawing.Font("微軟正黑體", 15F);
            this.Lazy.Location = new System.Drawing.Point(706, 68);
            this.Lazy.Name = "Lazy";
            this.Lazy.Size = new System.Drawing.Size(244, 25);
            this.Lazy.TabIndex = 16;
            this.Lazy.Text = "現在出什麼勝率都一樣1/3";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("微軟正黑體", 15F);
            this.label6.Location = new System.Drawing.Point(380, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(153, 25);
            this.label6.TabIndex = 17;
            this.label6.Text = "請選擇NPC出的";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微軟正黑體", 15F);
            this.label7.Location = new System.Drawing.Point(901, 160);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 25);
            this.label7.TabIndex = 18;
            this.label7.Text = "啊咧咧";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(985, 188);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.Lazy);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TipPon);
            this.Controls.Add(this.TipKen);
            this.Controls.Add(this.TipJan);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.CountPon);
            this.Controls.Add(this.CountKen);
            this.Controls.Add(this.CountJan);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnPon);
            this.Controls.Add(this.BtnKen);
            this.Controls.Add(this.BtnJan);
            this.Controls.Add(this.BtnReset);
            this.Controls.Add(this.BtnBack);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "楓谷活動小遊戲剪刀石頭布計算器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnBack;
        private System.Windows.Forms.Button BtnReset;
        private System.Windows.Forms.Button BtnJan;
        private System.Windows.Forms.Button BtnKen;
        private System.Windows.Forms.Button BtnPon;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label CountJan;
        private System.Windows.Forms.Label CountKen;
        private System.Windows.Forms.Label CountPon;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label TipJan;
        private System.Windows.Forms.Label TipKen;
        private System.Windows.Forms.Label TipPon;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label Lazy;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}

