using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aeronet.Dog;

namespace Aeronet.Chart.Options
{
    public partial class fmRegions : Form
    {
        private IDictionary<string, Region> _regions;
        // the currently selected region
        private string _currentSelected=string.Empty;
        // if true, it is editing a new region instance
        private bool _isNew=false;
        public fmRegions()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!AeronetDog.Default.IsAlive(true)) return;
            
            this.DialogResult=DialogResult.Cancel;
        }

        private void fmRegions_Load(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!AeronetDog.Default.IsAlive(true)) return;

            this.LoadRegions();
        }

        private void LoadRegions()
        {

            // initial regions
            this._regions = RegionStore.Singleton.GetRegions().ToDictionary(r => r.Name, r => r);

            // initial list view
            listView1.Items.Clear();
            foreach (var name in _regions.Keys)
            {
                var region = _regions[name];
                ListViewItem item=new ListViewItem(region.Name);
                item.Name = name;
                item.SubItems.Add(region.Lat.ToString());// 纬度
                item.SubItems.Add(region.Lon.ToString());// 经度
                listView1.Items.Add(item);
            }

            // reset editing state
            this._isNew = false;

            // reset property grid
            this.propertyGrid1.SelectedObject = null;

            // initial selected item
            if (listView1.Items.Count > 0)
            {
                if (!string.IsNullOrEmpty(this._currentSelected) && this._regions.ContainsKey(this._currentSelected))
                    listView1.Items[_currentSelected].Selected = true;
                else
                    listView1.Items[0].Selected = true;

            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!AeronetDog.Default.IsAlive(true)) return;

            var items = this.listView1.SelectedItems;
            if(items.Count==0) return;

            // always single-select
            var selectedItem = items[0];
            string name = selectedItem.Name;
            // pick region object
            if (!this._regions.ContainsKey(name)) return;

            var region = this._regions[name];
            // initial property grid
            this.propertyGrid1.SelectedObject = region;

            // set current selected
            this._currentSelected = name;

            // reset editing state
            this._isNew = false;
        }

        private bool SaveRegions()
        {
            var editRegion = this.propertyGrid1.SelectedObject as Region;
            string error;
            if (!ValidateRegion(editRegion, out error))
            {
                MessageBox.Show(error, DLG_TITLE_ERROR, MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (_isNew)
            {

                if (this._regions.ContainsKey(editRegion.Name))
                {
                    MessageBox.Show(string.Format("{0}已经存在不能重复添加", editRegion.Name), DLG_TITLE_ERROR, MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    this._regions.Add(editRegion.Name, editRegion);
                }
            }

            // set the currently selected item
            this._currentSelected = editRegion.Name;

            bool saved = RegionStore.Singleton.Save(this._regions.Values.OrderBy(r=>r.Name).ToList());

            return saved;
        }

        private bool DeleteRegion(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show(@"请选择要删除的站台", DLG_TITLE_ERROR, MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            if (!this._regions.ContainsKey(name))
            {
                MessageBox.Show(@"数据异常，请重新刷新站台数据", DLG_TITLE_ERROR, MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            this._regions.Remove(name);

            bool saved = RegionStore.Singleton.Save(this._regions.Values.OrderBy(r => r.Name).ToList());

            return saved;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!AeronetDog.Default.IsAlive(true)) return;

            this.LoadRegions();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!AeronetDog.Default.IsAlive(true)) return;

            // reset selected item
            this._currentSelected = string.Empty;
            // change editing state
            this._isNew = true;
            // initial property grid
            this.propertyGrid1.SelectedObject=new Region();
        }

        private bool ValidateRegion(Region region,out string error)
        {
            if (region == null)
            {
                error = "数据异常，请点击 [新建] 重新填写站点信息";
                return false;
            }
            if (string.IsNullOrEmpty(region.Name))
            {
                error = "请填写名称";
                return false;
            }
            if (region.Lat <= 0)
            {
                error = "请填写有效的纬度";
                return false;
            }
            if (region.Lon <= 0)
            {
                error = "请填写有效的经度";
                return false;
            }
            error = string.Empty;
            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!AeronetDog.Default.IsAlive(true)) return;

            bool saved=this.SaveRegions();

            if (saved)
            {
                this.LoadRegions();
                MessageBox.Show(@"保存成功！", DLG_TITLE, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // checks if the super dog is still working
            if (!AeronetDog.Default.IsAlive(true)) return;
            
            bool deleted = this.DeleteRegion(this._currentSelected);

            if (deleted)
            {
                // set the currently selected item
                this._currentSelected = string.Empty;

                this.LoadRegions();

                MessageBox.Show(@"删除成功！", DLG_TITLE, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
    }
}
