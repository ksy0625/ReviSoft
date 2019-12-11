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

namespace RevisoftApplication
{

    public partial class WindowGestioneLicenzaSigilloUtente : Window
    {
        public WindowGestioneLicenzaSigilloUtente()
        {
            InitializeComponent();

            //Interfaccia
            //RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
            //textBoxIntestatario.Text = l.IntestatarioSigillo;
            //textBoxUtente.Text = l.UtenteSigillo;
            //textBoxTipoLicenza.Text = l.NomeLicenza(App.TipologieLicenze.Sigillo);
            //textBoxCodiceMacchina.Text = l.CodiceMacchinaSigillo;
            //textBoxScadenza.Text = l.DataScadenzaLicenzaSigillo.ToShortDateString();
            //progressBarLicenza.Maximum = l.DurataLicenzaSigillo;
            //progressBarLicenza.Value = l.GiorniUtilizzatiSigillo;
            ////btn rinnovo licenza
            //buttonRinnova.Visibility = l.ScadenzaSigilloVicina ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        private void buttonChiudi_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void buttonRinnovo_Click(object sender, RoutedEventArgs e)
        {
            //Apertura maschera
            WindowGestioneLicenza w1 = new WindowGestioneLicenza();
            w1.ShowDialog();
        }

    }
}
