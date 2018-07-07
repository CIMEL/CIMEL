using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CIMEL.Installer
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            // initial the required params dirs
            string[] dirs = new string[] {
                "c:\\AYFY\\大气气溶胶光学参数处理软件\\ParamsData\\data",
                "c:\\AYFY\\大气气溶胶光学参数处理软件\\ParamsData\\modis_brdf",
                "c:\\AYFY\\大气气溶胶光学参数处理软件\\ParamsData\\input",
                "c:\\AYFY\\大气气溶胶光学参数处理软件\\ParamsData\\output",
                "c:\\AYFY\\大气气溶胶光学参数处理软件\\ParamsData\\chartset",
            };
            Parallel.ForEach(dirs, d =>
            {
                if (!Directory.Exists(d))
                    Directory.CreateDirectory(d);
            });
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }

    }
}
