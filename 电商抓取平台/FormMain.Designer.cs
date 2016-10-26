namespace 电商抓取平台
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CB_Type = new System.Windows.Forms.ComboBox();
            this.BTN_Search = new System.Windows.Forms.Button();
            this.CB_KeyWord = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.BTN_StopSearchProduct = new System.Windows.Forms.Button();
            this.TV_StoreList = new System.Windows.Forms.TreeView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.LV_ProductList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BG_Search = new System.ComponentModel.BackgroundWorker();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.BTN_OpenLogs = new System.Windows.Forms.Button();
            this.BTN_ClearLogs = new System.Windows.Forms.Button();
            this.CB_TxtLog = new System.Windows.Forms.CheckBox();
            this.CB_Kilo = new System.Windows.Forms.CheckBox();
            this.CB_SelLog = new System.Windows.Forms.CheckBox();
            this.LB_Log = new System.Windows.Forms.ListBox();
            this.BG_SearchProduct = new System.ComponentModel.BackgroundWorker();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CB_Type);
            this.groupBox1.Controls.Add(this.BTN_Search);
            this.groupBox1.Controls.Add(this.CB_KeyWord);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(792, 48);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "设置选项";
            // 
            // CB_Type
            // 
            this.CB_Type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CB_Type.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_Type.FormattingEnabled = true;
            this.CB_Type.Items.AddRange(new object[] {
            "天猫",
            "淘宝"});
            this.CB_Type.Location = new System.Drawing.Point(283, 19);
            this.CB_Type.Name = "CB_Type";
            this.CB_Type.Size = new System.Drawing.Size(85, 20);
            this.CB_Type.TabIndex = 3;
            // 
            // BTN_Search
            // 
            this.BTN_Search.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BTN_Search.Location = new System.Drawing.Point(374, 17);
            this.BTN_Search.Name = "BTN_Search";
            this.BTN_Search.Size = new System.Drawing.Size(67, 23);
            this.BTN_Search.TabIndex = 2;
            this.BTN_Search.Text = "搜索";
            this.BTN_Search.UseVisualStyleBackColor = true;
            this.BTN_Search.Click += new System.EventHandler(this.BTN_Search_Click);
            // 
            // CB_KeyWord
            // 
            this.CB_KeyWord.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CB_KeyWord.FormattingEnabled = true;
            this.CB_KeyWord.Location = new System.Drawing.Point(81, 19);
            this.CB_KeyWord.Name = "CB_KeyWord";
            this.CB_KeyWord.Size = new System.Drawing.Size(196, 20);
            this.CB_KeyWord.TabIndex = 1;
            this.CB_KeyWord.Text = "青菜";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "搜索关键字";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.BTN_StopSearchProduct);
            this.groupBox2.Controls.Add(this.TV_StoreList);
            this.groupBox2.Location = new System.Drawing.Point(2, 56);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(244, 403);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "店铺列表";
            // 
            // BTN_StopSearchProduct
            // 
            this.BTN_StopSearchProduct.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BTN_StopSearchProduct.Location = new System.Drawing.Point(84, 187);
            this.BTN_StopSearchProduct.Name = "BTN_StopSearchProduct";
            this.BTN_StopSearchProduct.Size = new System.Drawing.Size(67, 23);
            this.BTN_StopSearchProduct.TabIndex = 3;
            this.BTN_StopSearchProduct.Text = "停止";
            this.BTN_StopSearchProduct.UseVisualStyleBackColor = true;
            this.BTN_StopSearchProduct.Visible = false;
            this.BTN_StopSearchProduct.Click += new System.EventHandler(this.BTN_StopSearchProduct_Click);
            // 
            // TV_StoreList
            // 
            this.TV_StoreList.Location = new System.Drawing.Point(6, 20);
            this.TV_StoreList.Name = "TV_StoreList";
            this.TV_StoreList.Size = new System.Drawing.Size(232, 377);
            this.TV_StoreList.TabIndex = 0;
            this.TV_StoreList.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TV_StoreList_NodeMouseClick);
            this.TV_StoreList.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TV_StoreList_NodeMouseDoubleClick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.LV_ProductList);
            this.groupBox3.Location = new System.Drawing.Point(265, 56);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(661, 403);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "商品信息";
            // 
            // LV_ProductList
            // 
            this.LV_ProductList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LV_ProductList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.LV_ProductList.FullRowSelect = true;
            this.LV_ProductList.GridLines = true;
            this.LV_ProductList.Location = new System.Drawing.Point(6, 20);
            this.LV_ProductList.Name = "LV_ProductList";
            this.LV_ProductList.ShowItemToolTips = true;
            this.LV_ProductList.Size = new System.Drawing.Size(648, 377);
            this.LV_ProductList.TabIndex = 0;
            this.LV_ProductList.UseCompatibleStateImageBehavior = false;
            this.LV_ProductList.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "序号";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "标题";
            this.columnHeader2.Width = 178;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "价格";
            this.columnHeader3.Width = 104;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "单价";
            this.columnHeader4.Width = 97;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "销量";
            this.columnHeader5.Width = 91;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "ItemID";
            this.columnHeader6.Width = 112;
            // 
            // BG_Search
            // 
            this.BG_Search.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BG_Search_DoWork);
            this.BG_Search.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BG_Search_RunWorkerCompleted);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.BTN_OpenLogs);
            this.groupBox4.Controls.Add(this.BTN_ClearLogs);
            this.groupBox4.Controls.Add(this.CB_TxtLog);
            this.groupBox4.Controls.Add(this.CB_Kilo);
            this.groupBox4.Controls.Add(this.CB_SelLog);
            this.groupBox4.Controls.Add(this.LB_Log);
            this.groupBox4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox4.Location = new System.Drawing.Point(2, 466);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(924, 153);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "程序日志";
            // 
            // BTN_OpenLogs
            // 
            this.BTN_OpenLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BTN_OpenLogs.Font = new System.Drawing.Font("微软雅黑", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BTN_OpenLogs.Location = new System.Drawing.Point(427, 1);
            this.BTN_OpenLogs.Name = "BTN_OpenLogs";
            this.BTN_OpenLogs.Size = new System.Drawing.Size(75, 22);
            this.BTN_OpenLogs.TabIndex = 40;
            this.BTN_OpenLogs.Text = "打开日志";
            this.BTN_OpenLogs.UseVisualStyleBackColor = true;
            // 
            // BTN_ClearLogs
            // 
            this.BTN_ClearLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BTN_ClearLogs.Font = new System.Drawing.Font("微软雅黑", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BTN_ClearLogs.Location = new System.Drawing.Point(349, 1);
            this.BTN_ClearLogs.Name = "BTN_ClearLogs";
            this.BTN_ClearLogs.Size = new System.Drawing.Size(75, 22);
            this.BTN_ClearLogs.TabIndex = 39;
            this.BTN_ClearLogs.Text = "清空日志";
            this.BTN_ClearLogs.UseVisualStyleBackColor = true;
            // 
            // CB_TxtLog
            // 
            this.CB_TxtLog.AutoSize = true;
            this.CB_TxtLog.Checked = true;
            this.CB_TxtLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_TxtLog.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CB_TxtLog.Location = new System.Drawing.Point(244, -1);
            this.CB_TxtLog.Name = "CB_TxtLog";
            this.CB_TxtLog.Size = new System.Drawing.Size(99, 21);
            this.CB_TxtLog.TabIndex = 38;
            this.CB_TxtLog.Text = "文本记录日志";
            this.CB_TxtLog.UseVisualStyleBackColor = true;
            // 
            // CB_Kilo
            // 
            this.CB_Kilo.AutoSize = true;
            this.CB_Kilo.Checked = true;
            this.CB_Kilo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_Kilo.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CB_Kilo.Location = new System.Drawing.Point(154, -1);
            this.CB_Kilo.Name = "CB_Kilo";
            this.CB_Kilo.Size = new System.Drawing.Size(91, 21);
            this.CB_Kilo.TabIndex = 37;
            this.CB_Kilo.Text = "最大1000行";
            this.CB_Kilo.UseVisualStyleBackColor = true;
            // 
            // CB_SelLog
            // 
            this.CB_SelLog.AutoSize = true;
            this.CB_SelLog.Checked = true;
            this.CB_SelLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CB_SelLog.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CB_SelLog.Location = new System.Drawing.Point(76, -1);
            this.CB_SelLog.Name = "CB_SelLog";
            this.CB_SelLog.Size = new System.Drawing.Size(75, 21);
            this.CB_SelLog.TabIndex = 36;
            this.CB_SelLog.Text = "日志滚屏";
            this.CB_SelLog.UseVisualStyleBackColor = true;
            // 
            // LB_Log
            // 
            this.LB_Log.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LB_Log.FormattingEnabled = true;
            this.LB_Log.ItemHeight = 17;
            this.LB_Log.Location = new System.Drawing.Point(6, 24);
            this.LB_Log.Name = "LB_Log";
            this.LB_Log.Size = new System.Drawing.Size(912, 123);
            this.LB_Log.TabIndex = 0;
            // 
            // BG_SearchProduct
            // 
            this.BG_SearchProduct.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BG_SearchProduct_DoWork);
            this.BG_SearchProduct.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BG_SearchProduct_RunWorkerCompleted);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(931, 620);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormMain";
            this.Text = "电商数据抓取";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox CB_KeyWord;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BTN_Search;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.ComponentModel.BackgroundWorker BG_Search;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button BTN_OpenLogs;
        private System.Windows.Forms.Button BTN_ClearLogs;
        private System.Windows.Forms.CheckBox CB_TxtLog;
        private System.Windows.Forms.CheckBox CB_Kilo;
        private System.Windows.Forms.CheckBox CB_SelLog;
        private System.Windows.Forms.ListBox LB_Log;
        private System.Windows.Forms.TreeView TV_StoreList;
        private System.Windows.Forms.ListView LV_ProductList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.ComponentModel.BackgroundWorker BG_SearchProduct;
        private System.Windows.Forms.Button BTN_StopSearchProduct;
        private System.Windows.Forms.ComboBox CB_Type;
    }
}

