namespace NikeAuto
{
    partial class UpcomingProduct
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UpcomingProductImage = new System.Windows.Forms.PictureBox();
            this.ProductNameLabel = new System.Windows.Forms.Label();
            this.ProductUpcomingDateLabel = new System.Windows.Forms.Label();
            this.ProductPriceLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.UpcomingProductImage)).BeginInit();
            this.SuspendLayout();
            // 
            // UpcomingProductImage
            // 
            this.UpcomingProductImage.Location = new System.Drawing.Point(0, 0);
            this.UpcomingProductImage.Name = "UpcomingProductImage";
            this.UpcomingProductImage.Size = new System.Drawing.Size(131, 76);
            this.UpcomingProductImage.TabIndex = 0;
            this.UpcomingProductImage.TabStop = false;
            this.UpcomingProductImage.Click += new System.EventHandler(this.UpcomingProductImage_Click);
            // 
            // ProductNameLabel
            // 
            this.ProductNameLabel.AutoSize = true;
            this.ProductNameLabel.ForeColor = System.Drawing.Color.White;
            this.ProductNameLabel.Location = new System.Drawing.Point(139, 1);
            this.ProductNameLabel.Name = "ProductNameLabel";
            this.ProductNameLabel.Size = new System.Drawing.Size(0, 13);
            this.ProductNameLabel.TabIndex = 2;
            // 
            // ProductUpcomingDateLabel
            // 
            this.ProductUpcomingDateLabel.AutoSize = true;
            this.ProductUpcomingDateLabel.ForeColor = System.Drawing.Color.White;
            this.ProductUpcomingDateLabel.Location = new System.Drawing.Point(140, 26);
            this.ProductUpcomingDateLabel.Name = "ProductUpcomingDateLabel";
            this.ProductUpcomingDateLabel.Size = new System.Drawing.Size(0, 13);
            this.ProductUpcomingDateLabel.TabIndex = 2;
            // 
            // ProductPriceLabel
            // 
            this.ProductPriceLabel.AutoSize = true;
            this.ProductPriceLabel.ForeColor = System.Drawing.Color.White;
            this.ProductPriceLabel.Location = new System.Drawing.Point(140, 49);
            this.ProductPriceLabel.Name = "ProductPriceLabel";
            this.ProductPriceLabel.Size = new System.Drawing.Size(0, 13);
            this.ProductPriceLabel.TabIndex = 2;
            // 
            // UpcomingProduct
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(26)))));
            this.Controls.Add(this.ProductPriceLabel);
            this.Controls.Add(this.ProductUpcomingDateLabel);
            this.Controls.Add(this.ProductNameLabel);
            this.Controls.Add(this.UpcomingProductImage);
            this.Name = "UpcomingProduct";
            this.Size = new System.Drawing.Size(309, 73);
            ((System.ComponentModel.ISupportInitialize)(this.UpcomingProductImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label ProductNameLabel;
        public System.Windows.Forms.Label ProductUpcomingDateLabel;
        public System.Windows.Forms.Label ProductPriceLabel;
        public System.Windows.Forms.PictureBox UpcomingProductImage;
    }
}
