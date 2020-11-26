namespace LP
{
    partial class MenuForm
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
            this.buttonAppariement2Graphes = new System.Windows.Forms.Button();
            this.buttonTestMultiplesMethodes = new System.Windows.Forms.Button();
            this.buttonTestUneMethode = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonAppariement2Graphes
            // 
            this.buttonAppariement2Graphes.Location = new System.Drawing.Point(29, 51);
            this.buttonAppariement2Graphes.Name = "buttonAppariement2Graphes";
            this.buttonAppariement2Graphes.Size = new System.Drawing.Size(183, 23);
            this.buttonAppariement2Graphes.TabIndex = 0;
            this.buttonAppariement2Graphes.Text = "appariement de deux graphes";
            this.buttonAppariement2Graphes.UseVisualStyleBackColor = true;
            this.buttonAppariement2Graphes.Click += new System.EventHandler(this.buttonAppariement2Graphes_Click);
            // 
            // buttonTestMultiplesMethodes
            // 
            this.buttonTestMultiplesMethodes.Location = new System.Drawing.Point(29, 128);
            this.buttonTestMultiplesMethodes.Name = "buttonTestMultiplesMethodes";
            this.buttonTestMultiplesMethodes.Size = new System.Drawing.Size(183, 23);
            this.buttonTestMultiplesMethodes.TabIndex = 1;
            this.buttonTestMultiplesMethodes.Text = "expérimentations";
            this.buttonTestMultiplesMethodes.UseVisualStyleBackColor = true;
            this.buttonTestMultiplesMethodes.Click += new System.EventHandler(this.buttonTestMultiplesMethodes_Click);
            // 
            // buttonTestUneMethode
            // 
            this.buttonTestUneMethode.Location = new System.Drawing.Point(29, 89);
            this.buttonTestUneMethode.Name = "buttonTestUneMethode";
            this.buttonTestUneMethode.Size = new System.Drawing.Size(183, 23);
            this.buttonTestUneMethode.TabIndex = 2;
            this.buttonTestUneMethode.Text = "test sur une méthode";
            this.buttonTestUneMethode.UseVisualStyleBackColor = true;
            this.buttonTestUneMethode.Click += new System.EventHandler(this.buttonTestUneMethode_Click);
            // 
            // MenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(236, 195);
            this.Controls.Add(this.buttonTestUneMethode);
            this.Controls.Add(this.buttonTestMultiplesMethodes);
            this.Controls.Add(this.buttonAppariement2Graphes);
            this.Name = "MenuForm";
            this.Text = "Menu";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAppariement2Graphes;
        private System.Windows.Forms.Button buttonTestMultiplesMethodes;
        private System.Windows.Forms.Button buttonTestUneMethode;
    }
}