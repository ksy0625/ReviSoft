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
using System.Data;
using RevisoftApplication.BRL;


namespace RevisoftApplication
{
  /// <summary>
  /// Interaction logic for Login.xaml
  /// </summary>
  public partial class wUpsertUtenti : Window
  {
    private string _ruolo = string.Empty;
    public Utente _utente;
    private Utente _utente_old;
    private Dictionary<int, Ruolo> _ruoli;
    private int _indexNessunRuolo = -1;
    private List<ClientePerGriglia> _listClienti;
    public wUpsertUtenti(Utente utente)
    {
      InitializeComponent();
      label1.Foreground = App._arrBrushes[0];

      CaricaRuoli();

      if (utente == null)
      {
        _utente = new Utente();
        _utente.Id = -1;
        _utente.Cognome = "";
        _utente.Descr = "";
        _utente.Login = "";
        _utente.Nome = "";
        _utente.Psw = "";
        _utente.RuoDescr = "";
        _utente.UtePadre = -1;
        _utente.RuoId = (int)App.RuoloDesc.StandAlone;
        cmbRuolo.SelectedIndex = _indexNessunRuolo;

        rdbAlone.IsChecked = true;
        //GridClienti.Visibility = Visibility.Collapsed;
        HideGridClienti(true);
      }
      else
        ImpostaUtente(utente);

      _utente_old = new Utente();
      _utente_old.Login = _utente.Login;
      _utente_old.Nome = _utente.Nome;
      _utente_old.Cognome = _utente.Cognome;
      _utente_old.Descr = _utente.Descr;
      _utente_old.Psw = _utente.Psw;
      _utente_old.RuoId = _utente.RuoId;
      _utente_old.RuoDescr = _utente.RuoDescr;


      //_utente_old = _utente;
    }

    public void ImpostaUtente(Utente utente)
    {
      _utente = utente;
      txtUtente.Text = utente.Login;
      txtPassword.Text = utente.Psw;
      txtNome.Text = utente.Nome;
      txtCognome.Text = utente.Cognome;
      txtDescrizione.Text = utente.Descr;
      HideGridClienti(true);
      SetRuoloInWindow(_utente);
      //switch (utente.RuoId)
      //{
      //	case (int)App.RuoloDesc.Administrator:
      //		rdbAlone.Visibility = Visibility.Collapsed;
      //		rdbTeam.Visibility = Visibility.Collapsed;
      //		txtRuolo.Text = "Amministratore";
      //		break;
      //	case (int)App.RuoloDesc.StandAlone:
      //		rdbAlone.IsChecked = true;
      //		break;
      //	case (int)App.RuoloDesc.NessunRuolo:
      //	case (int)App.RuoloDesc.TeamLeader:
      //		rdbTeam.IsChecked = true;
      //		for (int i = 0; i < cmbRuolo.Items.Count; i++)
      //		{
      //			if (cmbRuolo.Items[i].ToString() == utente.RuoDescr.ToString())
      //			{
      //				cmbRuolo.SelectedIndex = i;
      //				break;
      //			}
      //		}
      //		break;
      //	case (int)App.RuoloDesc.Esecutore:
      //	case (int)App.RuoloDesc.Reviewer:
      //		rdbAlone.Visibility = Visibility.Collapsed;
      //		rdbTeam.Visibility = Visibility.Collapsed;
      //		txtRuolo.Text = $"{utente.RuoDescr}";
      //		break;
      //	case (int)App.RuoloDesc.RevisoreAutonomo:
      //		//rdbTeam.IsEnabled = false;
      //		//rdbAlone.IsEnabled = false;
      //		//txtRuolo.Text = "";
      //		//cmbRuolo.Visibility = Visibility.Collapsed;
      //		ckAutonomo.IsChecked = true;
      //		break;
      //}			         
    }

