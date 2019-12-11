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

    public partial class WindowGestioneLicenzaUtente : Window
    {
        public WindowGestioneLicenzaUtente ()
        {
            InitializeComponent();

            //Interfaccia
            RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
            textBoxIntestatario.Text = l.Intestatario;
            textBoxUtente.Text = l.Utente;
            textBoxTipoLicenza.Text = App.NumeroanAgrafiche.ToString();
            textBoxCodiceMacchina.Text = l.CodiceMacchina.Split('-')[0];
            textBoxScadenza.Text = l.DataScadenzaLicenza.ToShortDateString();
            progressBarLicenza.Maximum = l.DurataLicenza;
            progressBarLicenza.Value = l.GiorniUtilizzati;
            //btn rinnovo licenza
            buttonRinnova.Visibility = l.ScadenzaVicina ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
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
