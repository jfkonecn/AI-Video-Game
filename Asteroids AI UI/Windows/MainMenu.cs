using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AsteroidsHandler;

namespace AsteroidsAIUI.Windows
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
            _Game = new Game(ref mainPbox, false);
        }

        private void addShipBtn_Click(object sender, EventArgs e)
        {

        }

        private void frameTimer_Tick(object sender, EventArgs e)
        {
            mainPbox.Invalidate();
        }

        private Game _Game;

        private void mainPbox_Paint(object sender, PaintEventArgs e)
        {
            _Game.draw(e);
        }
    }
}
