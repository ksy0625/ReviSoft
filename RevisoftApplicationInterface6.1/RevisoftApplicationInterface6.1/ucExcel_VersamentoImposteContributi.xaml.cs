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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using System.Threading;
using RevisoftApplication;
using System.Data;


namespace UserControls
{
  public partial class ucExcel_VersamentoImposteContributi : UserControl
  {
    public int id;
    private DataTable dati = null;
    private DataTable datiT = null;
    private int CurrentTabSelectedIndex = 0;



    private bool _ReadOnly = false;
    private string _lastHeader = string.Empty;
    //private bool _bSaveEnabled = true;
    private string _waitTag = string.Empty;

    public ucExcel_VersamentoImposteContributi()
    {
     
        InitializeComponent();
        try
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
            new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }
        catch (Exception)
        {

        }
    }

    public bool ReadOnly
    {
      set
      {
        _ReadOnly = value;
      }
    }

    public void LoadDataSource(string ID, string IDCliente, string IDSessione)
    {

        id = int.Parse(ID.ToString());
        cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
        cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
        string head = "";
        datiT = cBusinessObjects.GetData(id, typeof(Excel_VersamentoImposteContributi));
        foreach (DataRow dtrow in datiT.Rows)
        {
            if (dtrow["periodo"] != null)
            {
                head = dtrow["periodo"].ToString();
                break;
            }
        }

        dati = cBusinessObjects.GetDataFiltered(datiT, head, "periodo");

        ArrayList Al = new ArrayList();
        foreach (DataRow dtrow in datiT.Rows)
        {
            if (dtrow["periodo"] != null)
            {
                if (!Al.Contains(dtrow["periodo"].ToString()))
                {
                    Al.Add(dtrow["periodo"].ToString());

                }
            }
        }
    
      if (Al.Count == 0)
      {
        Al.Add("Periodo");
      }

      foreach (string item in Al)
      {
        TabItem ti = new TabItem();
        ti.Header = item;

        tabControl.Items.Add(ti);
      }

      TabItem tiout = new TabItem();
      tiout.Header = App.NewTabHeaderText;

      tabControl.Items.Add(tiout);
    }

    public int Save()
    {
        foreach (DataRow dtrow in dati.Rows)
        {
            dtrow["periodo"] = ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString();
        }
        AggiornaCampiFissi("PeriodoDiRiferimento");
        AggiornaCampiFissi("DataDiPagamento");
        AggiornaCampiFissi("AMezzo");
        AggiornaCampiFissi("ProtocolloTelematico");

        datiT = cBusinessObjects.SetDataFiltered(dati, datiT, ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString(), "periodo");
        return cBusinessObjects.SaveData(id, datiT, typeof(Excel_VersamentoImposteContributi));
     }