    private bool CheckModifiche()
    {
      if (_utente.Login != _utente_old.Login) return true;
      if (_utente.Nome != _utente_old.Nome) return true;
      if (_utente.Cognome != _utente_old.Cognome) return true;
      if (_utente.Descr != _utente_old.Descr) return true;
      if (_utente.Psw != _utente_old.Psw) return true;
      if (_utente.RuoId != _utente_old.RuoId) return true;

      return false;
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      if (CheckModifiche())
      {
        if (MessageBox.Show("Sono state eseguite delle modifiche, si vuole uscire senza salvare?", "Chiudi Dati Utente", MessageBoxButton.YesNo) == MessageBoxResult.No)
          return;
      }

      base.Close();
    }

    private void buttonSalva_Click(object sender, RoutedEventArgs e)
    {
      // si salvano i dati sul database
      try
      {
        if (string.IsNullOrEmpty(txtUtente.Text) || string.IsNullOrEmpty(txtPassword.Text))
        {
          MessageBox.Show("E' necessario inserire il valore dell'utente e la password", "Errore dati", MessageBoxButton.OK, MessageBoxImage.Warning);
          return;
        }

        int idUtenteDB = cUtenti.GetUtente(txtUtente.Text);
        if (idUtenteDB != -1 && idUtenteDB != _utente.Id)
        {
          MessageBox.Show("Nel sistema è già presente un utente con questo identificativo, si prega di modficare il valore utente", "Errore dati", MessageBoxButton.OK, MessageBoxImage.Warning);
          return;
        }

        if (_utente.RuoId != _utente_old.RuoId && (_utente_old.RuoId == (int)App.RuoloDesc.RevisoreAutonomo || _utente_old.RuoId == (int)App.RuoloDesc.TeamLeader && _utente.RuoId != _utente_old.RuoId))
        {
          if (MessageBox.Show("La modifica del ruolo comporta la perdita delle eventuali associazioni, continuare?", "Modifica ruolo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            return;
        }
        string listaClienti = "";
        string listaClientiDisassociati = "";
        switch (_utente.RuoId)
        {
          case (int)App.RuoloDesc.Reviewer:
          case (int)App.RuoloDesc.Esecutore:
            // se l'utente ha ruolo Revisore o Esecutore l'amministratore non può modificare il ruolo
            cUtenti.UpdateUtente(_utente);
            break;
          case (int)App.RuoloDesc.RevisoreAutonomo:

            foreach (var data in GridClienti.Items)
            {
              ClientePerGriglia cliente = data as ClientePerGriglia;
              // associazioni con l'utente selezionate nella griglia
              if (cliente.AssociatoLeader && cliente.AssociatoValue != 2 && cliente.AssociatoValue != 3)
              {
                if (!string.IsNullOrEmpty(listaClienti))
                  listaClienti += ",";
                listaClienti += cliente.ID;
              }
              if (!cliente.AssociatoLeader && cliente.AssociatoValue == 1)
              {
                // il cliente non è più associato all'utente (AssociatoLeade = false) ma lo era prima (AssociatoValue = 1)
                if (!string.IsNullOrEmpty(listaClientiDisassociati))
                  listaClientiDisassociati += ",";
                listaClientiDisassociati += cliente.ID;
              }
            }
            cUtenti.UpsertUtente(_utente, listaClienti, listaClientiDisassociati);
            break;
          default:
            cUtenti.UpsertUtente(_utente, listaClienti, listaClientiDisassociati);
            break;
        }
        // se l'utente ha ruolo Revisore o Esecutore l'amministratore non può modificare il ruolo
        //if (_utente.RuoId == (int)App.RuoloDesc.Reviewer || _utente.RuoId == (int)App.RuoloDesc.Esecutore)
        //	cUtenti.UpdateUtente(_utente);
        //else
        //cUtenti.UpsertUtente(_utente);
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wUpsertUtenti.buttonSalva_Click exception");
        App.GestioneLog(ex.Message);
      }

      base.Close();
    }

    private void cmbRuolo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      _utente.RuoId = _ruoli[cmbRuolo.SelectedIndex].Id;
    }

    private void CaricaRuoli()
    {
      //legge i ruoli dal DB e carica i dati nella combo ruoli
      try
      {
        _ruoli = cRuoli.GetRuoliPerAdministrator();
        for (int i = 0; i < _ruoli.Count; i++)
        {
          cmbRuolo.Items.Add(_ruoli[i].Descr);
          if (_ruoli[i].Id == (int)App.RuoloDesc.NessunRuolo)
            _indexNessunRuolo = i;
        }
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wUpsertUtenti.CaricaRuoli exception");
        App.GestioneLog(ex.Message);
      }
    }

    private void txtUtente_TextChanged(object sender, TextChangedEventArgs e)
    {
      _utente.Login = txtUtente.Text;
    }

    private void txtPassword_TextChanged(object sender, TextChangedEventArgs e)
    {
      _utente.Psw = txtPassword.Text;
    }

    private void txtNome_TextChanged(object sender, TextChangedEventArgs e)
    {
      _utente.Nome = txtNome.Text;
    }

    private void txtCognome_TextChanged(object sender, TextChangedEventArgs e)
    {
      _utente.Cognome = txtCognome.Text;
    }

    private void txtDescrizione_TextChanged(object sender, TextChangedEventArgs e)
    {
      _utente.Descr = txtDescrizione.Text;
    }

    private void rdbAlone_Checked(object sender, RoutedEventArgs e)
    {
      rdbTeam.IsChecked = false;
      cmbRuolo.Visibility = Visibility.Hidden;
      txtRuolo.Visibility = Visibility.Visible;
      txtRuolo.Text = App.RuoloDesc.StandAlone.ToString().ToUpper();
      _utente.RuoId = (int)App.RuoloDesc.StandAlone;

    }

    private void rdbTeam_Checked(object sender, RoutedEventArgs e)
    {
      rdbAlone.IsChecked = false;
      cmbRuolo.Visibility = Visibility.Visible;
      txtRuolo.Visibility = Visibility.Hidden;
      cmbRuolo.SelectedIndex = _indexNessunRuolo;
      _utente.RuoId = _ruoli[_indexNessunRuolo].Id;
    }

    private void ckAutonomo_Checked(object sender, RoutedEventArgs e)
    {
      rdbTeam.IsEnabled = false;
      rdbAlone.IsEnabled = false;
      rdbTeam.IsChecked = false;
      rdbAlone.IsChecked = false;
      txtRuolo.Text = "";
      cmbRuolo.Visibility = Visibility.Collapsed;
      labelRuolo.Visibility = Visibility.Hidden;

      // caricamento clienti associati all'utente
      if (_listClienti != null && _listClienti.Count > 0)
      {
        GridClienti.ItemsSource = null;
        _listClienti.Clear();
      }


      HideGridClienti(false);
      _listClienti = cCliente.GetClientiPerRevisoreAutonomo(_utente.Id);
      GridClienti.ItemsSource = _listClienti;
      _utente.RuoId = (int)App.RuoloDesc.RevisoreAutonomo;
    }

    private void ckAutonomo_Unchecked(object sender, RoutedEventArgs e)
    {
      //GridClienti.Visibility = Visibility.Hidden;
      //_utente.RuoId = (int)App.RuoloDesc.StandAlone;
      //cmbRuolo.SelectedIndex = _indexNessunRuolo;

      //rdbAlone.IsChecked = true;
      if (_utente_old.RuoId == (int)App.RuoloDesc.RevisoreAutonomo && _utente.RuoId == (int)App.RuoloDesc.RevisoreAutonomo)
      {
        _utente.RuoId = (int)App.RuoloDesc.StandAlone;
        SetRuoloInWindow(_utente);
        return;
      }
      SetRuoloInWindow(_utente_old);
      //HideGridClienti(true);
    }

    private void GridClienti_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
      var grid = sender as DataGrid;
      if (e != null && e.AddedCells != null && e.AddedCells.Count > 0 && e.AddedCells[0] != null && e.AddedCells[6] != null && e.AddedCells[6].Item != null)
      {
        ClientePerGriglia item = (ClientePerGriglia)e.AddedCells[6].Item;
        if (item.AssociatoValue == 2 || item.AssociatoValue == 3)
          e.AddedCells[0].Column.IsReadOnly = true;
        else
        {
          e.AddedCells[0].Column.IsReadOnly = false;
          //if (grid.CurrentCell.Column.Header.ToString() == "")
          //{
          //	// click sulla colonna check
          //	((ClientePerGriglia)e.AddedCells[0].Item).AssociatoValue = 1;
          //	((ClientePerGriglia)e.AddedCells[0].Item).AssociatoLeader = false;

          //}
        }

      }

    }

