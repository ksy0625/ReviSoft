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

    public partial class WindowGestioneLicenza : Window
    {
        public WindowGestioneLicenza()
        {
            InitializeComponent();

            //Interfaccia
            RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
            //licenza prova disponibile
            buttonProva.IsEnabled = l.LicenzaProvaDisponibile;
            //licenza ordinaria disponibile
           // buttonAcquisto.IsEnabled = l.LicenzaDisponibile;
            //codice macchina
            //tb_CodiceMacchina.Text = App.CodiceMacchina.Split('-')[0];
            tb_CodiceMacchina.Text = App.CodiceMacchina;
        }

        private void buttonChiudi_Click_1(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void buttonProva_Click(object sender, RoutedEventArgs e)
        {
            //Richiesta conferma
            if (MessageBox.Show("Confermi la creazione di una licenza di prova di 10 giorni?", "Licenza di prova", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //attivo licenza prova
                RevisoftApplication.GestioneLicenza l = new GestioneLicenza();
                l.AttivaLicenzaProva();
            }

            //esco e attivo programma
            base.Close();
        }

        private void buttonAcquisto_Click(object sender, RoutedEventArgs e)
        {
            //copio codice macchina in clipboard
            Clipboard.Clear();
            //Clipboard.SetText(App.CodiceMacchina.Split('-')[0]);
            Clipboard.SetText(App.CodiceMacchina);
            //rimando alla pagina di attivazione, l'utente rivecerà via mail chiave, con doppio clic la attiverà
            //System.Diagnostics.Process.Start(RevisoftApplication.Properties.Settings.Default["RevisoftUrlAttivazione"].ToString());
            RevisoftApplication.GestioneLicenza l = new GestioneLicenza();

            if(l.StatoLicenza && !App.Prova)
            {
                List<string> descLicenza = new List<string>();
                if (App.Server)
                {
                    descLicenza.Add("Server");
                }
                else if(App.Client)
                {
                    descLicenza.Add("Client");
                }
                else if (App.RemoteDesktop)
                {
                    descLicenza.Add("Remote Desktop");
                }
                else if (App.Cloud)
                {
                    descLicenza.Add("Cloud");
                }
                else if (App.Multilicenza)
                {
                    descLicenza.Add("Multilicenza");
                }
                else if (App.Sigillo)
                {
                    descLicenza.Add("Sigillo");
                }
                else if (App.Guest)
                {
                    descLicenza.Add("Guest");
                }

                System.Diagnostics.Process.Start("mailto:assistenza@revisoft.it?subject=" + l.Utente + ",%20" + l.Intestatario + "&body=" + App.CodiceMacchina + "%20" + String.Join(",", descLicenza.ToArray()));
                //HttpUtility.HtmlAttributeEncode(memoEdit1.Text)
            }
            else
            {
                System.Diagnostics.Process.Start("mailto:assistenza@revisoft.it?subject=Attivazione%20Revisoft" + "&body=" + App.CodiceMacchina + "%20Si%20prega%20di%20indicare%20i%20nomi%20degli%20utilizzatori%20del%20software");
                
            }

            
    
        }

    }
}
