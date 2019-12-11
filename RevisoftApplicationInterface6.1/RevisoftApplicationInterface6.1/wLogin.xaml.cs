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
using RevisoftApplication.BRL;

namespace RevisoftApplication
{
   /// <summary>
   /// Interaction logic for Login.xaml
   /// </summary>
   public partial class wLogin : Window
   {
      public Boolean  loginOk { get; set; }
		
		public wLogin()
      {
         InitializeComponent();
			txtUtente.Focus();

		}

      private void buttonLogin_Click(object sender, RoutedEventArgs e)
      {
         // effettua il login al sistema
         try
         {
            Utente ute = new Utente();
            ute.Login = txtUtente.Text;
            ute.Psw = txtPassword.Password;
            bool esito = cUtenti.EseguiLogIn(ref ute);
            if (!esito)
            {
               loginOk = false;
               // TO DO inserire messaggio in gestione messaggi
               MessageBox.Show("Le credenziali non corrispondono con quelle presenti nel sistema.\r\nL'accesso al sistema Revisoft è negato.","Accesso negato",MessageBoxButton.OK,MessageBoxImage.Stop);
					return;
            }
            else
            {               
               switch(ute.RuoId)
               {
                  case 0:
                     // se il ruolo = nessun ruolo l'utente non può accedere perchè il team leader deve ancora definire il suo ruolo all'interno del team
                     // TO DO inserire messaggio in gestione messaggi
                     loginOk = false;
                     MessageBox.Show("Il Team Leader non ha ancora associato a questa utenza un ruolo all'interno del team, il ruolo è necessario per l'impostazione del lavoro.\r\nL'accesso al sistema Revisoft è negato.", "Accesso negato", MessageBoxButton.OK, MessageBoxImage.Stop);
                     break;
                  case 1:
                     // Administrator
                     loginOk = true;
                     App.AppTipo = App.ModalitaApp.Administrator;
                     App.AppRuolo = App.RuoloDesc.Administrator;
                     break;
                  case 2:
                     // Team Leader
                     loginOk = true;
                     App.AppTipo = App.ModalitaApp.Team;
                     App.AppRuolo = App.RuoloDesc.TeamLeader;
                     break;
                  case 3:
                     //Reviewer
                     loginOk = true;
                     App.AppTipo = App.ModalitaApp.Team;
                     App.AppRuolo = App.RuoloDesc.Reviewer;
                     break;
                  case 4:
                     //esecutore
                     loginOk = true;
                     App.AppTipo = App.ModalitaApp.Team;
                     App.AppRuolo = App.RuoloDesc.Esecutore;
                     break;
                  case 5:
                     // standalone
                     loginOk = true;
                     App.AppTipo = App.ModalitaApp.StandAlone;
                     App.AppRuolo = App.RuoloDesc.StandAlone;
                     break;
						case 6:
							// revisore autonomo standalone
							loginOk = true;
							App.AppTipo = App.ModalitaApp.StandAlone;
							App.AppRuolo = App.RuoloDesc.StandAlone;
							break;
               }
					App.AppUtente = ute;
            }
            base.Close();
         }
         catch(Exception ex)
         {
                cBusinessObjects.logger.Error(ex, "wLogin.buttonLogin_Click exception");
                App.GestioneLog(ex.Message);
			}
         
      }
      private void buttonEsci_Click(object sender, RoutedEventArgs e)
      {
			// uscita dall'applicazione
         base.Close();
      }
   }
}
