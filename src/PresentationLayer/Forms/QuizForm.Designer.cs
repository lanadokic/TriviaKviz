namespace PresentationLayer.Forms
{
    partial class QuizForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblFeedback = new System.Windows.Forms.Label();
            this.lblQuestion = new System.Windows.Forms.Label();
            this.panelAnswers = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNext = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(12, 13);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(0, 13);
            this.lblProgress.TabIndex = 0;
            // 
            // lblFeedback
            // 
            this.lblFeedback.AutoSize = true;
            this.lblFeedback.Location = new System.Drawing.Point(280, 13);
            this.lblFeedback.Name = "lblFeedback";
            this.lblFeedback.Size = new System.Drawing.Size(0, 13);
            this.lblFeedback.TabIndex = 1;
            // 
            // lblQuestion
            // 
            this.lblQuestion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblQuestion.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.lblQuestion.Location = new System.Drawing.Point(12, 40);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(736, 70);
            this.lblQuestion.TabIndex = 2;
            this.lblQuestion.Text = "";
            // 
            // panelAnswers
            // 
            this.panelAnswers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelAnswers.AutoScroll = true;
            this.panelAnswers.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.panelAnswers.Location = new System.Drawing.Point(15, 113);
            this.panelAnswers.Name = "panelAnswers";
            this.panelAnswers.Size = new System.Drawing.Size(733, 336);
            this.panelAnswers.TabIndex = 3;
            this.panelAnswers.WrapContents = false;
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(15, 462);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(733, 35);
            this.btnNext.TabIndex = 4;
            this.btnNext.Text = "Potvrdi / Sljedeće";
            this.btnNext.UseVisualStyleBackColor = true;
            // 
            // QuizForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(760, 509);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.panelAnswers);
            this.Controls.Add(this.lblQuestion);
            this.Controls.Add(this.lblFeedback);
            this.Controls.Add(this.lblProgress);
            this.Name = "QuizForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Kviz";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblFeedback;
        private System.Windows.Forms.Label lblQuestion;
        private System.Windows.Forms.FlowLayoutPanel panelAnswers;
        private System.Windows.Forms.Button btnNext;
    }
}
