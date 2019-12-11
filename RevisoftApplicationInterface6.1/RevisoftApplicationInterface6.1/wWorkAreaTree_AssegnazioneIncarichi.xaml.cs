using RevisoftApplication.BRL;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RevisoftApplication
{
  /// <summary>
  /// Interaction logic for wWorkAreaTree_AssegnazioneIncarichi.xaml
  /// </summary>
  public partial class wWorkAreaTree_AssegnazioneIncarichi : Window
  {

    private List<Cliente> Clienti { get; set; }
    private List<UtentexCartella> Utenti { get; set; }
    private Cliente Cliente { get; set; }
    private Utente Revisore { get; set; }
    private List<Cartella> CartelleCliente { get; set; }
    private List<Cartella> CartelleAreaCliente { get; set; }
    public AreaCartella Area { get; set; }

    public wWorkAreaTree_AssegnazioneIncarichi()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      searchLabel.Visibility = Visibility.Hidden;
      searchTextBox.Visibility = Visibility.Hidden;
      //Clienti = cCliente.GetClientiByIdUtente(App.AppUtente.Id)?.Where(cliente => cliente.Stato == "0").ToList();
      Clienti = cCliente.GetClientiByIdUtente(App.AppUtente.Id, App.AppRuolo);
      cmbClienti.ItemsSource = Clienti;

      cmbClientiRiepilogo.ItemsSource = Clienti;

    }

    private void CmbClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ConfermaSalvataggio();
      Cliente = (sender as ComboBox)?.SelectedItem as Cliente;
      SetRevisore(null);
      SetArea(null);
      cmbExecutor.ItemsSource = null;
      if (Cliente == null || Cliente.ID == null)
        return;

      // Utenti = cUtenti.GetUtentiByIdCliente(Cliente.ID);  
      Utenti = cUtenti.GetUtentiXCliente(Cliente.ID);

      cmbReviewer.ItemsSource = Utenti?.Where(utente => utente.RuoId == (int)App.RuoloDesc.Reviewer);
      CartelleCliente = cCartelle.GetCartelleByCliente(Cliente.ID);
      CaricaRiepilogo(trvRiepilogo, Utenti, CartelleCliente);
      cmbAree.ItemsSource = cCartelle.GetAree().Where(a => CartelleCliente.Any(c => a.Codice == c.Codice));
    }

    private void cmbClientiRiepilogo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      Cliente cli = (sender as ComboBox)?.SelectedItem as Cliente;
      List<UtentexCartella> ute;
      List<Cartella> cartelle;

      if (cli == null || cli.ID == null)
        return;

      // Utenti = cUtenti.GetUtentiByIdCliente(Cliente.ID);  
      ute = cUtenti.GetUtentiXCliente(cli.ID);

      cartelle = cCartelle.GetCartelleByCliente(cli.ID);
      CaricaRiepilogo(trvRiepilogoRiepilogo, ute, cartelle);

    }

    private void CmbReviewer_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ConfermaSalvataggio();
      var revisore = (sender as ComboBox)?.SelectedItem as Utente;
      SetRevisore(revisore);
    }

    private void CmbExecutor_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ConfermaSalvataggio();
      var esecutore = (sender as ComboBox)?.SelectedItem as Utente;
      SetEsecutore(esecutore);
    }

    private void CmbAree_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ConfermaSalvataggio();
      var area = (sender as ComboBox)?.SelectedItem as AreaCartella;
      SetArea(area);
    }

    private void SetEsecutore(Utente esecutore)
    {
      if (esecutore == null)
        cmbExecutor.SelectedItem = null;
      Cartella.EsecutoreIdSelected = esecutore?.Id;
      Cartella.EsecutoreSelected = esecutore == null ? null : $"{esecutore.Login} - {esecutore.Nome} {esecutore.Cognome}";
    }

    private void SetRevisore(Utente revisore)
    {
      Revisore = revisore;
      SetEsecutore(null);
      if (revisore == null)
        cmbReviewer.SelectedItem = null;
      else
        cmbExecutor.ItemsSource = Utenti?.Where(utente => utente.RuoId == (short)App.RuoloDesc.Esecutore && utente.RevisoreID == Revisore.Id);
    }

    private void SetArea(AreaCartella area)
    {
      Area = area;
      if (area == null)
      {
        cmbAree.SelectedItem = null;
        CartelleAreaCliente = null;
      }
      else
        CartelleAreaCliente = CartelleCliente.Where(c => c.Codice == Area.Codice).Select(c => c.Clone()).ToList();
      trvCartelle.ItemsSource = CartelleAreaCliente;
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
      ConfermaSalvataggio();
    }

    private bool isFirstWindow_SizeChanged = true;
    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (isFirstWindow_SizeChanged)
      {
        isFirstWindow_SizeChanged = false;
        return;
      }
      var heightCartelle = trvCartelle.Height + e.NewSize.Height - e.PreviousSize.Height;
      trvCartelle.Height = heightCartelle < 0 ? 0 : heightCartelle;
      var heightRiepilog = trvRiepilogo.Height + e.NewSize.Height - e.PreviousSize.Height;
      trvRiepilogo.Height = heightRiepilog < 0 ? 0 : heightRiepilog;
    }

    private bool isChanged = false;
    private void CheckBoxChanged(object sender, RoutedEventArgs e)
    {
      isChanged = true;
    }

    private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
    {
    }

    private void ButtonChiudi_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void ButtonSalva_Click(object sender, RoutedEventArgs e)
    {
      if (isChanged)
        SalvaArea();
      isChanged = false;
      CartelleAreaCliente = CartelleCliente.Where(c => c.Codice == Area.Codice).Select(c => c.Clone()).ToList();
      trvCartelle.ItemsSource = CartelleAreaCliente;
    }

    private void ConfermaSalvataggio()
    {
      if (isChanged)
      {
        var messageBoxResult = MessageBox.Show("Vuoi salvare i cambiamenti?", "Assegnazione Incarichi", MessageBoxButton.YesNo);
        if (messageBoxResult == MessageBoxResult.Yes)
          SalvaArea();
        isChanged = false;
        CartelleAreaCliente = CartelleCliente.Where(c => c.Codice == Area.Codice).Select(c => c.Clone()).ToList();
        trvCartelle.ItemsSource = CartelleAreaCliente;
      }
    }

    private void SalvaArea()
    {
      if (!isChanged)
        return;
      var dic = new Dictionary<string, bool>();
      CartelleAreaCliente.ForEach(c => dic = dic.Concat(c.GetCodici()).ToDictionary(x => x.Key, x => x.Value));
      cCartelle.UpsertCartelleCliente(Cliente.ID, Cartella.EsecutoreIdSelected, dic);

      MessageBox.Show("Salvataggio avvenuto con successo", "Salvataggio dati", MessageBoxButton.OK, MessageBoxImage.Information);

      CartelleCliente = cCartelle.GetCartelleByCliente(Cliente.ID);
      CaricaRiepilogo(trvRiepilogo, Utenti, CartelleCliente);

      // se nel riepilogo è visualizzata la situazione per lo stesso cliente si aggiornano i dati
      Cliente cli = cmbClientiRiepilogo?.SelectedItem as Cliente;
      if (cli == null || cli.ID == null)
        return;
      if (cli.ID == Cliente.ID)
      {
        CaricaRiepilogo(trvRiepilogoRiepilogo, Utenti, CartelleCliente);
      }

    }

    private void CaricaRiepilogo(TreeView tree, List<UtentexCartella> utenti, List<Cartella> cartelle)
    {
      var revisori = utenti?.Where(utente => utente.RuoId == (short)App.RuoloDesc.Reviewer);
      if (revisori == null)
      {
        tree.ItemsSource = null;
        return;
      }
      var items = new List<RiepilogoItem>();
      foreach (var revisore in revisori)
      {
        var itemRevisore = new RiepilogoItem
        {
          Etichetta = $"{revisore.Login} - {revisore.Nome} {revisore.Cognome}",
          IsRevisore = true
        };
        var esecutori = utenti?.Where(utente => utente.RuoId == (short)App.RuoloDesc.Esecutore && utente.RevisoreID == revisore.Id);
        if (esecutori == null)
        {
          items.Add(itemRevisore);
          continue;
        }
        foreach (var esecutore in esecutori)
        {
          var itemEsecutore = new RiepilogoItem
          {
            Etichetta = $"{esecutore.Login} - {esecutore.Nome} {esecutore.Cognome}",
            IsEsecutore = true
          };
          if (cartelle == null)
          {
            itemRevisore.Figli.Add(itemEsecutore);
            continue;
          }
          foreach (var cartella in cartelle)
          {
            var cartelleByEsecutre = cartella.GetCartelleByEsecutore(esecutore.Id);
            if (cartelleByEsecutre == null)
              continue;
            var itemArea = cCartelle.CartellaToRiepilogoItem(cartelleByEsecutre);
            itemArea.IsArea = true;
            itemEsecutore.Figli.Add(itemArea);
          }
          itemRevisore.Figli.Add(itemEsecutore);
        }
        items.Add(itemRevisore);
      }
      tree.ItemsSource = items;
    }


  }

}
