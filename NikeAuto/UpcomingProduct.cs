using System;
using System.Windows.Forms;
using NikeAuto.Engine;

namespace NikeAuto
{
    public partial class UpcomingProduct : UserControl
    {
        public int productId;
        private static MainForm _MainForm;

        public UpcomingProduct(MainForm mainForm)
        {
            if(mainForm!=null)
            {
                _MainForm = mainForm;
            }            
            InitializeComponent();
        }


        private void UpcomingProductImage_Click(object sender, EventArgs e)
        {
            NikeCore nc = new NikeCore(null);
            NikeCore.upcomingProductId = productId;
            var productList = nc.LoadProductSKU(productId);
            if(productList.Count>0)
            {
                _MainForm.SKUComboBox.Items.Clear();
                _MainForm.SKUComboBox.Items.Add(productList[0].SKU.ToString());

                _MainForm.SKUComboBox.SelectedIndex = 0;
            }
            ////var productList = NikeCore.LoadProductSKU.LoadProduct(upcomingDate);
        }
    }
}
