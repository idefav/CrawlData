using Ebend.DataSplider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 电商抓取平台
{
    public partial class FormMain : Form
    {
        private int iSearchType = 0;
        private string sSearchTypeName = "";
       public static string sCurrentDir;//当前工作目录
        public delegate void set_LB_Log(string s); //定义委托
        set_LB_Log Set_LB_Log;
        private delegate void addStore(Ebend.DataSplider.SpliderType.TypeStore TS); //定义委托
        addStore AddStore;
        private delegate void addProduct(Ebend.DataSplider.SpliderType.TypeProduct TP); //定义委托
        addProduct AddProduct;
        TreeNode TNCur;
        bool bStopFlagSearch = true;
        bool bStopSearchProduct = true;
        string sKeyWord = "";
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            sCurrentDir = Directory.GetCurrentDirectory();//获取当前根目录
            try
            {
                if (!Directory.Exists(sCurrentDir + "/Logs/"))
                {
                    Directory.CreateDirectory(sCurrentDir + "/Logs/");
                }
                if (!Directory.Exists(sCurrentDir + "/ImgTemp/"))
                {
                    Directory.CreateDirectory(sCurrentDir + "/ImgTemp/");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("创建系统缓存目录的时候，出现错误,可能会导致无法查询\r\n错误描述:" + ex.Message, "创建系统缓存目录失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
            Set_LB_Log = new set_LB_Log(Add_LogInvok);
            AddStore = new addStore(Add_Store);
            AddProduct = new addProduct(Add_Product);
            CB_Type.SelectedIndex = 0;
        }
        public void Add_LogInvok(string sStr)//添加日志
        {
            LB_Log.Items.Add(DateTime.Now.ToLongTimeString() + "  " + sStr);
            if (CB_SelLog.Checked)
            {
                LB_Log.SelectedIndex = LB_Log.Items.Count - 1;

            }
            if (CB_Kilo.Checked && LB_Log.Items.Count >= 1000)
            {
                LB_Log.Items.Clear();
            }
            if (CB_TxtLog.Checked)
            {
                try
                {
                    StreamWriter sw = new StreamWriter(sCurrentDir + "/Logs/Log_" + DateTime.Now.ToString("yy-MM-dd") + ".log", true);

                    //Write a line of text
                    sw.WriteLine(DateTime.Now.ToShortTimeString() + " " + sStr);

                    //Close the file
                    sw.Close();
                    sw.Dispose();
                }
                catch (Exception ex)
                {
                    LB_Log.Items.Add(DateTime.Now.ToLongTimeString() + "  写日志错误：" + ex.Message);
                }
            }
        }
        public void Add_Log(string sStr)
        {
            LB_Log.Invoke(Set_LB_Log, new object[] { sStr });
        }
        private void BTN_Search_Click(object sender, EventArgs e)
        {
            if (BG_Search.IsBusy)
            {
                BTN_Search.Text = "正在终止";
                BTN_Search.Enabled = false;
                bStopFlagSearch= true;
                //BG_Search.CancelAsync();
            }
            else
            {
                iSearchType = CB_Type.SelectedIndex;
                sSearchTypeName = CB_Type.Text;
                CB_KeyWord.Enabled = false;
                sKeyWord = CB_KeyWord.Text;
                 if (sKeyWord.Trim() == "")
                {
                    CB_KeyWord.Focus();
                    return;
                }
                 TNCur = TV_StoreList.Nodes.Add(sSearchTypeName + "店铺-" + sKeyWord+"["+DateTime.Now.ToString("hh:mm:ss")+"]");
                 bStopFlagSearch = false;
                BG_Search.RunWorkerAsync();
                BTN_Search.Text = "停止搜索";
            }
        }

        private void BG_Search_DoWork(object sender, DoWorkEventArgs e)
        {

            Add_Log("开始抓取" + sSearchTypeName + ":" + sKeyWord + "[第1页]");
            int iTotalPage = 0;
            TMallSplider TMS = new TMallSplider(); ;
            TaobaoSplider TBS = new TaobaoSplider();
            TMS.sCookies = "x=__ll%3D-1%26_ato%3D0; cna=97ykDz7G2GYCAXVZmbuJZpdH; otherx=e%3D1%26p%3D*%26s%3D0%26c%3D0%26f%3D0%26g%3D0%26t%3D0; uc3=nk2=AnWUh%2Bs%3D&id2=UNk1%2BrxWtNk%3D&vt3=F8dASmgq691MKE0n%2BJg%3D&lg2=UIHiLt3xD8xYTw%3D%3D; uss=UtJSnXzx31ekK4NBPk9D5ChUqQKCyM0Iw%2BMNDs7w1BQltILkn%2BmmiykAQw%3D%3D; lgc=adobo; tracknick=adobo; cookie2=3c8fda0838e9f0f989dc5c427f4239e6; t=18fbd460625667ad4fc53c42619f98bf; _tb_token_=e5e15b5e9e591; pnm_cku822=157UW5TcyMNYQwiAiwQRHhBfEF8QXtHcklnMWc%3D%7CUm5OcktwT3tFe0Z7Q3xBeiw%3D%7CU2xMHDJ7G2AHYg8hAS8RJQsrBVk4XjJVK1F%2FKX8%3D%7CVGhXd1llXGdYbFJsUWxUa1ZtWmdFeEZ9RX5AdEF8R3hAdE1yTHRaDA%3D%3D%7CVWldfS0QMAs0CioWKwslTjIXIVU4XCZLZ0kfSQ%3D%3D%7CVmhIGC0ZOQA0FCgXKxAwDDEMOQYmGiUQLQ0xDDMKKhYpHCEBNQA5bzk%3D%7CV25Tbk5zU2xMcEl1VWtTaUlwJg%3D%3D; res=scroll%3A1216*10082-client%3A1216*866-offset%3A1216*10082-screen%3A1280*1024; cq=ccp%3D1; l=AgsLWcRfWvyTxboIhs4ASAfcm6H1oh8i";

            SpliderType.TypeStore[] TS=null;
             switch(iSearchType)
            {
                case 0:
                   TS = TMS.SearchStore(sKeyWord, 0, out iTotalPage);
                    break;
                case 1:
                    TS = TBS.SearchStore(sKeyWord, 0, out iTotalPage);
                   break;
            }
            Add_Log("抓取" + sSearchTypeName + "结果:" + sKeyWord + "[条数:" + TS.Length.ToString() + "][第1/未知页]");
            for (int i = 0; i < TS.Length; i++)
            {
                TV_StoreList.Invoke(AddStore, new object[] { TS[i] });
            }
            for (int iPage = 1; iPage < iTotalPage; iPage++)
            {
                Add_Log("开始抓取" + sSearchTypeName + ":" + sKeyWord + "[第" + Convert.ToString(iPage + 1) + " / " + iTotalPage.ToString() + "页]");
                if (bStopFlagSearch)
                {
                    break;
                }
                switch (iSearchType)
                {
                    case 0:
                        TS = TMS.SearchStore(sKeyWord, iPage, out iTotalPage);
                        break;
                    case 1:
                        TS = TBS.SearchStore(sKeyWord, iPage, out iTotalPage);
                        break;
                }

                Add_Log("抓取" + sSearchTypeName + "结果:" + sKeyWord + "[条数:" + TS.Length.ToString() + "][第" + Convert.ToString(iPage + 1) + "/" + iTotalPage.ToString() + "页]");
                for (int i = 0; i < TS.Length; i++)
                {
                    TV_StoreList.Invoke(AddStore, new object[] { TS[i] });
                }
            }
     
        }

        private void Add_Store(SpliderType.TypeStore TS)
        {
            TreeNode TN = TNCur.Nodes.Add(TS.sName);
          TN.Nodes.Add(TS.sUrl);
          TN.Nodes.Add(TS.sMainProduct);
          TN.Nodes.Add(TS.sProvince);
          TN.Nodes.Add(TS.sCity);
          groupBox2.Text = "店铺信息[共" + TV_StoreList.Nodes.Count.ToString() + "条]";
        }
        private void Add_Product(SpliderType.TypeProduct TP)
        {
            ListViewItem LVI = new ListViewItem((LV_ProductList.Items.Count+1).ToString());
            LVI.SubItems.Add(TP.sTitle);
            LVI.SubItems.Add(TP.sPrice);
            LVI.SubItems.Add(TP.sPriceAVG);
            LVI.SubItems.Add(TP.sSellCount);
            LVI.SubItems.Add(TP.sItemID);
            LV_ProductList.Items.Add(LVI);
            groupBox3.Text = "商品信息[共" + LV_ProductList.Items.Count.ToString() + "条]";

        }
        private void BG_Search_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BTN_Search.Text = "搜索";
            BTN_Search.Enabled = true;
            CB_KeyWord.Enabled = true;
            Add_Log("抓取天猫:" + sKeyWord + " 完毕");
        }

        private void TV_StoreList_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
           
        }
        private void SearchProductTMall(string sUrl)
        {
            Add_Log("开始抓取店铺产品:" + sUrl);
            int iTotalPage = 0;
            TMallSplider TMS = new TMallSplider();
            TMS.sCookies = "x=__ll%3D-1%26_ato%3D0; cna=97ykDz7G2GYCAXVZmbuJZpdH; otherx=e%3D1%26p%3D*%26s%3D0%26c%3D0%26f%3D0%26g%3D0%26t%3D0; uc3=nk2=AnWUh%2Bs%3D&id2=UNk1%2BrxWtNk%3D&vt3=F8dASmgq691MKE0n%2BJg%3D&lg2=UIHiLt3xD8xYTw%3D%3D; uss=UtJSnXzx31ekK4NBPk9D5ChUqQKCyM0Iw%2BMNDs7w1BQltILkn%2BmmiykAQw%3D%3D; lgc=adobo; tracknick=adobo; t=18fbd460625667ad4fc53c42619f98bf; _tb_token_=c9WEhGkKScq4; cookie2=8eaa38810f2e441b25beaabdb799572a; pnm_cku822=119UW5TcyMNYQwiAiwQRHhBfEF8QXtHcklnMWc%3D%7CUm5OcktwT3pPcElzT3NNdyE%3D%7CU2xMHDJ7G2AHYg8hAS8RJQsrBVk4XjJVK1F%2FKX8%3D%7CVGhXd1llXGdYbVhnXmRYZFpgV2pId0t1THJLdE5zRnpCdkl3Qmw6%7CVWldfS0TMw8wDTERKQknXQ1vDSkHUQc%3D%7CVmhIGC0ZOQA0FCgXKxAwCDYKNRUpFiMePgI%2FADkZJRovEjIGMwpcCg%3D%3D%7CV25Tbk5zU2xMcEl1VWtTaUlwJg%3D%3D; res=scroll%3A1216*5312-client%3A1216*866-offset%3A1216*5312-screen%3A1280*1024; cq=ccp%3D1; l=Avb2GMNtP2-m6vcre7nFeHS4RiL4aTpR";
            string sUid = ClassRegExp.GetExpString(sUrl, "user_id=(\\d+)&");
            Add_Log("开始抓取店铺产品:" + sUrl+"[第1页]");
            SpliderType.TypeProduct[] TP = TMS.SearchProduct(sUid, 0, out iTotalPage);
            for (int i = 0; i < TP.Length; i++)
            {
                LV_ProductList.Invoke(AddProduct, new object[] { TP[i] });
            }

            for (int iPage = 1; iPage < iTotalPage; iPage++)
            {
                if (bStopSearchProduct)
                {
                    break;
                }
                Add_Log("开始抓取店铺产品:" + sUrl + "[第" + Convert.ToString(iPage + 1) + "/" + iTotalPage.ToString() + "页]");
                TP = TMS.SearchProduct(sUid, iPage, out iTotalPage);
                for (int i = 0; i < TP.Length; i++)
                {
                    LV_ProductList.Invoke(AddProduct, new object[] { TP[i] });
          
                }
            }
            Add_Log("抓取店铺产品信息完毕:" + sUrl);
          
        }
        private void SearchProductTaobao(string sUrl)
        {
            Add_Log("开始抓取店铺产品:" + sUrl);
            int iTotalPage = 0;
            TaobaoSplider TBS = new TaobaoSplider();
            TBS.InitShop(sUrl);
            TBS.sCookies = " thw=cn; ali_ab=114.221.152.238.1456796750675.0; lzstat_uv=26429925573013598357|3555462@2144678; uc3=sg2=B0T4xvKU3WRD5Hk2%2Fv%2B4Q9kggge1Dr3ExU5qweNLeH4%3D&nk2=AnWUh%2Bs%3D&id2=UNk1%2BrxWtNk%3D&vt3=F8dASmgq691MKE0n%2BJg%3D&lg2=UIHiLt3xD8xYTw%3D%3D; uss=UtJSnXzx31ekK4NBPk9D5ChUqQKCyM0Iw%2BMNDs7w1BQltILkn%2BmmiykAQw%3D%3D; lgc=adobo; tracknick=adobo; _cc_=VT5L2FSpdA%3D%3D; tg=0; cna=97ykDz7G2GYCAXVZmbuJZpdH; x=e%3D1%26p%3D*%26s%3D0%26c%3D0%26f%3D0%26g%3D0%26t%3D0%26__ll%3D-1; mt=ci=-1_0; v=0; cookie2=3c8fda0838e9f0f989dc5c427f4239e6; t=18fbd460625667ad4fc53c42619f98bf; _tb_token_=e5e15b5e9e591; l=ApWVxAvyiABLxfSmbBwWjTf4pZ9P0Uml; uc1=cookie14=UoWxM%2FY4orO16g%3D%3D; pnm_cku822=251UW5TcyMNYQwiAiwQRHhBfEF8QXtHcklnMWc%3D%7CUm5OcktwT3pOcEp%2BSnJKdiA%3D%7CU2xMHDJ7G2AHYg8hAS8RJQsrBVk4XjJVK1F%2FKX8%3D%7CVGhXd1llXGdYbVlnXWldZV1hVmtJdk50SnROck57QX9Ddkp2TXRaDA%3D%3D%7CVWldfS0QMA43Di4QMB4iHjsVQxU%3D%7CVmhIGC0ZOQA0FCgXKRc3CD0EJBgnEi8PMw4xCCgUKx4jAzcCO207%7CV25Tbk5zU2xMcEl1VWtTaUlwJg%3D%3D";
            Add_Log("开始抓取店铺产品:" + sUrl + "[第1页]");
            SpliderType.TypeProduct[] TP = TBS.SearchProduct(sUrl, 0, out iTotalPage);
            for (int i = 0; i < TP.Length; i++)
            {
                LV_ProductList.Invoke(AddProduct, new object[] { TP[i] });
            }

            for (int iPage = 1; iPage < iTotalPage; iPage++)
            {
                if (bStopSearchProduct)
                {
                    break;
                }
                Add_Log("开始抓取店铺产品:" + sUrl + "[第" + Convert.ToString(iPage + 1) + "/" + iTotalPage.ToString() + "页]");
                TP = TBS.SearchProduct(sUrl, iPage, out iTotalPage);
                for (int i = 0; i < TP.Length; i++)
                {
                    LV_ProductList.Invoke(AddProduct, new object[] { TP[i] });

                }
            }
            Add_Log("抓取店铺产品信息完毕:" + sUrl);

        }
        private void BG_SearchProduct_DoWork(object sender, DoWorkEventArgs e)
        {
            string sUrl = (string)e.Argument;
            if (sUrl.StartsWith("search_shopitem.htm"))
            { SearchProductTMall(sUrl); }
            else
            {
                SearchProductTaobao(sUrl);

            }
            
        }

        private void BG_SearchProduct_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TV_StoreList.Enabled = true;
            BTN_StopSearchProduct.Visible = false;
        }

        private void BTN_StopSearchProduct_Click(object sender, EventArgs e)
        {
            bStopSearchProduct = true;
            BTN_StopSearchProduct.Enabled = false;
        }

        private void TV_StoreList_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (BG_SearchProduct.IsBusy)
            {
                return;
            }
            try
            {
                    string sUrl = e.Node.Text;
                    if (sUrl.StartsWith("search_shopitem.htm")||sUrl.IndexOf(".taobao.com")>0)
                    {
                        bStopSearchProduct = false;
                        BTN_StopSearchProduct.Visible = true;
                        TV_StoreList.Enabled = false;
                        LV_ProductList.Items.Clear();
                        BG_SearchProduct.RunWorkerAsync(sUrl);

                    } 
               
            }
            catch
            {

            }
        }
    }
}
