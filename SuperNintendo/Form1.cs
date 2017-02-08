using System;
using System.Windows.Forms;
using SuperNintendo.Core;

namespace SuperNintendo
{
    /// <summary>
    /// main form
    /// </summary>
    public partial class Form1 : Form
    {
        #region Constructors

        /// <summary>
        /// default constructor
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// open clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Loader.Load(openFileDialog1.OpenFile());
            }
        }

        #endregion
    }
}
