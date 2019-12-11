using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using System.Collections;
using System.ComponentModel;

namespace RevisoftApplication
{
	public partial class wXBRLContestiRiferimento : Window
	{
		public ArrayList ContestiRiferimento = new ArrayList();

		public wXBRLContestiRiferimento()
		{
			InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
    }

    public void ConfiguraMaschera()
		{
			foreach (string item in ContestiRiferimento)
			{
				cmbEA.Items.Add(item);
				cmbEP.Items.Add(item);
			}			
		}

        private void buttonChiudi_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }
	}
}
