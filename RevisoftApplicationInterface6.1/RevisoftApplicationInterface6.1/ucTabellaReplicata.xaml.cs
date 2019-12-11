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
using System.Xml;
using RevisoftApplication;
using System.Collections;
using System.Threading;
using System.Data;


namespace UserControls
{
  public partial class ucTabellaReplicata : UserControl
  {
    public int id;
    private DataTable dati= null;

    private XmlDataProviderManager _x;
    private string _ID;
    private string _IDTree;
    private bool _ReadOnly = true;
    private Dictionary<string, string> _NewTabModifiedNames = new Dictionary<string, string>();

    //----------------------------------------------------------------------------+
    //                             ucTabellaReplicata                             |
    //----------------------------------------------------------------------------+
    public ucTabellaReplicata()
    {
      InitializeComponent();
    
    }

    public bool ReadOnly
    {
      set
      {
        _ReadOnly = value;
      }
    }

    //----------------------------------------------------------------------------+
    //                                    Load                                    |
    //----------------------------------------------------------------------------+
    public void Load(string ID, string tab, string IDTree,string IDCliente,string IDSessione)
    {
            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());

            _ID = ID;


       _IDTree = IDTree;
       TabItem ti;
       ArrayList Tabs = new ArrayList();
       dati = cBusinessObjects.GetData(id, typeof(Tabella));
       if (dati.Rows.Count == 0)
        {
            ucTabella t = new ucTabella();
            t.ReadOnly = _ReadOnly;
            t.Load( ID, "", _IDTree, "", IDCliente, IDSessione);
            if (tab == "")
            {
              tab = "Tabella Principale";
            }
            else if (_NewTabModifiedNames.ContainsKey(IDTree + "_" + ID))
            {
              tab = String.Format(_NewTabModifiedNames[IDTree + "_" + ID], 1);//in questo caso è sempre la prima
            }
            ti = new TabItem();
            ti.Header = tab;
            t.ChangeAlias(tab);
            ti.Content = t;
            //tabControl.Items.Add(ti); // E.B. comment
            Tabs.Add(tab); // E.B.
        }
       else
        {
                bool trovata = false;
                foreach (DataRow dtrow in dati.Rows)
                {
                    try
                    {
                        string tabtmp = dtrow["Tab"].ToString();
                        if (tabtmp!="")
                        {
                            trovata = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        string log = ex.Message;
                    }
                }
                if(!trovata)
                {
                    foreach (DataRow dtrow in dati.Rows)
                    {
                        dtrow["Tab"] = "Tabella Principale";
                    
                    }
                    cBusinessObjects.SaveData(id, dati, typeof(Tabella));
                }
            }

     foreach (DataRow dtrow in dati.Rows)
        {
          try
          {
            string tabtmp = dtrow["Tab"].ToString();
            if (!Tabs.Contains(tabtmp))
            {
              Tabs.Add(tabtmp);
            }
          }
          catch (Exception ex)
          {
            string log = ex.Message;
          }
        }

   
      for (int i = 0; i < Tabs.Count; i++)
      {
        ti = new TabItem();
        ti.Header = Tabs[i];
        ucTabella t_int = new ucTabella();
        t_int.ReadOnly = _ReadOnly;
        t_int.Load( ID, (string)(Tabs[i]), _IDTree, "", IDCliente, IDSessione);
        t_int.ChangeAlias(Tabs[i].ToString());
        ti.Content = t_int;
        
        tabControl.Items.Add(ti);
      }
      ti = new TabItem();
      ti.Header = App.NewTabHeaderText;
      tabControl.Items.Add(ti);
    }


    //----------------------------------------------------------------------------+
    //                                    Save                                    |
    //----------------------------------------------------------------------------+
    public int Save()
    {
       

        DataTable dd = null;
        foreach (TabItem item in tabControl.Items)
        {
            if (item.Content != null)
            {
                ((ucTabella)(item.Content)).UserControl_Loaded(new object(), new RoutedEventArgs());
                    if (dd == null)
                        dd = ((ucTabella)(item.Content)).Merge();
                    else
                        dd.Merge(((ucTabella)(item.Content)).Merge());
            }
        }
        return cBusinessObjects.SaveData(id, dd, typeof(Tabella));
      
    }

