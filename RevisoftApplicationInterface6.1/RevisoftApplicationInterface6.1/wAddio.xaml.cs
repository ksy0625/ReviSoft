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
using UserControls;

namespace RevisoftApplication
{
	public partial class wAddio : Window
	{   
        public wAddio()
		{
			InitializeComponent();

            //interfaccia
            checkBoxBackupRevisoft.IsChecked = App.AppSetupAddioBackupRevisoft;
            checkBoxBackupUtente.IsChecked = App.AppSetupAddioBackupUtente;
        }

        //4.6 tipologia backup
        private static bool _ChiudiRevisoft;

        public bool ChiudiRevisoft
        {
            get { return (bool)_ChiudiRevisoft; }
        }

        private void AddioWindow_Closed(object sender, CancelEventArgs e)
        {

        }

        private void checkBoxBackupRevisoft_Check(object sender, RoutedEventArgs e)
        {
            App.AppSetupAddioBackupRevisoft = (bool)checkBoxBackupRevisoft.IsChecked;

            //salvo nuova configurazione
            GestioneLicenza l = new GestioneLicenza();
            l.SalvaInfoDataUltimoUtilizzo();
        }

        private void checkBoxBackupUtente_Check(object sender, RoutedEventArgs e)
        {

            //richiamo maschera impostazioni per configurazione
            if ((bool)checkBoxBackupUtente.IsChecked)
            {
                if (!App.AppSetupBackupPersonalizzato)
                {
                    wConfigurazione c = new wConfigurazione();
                    c.ShowDialog();
                }

                checkBoxBackupUtente.IsChecked = App.AppSetupBackupPersonalizzato;
                App.AppSetupAddioBackupUtente = App.AppSetupBackupPersonalizzato;
            }
            else
            {
                App.AppSetupAddioBackupUtente = false;
            }

            //salvo nuova configurazione
            GestioneLicenza l = new GestioneLicenza();
            l.SalvaInfoDataUltimoUtilizzo();
        }

        //esco con backup
        private void buttonBackupExit_Click(object sender, RoutedEventArgs e)
        {
            

            BackUpFile bkf = new BackUpFile();
            Hashtable ht = new Hashtable();

            //Backup Revisoft
            if (App.AppSetupAddioBackupRevisoft)
            {
                bkf.BackupPersonalizzato = false;
                bkf.SetBackUp(ht, -1);
            }

            //Backup Personalizzato
            if (App.AppSetupBackupPersonalizzato)
            {
                bkf.BackupPersonalizzato = true;
                bkf.SetBackUp(ht, -1);
            }

         

            //esco
            _ChiudiRevisoft = true;
            base.Close();
        }

        //esco senza backup
        private void buttonNoBackupExit_Click(object sender, RoutedEventArgs e)
        {
            //esco
            _ChiudiRevisoft = true;
            base.Close();
        }

        //annullo
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            //non esco
            _ChiudiRevisoft = false;
            base.Close();
        }


    }
}