    private void AggiungiNodo(string Alias, string Codice)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }
      if (tabControl.SelectedItem == null)
      {
        return;
      }

      dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione, (string)((TabItem)(tabControl.SelectedItem)).Header);


    }

    private void DeleteTotal()
    {
    
        foreach (DataRow dtrow in this.dati.Rows)
        {
                    if ("Totale" == dtrow["name"].ToString())
                    {
                        dtrow.Delete();
                        break;
                    }
        }
        dati.AcceptChanges();

    }

    private void GenerateTotal()
    {
      if (this.dati.Rows.Count == 0)
            return;

      DeleteTotal();

      double importoPagato = 0.0;
      double importoCompensato = 0.0;

      if (tabControl.SelectedItem == null)
      {
        return;
      }

       foreach (DataRow dtrow in this.dati.Rows)
        {
            if ((dtrow["periodo"].ToString() != "") && (dtrow["periodo"].ToString() == (string)((TabItem)(tabControl.SelectedItem)).Header))
            {
                    if (dtrow["importoPagato"] != System.DBNull.Value)
                        importoPagato += Convert.ToDouble(dtrow["importoPagato"].ToString());
                    if (dtrow["importoCompensato"] != System.DBNull.Value)
                        importoCompensato += Convert.ToDouble(dtrow["importoCompensato"].ToString());
                }
        }
           
        dati.Rows.Add(id,cBusinessObjects.idcliente,cBusinessObjects.idsessione, (string)((TabItem)(tabControl.SelectedItem)).Header,"Totale","", importoPagato, importoCompensato);


    }

    private void DataGrid_SourceUpdated(object sender, DataTransferEventArgs e)
    {
      DataGrid grd = (DataGrid)sender;
      grd.CommitEdit(DataGridEditingUnit.Cell, true);
    }

    public T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
    {
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
      {
        var child = VisualTreeHelper.GetChild(parent, i);

        string controlName = child.GetValue(Control.NameProperty) as string;

        if (controlName == name)
        {
          return child as T;
        }

        else
        {
          T result = FindVisualChildByName<T>(child, name);

          if (result != null)
          {
            return result;
          }
        }
      }

      return null;
    }

    private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
    {
      if (e.OriginalSource.GetType() == typeof(DataGridCell))
      {
        DataGrid grd = (DataGrid)sender;
        grd.BeginEdit(e);
      }
    }

    private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        e.Cancel = true;
        return;
      }
    }

    private void dtgErroriRilevati_Loaded(object sender, RoutedEventArgs e)
    {
      GenerateTotal();
    }

    private void dtgErroriRilevati_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
      GenerateTotal();
    }

    private void AddRowErroriRilevati(object sender, RoutedEventArgs e)
    {
      AggiungiNodo("",  "");
      GenerateTotal();
    }

    private void DeleteRowErroriRilevati(object sender, RoutedEventArgs e)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }

      if (MessageBox.Show("Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        

        if (dtgVersamentoImposteContributi.SelectedCells.Count >= 1)
        {
          
        }
        else
        {
          MessageBox.Show("Selezionare una riga");
          return;
        }

        try
        {
            int k = 0;
            foreach (DataRow dtrow in this.dati.Rows)
            {
                if (k == dtgVersamentoImposteContributi.Items.IndexOf(dtgVersamentoImposteContributi.SelectedCells[0].Item))
                {
                    dtrow.Delete();
                    break;
                }

                k++;

            }
            this.dati.AcceptChanges();

            GenerateTotal();

          return;
        }
        catch (Exception ex)
        {
          string log = ex.Message;

          MessageBox.Show("Solo le righe inserite dall'utente possono essere cancellate");
        }
      }
    }

    private void obj_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }
    }

    private void obj_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }
    }

   

    private void AggiornaCampiFissi(string nomeCampo = null)
    {
      if (tabControl.SelectedItem == null || string.IsNullOrEmpty(nomeCampo))
      {
        return;
      }

      string valore = string.Empty;
     
      bool isValid;

      
      isValid = true;
      switch (nomeCampo)
      {
        case "PeriodoDiRiferimento": valore = txtPeriodoDiRiferimento.Text; break;
        case "DataDiPagamento": valore = txtDataDiPagamento.Text; break;
        case "AMezzo": valore = txtAMezzo.Text; break;
        case "ProtocolloTelematico": valore = txtProtocolloTelematico.Text; break;
        default: isValid = false; break;
      }
      if (!isValid) return;
        foreach (DataRow dtrow in this.dati.Rows)
        {
           if (dtrow["periodo"].ToString() == (((TabItem)(tabControl.SelectedItem)).Header).ToString())
           {
                   dtrow[nomeCampo] = valore;
            }
        }
          
    }

    //----------------------------------------------------------------------------+
    //                     txtPeriodoDiRiferimento_LostFocus                      |
    //----------------------------------------------------------------------------+
    private void txtPeriodoDiRiferimento_LostFocus(object sender, RoutedEventArgs e)
    {
      AggiornaCampiFissi("PeriodoDiRiferimento");
    }

    private void txtDataDiPagamento_LostFocus(object sender, RoutedEventArgs e)
    {
      AggiornaCampiFissi("DataDiPagamento");
    }

    private void txtAMezzo_LostFocus(object sender, RoutedEventArgs e)
    {
      AggiornaCampiFissi("AMezzo");
    }

    private void txtProtocolloTelematico_LostFocus(object sender, RoutedEventArgs e)
    {
      AggiornaCampiFissi("ProtocolloTelematico");
    }

    private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
       
        string head = "";
        if (e.RemovedItems.Count > 0)
        {

            head = ((TabItem)tabControl.Items[CurrentTabSelectedIndex]).Header.ToString();
            foreach (DataRow dtrow in dati.Rows)
            {
                dtrow["periodo"] = head;
                dtrow["PeriodoDiRiferimento"] = txtPeriodoDiRiferimento.Text;
                dtrow["DataDiPagamento"] = txtDataDiPagamento.Text;
                dtrow["AMezzo"] = txtAMezzo.Text;
                dtrow["ProtocolloTelematico"] = txtProtocolloTelematico.Text;
             

            }
            datiT = cBusinessObjects.SetDataFiltered(dati, datiT, head, "periodo");
        }

        if (e.AddedItems.Count > 0 && (e.AddedItems[0]).GetType().Name == "TabItem")
        {
            if (((string)(((TabItem)(e.AddedItems[0])).Header)) == App.NewTabHeaderText)
            {
                    if (_ReadOnly)
                    {
                        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                        return;
                    }

                    var dialog = new wInputBox("Inserire un periodo per la nuova Tabella");
                    dialog.ShowDialog();
                    if (!dialog.diagres)
                    {
                        return;
                    }
                    string newHeader = dialog.ResponseText;

                    if (newHeader == "")
                    {
                        MessageBox.Show("Attenzione, periodo non valido");
                        tabControl.SelectedIndex = 0;
                        return;
                    }

                    foreach (TabItem item in tabControl.Items)
                    {
                        if (((string)(item.Header)) == newHeader)
                        {
                            MessageBox.Show("Attenzione, periodo già esistente");
                            tabControl.SelectedIndex = 0;
                            return;
                        }
                    }

                    TabItem ti = new TabItem();
                    ti.Header = newHeader;



                    tabControl.Items.Insert(tabControl.Items.Count - 1, ti);
                    tabControl.SelectedIndex = tabControl.Items.Count - 2;


                }
            else
            {
               dati = cBusinessObjects.GetDataFiltered(datiT, ((TabItem)tabControl.Items[tabControl.SelectedIndex]).Header.ToString(), "periodo");

               if (dati.Rows.Count == 0)
                {
                    AggiungiNodo("", "");

                }
                foreach (DataRow dtrow in this.dati.Rows)
                {
                            txtPeriodoDiRiferimento.Text = dtrow["PeriodoDiRiferimento"].ToString();
                            txtDataDiPagamento.Text = dtrow["DataDiPagamento"].ToString();
                            txtAMezzo.Text = dtrow["AMezzo"].ToString();
                            txtProtocolloTelematico.Text = dtrow["ProtocolloTelematico"].ToString();
                        }
                Binding b = new Binding();
                b.Source = dati;
                dtgVersamentoImposteContributi.SetBinding(ItemsControl.ItemsSourceProperty, b);

            }
        }
        CurrentTabSelectedIndex = tabControl.SelectedIndex;
   }

  
    //----------------------------------------------------------------------------+
    //                            btnDeleteTable_Click                            |
    //----------------------------------------------------------------------------+
    private void btnDeleteTable_Click(object sender, RoutedEventArgs e)
    {

      string str;

      //----------------------------------- deve sempre esistere almeno una tabella
      if (tabControl.Items.Count <= 2)
      {
        str = "L'unica tabella presente non è cancellabile; " +
          "usa il comando Cancella Contenuto.";
        MessageBox.Show(str, "Attenzione");
        return;
      }
      //--------------------- componenti in sola lettura, cancellazione non ammessa
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }
      //----------------------------------------------- nessuna tabella selezionata
      if (tabControl.SelectedItem == null) return;
      //------------------------------------------ richiesta conferma cancellazione
      if (MessageBox.Show("La tabella verrà cancellata. Procedere?",
        "Attenzione", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

      

       TabItem oldSelected = ((TabItem)(tabControl.SelectedItem));
      //tabControl.SelectedIndex = (tabControl.Items.Count > 0) ? 0 : -1;
        _lastHeader = ((TabItem)tabControl.Items[0]).Header.ToString();
        dati.Clear();
        datiT = cBusinessObjects.SetDataFiltered(dati, datiT, _lastHeader, "periodo");
        tabControl.SelectedIndex = 0;
        tabControl.Items.Remove(oldSelected);
    }

   

    //----------------------------------------------------------------------------+
    //                            btnRenameTable_Click                            |
    //----------------------------------------------------------------------------+
    private void btnRenameTable_Click(object sender, RoutedEventArgs e)
    {

      string newName, oldName;

      //--------------------------------------- sola lettura, modifiche non ammesse
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }
      //----------------------------------------------- nessuna tabella selezionata
      if (tabControl.SelectedItem == null) return;
      oldName = ((TabItem)(tabControl.SelectedItem)).Header.ToString();
      //--------------------------------------- richiesta nuovo titolo da assegnare
      var dialog = new wInputBox("Inserire Titolo della nuova Tabella");
      dialog.ResponseText = ((TabItem)(tabControl.SelectedItem)).Header.ToString();
      dialog.ShowDialog();

      newName = dialog.ResponseText;
      //------------------------------------- il nuovo titolo non puo' essere vuoto
      if (newName == "")
      {
        MessageBox.Show("Attenzione, Titolo non valido");
        return;
      }
      //------------------------------------------------- verifica titolo duplicato
      foreach (TabItem item in tabControl.Items)
      {
        if ((string)(item.Header) == newName)
        {
          MessageBox.Show("Attenzione, Titolo già esistente");
          return;
        }
      }
     

     ChangeNameTab(newName, ((TabItem)(tabControl.SelectedItem)).Header.ToString());
     Binding b = new Binding();
     b.Source = dati;
     dtgVersamentoImposteContributi.SetBinding(ItemsControl.ItemsSourceProperty, b);
     ((TabItem)(tabControl.SelectedItem)).Header = newName;
      _lastHeader = newName;

    }

    private void ChangeNameTab(string newname, string oldheader)
    {
            foreach (DataRow dtrow in dati.Rows)
            {
                if (dtrow["periodo"].ToString() == oldheader)
                {
                    dtrow["periodo"] = newname;
                }
            }

            datiT = cBusinessObjects.SetDataFiltered(dati, datiT, oldheader, "periodo");
   
    }

    private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      var tabItem = e.Source as TabItem;

      if (tabItem == null)
        return;

      if (tabItem.Header.ToString() == App.NewTabHeaderText)
      {
        return;
      }

      if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
      {
        DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
      }
    }

    private void TabItem_Drop(object sender, DragEventArgs e)
    {
      if (_ReadOnly)
      {
        return;
      }

      var tabItemTarget = e.Source as TabItem;

      if (tabItemTarget.Header.ToString() == App.NewTabHeaderText)
      {
        return;
      }

      var tabItemSource = e.Data.GetData(typeof(TabItem)) as TabItem;

      if (!tabItemTarget.Equals(tabItemSource))
      {
        string sourceHeader = tabItemSource.Header.ToString();
        string targetHeader = tabItemTarget.Header.ToString();


                foreach (DataRow dtrow in dati.Rows)
                {
                    dtrow["periodo"] = "temp";
                }
                datiT = cBusinessObjects.SetDataFiltered(dati, datiT, sourceHeader, "periodo");

                DataTable dati2 = cBusinessObjects.GetDataFiltered(datiT, targetHeader, "periodo");
        if (dati2.Rows.Count > 0)
        {
          txtPeriodoDiRiferimento.Text = dati2.Rows[0]["PeriodoDiRiferimento"].ToString();
          txtDataDiPagamento.Text = dati2.Rows[0]["DataDiPagamento"].ToString();
          txtAMezzo.Text = dati2.Rows[0]["AMezzo"].ToString();
          txtProtocolloTelematico.Text = dati2.Rows[0]["ProtocolloTelematico"].ToString();
        }
                foreach (DataRow dtrow in dati.Rows)
                {
                    dtrow["periodo"] = sourceHeader;
                }
                datiT = cBusinessObjects.SetDataFiltered(dati, datiT, targetHeader, "periodo");

                datiT = cBusinessObjects.SetDataFiltered(dati2, datiT, "temp", "periodo");
                dati = cBusinessObjects.GetDataFiltered(datiT, targetHeader, "periodo");
                tabItemTarget.Header = sourceHeader;
                tabItemSource.Header = targetHeader;
                Binding b = new Binding();
                b.Source = dati;
                dtgVersamentoImposteContributi.SetBinding(ItemsControl.ItemsSourceProperty, b);
              
      }
    }

    private void tabControl_LostFocus(object sender, RoutedEventArgs e)
    {
      AggiornaCampiFissi("PeriodoDiRiferimento");
      AggiornaCampiFissi("DataDiPagamento");
      AggiornaCampiFissi("AMezzo");
      AggiornaCampiFissi("ProtocolloTelematico");
    }

   
  }
}
