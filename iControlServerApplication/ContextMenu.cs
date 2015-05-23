using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace iControlServerApplication {
    class iControlContextMenu {

        public ContextMenuStrip Create() {
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem item;

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
    }
}