    public XmlDataProviderManager SaveXMLno()
    {
      XmlDocument tempDoc = new XmlDocument();
      foreach (TabItem item in tabControl.Items)
      {
        if (item.Content != null)
        {
          ((ucTabella)(item.Content)).UserControl_Loaded(new object(), new RoutedEventArgs());
           ((ucTabella)(item.Content)).Save();
          //Thread.Sleep(100);
          XmlNodeList listNode = _x.Document.SelectNodes(((ucTabella)(item.Content)).XPath);
          foreach (XmlNode node in listNode)
          {
            node.ParentNode.RemoveChild(node);
          }
          foreach (string path in ((ucTabella)(item.Content)).OldXPath)
          {
            foreach (XmlNode node in _x.Document.SelectNodes(path))
            {
              node.ParentNode.RemoveChild(node);
            }
          }
          foreach (XmlNode node in tempDoc.SelectNodes(((ucTabella)(item.Content)).XPath))
          {
            XmlNode NodoImportato = _x.Document.ImportNode(node, true);
            _x.Document.SelectSingleNode("/Dati/Dato[@ID=" + _ID + "]").AppendChild(NodoImportato);
            StaticUtilities.MarkNodeAsModified(NodoImportato.ParentNode, App.MOD_ATTRIB);
          }
        }
      }
      //Thread.Sleep(100);
      _x.isModified = true;
      _x.Save(true);
      return _x;
    }

    //----------------------------------------------------------------------------+
    //                          UserControl_SizeChanged                           |
    //----------------------------------------------------------------------------+
    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      try
      {
        double tmp = e.NewSize.Width - 30.0;
        tabControl.Width = tmp;
      }
      catch (Exception ex)
      {
        string log = ex.Message;
      }
    }

