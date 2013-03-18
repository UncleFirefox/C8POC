using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace C8POC.WinFormsUI
{
    using System.IO;
    using System.Reflection;

    using C8POC.Interfaces;

    public partial class PluginSettings : Form
    {
        public PluginSettings()
        {
            InitializeComponent();
            LoadPluginAssemblies();
        }

        private void LoadPluginAssemblies()
        {
            var files = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "Plugins"), "*.dll");
            var assemblies = files.Select(Assembly.LoadFile);

            var assemblyFiltered = from asm in assemblies
                                    from type in asm.GetTypes()
                                    from iface in type.GetInterfaces()
                                    where iface.IsAssignableFrom(typeof(ISoundPlugin)) &&
                                          iface.FullName == typeof (ISoundPlugin).FullName
                                    //select new {AsmName = asm.FullName, ObjectType = type};
                                    select asm;

            var assemblylist = assemblyFiltered.ToList();
            
            var assemblynameslist =
                assemblyFiltered.Select(
                    x =>
                    ((AssemblyTitleAttribute)
                     x.GetCustomAttributes(typeof (AssemblyTitleAttribute), false).FirstOrDefault()).Title);
        }
    }
}
