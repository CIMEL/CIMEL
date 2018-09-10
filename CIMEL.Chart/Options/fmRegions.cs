using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CIMEL.Dog;

namespace CIMEL.Chart.Options
{
    public partial class fmRegions : Form
    {
        private IDictionary<string, Region> _regions=new Dictionary<string, Region>();
        // the currently selected region
        private string _currentSelected=string.Empty;
        // if true, it is editing a new region instance
        private bool _isNew=false;

        private LicenseInfo _activeLicense;
        public fmRegions()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (!ActiveChecker.Singleton.IsActive(true)) return;

            this.DialogResult=DialogResult.Cancel;
        }

        private void fmRegions_Load(object sender, EventArgs e)
        {
            // checks if the state is active
            if (!ActiveChecker.Singleton.IsActive(true)) return;

            this.LoadRegions();
        }

        private void LoadRegions()
        {
            try
            {
                // initial regions
                this._regions = RegionStore.Singleton.GetRegions().ToDictionary(r => r.Name, r => r);
            }
            catch (Exception ex)
            {
                this.ShowAlert(ex.Message, DLG_TITLE_ERROR);
            }

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

            // display the max number of regions
            this._activeLicense = Register.Singleton.CheckLicense();
            if (this._activeLicense.IsValid)
                lblMaxRegions.Text = string.Format("最大站点数：{0}", this._activeLicense.MaxRegions);
            else
                lblMaxRegions.Text = "最大站点数：未知";
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // checks if the state is active
            if (!ActiveChecker.Singleton.IsActive(true)) return;

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
                this.ShowAlert(error, DLG_TITLE_ERROR);
                return false;
            }

            if (_isNew)
            {
                if (this._regions.ContainsKey(editRegion.Name))
                {
                    this.ShowAlert(string.Format("{0}已经存在不能重复添加", editRegion.Name), DLG_TITLE_ERROR);
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
                this.ShowAlert(@"请选择要删除的站点", DLG_TITLE_ERROR);
                return false;
            }

            if (!this._regions.ContainsKey(name))
            {
                this.ShowAlert(@"数据异常，请重新刷新站点数据", DLG_TITLE_ERROR);
                return false;
            }

            this._regions.Remove(name);

            bool saved = RegionStore.Singleton.Save(this._regions.Values.OrderBy(r => r.Name).ToList());

            return saved;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // checks if the state is active
            if (!ActiveChecker.Singleton.IsActive(true)) return;

            this.LoadRegions();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            // checks if the state is active
            if (!ActiveChecker.Singleton.IsActive(true)) return;

            if (this._regions.Count >= this._activeLicense.MaxRegions)
            {
                this.ShowAlert(@"无法新建站点，已达最大站点数！", DLG_TITLE);
                return;
            }

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
            // checks if the state is active
            if (!ActiveChecker.Singleton.IsActive(true)) return;

            bool saved = this.SaveRegions();

            if (saved)
            {
                this.LoadRegions();
                this.ShowInfo(@"保存成功！", DLG_TITLE);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // checks if the state is active
            if (!ActiveChecker.Singleton.IsActive(true)) return;

            bool deleted = this.DeleteRegion(this._currentSelected);

            if (deleted)
            {
                // set the currently selected item
                this._currentSelected = string.Empty;

                this.LoadRegions();

                this.ShowInfo(@"删除成功！", DLG_TITLE);
            }
        }
    }
}
