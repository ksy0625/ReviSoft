using RevisoftApplication.BRL;
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
  /// <summary>
  /// Interaction logic for wRiepilogoUtenti.xaml
  /// </summary>
  public partial class wRiepilogoUtenti : Window
  {

    private List<Cliente> Clienti { get; set; }
    private List<UtentexCartella> Utenti { get; set; }
    private List<Cartella> CartelleCliente { get; set; }


    public wRiepilogoUtenti()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      if (App.AppRuolo == App.RuoloDesc.Administrator)
      {
        gridClienti.Visibility = Visibility.Hidden;
        CaricaRiepilogoAmministratore();
        return;
      }
      Clienti = cCliente.GetClientiByIdUtente(App.AppUtente.Id, App.AppRuolo);
      cmbClienti.ItemsSource = Clienti;
    }

    private void CmbClienti_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      Utenti = null;
      CartelleCliente = null;
      if (!((sender as ComboBox)?.SelectedItem is Cliente cliente) || cliente.ID == null)
        return;
      Utenti = cUtenti.GetUtentiXCliente(cliente.ID);
      CartelleCliente = cCartelle.GetCartelleByCliente(cliente.ID);
      CaricaRiepilogo(App.AppUtente.Id, App.AppRuolo);
    }

    private void CaricaRiepilogo(int id, App.RuoloDesc ruolo)
    {
      var items = GetRiepilogo(id, ruolo);
      if (items != null)
        trvRiepilogo.ItemsSource = items;
    }

    private List<RiepilogoItem> GetRiepilogo(int id, App.RuoloDesc ruolo)
    {
      IEnumerable<UtentexCartella> revisori = null;
      if (ruolo == App.RuoloDesc.TeamLeader)
        revisori = Utenti?.Where(utente => utente.RuoId == (short)App.RuoloDesc.Reviewer);
      else if (ruolo == App.RuoloDesc.Reviewer)
        revisori = Utenti?.Where(utente => utente.RuoId == (short)App.RuoloDesc.Reviewer && utente.Id == id);
      else if (ruolo == App.RuoloDesc.Esecutore)
      {
        var idRevisori = Utenti?.Where(utente => utente.RuoId == (short)App.RuoloDesc.Esecutore && utente.Id == id)?.Select(utente => utente.RevisoreID);
        if (idRevisori != null && idRevisori.Count() > 0)
          revisori = Utenti?.Where(utente => utente.RuoId == (short)App.RuoloDesc.Reviewer && idRevisori.Contains(utente.Id));
      }
      if (revisori == null)
      {
        trvRiepilogo.ItemsSource = null;
        return null;
      }
      var items = new List<RiepilogoItem>();
      foreach (var revisore in revisori)
      {
        var itemRevisore = new RiepilogoItem
        {
          Etichetta = $"{revisore.Login} - {revisore.Nome} {revisore.Cognome}",
          IsRevisore = true
        };
        IEnumerable<UtentexCartella> esecutori = null;
        if (ruolo == App.RuoloDesc.Esecutore)
          esecutori = Utenti?.Where(utente => utente.RuoId == (short)App.RuoloDesc.Esecutore && utente.RevisoreID == revisore.Id && utente.Id == id);
        else
          esecutori = Utenti?.Where(utente => utente.RuoId == (short)App.RuoloDesc.Esecutore && utente.RevisoreID == revisore.Id);
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
          if (CartelleCliente == null)
          {
            itemRevisore.Figli.Add(itemEsecutore);
            continue;
          }
          foreach (var cartella in CartelleCliente)
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
      return items;
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

  }

}