    //----------------------------------------------------------------------------+
    //                        tabControl_SelectionChanged                         |
    //----------------------------------------------------------------------------+
    private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      tabControl.Focus();
      if (e.AddedItems.Count > 0 && (e.AddedItems[0]).GetType().Name == "TabItem")
      {
        if (((string)(((TabItem)(e.AddedItems[0])).Header)) == App.NewTabHeaderText)
        {
          if (_ReadOnly)
          {
            try
            {
              MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
            }
            catch (Exception ex)
            {
              string log = ex.Message;
            }
            tabControl.SelectedIndex = 0;
            return;
          }
          var dialog = new wInputBox("Inserire Titolo della nuova Tabella");
          string newTabName = String.Empty;
          if (_NewTabModifiedNames.ContainsKey(_IDTree + "_" + _ID))
          {
            newTabName = String.Format(_NewTabModifiedNames[_IDTree + "_" + _ID], tabControl.Items.Count);//in questo caso è sempre la prima
          }
          dialog.ResponseText = newTabName;
          dialog.ShowDialog(); // E.B. solleva eccezione
          if (!dialog.diagres)
            {
                return;
            }
          string newHeader = dialog.ResponseText;
          if (newHeader == "")
          {
            MessageBox.Show("Attenzione, Titolo non valido");
            tabControl.SelectedIndex = 0;
            return;
          }
          foreach (TabItem item in tabControl.Items)
          {
            if (((string)(item.Header)) == newHeader)
            {
              MessageBox.Show("Attenzione, Titolo già esistente");
              tabControl.SelectedIndex = 0;
              return;
            }
          }
          TabItem ti = new TabItem();
          ti.Header = newHeader;
          ucTabella t = new ucTabella();
          t.ReadOnly = _ReadOnly;
                    
          if (((TabItem)(tabControl.Items[tabControl.Items.Count - 2])).Content != null)
          {
            ((ucTabella)(((TabItem)(tabControl.Items[tabControl.Items.Count - 2])).Content)).Save();
            ucTabella tt = ((ucTabella)(((TabItem)(tabControl.Items[tabControl.Items.Count - 2])).Content));
            t.Load(_ID, newHeader, _IDTree, ((TabItem)(tabControl.Items[tabControl.Items.Count - 2])).Header.ToString(), cBusinessObjects.idcliente.ToString(), cBusinessObjects.idsessione.ToString());
            t.dati.Clear();
            for (int i =0 ; i <= tt.dati.Rows.Count - 1; i++)
            {

                DataRow dtrow = tt.dati.Rows[i];
            //    if (dtrow["isnew"].ToString() != "1" && dtrow["tab"].ToString() == ((TabItem)(tabControl.Items[tabControl.Items.Count - 2])).Header.ToString())
              if ( dtrow["tab"].ToString() == ((TabItem)(tabControl.Items[tabControl.Items.Count - 2])).Header.ToString())      
               {
                    DataRow dd = t.dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
                    dd["Tab"] = newHeader;
                    dd["name"] = dtrow["name"];
                    dd["value"] = "";
                    dd["ID"] = dtrow["ID"];
                    dd["isnew"] = dtrow["isnew"];
                }

            }
            t.DoGenerateTable();
          }
          else
          {
            t.Load( _ID, newHeader, _IDTree, "",cBusinessObjects.idcliente.ToString(),cBusinessObjects.idsessione.ToString());
          }
          ti.Content = t;
          tabControl.Items.Insert(tabControl.Items.Count - 1, ti);
          tabControl.SelectedIndex = tabControl.Items.Count - 2;
        }
      }
    }

    //----------------------------------------------------------------------------+
    //                            btnDeleteTable_Click                            |
    //----------------------------------------------------------------------------+
    private void btnDeleteTable_Click(object sender, RoutedEventArgs e)
    {
      if (tabControl.Items.Count <= 2)
      {
        MessageBox.Show("L'unica tabella presente non è cancellabile; usa il comando Cancella Contenuto.", "Attenzione");
        return;
      }
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }
      if (tabControl.SelectedItem == null)
      {
        return;
      }
      if (MessageBox.Show("La tabella verrà cancellata. Procedere?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
      {
        ((ucTabella)(((TabItem)(tabControl.SelectedItem)).Content)).DeleteAll();
        string newHeader = ((TabItem)(tabControl.SelectedItem)).Header.ToString();
      
        }
     

        TabItem oldSelected = ((TabItem)(tabControl.SelectedItem));
        if (tabControl.Items.Count > 0)
        {
          tabControl.SelectedIndex = 0;
        }
        else
        {
          tabControl.SelectedIndex = -1;
        }
        tabControl.Items.Remove(oldSelected);
    }

    //----------------------------------------------------------------------------+
    //                            btnRenameTable_Click                            |
    //----------------------------------------------------------------------------+
    private void btnRenameTable_Click(object sender, RoutedEventArgs e)
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
      var dialog = new wInputBox("Inserire Titolo della nuova Tabella");
      dialog.ResponseText = ((TabItem)(tabControl.SelectedItem)).Header.ToString();
      dialog.ShowDialog();
      if (!dialog.diagres)
        {
            return;
        }

      string newHeader = dialog.ResponseText;
      if (newHeader == "")
      {
        MessageBox.Show("Attenzione, Titolo non valido");
        tabControl.SelectedIndex = 0;
        return;
      }
      foreach (TabItem item in tabControl.Items)
      {
        if (((string)(item.Header)) == newHeader)
        {
          MessageBox.Show("Attenzione, Titolo già esistente");
          tabControl.SelectedIndex = 0;
          return;
        }
      }
      ((TabItem)(tabControl.SelectedItem)).Header = newHeader;
      ((ucTabella)(((TabItem)(tabControl.SelectedItem)).Content)).ChangeAlias(newHeader);
    }

    //----------------------------------------------------------------------------+
    //                          TabItem_PreviewMouseMove                          |
    //----------------------------------------------------------------------------+
    private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      var tabItem = e.Source as TabItem;
      if (tabItem == null) return;
      if (tabItem.Header.ToString() == App.NewTabHeaderText)
      {
        return;
      }
      if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
      {
        DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
      }
    }

    //----------------------------------------------------------------------------+
    //                                TabItem_Drop                                |
    //----------------------------------------------------------------------------+
    private void TabItem_Drop(object sender, DragEventArgs e)
    {
      if (_ReadOnly) return;
      var tabItemTarget = e.Source as TabItem;
      if (tabItemTarget.Header.ToString() == App.NewTabHeaderText)
      {
        return;
      }
      var tabItemSource = e.Data.GetData(typeof(TabItem)) as TabItem;
      if (!tabItemTarget.Equals(tabItemSource))
      {
        string sourceHeader = tabItemSource.Header.ToString();
        ucTabella sourceContent = tabItemSource.Content as ucTabella;
        tabItemSource.Content = null;
        sourceContent.ChangeAlias(sourceHeader);
        string targetHeader = tabItemTarget.Header.ToString();
        ucTabella targetContent = tabItemTarget.Content as ucTabella;
        tabItemTarget.Content = null;
        targetContent.ChangeAlias(targetHeader);
        tabItemSource.Header = targetHeader;
        tabItemSource.Content = targetContent;
        tabItemTarget.Header = sourceHeader;
        tabItemTarget.Content = sourceContent;
      }
    }

  } // class ucTabellaReplicata
} // namespace UserControls
