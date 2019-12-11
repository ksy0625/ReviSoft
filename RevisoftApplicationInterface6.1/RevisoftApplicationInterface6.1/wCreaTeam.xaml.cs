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
	/// Interaction logic for wCreaTeam.xaml
	/// </summary>
	public partial class wCreaTeam : Window
	{
		private List<UtenteGriglia> _listUtenti;
		public Dictionary<int, Utente> _teamList;

		public wCreaTeam()
		{
			InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
    }

    private void CaricaUtentiTeamLeader()
		{
			try
			{																		 				
				int i = 0;
				for (i = 0; i < _teamList.Count; i++)
				{
					cmbTeamLeader.Items.Add(_teamList[i].Login);
				} 				
			}
			catch (Exception ex)
			{
                cBusinessObjects.logger.Error(ex, "wCreaTeam.CaricaUtentiTeamLeader exception");
                App.GestioneLog(ex.Message);
			}
		}

		private void cmbTeamLeader_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{						
			// se non è selezionato nessun team leader si svuota e si disabilita la griglia
			var combo = sender as ComboBox;
			if (_listUtenti != null && _listUtenti.Count > 0)
			{
				GridUtenti.ItemsSource = null;
				_listUtenti.Clear();
			}

			if (combo.SelectedItem.ToString() == "nessuno")
			{
				GridUtenti.IsReadOnly = true;
			}
			else
			{
				GridUtenti.IsReadOnly = false;
				int idTeamLeader = _teamList[combo.SelectedIndex].Id;
				_listUtenti = cUtenti.GetUtentiGrigliaTeam(idTeamLeader);
				GridUtenti.ItemsSource = _listUtenti;					
			} 			
		}

		private void GridUtenti_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{	  		
			var grid = sender as DataGrid;
			if (e != null && e.AddedCells != null && e.AddedCells.Count > 0 && e.AddedCells[0] != null && e.AddedCells[6] != null && e.AddedCells[6].Item != null)
			{
				UtenteGriglia item = (UtenteGriglia)e.AddedCells[6].Item;
				if (item.ReadOnly)
				{ 
					//if (e != null && e.AddedCells[0] != null)
					//{
						//grid.BeginEdit();
						e.AddedCells[0].Column.IsReadOnly = true;
					//grid.CommitEdit();
					//}

				}
				else
					e.AddedCells[0].Column.IsReadOnly = false;
			} 			
		}

		private void btnSalva_Click(object sender, RoutedEventArgs e)
		{
			// si salvano i dati del team
			// se il team non esiste si crea altrimenti si aggiorna

			string listaUtenti = "";
			foreach (var data in GridUtenti.Items)
			{
				UtenteGriglia utente = data as UtenteGriglia;
				if (utente.InTeam && !utente.ReadOnly)
				{
					if (!string.IsNullOrEmpty(listaUtenti))
						listaUtenti += ",";
					listaUtenti += utente.Id;
				}
			} 
			cUtenti.UpsertTeam(_teamList[cmbTeamLeader.SelectedIndex].Id, listaUtenti);

			MessageBox.Show("Salvataggio avvenuto con successo","Salvataggio dati",MessageBoxButton.OK,MessageBoxImage.Information);
			//base.Close();
		}

		private void btnChiudi_Click(object sender, RoutedEventArgs e)
		{
			// to do inserire messaggio in caso di mancato salvataggio
			base.Close(); 
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			CaricaUtentiTeamLeader();
		}
	}



	

}
