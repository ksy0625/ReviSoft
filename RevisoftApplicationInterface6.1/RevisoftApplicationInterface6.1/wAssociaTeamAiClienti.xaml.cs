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
	public partial class wAssociaTeamAiClienti : Window
	{
		private List<ClientePerGriglia> _listClienti;
		private List<ClientePerGriglia> _listClientiRiepilogo;
		public Dictionary<int, Utente> _teamList;

		public wAssociaTeamAiClienti()
		{
			InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      labelTitoloRiepilogo.Foreground = App._arrBrushes[0];
      RiepAssTeamLeader.Foreground = App._arrBrushes[0];
      if (App.AppRuolo == App.RuoloDesc.Administrator)
        CaricaRiepilogoAmministratore();
    }

    private void CaricaRiepilogoAmministratore()
    {
      var leaders = cUtenti.GetUtentiTeamLeader();
      if (leaders == null)
        return;
      var items = new List<RiepilogoItem>();
      foreach (var leader in leaders)
      {
        var itemLeader = new RiepilogoItem
        {
          Etichetta = $"{leader.Value.Login} - {leader.Value.Nome} {leader.Value.Cognome}",
          IsLeader = true
        };
        var utentiTeam = cUtenti.GetTeamFromLeader(leader.Value.Id);
        if (utentiTeam != null)
        {
          var itemTeam = new RiepilogoItem
          {
            Etichetta = string.Empty,
            IsTeam = true
          };
          foreach (var utente in utentiTeam)
          {
            var itemUtente = new RiepilogoItem
            {
              Etichetta = $"{utente.Value.Login} - {utente.Value.Nome} {utente.Value.Cognome}",
              IsLeader = utente.Value.RuoId == (short)App.RuoloDesc.TeamLeader,
              IsRevisore = utente.Value.RuoId == (short)App.RuoloDesc.Reviewer,
              IsEsecutore = utente.Value.RuoId == (short)App.RuoloDesc.Esecutore,
              IsNonAssegnato = utente.Value.RuoId == (short)App.RuoloDesc.NessunRuolo
            };
            itemTeam.Figli.Add(itemUtente);
          }
          itemLeader.Figli.Add(itemTeam);
        }
        var clienti = cCliente.GetClientiByIdUtente(leader.Value.Id, App.RuoloDesc.TeamLeader);
        if (clienti == null)
        {
          items.Add(itemLeader);
          continue;
        }
        foreach (var cliente in clienti)
        {
          var itemCliente = new RiepilogoItem
          {
            Etichetta = $"{cliente.RagioneSociale}",
            IsCliente = true
          };
          //Utenti = cUtenti.GetUtentiXCliente(cliente.ID);
          //CartelleCliente = cCartelle.GetCartelleByCliente(cliente.ID);
          //var figli = GetRiepilogo(leader.Value.Id, App.RuoloDesc.TeamLeader);
          //if (figli == null)
          //{
          //   itemLeader.Figli.Add(itemCliente);
          //   continue;
          //}
          //figli.ForEach(f => itemCliente.Figli.Add(f));
          itemLeader.Figli.Add(itemCliente);
        }
        items.Add(itemLeader);
      }
      trvRiepilogo.ItemsSource = items;
    }

    private void CaricaUtentiTeamLeader()
		{
			try
			{																		 				
				int i = 0;
				for (i = 0; i < _teamList.Count; i++)
				{
					cmbTeamLeader.Items.Add(_teamList[i].Login);
					cmbTeamLeaderRiepilogo.Items.Add(_teamList[i].Login);
				} 				
			}
			catch (Exception ex)
			{
                cBusinessObjects.logger.Error(ex, "wAssociaTeamAiClienti.CaricaUtentiTeamLeader exception");
                App.GestioneLog(ex.Message);
			}
		}

		private void cmbTeamLeader_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// se non è selezionato nessun team leader si svuota e si disabilita la griglia
			
			var combo = sender as ComboBox;
			if (_listClienti != null && _listClienti.Count > 0)
			{
				GridClienti.ItemsSource = null;
				_listClienti.Clear();
			}

			if (combo.SelectedItem.ToString() == "nessuno")
			{
				GridClienti.IsReadOnly = true;
			}
			else
			{
				GridClienti.IsReadOnly = false;
				int idTeamLeader = _teamList[combo.SelectedIndex].Id;
				_listClienti = cCliente.GetClientiPerTeam(idTeamLeader);
				GridClienti.ItemsSource = _listClienti;					
			}
			
		}

		private void cmbTeamLeaderRiepilogo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var combo = sender as ComboBox;
			if (_listClientiRiepilogo != null && _listClientiRiepilogo.Count > 0)
			{
				GridClientiRiepilogo.ItemsSource = null;
				_listClientiRiepilogo.Clear();
			}

			if (combo.SelectedItem.ToString() == "nessuno")
			{
				GridClientiRiepilogo.IsReadOnly = true;
			}
			else
			{
				GridClientiRiepilogo.IsReadOnly = false;
				int idTeamLeader = _teamList[combo.SelectedIndex].Id;
				_listClientiRiepilogo = cCliente.GetClientiPerTeamRiepilogo(idTeamLeader);
				GridClientiRiepilogo.ItemsSource = _listClientiRiepilogo;
			}
		}

		private void GridClienti_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			var grid = sender as DataGrid;
			if (e != null && e.AddedCells != null && e.AddedCells.Count > 0 && e.AddedCells[0] != null && e.AddedCells[6] != null && e.AddedCells[6].Item != null)
			{
				ClientePerGriglia item = (ClientePerGriglia)e.AddedCells[6].Item;
				if (item.AssociatoValue == 2)
					e.AddedCells[0].Column.IsReadOnly = true;
				else
					e.AddedCells[0].Column.IsReadOnly = false;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			CaricaUtentiTeamLeader();
		}

		private void btnSalva_Click(object sender, RoutedEventArgs e)
		{
			string listaClienti = "";
			string listaClientiDisassociati = "";
			foreach (var data in GridClienti.Items)
			{
				ClientePerGriglia cliente = data as ClientePerGriglia;
				if (cliente.AssociatoLeader && cliente.AssociatoValue != 2)
				{
					if (!string.IsNullOrEmpty(listaClienti))
						listaClienti += ",";
					listaClienti += cliente.ID;
				}
				if (!cliente.AssociatoLeader && cliente.AssociatoValue == 1)
				{
					// il cliente non è più associato al leader (AssociatoLeade = false) ma era associato al leader (AssociatoValue = 1)
					if (!string.IsNullOrEmpty(listaClientiDisassociati))
						listaClientiDisassociati += ",";
					listaClientiDisassociati += cliente.ID;
				}
				
			}
			
			cUtenti.UpsertClientiPerUtente(_teamList[cmbTeamLeader.SelectedIndex].Id, listaClienti, listaClientiDisassociati);

			MessageBox.Show("Salvataggio avvenuto con successo","Salvataggio dati",MessageBoxButton.OK,MessageBoxImage.Information);

		}

		private void btnChiudi_Click(object sender, RoutedEventArgs e)
		{
			// to do inserire messaggio in caso di mancato salvataggio
			base.Close(); 
		}

		private void btnChiudiRiepilogo_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}
	}



	

}