    private void HideGridClienti(bool hide)
    {
      if (hide)
      {
        secondaRiga.Height = new GridLength(0, GridUnitType.Star);
        terzaRiga.Height = new GridLength(0, GridUnitType.Star);

        //GridClienti.Visibility = Visibility.Collapsed;
        //lblClienti.Visibility = Visibility.Collapsed;
        //GridLength.Auto
        // GridClienti.Visibility = Visibility.Hidden;
        //lblClienti.Visibility = Visibility.Hidden;
      }

      else
      {
        //GridClienti.Visibility = Visibility.Visible;
        //lblClienti.Visibility = Visibility.Visible;
        secondaRiga.Height = new GridLength(24, GridUnitType.Star);
        terzaRiga.Height = new GridLength(248, GridUnitType.Star);
      }


      this.SizeToContent = SizeToContent.WidthAndHeight;
    }

    private void SetRuoloInWindow(Utente ute)
    {
      HideGridClienti(true);
      rdbAlone.IsEnabled = true;
      rdbTeam.IsEnabled = true;
      labelRuolo.Visibility = Visibility.Visible;

      switch (ute.RuoId)
      {
        case (int)App.RuoloDesc.Administrator:
          rdbAlone.Visibility = Visibility.Collapsed;
          rdbTeam.Visibility = Visibility.Collapsed;
          txtRuolo.Text = "Amministratore";
          ckAutonomo.Visibility = Visibility.Hidden;
          break;
        case (int)App.RuoloDesc.StandAlone:
          rdbAlone.IsChecked = true;
          break;
        case (int)App.RuoloDesc.NessunRuolo:
        case (int)App.RuoloDesc.TeamLeader:
          rdbTeam.IsChecked = true;
          cmbRuolo.Visibility = Visibility.Visible;
          txtRuolo.Visibility = Visibility.Hidden;
          for (int i = 0; i < cmbRuolo.Items.Count; i++)
          {
            if (cmbRuolo.Items[i].ToString() == ute.RuoDescr.ToString())
            {
              cmbRuolo.SelectedIndex = i;
              break;
            }
          }
          break;
        case (int)App.RuoloDesc.Esecutore:
        case (int)App.RuoloDesc.Reviewer:
          rdbAlone.Visibility = Visibility.Collapsed;
          rdbTeam.Visibility = Visibility.Collapsed;
          txtRuolo.Text = $"{ute.RuoDescr}";
          break;
        case (int)App.RuoloDesc.RevisoreAutonomo:
          rdbTeam.IsEnabled = false;
          rdbAlone.IsEnabled = false;
          rdbTeam.IsChecked = false;
          rdbAlone.IsChecked = false;
          txtRuolo.Text = "";
          cmbRuolo.Visibility = Visibility.Collapsed;
          labelRuolo.Visibility = Visibility.Hidden;
          ckAutonomo.IsChecked = true;
          break;
      }
    }

    //void OnChecked(object sender, RoutedEventArgs e)
    //{
    //	var p = "";
    //}

  }
}
