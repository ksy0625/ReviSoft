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
  /// Interaction logic for wUtenti.xaml
  /// </summary>
  public partial class wUtenti : Window
  {
    //costanti Colori
    Brush GridAlternateColorOdd = new SolidColorBrush(Color.FromArgb(126, 241, 241, 241));
    Brush GridAlternateColorEven = new SolidColorBrush(Color.FromArgb(126, 211, 211, 211));
    Brush GridHoverColor = new SolidColorBrush(Color.FromArgb(126, 245, 164, 28));
    Brush GridSelectedColor = new SolidColorBrush(Color.FromArgb(126, 130, 189, 228));

    int IndexSelected = -1;
    Grid gridSelected = null;
    Brush GridOldBackground = null;
    Brush GridSelectedBackground = null;
    public wUtenti()
    {
      InitializeComponent();
      ButtonBar.Background = App._arrBrushes[12];
      SolidColorBrush tmpBrush = (SolidColorBrush)Resources["buttonHover"];
      tmpBrush.Color = ((SolidColorBrush)App._arrBrushes[13]).Color;
      CaricaUtenti();
    }

    private void CaricaUtenti()
    {
      try
      {
        stpUTE_ID.Children.Clear();
        stpRUO_ID.Children.Clear();
        stpUTE_LOGIN.Children.Clear();
        stpRUO_DESCR.Children.Clear();
        stpUTE_PSW.Children.Clear();
        stpUTE_NOME.Children.Clear();
        stpUTE_COGNOME.Children.Clear();
        stpUTE_DESCR.Children.Clear();
        stpUTE_TIPO.Children.Clear();

        Dictionary<int, Utente> utenti = cUtenti.GetUtenti();
        if (utenti == null)
          return;

        for (int i = 0; i < utenti.Count; i++)
        {
          ColonnaTesto(stpUTE_ID, i, utenti[i].Id.ToString());
          ColonnaTesto(stpRUO_ID, i, utenti[i].RuoId.ToString());
          ColonnaTesto(stpUTE_LOGIN, i, utenti[i].Login.ToString());
          ColonnaTesto(stpRUO_DESCR, i, utenti[i].RuoDescr.ToString());
          ColonnaTesto(stpUTE_PSW, i, utenti[i].Psw);
          ColonnaTesto(stpUTE_NOME, i, utenti[i].Nome);
          ColonnaTesto(stpUTE_COGNOME, i, utenti[i].Cognome);
          ColonnaTesto(stpUTE_DESCR, i, utenti[i].Descr);
          ColonnaImmagine(stpUTE_TIPO, i, utenti[i].RuoId);
        }
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wUtenti.CaricaUtenti exception");
        App.GestioneLog(ex.Message);
      }
    }

    private void ColonnaTesto(StackPanel stp, int counter, string testo)
    {
      Border b = new Border();
      b.MinHeight = 20.0;
      b.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
      b.VerticalAlignment = System.Windows.VerticalAlignment.Center;

      if (counter < 0)
      {
        b.Background = Brushes.White;
      }
      else if (counter % 2 == 0)
      {
        b.Background = GridAlternateColorOdd;
      }
      else
      {
        b.Background = GridAlternateColorEven;
      }

      TextBlock t = new TextBlock();
      t.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
      t.VerticalAlignment = System.Windows.VerticalAlignment.Center;
      t.FontSize = 13;
      t.FontWeight = FontWeights.Regular;
      t.Foreground = Brushes.Black;
      t.Margin = new Thickness(3, 0, 0, 0);
      t.Text = testo;

      b.Child = t;

      b.MouseEnter += new MouseEventHandler(Border_MouseEnter);
      b.MouseLeave += new MouseEventHandler(Border_MouseLeave);
      b.MouseLeftButtonDown += new MouseButtonEventHandler(Border_MouseCLick);

      stp.Children.Add(b);
    }

    private void ColonnaImmagine(StackPanel stp, int counter, int ruolo)
    {
      Border b = new Border();
      b.Height = 20.0;
      b.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
      b.VerticalAlignment = System.Windows.VerticalAlignment.Center;
      if (counter < 0)
      {
        b.Background = Brushes.White;
      }
      else if (counter % 2 == 0)
      {
        b.Background = GridAlternateColorOdd;
      }
      else
      {
        b.Background = GridAlternateColorEven;
      }

      //stackpanel
      StackPanel s = new StackPanel();
      s.Orientation = Orientation.Horizontal;
      b.Child = s;

      //image
      Image i = new Image();
      Uri uriSource = null;
      if (ruolo == (int)App.RuoloDesc.StandAlone || ruolo == (int)App.RuoloDesc.Administrator)
        uriSource = new Uri("./Images/icone/Stato/ana_stato_vuoto.png", UriKind.Relative);
      else
        uriSource = new Uri("./Images/icone/Stato/ana_stato_ok.png", UriKind.Relative);
      i.Source = new BitmapImage(uriSource);
      i.Width = 16.0;
      i.Margin = new Thickness(5, 0, 0, 0);
      i.ToolTip = "Team";


      //aggiungo oggetti  
      s.Children.Add(i);

      //eventi mouse
      b.MouseEnter += new MouseEventHandler(Border_MouseEnter);
      b.MouseLeave += new MouseEventHandler(Border_MouseLeave);
      b.MouseLeftButtonDown += new MouseButtonEventHandler(Border_MouseCLick);

      stp.Children.Add(b);
    }

    private void ColonnaCheckTipo(StackPanel stp, int counter, int ruolo)
    {
      Border b = new Border();
      b.MinHeight = 20.0;
      b.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
      b.VerticalAlignment = System.Windows.VerticalAlignment.Center;

      if (counter < 0)
      {
        b.Background = Brushes.White;
      }
      else if (counter % 2 == 0)
      {
        b.Background = GridAlternateColorOdd;
      }
      else
      {
        b.Background = GridAlternateColorEven;
      }

      CheckBox ck = new CheckBox();
      ck.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
      ck.VerticalAlignment = System.Windows.VerticalAlignment.Center;
      ck.Margin = new Thickness(3, 0, 0, 0);
      if (ruolo != (int)App.RuoloDesc.StandAlone)
        ck.IsChecked = true;

      //TextBlock t = new TextBlock();
      //t.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
      //t.VerticalAlignment = System.Windows.VerticalAlignment.Center;
      //t.FontSize = 13;
      //t.FontWeight = FontWeights.Regular;
      //t.Foreground = Brushes.Black;
      //t.Margin = new Thickness(3, 0, 0, 0);
      //t.Text = testo;

      b.Child = ck;

      b.MouseEnter += new MouseEventHandler(Border_MouseEnter);
      b.MouseLeave += new MouseEventHandler(Border_MouseLeave);
      b.MouseLeftButtonDown += new MouseButtonEventHandler(Border_MouseCLick);

      stp.Children.Add(b);
    }
    //gestore eventi mouse
    private void Border_MouseEnter(object sender, MouseEventArgs e)
    {
      Border b = (Border)sender;
      int index = ((StackPanel)(b).Parent).Children.IndexOf(b);

      if (index == IndexSelected && gridSelected == ((Grid)((StackPanel)((Border)sender).Parent).Parent))
      {
        return;
      }
      GridOldBackground = b.Background;
      Grid g_ext = ((Grid)((StackPanel)((Border)sender).Parent).Parent);
      g_ext = ((Grid)(((Border)(g_ext.Parent)).Parent));
      foreach (UIElement item_ext in g_ext.Children)
      {
        if (item_ext.GetType().Name == "Border")
        {
          if (((Border)item_ext).Child.GetType().Name == "Grid")
          {
            Grid g = ((Grid)(((Border)item_ext).Child));
            foreach (UIElement item in g.Children)
            {
              if (item.GetType().Name == "StackPanel")
              {
                ((Border)(((StackPanel)item).Children[index])).Background = GridHoverColor;
              }
            }
          }
        }

      }
    }

    private void Border_MouseLeave(object sender, MouseEventArgs e)
    {
      Border b = (Border)sender;
      int index = -1;
      try
      {
        index = ((StackPanel)(b).Parent).Children.IndexOf(b);
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wUtenti.Border_MouseLeave exception");
        string log = ex.Message;
        return;
      }

      if (index == IndexSelected && gridSelected == ((Grid)((StackPanel)((Border)sender).Parent).Parent))
      {
        return;
      }
      Grid g_ext = ((Grid)((StackPanel)((Border)sender).Parent).Parent);
      g_ext = ((Grid)(((Border)(g_ext.Parent)).Parent));
      foreach (UIElement item_ext in g_ext.Children)
      {
        if (item_ext.GetType().Name == "Border")
        {
          if (((Border)item_ext).Child.GetType().Name == "Grid")
          {
            Grid g = ((Grid)(((Border)item_ext).Child));
            foreach (UIElement item in g.Children)
            {
              if (item.GetType().Name == "StackPanel")
              {
                ((Border)(((StackPanel)item).Children[index])).Background = GridOldBackground;
              }
            }
          }
        }
      }

    }

    private void Border_MouseCLick(object sender, MouseButtonEventArgs e)
    {
      bool doubleclick = false;
      if (e.ClickCount > 1)
      {
        doubleclick = true;
      }
      Border b = (Border)sender;
      int index = ((StackPanel)(b).Parent).Children.IndexOf(b);
      Grid g_ext;

      if (IndexSelected != -1 && !doubleclick)
      {
        //trovo l'attuale index reale
        IndexSelected = -1;
        foreach (UIElement item in gridSelected.Children)
        {
          if (item.GetType().Name == "StackPanel")
          {
            StackPanel s = ((StackPanel)item);
            foreach (UIElement item_int in s.Children)
            {
              if (item_int.GetType().Name == "Border")
              {
                if (((Border)item_int).Background == GridSelectedColor)
                {
                  IndexSelected = s.Children.IndexOf(item_int);
                  break;
                }
              }
            }
          }
          if (IndexSelected != -1)
          {
            break;
          }
        }
        if (IndexSelected != -1)
        {
          g_ext = gridSelected;
          g_ext = ((Grid)(((Border)(g_ext.Parent)).Parent));
          foreach (UIElement item_ext in g_ext.Children)
          {
            if (item_ext.GetType().Name == "Border")
            {
              if (((Border)item_ext).Child.GetType().Name == "Grid")
              {
                Grid g = ((Grid)(((Border)item_ext).Child));
                foreach (UIElement item in g.Children)
                {
                  if (item.GetType().Name == "StackPanel")
                  {
                    StackPanel s = ((StackPanel)item);
                    ((Border)(s.Children[IndexSelected])).Background = GridSelectedBackground;
                  }
                }
              }
            }
          }
        }
      }
      if (IndexSelected == index && gridSelected == ((Grid)((StackPanel)((Border)sender).Parent).Parent) && !doubleclick)
      {
        IndexSelected = -1;
        gridSelected = null;
        GridSelectedBackground = null;
        return;
      }

      IndexSelected = index;
      gridSelected = ((Grid)((StackPanel)((Border)sender).Parent).Parent);
      GridSelectedBackground = GridOldBackground;

      //ConfiguraInterfacciaClientiPerStato();

      g_ext = gridSelected;
      g_ext = ((Grid)(((Border)(g_ext.Parent)).Parent));
      foreach (UIElement item_ext in g_ext.Children)
      {
        if (item_ext.GetType().Name == "Border")
        {
          if (((Border)item_ext).Child.GetType().Name == "Grid")
          {
            Grid g = ((Grid)(((Border)item_ext).Child));
            foreach (UIElement item in g.Children)
            {
              if (item.GetType().Name == "StackPanel")
              {
                ((Border)(((StackPanel)item).Children[IndexSelected])).Background = GridSelectedColor;
              }
            }
          }
        }
      }
      if (doubleclick)
      {

      }
      if (IndexSelected != -1)
      {
        // se l'utente è l'amministratore non può essere cancellato
        if (Convert.ToInt16(((TextBlock)(((Border)(stpRUO_ID.Children[IndexSelected])).Child)).Text) == 1)
          btn_EliminaUtente.IsEnabled = false;
        else
          btn_EliminaUtente.IsEnabled = true;
      }
    }

    private void btn_NuovoUtente_Click(object sender, RoutedEventArgs e)
    {
      // apertura finestra utenti in inserimento
      wUpsertUtenti w = new wUpsertUtenti(null);
      w.ShowDialog();
      CaricaUtenti();
    }

    private void btn_ModificaUtente_Click(object sender, RoutedEventArgs e)
    {
      // apertura finestra utenti in modifica
      if (IndexSelected == -1)
      {
        MessageBox.Show("E' necessario selezionare l'utente da modificare", "Selezione utente", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }
      Utente utente = new Utente();
      utente.Id = Convert.ToInt32(((TextBlock)(((Border)(stpUTE_ID.Children[IndexSelected])).Child)).Text);
      utente.RuoId = Convert.ToInt16(((TextBlock)(((Border)(stpRUO_ID.Children[IndexSelected])).Child)).Text);
      utente.Login = ((TextBlock)(((Border)(stpUTE_LOGIN.Children[IndexSelected])).Child)).Text;
      utente.Nome = ((TextBlock)(((Border)(stpUTE_NOME.Children[IndexSelected])).Child)).Text;
      utente.Cognome = ((TextBlock)(((Border)(stpUTE_COGNOME.Children[IndexSelected])).Child)).Text;
      utente.Psw = ((TextBlock)(((Border)(stpUTE_PSW.Children[IndexSelected])).Child)).Text;
      utente.Descr = ((TextBlock)(((Border)(stpUTE_DESCR.Children[IndexSelected])).Child)).Text;
      utente.RuoDescr = ((TextBlock)(((Border)(stpRUO_DESCR.Children[IndexSelected])).Child)).Text;

      wUpsertUtenti w = new wUpsertUtenti(utente);
      w.ShowDialog();

      CaricaUtenti();
    }

    private void btn_EliminaUtente_Click(object sender, RoutedEventArgs e)
    {
      if (IndexSelected == -1)
      {
        MessageBox.Show("E' necessario selezionare l'utente da eliminare", "Elimina utente", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      Utente utente = new Utente();
      utente.Id = Convert.ToInt32(((TextBlock)(((Border)(stpUTE_ID.Children[IndexSelected])).Child)).Text);
      utente.RuoId = Convert.ToInt16(((TextBlock)(((Border)(stpRUO_ID.Children[IndexSelected])).Child)).Text);
      utente.Login = ((TextBlock)(((Border)(stpUTE_LOGIN.Children[IndexSelected])).Child)).Text;



      if (cUtenti.EsistonoCartelleAssociate(utente))
      {
        if (MessageBox.Show(string.Format("L'eliminazione dell'utente comporta l'eliminazione delle associazioni con le cartelle da lavorare, si conferma l'eliminazione dell'utente {0} dal sistema Revisoft?", utente.Login), "Elimina utente", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
          return;
      }
      else
      {
        if (MessageBox.Show(string.Format("Eliminare l'utente {0} dal sistema Revisoft?", utente.Login), "Elimina utente", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
          return;
      }

      cUtenti.EliminaUtente(utente);

      CaricaUtenti();
    }

    private void btn_Chiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    private void btn_AssociaUtenti_Click(object sender, RoutedEventArgs e)
    {
      // apertura pagina di associazione utenti al team leader
      wCreaTeam wCrea = new wCreaTeam();
      wCrea._teamList = cUtenti.GetUtentiTeamLeader();
      if (wCrea._teamList == null)
      {
        MessageBox.Show("Attenzione: non è possibile eseguire associazioni poichè non sono presenti utenti con ruolo Team leader", "Assenza utenze con ruolo team leader", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      wCrea.ShowDialog();
      CaricaUtenti();

    }
  }
}
