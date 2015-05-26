using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

using iControlInterfaces;

namespace iControlServerApplication {
    class iControlContextMenu {

        public ContextMenuStrip Create() {
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem item;
            ToolStripMenuItem subitem;
            ToolStripSeparator sep;

            item = new ToolStripMenuItem();
            item.Text = "Plugins";

            foreach (IiControlPlugin plugin in Program.Plugins) {
                subitem = new ToolStripMenuItem();
                subitem.Text = String.Format("{0} ({1})", plugin.Name, plugin.Version);
                item.DropDownItems.Add(subitem);
	        }

            menu.Items.Add(item);

            item = new ToolStripMenuItem();
            item.Checked = Program.Autostart;
            item.Text = "Autostart";
            item.Click += new System.EventHandler(autostart_Click);
            menu.Items.Add(item);

            sep = new ToolStripSeparator();
            menu.Items.Add(sep);

            item = new ToolStripMenuItem();
            item.Text = "Exit";
            item.Click += new System.EventHandler(exit_Click);
            item.Image = Properties.Resources.exit;
            menu.Items.Add(item);

            return menu;
        }

        void exit_Click(object sender, EventArgs e) {
            Program.Exit();
        }

        void autostart_Click(object sender, EventArgs e) {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.Checked = Program.ToggleAutostart();
        }
    }
}