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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RevisoftApplication
{
	/// <summary>
	/// Interaction logic for wAssociaRuoliUtenti.xaml
	/// </summary>
	public partial class wAssociaRuoliUtenti : Window
	{
		private List<Utente> _listUtenti;		 		
		public List<Cliente> _listClienti { get; set; }
		//public List<UtenteGriglia> _listRevisori { get; set; }
		public List<Utente> _listRevisori { get; set; }
		public List<UtenteGriglia> _listEsecutori { get; set; }
		private bool _modificato = false;
		private string _tabSelezionato = "";
		private int _idRevisore = -1;
		private int _idCliente = -1;

		public wAssociaRuoliUtenti()
		{
			InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      DataContext = this; //_listUtenti;

			_modificato = false;
			_idRevisore = -1;
			_idCliente = -1;

			if (!cUtenti.EsistAssociazioneEsecutoriEClienti())
				tabItemClienti.IsEnabled = false;

			CaricaDatiTeamLeader();
			CaricaClientiPerTeamLeader();
			
		}

		private void CaricaDatiTeamLeader()
		{
			try
			{
				_listUtenti = cUtenti.GetUtentiPerTeamLeaderGriglia(App.AppUtente.Id);
				RuoloColumn.ItemsSource = cRuoli.GetRuoliPerTeamLeader();
				GridImpostaRuoli.ItemsSource = _listUtenti;

				int i = 0;
				foreach (Utente utente in GridImpostaRuoli.Items)
				{
					//this.GridImpostaRuoli.SelectedIndex
					//DataGridComboBoxColumn combo = (DataGridComboBoxColumn)this.GridImpostaRuoli.Row .Columns[3];
					//ComboBox ele = this.GridImpostaRuoli.Columns[3].GetCellContent(row) as ComboBox;
					//ele.SelectedItem = utente.RuoDescr;
					
					i++;
				}
			}
			catch (Exception ex)
			{
                cBusinessObjects.logger.Error(ex, "wAssociaRuoliUtenti.CaricaDatiTeamLeader exception");
                App.GestioneLog(ex.Message);
			}
		}

		public void LoadRevisori()
		{

			Cliente item = (Cliente)cmbCliente.SelectedItem;
			if (item == null)
				return;
			if (_idCliente == -1)
				_idCliente = Convert.ToInt32(item.ID);

			_listRevisori = cUtenti.GetRevisoriPerCliente(App.AppUtente.Id,Convert.ToInt32(item.ID));
			_idRevisore = -1;
			cmbRevisori.ItemsSource = _listRevisori;

		}

		public void LoadEsecutori()
		{
			Cliente item = (Cliente)cmbCliente.SelectedItem;
			if (item == null)
				return;
			Utente itemRevisore = (Utente)cmbRevisori.SelectedItem;
			if (itemRevisore == null)
				return;
			//if (_idRevisore == -1)
				_idRevisore = itemRevisore.Id;
			_listEsecutori = cUtenti.GetEsecutoriSelezionePerClienteRevisori(App.AppUtente.Id, Convert.ToInt32(item.ID), itemRevisore.Id);
			lvEsecutori.ItemsSource = _listEsecutori;
			
		}

		private void CaricaClientiPerTeamLeader()
		{
			try
			{
				_idCliente = -1;
				_listClienti = cCliente.GetClientiByIdUtente(App.AppUtente.Id, App.AppRuolo);
			}
			catch(Exception ex)
			{
                cBusinessObjects.logger.Error(ex, "wAssociaRuoliUtenti.CaricaClientiPerTeamLeader exception");
                App.GestioneLog(ex.Message);
			}
		}

		private bool VerificaDati()
		{
			return true;
		}

		private void SalvaAssociazioniUtentiCliente()
		{
			string elencoEseAssociati = "";
			string elencoEseNonAssociati = "";
			_modificato = false;

			if (_idRevisore == -1)
				return;
			if (_idCliente == -1)
				return;
			try
			{
				//salvataggio associazioni utente esecutore - cliente
				foreach(UtenteGriglia ute in _listEsecutori)
				{
					if (ute.ReadOnly)
					{
						// fa parte del revisore
						if (ute.InTeam)
						{
							// è stato selezionato
							if (!string.IsNullOrEmpty(elencoEseAssociati))
								elencoEseAssociati += ",";
							elencoEseAssociati += ute.Id;
						}
						else
						{
							if (!string.IsNullOrEmpty(elencoEseNonAssociati))
								elencoEseNonAssociati += ",";
							elencoEseNonAssociati += ute.Id;
						}
					}
				}
				cUtenti.AssociaUtentiCliente(elencoEseAssociati, elencoEseNonAssociati, _idCliente, App.AppUtente.Id, _idRevisore);
			}
			catch(Exception ex)
			{
                cBusinessObjects.logger.Error(ex, "wAssociaRuoliUtenti.SalvaAssociazioniUtentiCliente exception");
                App.GestioneLog(ex.Message);
			}

		}

		private void SalvaAssociazioneUtentiRuoli()
		{
			try
			{ 
				// si salvano le associazioni tra gli utenti e i ruoli
				string elencoIdRevisori = "";
				string elencoIdEsecutori = "";
				string elencoIdDaAssegnare = "";
				int i = 0;
				foreach (Utente utente in GridImpostaRuoli.Items)
				{
					//this.GridImpostaRuoli.SelectedIndex
					DataGridRow row = this.GridImpostaRuoli.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
					ComboBox ele = this.GridImpostaRuoli.Columns[3].GetCellContent(row) as ComboBox;
					switch(ele.Text.ToUpper())
					{
						case "DA ASSEGNARE AL TEAM":
							if (!string.IsNullOrEmpty(elencoIdDaAssegnare))
								elencoIdDaAssegnare += ",";
							elencoIdDaAssegnare += utente.Id;
							break;
						case "REVIEWER":
							if (!string.IsNullOrEmpty(elencoIdRevisori))
								elencoIdRevisori += ",";
							elencoIdRevisori += utente.Id;
							break;
						case "ESECUTORE":
							if (!string.IsNullOrEmpty(elencoIdEsecutori))
								elencoIdEsecutori += ",";
							elencoIdEsecutori += utente.Id;
							break;
					}
					i++;
				}
 
					// TO DO catturare l'evento change della combo ruoli per chiedere conferma all'utente se si cambia il ruolo da esecutore a revisore e viceversa

					cUtenti.AssociaRuoliUtenti(elencoIdRevisori, elencoIdEsecutori, elencoIdDaAssegnare, App.AppUtente.Id);

			}
			catch (Exception ex)
			{
                cBusinessObjects.logger.Error(ex, "wAssociaRuoliUtenti.SalvaAssociazioneUtentiRuoli exception");
                App.GestioneLog(ex.Message);
			}
		}

		private void btnSalva_Click(object sender, RoutedEventArgs e)
		{
			if (_tabSelezionato == "UR")
			{
				if (VerificaDati())
				{
					SalvaAssociazioneUtentiRuoli();
					if (cUtenti.EsistAssociazioneEsecutoriEClienti())
						tabItemClienti.IsEnabled = true;
				}
			} 
			else
				SalvaAssociazioniUtentiCliente();

			MessageBox.Show("Salvataggio avvenuto con successo", "Salvataggio dati", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void btnChiudi_Click(object sender, RoutedEventArgs e)
		{
			if (_modificato)
			{
				_modificato = false;
				if (MessageBox.Show("Sono state eseguite delle modifiche senza salvare, uscendo le modifiche andranno perse.\n\rContinuare?", "Uscita senza salvare", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
					return;
			}
				
			base.Close();
		}

		private void cmbCliente_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (_modificato)
			{
				if (AskForSave())
				{
					if (e.RemovedItems != null)
						_idCliente = Convert.ToInt32(((RevisoftApplication.BRL.Cliente)((object[])e.RemovedItems)[0]).ID);
					else
						_idCliente = Convert.ToInt32(((RevisoftApplication.BRL.Cliente)((object[])e.AddedItems)[0]).ID);
				}
				SalvaAssociazioniUtentiCliente();
			}
			else
				_idCliente = Convert.ToInt32(((Cliente)cmbCliente.SelectedItem).ID);

			if (_listEsecutori != null)
			{
				_listEsecutori.Clear();
				lvEsecutori.ItemsSource = null;
			}
			
			LoadRevisori();
		}

		private void cmbRevisori_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{				
			if (_modificato)
			{
				if (AskForSave())
				{
					if (e.RemovedItems != null)
						_idRevisore = ((RevisoftApplication.BRL.Utente)((object[])e.RemovedItems)[0]).Id;
					else
						_idRevisore = ((RevisoftApplication.BRL.Utente)((object[])e.AddedItems)[0]).Id;
					SalvaAssociazioniUtentiCliente();
				} 					
			}
			//else
			//{
			//	if (cmbRevisori.SelectedItem != null)
			//		_idRevisore = ((Utente)cmbRevisori.SelectedItem).Id;
			//}
			LoadEsecutori();
		}

		private void tcAssociazione_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var item = sender as TabControl;
			var selected = item.SelectedItem as TabItem;

			if (selected.Header.ToString() == "Associazione clienti con rev./esec.")
			{
				_tabSelezionato = "CU";
				//if (!cUtenti.EsistAssociazioneUtentiRuoliEsecutori())
				//{
				//	// non si può associare nssun utente perchè non sono stati definiti i ruoli all'interno del team
				//	MessageBox.Show("Attenzione: per associare i clienti agli utenti è necessario aver formato il team associando agli utenti il proprio ruolo","Mancanza associazione con ruoli esecutori",MessageBoxButton.OK,MessageBoxImage.Warning);
				//	return;
				//}

			}
			else
			{
				_tabSelezionato = "UR";
			}
		}

		
		
		private bool AskForSave()
		{
			if (MessageBox.Show("La selezione comporta la perdita delle modifiche non salvate,\n\rsalvare le modifche prima di continuare?", "Salvataggio dati", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				return true;
			}
			return false;
		}

		private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			_modificato = true;
		}
		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			_modificato = true;
		}

		//private void GridImpostaRuoli_LoadingRow(object sender, DataGridRowEventArgs e)
		//{
		//	DataGridRow row = e.Row;
		//	//DataGridComboBoxColumn ele = this.GridImpostaRuoli.Columns[3].GetCellContent(row).SetValue( as DataGridComboBoxColumn;
		//	DataGridColumn ele = this.GridImpostaRuoli.Columns[3];


		//}
	}
}
