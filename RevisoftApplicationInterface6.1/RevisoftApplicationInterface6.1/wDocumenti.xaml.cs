//----------------------------------------------------------------------------+
//                             wDocumenti.xaml.cs                             |
//----------------------------------------------------------------------------+
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
using System.Globalization;
using System.IO;
using System.Data;
using System.Runtime.InteropServices;
using System.Data;

namespace RevisoftApplication
{
  public enum TipoDocumento { Sconosciuto = -1, Corrente = 0, Permanente = 1 }
  public enum TipoVisualizzazione { Modelli = 0, Documenti = 1 }

  public partial class wDocumenti : System.Windows.Window
  {
    private DataTable dati = null;

    private XmlDataProviderManager _x;
    public string File = "";
    public string Cliente = "-1";
    public string Sessione = "-1";
    public string Tree = "-1";
    public string Nodo = "-1";
    public string NodoAlias = "";
    public string Titolo = "";
    public string Permanente = "";
    public string InitialDirectory = "";

    public string xpathprostampe = "";

    public TipoVisualizzazione Tipologia = TipoVisualizzazione.Modelli;

    private int SelectedIndexBefore = -1;

    public wDocumenti()
    {
      InitializeComponent();
      txtTitoloWindow.Foreground = App._arrBrushes[0];
      ButtonBar.Background = App._arrBrushes[12];
      SolidColorBrush tmpBrush = (SolidColorBrush)Resources["buttonHover"];
      tmpBrush.Color = ((SolidColorBrush)App._arrBrushes[13]).Color;
    }

    public bool ReadOnly
    {
      set
      {
        dtgMain.IsReadOnly = value;
        txtTitolo.IsReadOnly = value;
        txtDescrizione.IsReadOnly = value;
        rdbCorrente.IsHitTestVisible = !value;
        rdbPermanente.IsHitTestVisible = !value;
      }
      get
      {
        return dtgMain.IsReadOnly;
      }
    }

    //ProgressWindow pw;

    //------------------------------------------------------------------------+
    //                                  Load                                  |
    //------------------------------------------------------------------------+
    public void Load()
    {
      Binding b;
      int valore;
      string estensione, image, query, str, tipo;
      Utilities u;

      txtTitoloWindow.Text = Titolo;
      u = new Utilities();
      btnPrint.Visibility = System.Windows.Visibility.Visible;
      cBusinessObjects.idcliente = int.Parse(Cliente);
   
      switch (Tipologia)
      {
        case TipoVisualizzazione.Modelli:
          _x = new XmlDataProviderManager(App.AppModelliFile, true);
          if (!u.CheckXmlDocument(_x.Document, App.TipoFile.ModellPredefiniti))
          {
            this.Close();
            return;
          }
          btnAdd.Visibility = System.Windows.Visibility.Collapsed;
          btnView.Visibility = System.Windows.Visibility.Collapsed;
          btnDelete.Visibility = System.Windows.Visibility.Collapsed;
          btnCopia.Visibility = System.Windows.Visibility.Visible;
          btnViewTmp.Visibility = System.Windows.Visibility.Visible;
          btnSave.Visibility = System.Windows.Visibility.Visible;
          grdSelectedData.Visibility = System.Windows.Visibility.Collapsed;
          btnSalva.Visibility = System.Windows.Visibility.Collapsed;
          btnUploadFile.Visibility = System.Windows.Visibility.Collapsed;
          dtgMain.Columns[0].Visibility = System.Windows.Visibility.Collapsed;
          dtgMain.Columns[2].Visibility = System.Windows.Visibility.Collapsed;
          dtgMain.Columns[3].Visibility = System.Windows.Visibility.Collapsed;
          dtgMain.Columns[6].Visibility = System.Windows.Visibility.Collapsed;
          query =
            @"select<cr>" +
              @"null as ID_SCHEDA,<cr>" +
              @"null as ID_CLIENTE,<cr>" +
              @"null as ID_SESSIONE,<cr>" +
              @"ID as ID,<cr>" +
              @"cast(Tree as varchar(50)) as Tree,<cr>" +
              @"'0' as Tipo,<cr>" +
              @"Titolo as Titolo,<cr>" +
              @"Descrizione as Descrizione,<cr>" +
              @"[File] as [File],<cr>" +
              @"Visualizza as Visualizza,<cr>" +
              @"cast(null as varchar(50)) as ClienteExtended,<cr>" +
              @"cast(TreeExtended as varchar(50)) as TreeExtended,<cr>" +
              @"cast(null as varchar(50)) as SessioneExtended,<cr>" +
              @"cast(null as varchar(50)) as NodoExtended,<cr>" +
              @"FileExtended as FileExtended,<cr>" +
              @"cast(null as varchar(50)) as TipoExtended<cr>" +
            @"from dbo.Modelli";
          if (Nodo != "-1")
          {
            str = string.Format(@"<cr>where (Nodo={0})", Nodo); query += str;
          }
          query = query.Replace("<cr>", Environment.NewLine);
          dati = cBusinessObjects.ExecutesqlDataTable(query);
          foreach (System.Data.DataColumn col in dati.Columns) col.ReadOnly = false;
          break;
        case TipoVisualizzazione.Documenti:
       
          if (Nodo == "-1")
          {
            btnAdd.Visibility = System.Windows.Visibility.Collapsed;
          }
          btnCopia.Visibility = System.Windows.Visibility.Collapsed;
          btnViewTmp.Visibility = System.Windows.Visibility.Collapsed;
          btnSave.Visibility = System.Windows.Visibility.Collapsed;
          btnSalva.Visibility = System.Windows.Visibility.Collapsed;
          btnUploadFile.Visibility = System.Windows.Visibility.Collapsed;
          dati = cBusinessObjects.GetData(int.Parse(Nodo), typeof(ArchivioDocumenti), int.Parse(Cliente), int.Parse(Sessione));
          for (int i = dati.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dtrow = dati.Rows[i];
                    if (dtrow["ID_SESSIONE"].ToString() !=  cBusinessObjects.idsessione.ToString() || dtrow["Tree"].ToString() !=   Tree )
                        dtrow.Delete();
                }
               
          this.dati.AcceptChanges();
           

          if (Permanente == "1")
            dati = cBusinessObjects.GetDataFiltered(dati, (Convert.ToInt32(TipoDocumento.Permanente)).ToString(), "Tipo");
          break;
        default:
          break;
      }

      foreach (DataRow dtrow in dati.Rows)
      {
        if (dtrow["Visualizza"].ToString() != "True") dtrow["Visualizza"] = "True";
        if (dtrow["ClienteExtended"].ToString() == "" && dtrow["ID_CLIENTE"].ToString() != "")
        {
          dtrow["ClienteExtended"] = wDocumenti.GetClienteString(dtrow["ID_CLIENTE"].ToString());
        }
        if (dtrow["TreeExtended"].ToString() == "" && dtrow["Tree"].ToString() != "")
        {
          tipo = dtrow["Tree"].ToString();
          switch (((App.TipoFile)(System.Convert.ToInt32(tipo))))
          {
            case App.TipoFile.Incarico: 
            case App.TipoFile.IncaricoCS:
            case App.TipoFile.IncaricoSU:
            case App.TipoFile.IncaricoREV:
            tipo = "1"; break;
            case App.TipoFile.ISQC: tipo = "ISQC"; break;
            case App.TipoFile.Revisione: tipo = "2"; break;
            case App.TipoFile.Bilancio: tipo = "3"; break;
            case App.TipoFile.Conclusione: tipo = "9"; break;
            case App.TipoFile.Verifica: tipo = "4"; break;
            case App.TipoFile.Vigilanza: tipo = "5"; break;
            case App.TipoFile.PianificazioniVerifica: tipo = "P4"; break;
            case App.TipoFile.PianificazioniVigilanza: tipo = "P5"; break;
            default:
              break;
          }
          dtrow["TreeExtended"] = tipo;
        }
        if (dtrow["SessioneExtended"].ToString() == "" && dtrow["ID_SESSIONE"].ToString() != "")
        {
          dtrow["SessioneExtended"] = wDocumenti.GetSessioneString(dtrow["Tree"].ToString(), dtrow["ID_SESSIONE"].ToString());
        }
        if (dtrow["NodoExtended"].ToString() == "" && dtrow["Tree"].ToString() != "" && dtrow["ID_SESSIONE"].ToString() != "" && dtrow["ID_SCHEDA"].ToString() != "")
        {
          dtrow["NodoExtended"] = wDocumenti.GetNodeString(dtrow["Tree"].ToString(), dtrow["ID_SESSIONE"].ToString(), dtrow["ID_SCHEDA"].ToString());
        }
        if (dtrow["TipoExtended"].ToString() == "" && dtrow["Tipo"].ToString() != "")
        {
          tipo = dtrow["Tipo"].ToString();
          if (tipo.Trim() == "") tipo = "";
          else
          {
            valore = 0; int.TryParse(tipo, out valore);
            if (valore == 0 || valore == 1)
            {
              tipo = ((TipoDocumento)(System.Convert.ToInt32(tipo))).ToString();
            }
            else tipo = "";
          }
          dtrow["NodoExtended"] = tipo;
        }
        if (dtrow["FileExtended"].ToString() == "" && dtrow["File"].ToString() != "")
        {
          tipo = dtrow["File"].ToString();
          estensione = "";
          image = ".\\Images\\icone\\Stato\\nothing.png";
          if (tipo.Split('.').Count() > 0) estensione = tipo.Split('.').Last();
          switch (estensione)
          {
            case "doc":
            case "docx":
              image = ".\\Images\\icone\\Documenti\\word.png";
              break;
            case "pdf":
              image = ".\\Images\\icone\\Documenti\\pdf.png";
              break;
            case "xls":
            case "xlsx":
              image = ".\\Images\\icone\\Documenti\\excel.png";
              break;
            case "zip":
              image = ".\\Images\\icone\\Documenti\\zip.png";
              break;
            default:
              image = ".\\Images\\icone\\Documenti\\nothing.png";
              break;
          }
          dtrow["FileExtended"] = image;
        }
      }

      if (Permanente != "1" && Tipologia == TipoVisualizzazione.Documenti && Sessione!="-1")
        cBusinessObjects.SaveData(int.Parse(Nodo), dati, typeof(ArchivioDocumenti));

      b = new Binding();
      b.Source = dati;
      xpathprostampe = b.XPath;
      dtgMain.SetBinding(ItemsControl.ItemsSourceProperty, b);
      dtgMain.CanUserSortColumns = true;
      if (Permanente != "")
      {
        btnAdd.Visibility = System.Windows.Visibility.Collapsed;
        btnDelete.Visibility = System.Windows.Visibility.Collapsed;
        btnSalva.Visibility = System.Windows.Visibility.Collapsed;
        btnUploadFile.Visibility = System.Windows.Visibility.Collapsed;
        txtCliente.IsReadOnly = true;
        txtAttivita.IsReadOnly = true;
        txtSessione.IsReadOnly = true;
        txtNodo.IsReadOnly = true;
        txtTitolo.IsReadOnly = true;
        rdbPermanente.IsHitTestVisible = false;
        rdbCorrente.IsHitTestVisible = false;
        txtDescrizione.IsReadOnly = true;
      }
    }


    public void SelectFirst()
    {
      if (dtgMain.Items.Count > 0)
      {
        dtgMain.SelectedIndex = 0;
        selectionchanged();
      }
    }

    private void DataGrid_SourceUpdated(object sender, DataTransferEventArgs e)
    {
            cBusinessObjects.SaveData(int.Parse(Nodo), dati, typeof(ArchivioDocumenti));
    }

  
    private void AddRow(object sender, RoutedEventArgs e)
    {

      if (ReadOnly)
      {
        MessageBox.Show("Finestra in sola lettura. Non è possibile aggiungere nuovi documenti.", "Attenzione");
        return;
      }
      if (dtgMain.SelectedIndex != -1
        && CheckValoriCompleti(((DataRowView)(dtgMain.SelectedItem))) == false)
      {
        return;
      }
      string xml = "";
      XmlNode root = null;
      int newID = 0;
      string xpath = "";

      DataRow dt = null;

          

            switch (Tipologia)
      {
        case TipoVisualizzazione.Modelli:
          root = _x.Document.SelectSingleNode("//MODELLI");
          newID = Convert.ToInt32(root.Attributes["LastID"].Value) + 1;
          xml = "<MODELLO ID=\"" + newID.ToString() + "\" Tree=\"" + Tree + "\" Nodo=\"" + Nodo + "\" Titolo=\"\" Descrizione=\"\" File=\"\" Visualizza=\"True\"/>";
          xpath = "/MODELLO";
          break;
        case TipoVisualizzazione.Documenti:
         dt=  dati.Rows.Add(Nodo, int.Parse(Cliente), int.Parse(Sessione));
         DataTable tempdt=cBusinessObjects.ExecutesqlDataTable("SELECT MAX(ID) AS LASTID FROM ArchivioDocumenti");
         foreach (DataRow dd in tempdt.Rows)
         {
            if (dd["LASTID"].ToString() == "")
                dt["ID"] = 1;
            else
                dt["ID"] = int.Parse(dd["LASTID"].ToString()) + 1;
         }
         dt["Tree"] = Tree;
         dt["ID_SCHEDA"] = Nodo;
         txtID.Text = dt["ID"].ToString();
         break;
        default:
          break;
      }

       string file = dialogSaveFile(ref dt, false);
       if (file == "")
        {
            dt.Delete();
            dati.AcceptChanges();
            return;
        }
     
                

      dt["ClienteExtended"] = wDocumenti.GetClienteString(dt["ID_CLIENTE"].ToString());
      string tipo = dt["Tree"].ToString();
      switch (((App.TipoFile)(System.Convert.ToInt32(tipo))))
        {
          case App.TipoFile.Incarico:
            case App.TipoFile.IncaricoCS:
            case App.TipoFile.IncaricoSU:
            case App.TipoFile.IncaricoREV:
            tipo = "1";
            break;
          case App.TipoFile.ISQC:
            tipo = "ISQC";
            break;
          case App.TipoFile.Revisione:
            tipo = "2";
            break;
          case App.TipoFile.Bilancio:
            tipo = "3";
            break;
          case App.TipoFile.Conclusione:
            tipo = "9";
            break;
          case App.TipoFile.Verifica:
            tipo = "4";
            break;
          case App.TipoFile.Vigilanza:
            tipo = "5";
            break;
          case App.TipoFile.PianificazioniVerifica:
            tipo = "P4";
            break;
          case App.TipoFile.PianificazioniVigilanza:
            tipo = "P5";
            break;
          default:
            break;
        }
        dt["TreeExtended"] = tipo;
    
     
      if (dt["SessioneExtended"].ToString() == "")
      {
        dt["SessioneExtended"] = wDocumenti.GetSessioneString(dt["Tree"].ToString(), dt["ID_SESSIONE"].ToString());
      }

      if (dt["NodoExtended"].ToString() == "")
      {

        dt["NodoExtended"] = wDocumenti.GetNodeString(dt["Tree"].ToString(), dt["ID_SESSIONE"].ToString(), dt["ID_SCHEDA"].ToString());
      }

      if (dt["TipoExtended"].ToString() == "")
      {
      
        string tipo2 = dt["Tipo"].ToString();
        if (tipo2.Trim() == "")
        {
                    tipo2 = "";
        }
        else
        {
          int valore = 0;
          int.TryParse(tipo2, out valore);
          if (valore == 0 || valore == 1)
          {
                        tipo2 = ((TipoDocumento)(System.Convert.ToInt32(tipo))).ToString();
          }
          else
          {
                        tipo2 = "";
          }
        }
        dt["TipoExtended"] = tipo2;
      }

      if (dt["FileExtended"].ToString() == "")
      {
      
        string estensione = "";
        string image = ".\\Images\\icone\\Stato\\nothing.png";
        if (tipo.Split('.').Count() > 0)
        {
          estensione = tipo.Split('.').Last();
        }
        switch (estensione)
        {
          case "doc":
          case "docx":
            image = ".\\Images\\icone\\Documenti\\word.png";
            break;
          case "pdf":
            image = ".\\Images\\icone\\Documenti\\pdf.png";
            break;
          case "xls":
          case "xlsx":
            image = ".\\Images\\icone\\Documenti\\excel.png";
            break;
          case "zip":
            image = ".\\Images\\icone\\Documenti\\zip.png";
            break;
          default:
            image = ".\\Images\\icone\\Documenti\\nothing.png";
            break;
        }
        dt["FileExtended"] = image;
       
      }

      FileInfo fitmp = new FileInfo(file);
      if (fitmp.Exists)
      {
        SelectedIndexBefore = dtgMain.Items.Count - 1;
      }
      cBusinessObjects.SaveData(int.Parse(Nodo), dati, typeof(ArchivioDocumenti));
      bypass = true;
      dtgMain.SelectedIndex = dtgMain.Items.Count - 1;
      selectionchanged();
      bypass = false;
    }

   
    private void DeleteRow(object sender, RoutedEventArgs e)
    {

      if (ReadOnly)
      {
        MessageBox.Show("Finestra in sola lettura. Non è possibile rimuovere documenti.", "Attenzione");
        return;
      }
      //richiesta conferma
      Utilities u = new Utilities();
      if (MessageBoxResult.No == u.ConfermaCancellazione()) return;
            DataRowView node = null;
      if (dtgMain.SelectedIndex != -1)
      {
        node = (DataRowView)(dtgMain.SelectedItem);
        if (node == null)
        {
          MessageBox.Show("Selezionare un documento");
          return;
        }
      }
      string directory = "";
      switch (Tipologia)
      {
        case TipoVisualizzazione.Modelli:
          directory = App.AppModelliFolder;
          break;
        case TipoVisualizzazione.Documenti:
          directory = App.AppDocumentiFolder;
          break;
        default:
          break;
      }
      var file = directory + "\\" + node["File"].ToString();
      FileInfo fitmp = new FileInfo(file);
      if (fitmp.Exists) fitmp.Delete();

        foreach (DataRow dtrow in this.dati.Rows)
        {
            if (dtrow["File"].ToString() == node["File"].ToString())
            {
                dtrow.Delete();
                break;
            }
        }
        dati.AcceptChanges();

      cBusinessObjects.SaveData(int.Parse(Nodo), dati, typeof(ArchivioDocumenti));

    }

        private void dtgMain_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
    {
            return;
      if (e.Column.SortMemberPath != "@value")
      {
        if (e.Row != null && e.Row.Item != null && ((XmlNode)(e.Row.Item)).Attributes["ID"] != null)
        {
          string ID = ((XmlNode)(e.Row.Item)).Attributes["ID"].Value;
          e.Cancel = true;
        }
      }
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      //double tmp = e.NewSize.Width - 30.0;
      //dtgMain.Columns[0].Width = tmp / 3.0 - 5;
      //dtgMain.Columns[1].Width = tmp / 3.0 * 2.0 - 5;
      //dtgMain.Width = tmp;
    }

  
    private void btnSalva_Click(object sender, RoutedEventArgs e)
    {

      if (ReadOnly)
      {
        MessageBox.Show("Finestra in sola lettura. Non è possibile salvare.", "Attenzione");
        return;
      }
      if (txtTitolo.Text.Trim() == "")
      {
        MessageBox.Show("Titolo Obbligatorio");
        return;
      }
      if (rdbCorrente.IsChecked == false && rdbPermanente.IsChecked == false)
      {
        MessageBox.Show("Scegliere tra tipologia Corrente o Permanente");
        return;
      }
      if (dtgMain.SelectedIndex != -1)
      {
        SelectedIndexBefore = dtgMain.SelectedIndex;
        DataRowView node = (DataRowView)(dtgMain.SelectedItem);
        if (node != null)
        {
         foreach(DataRow dt in dati.Rows)
           {
                if (dt["ID"].ToString() != txtID.Text)
                    continue;
              dt["Titolo"] = txtTitolo.Text;
              dt["Descrizione"] = txtDescrizione.Text;
              if (rdbCorrente.IsChecked == true)
              {
                            dt["Tipo"] = (Convert.ToInt32(TipoDocumento.Corrente)).ToString();
              }
              else
              {
                            dt["Tipo"] = (Convert.ToInt32(TipoDocumento.Permanente)).ToString();
              }
              string tipo = dt["Tipo"].ToString();
              if (tipo.Trim() == "")
              {
                tipo = "";
              }
              else
              {
                int valore = 0;
                int.TryParse(tipo, out valore);
                if (valore == 0 || valore == 1)
                {
                  tipo = ((TipoDocumento)(System.Convert.ToInt32(tipo))).ToString();
                }
                else
                {
                  tipo = "";
                }
              }
              dt["TipoExtended"] = tipo;
         }
         cBusinessObjects.SaveData(int.Parse(Nodo), dati, typeof(ArchivioDocumenti));

          MessageBox.Show("Documento salvato con successo");
          }
      }
    }

    bool bypass = false;

    private void dtgMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (!bypass)
      {
        if (e.RemovedItems.Count > 0 && SelectedIndexBefore == -1 && dtgMain.SelectedIndex != -1)
        {
          if (CheckValoriCompleti(((DataRowView)(e.RemovedItems[0]))) == false)
          {
            return;
          }
        }
        if (SelectedIndexBefore != -1)
        {
          dtgMain.SelectedIndex = SelectedIndexBefore;
          SelectedIndexBefore = -1;
        }
        selectionchanged();
      }
    }

    private void selectionchanged()
    {
      grdSelectedData.Visibility = System.Windows.Visibility.Collapsed;
      //andrea
      btnSalva.Visibility = System.Windows.Visibility.Collapsed;
      btnUploadFile.Visibility = System.Windows.Visibility.Collapsed;
      txtCliente.Text = "";
      txtAttivita.Text = "";
      txtSessione.Text = "";
      txtNodo.Text = "";
      txtID.Text = "-1";
      txtTitolo.Text = "";
      txtDescrizione.Text = "";
      if (dtgMain.SelectedIndex != -1 && Tipologia != TipoVisualizzazione.Modelli)
      {
        DataRowView node = (DataRowView)(dtgMain.SelectedItem);
        if (node != null)
        {
          grdSelectedData.Visibility = System.Windows.Visibility.Visible;
          //andrea
          btnSalva.Visibility = System.Windows.Visibility.Visible;
          btnUploadFile.Visibility = System.Windows.Visibility.Visible;
          string file = GetFile(node);
          if (file != "")
          {
            btnView.Visibility = System.Windows.Visibility.Visible;
          }
          else
          {
            btnView.Visibility = System.Windows.Visibility.Collapsed;
          }
          txtCliente.Text = GetClienteString(node["ID_CLIENTE"].ToString());
          txtAttivita.Text = ((App.TipoFile)(Convert.ToInt32(node["Tree"].ToString()))).ToString();
          txtSessione.Text = GetSessioneString(node["Tree"].ToString(), node["ID_SESSIONE"].ToString());
          txtNodo.Text = GetNodeString(node["Tree"].ToString(), node["ID_SESSIONE"].ToString(), node["ID_SCHEDA"].ToString());
          if (node["Tipo"].ToString() != "")
          {
            switch (((TipoDocumento)(Convert.ToInt32(node["Tipo"].ToString()))))
            {
              case TipoDocumento.Corrente:
                rdbCorrente.IsChecked = true;
                rdbPermanente.IsChecked = false;
                break;
              default:
                rdbCorrente.IsChecked = false;
                rdbPermanente.IsChecked = true;
                break;
            }
          }
          else
          {
            rdbCorrente.IsChecked = false;
            rdbPermanente.IsChecked = false;
          }
          txtTitolo.Text = node["Titolo"].ToString();
          txtDescrizione.Text = node["Descrizione"].ToString();
          txtID.Text = node["ID"].ToString();
        }
      }
      if (Permanente != "")
      {
        btnAdd.Visibility = System.Windows.Visibility.Collapsed;
        btnDelete.Visibility = System.Windows.Visibility.Collapsed;
        btnSalva.Visibility = System.Windows.Visibility.Collapsed;
        btnUploadFile.Visibility = System.Windows.Visibility.Collapsed;
        txtCliente.IsReadOnly = true;
        txtAttivita.IsReadOnly = true;
        txtSessione.IsReadOnly = true;
        txtNodo.IsReadOnly = true;
        txtTitolo.IsReadOnly = true;
        rdbPermanente.IsHitTestVisible = false;
        rdbCorrente.IsHitTestVisible = false;
        txtDescrizione.IsReadOnly = true;
      }
      //txtTitolo.Focus();
    }

    
    private string dialogSaveFile(ref  DataRow dt, bool ToBeSaved)
    {

      string file = "";
      string newName = "";
      Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
      dlg.InitialDirectory = InitialDirectory;
      if (dlg.ShowDialog() == true)
      {
        FileInfo fi = new FileInfo(dlg.FileName);
        if (fi.Exists)
        {
          string pathnew = fi.FullName.Replace(fi.FullName.Split('\\').Last(), "");
          DirectoryInfo dinew = new DirectoryInfo(pathnew);
          if (dinew.Exists)
          {
            InitialDirectory = pathnew;
          }
          string directory = "";
          switch (Tipologia)
          {
            case TipoVisualizzazione.Modelli:
              directory = App.AppModelliFolder;
              break;
            case TipoVisualizzazione.Documenti:
              directory = App.AppDocumentiFolder;
              break;
            default:
              break;
          }
          string oldfile = directory + "\\" + dt["File"].ToString();
          FileInfo oldfitmp = new FileInfo(oldfile);
          if (oldfitmp.Exists)
          {
            oldfitmp.Delete();
          }
          newName = txtID.Text + "." + dlg.FileName.Split('.').Last();
          file = directory + "\\" + newName;
          FileInfo fitmp = new FileInfo(file);
          if (fitmp.Exists)
          {
            fitmp.Delete();
          }
          fi.IsReadOnly = false;
          fi.CopyTo(file);
           dt["File"] = newName;
         
       //MM     StaticUtilities.MarkNodeAsModified(node,App.OBJ_MOD);
           

        }
      }
      return file;
    }

    private void btnUploadFile_Click(object sender, RoutedEventArgs e)
    {
      if (ReadOnly)
      {
        MessageBox.Show("Finestra in sola lettura. Non è possibile caricare file.", "Attenzione");
        return;
      }
      if (dtgMain.SelectedIndex != -1 && CheckValoriCompleti(((DataRowView)(dtgMain.SelectedItem))) == false)
      {
        return;
      }
      DataRowView node = null;
      DataRow cdd = null;
      if (dtgMain.SelectedIndex != -1)
      {
        node = (DataRowView)(dtgMain.SelectedItem);
        if (node == null)
        {
          MessageBox.Show("Selezionare un documento");
          return;
        }
      
        foreach(DataRow dt in dati.Rows)
        {
            if (dt["ID"].ToString() == node["ID"].ToString())
                cdd = dt;
        }
         if (cdd == null)
        {
          MessageBox.Show("Selezionare un documento");
          return;
        }
      }
      dialogSaveFile(ref cdd, true);
    }

    private string GetFile(DataRowView node)
     { 
      string directory = "";
      switch (Tipologia)
      {
        case TipoVisualizzazione.Modelli:
          directory = App.AppModelliFolder;
          break;
        case TipoVisualizzazione.Documenti:
          directory = App.AppDocumentiFolder;
          break;
        default:
          break;
      }
      string file = directory + "\\" + node["File"].ToString();
      FileInfo fi = new FileInfo(file);
      if (!fi.Exists)
      {
        file = "";
      }
      return file;
    }

    private void btnPreviewFile_Click(object sender, RoutedEventArgs e)
    {
      DataRowView node = null;
      if (dtgMain.SelectedIndex != -1)
      {
         node = (DataRowView)(dtgMain.SelectedItem);
        if (node == null)
        {
          MessageBox.Show("Selezionare un documento");
          return;
        }
      }
      if (node != null)
      {
        try
        {
          string file = GetFile(node);
          if (file != "")
          {
            ////DocumentViewer dd = new DocumentViewer(file);
            ////dd.Show();
            ////string cmd = @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
            //ANDREA
            //WebBrowser aa = new WebBrowser();
            //aa.Navigate("C:/Users/AndreaBarbieri/AppData/Roaming/Revisoft/Revisoft/UserDoc/1.pdf");
            ////string cmd = "/C start AcroRd32.exe";
            ////System.Diagnostics.Process.Start("CMD.exe", cmd);
            System.Diagnostics.Process.Start(file);
            ////MessageBox.Show(file);
            ////System.Diagnostics.Process process = new System.Diagnostics.Process();
            ////process.Refresh();
            ////process.StartInfo.FileName = file;
            ////process.StartInfo.ErrorDialog = false;
            ////process.StartInfo.Verb = "open";
            ////process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            ////process.Start();
            ////process.WaitForInputIdle(3000);
            ////IntPtr hWnd = process.MainWindowHandle;
            ////SetFocus(hWnd);
            ////SetForegroundWindow(hWnd);
            ////System.Diagnostics.Process.Start("C:\\Windows\\SystemApps\\Microsoft.MicrosoftEdge_8wekyb3d8bbwe\\MicrosoftEdge.exe", file);
          }
        }
        catch (Exception ex)
        {
          string log = ex.Message;
        }
      }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SetFocus(IntPtr hWnd);

    private void btnPreviewFileTmp_Click(object sender, RoutedEventArgs e)
    {
      if (Tipologia != TipoVisualizzazione.Modelli)
      {
        return;
      }
            DataRowView node = null;
      if (dtgMain.SelectedIndex != -1)
      {
        node = (DataRowView)(dtgMain.SelectedItem);
        if (node == null)
        {
          MessageBox.Show("Selezionare un documento");
          return;
        }
      }
      if (node != null)
      {
        try
        {
          string directory = App.AppModelliFolder;
          string file = directory + "\\" + node["File"].ToString();
          FileInfo fi = new FileInfo(file);
          if (fi.Exists)
          {
            string newfile = System.IO.Path.GetTempPath() + node["File"].ToString();
            FileInfo newfi = new FileInfo(newfile);
            if (newfi.Exists)
            {
              newfi.Delete();
            }
            fi.CopyTo(newfile);
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.Refresh();
            process.StartInfo.FileName = newfile;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.Verb = "open";
            process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            process.Start();
            //process.WaitForInputIdle(3000);
            IntPtr hWnd = process.MainWindowHandle;
            SetFocus(hWnd);
            SetForegroundWindow(hWnd);
          }
        }
        catch (Exception ex)
        {
          string log = ex.Message;
        }
      }
    }

    private void SaveToDisck(object sender, RoutedEventArgs e)
    {
      if (Tipologia != TipoVisualizzazione.Modelli)
      {
        return;
      }
      DataRowView node = null;
      if (dtgMain.SelectedIndex != -1)
      {
        node = (DataRowView)(dtgMain.SelectedItem);
        if (node == null)
        {
          MessageBox.Show("Selezionare un documento");
          return;
        }
      }
      if (node != null)
      {
        try
        {
          string directory = App.AppModelliFolder;
          string file = directory + "\\" + node["File"].ToString();
          FileInfo fi = new FileInfo(file);
          if (fi.Exists)
          {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.AddExtension = true;
            string estensione = node["File"].ToString().Split('.').Last();
            dlg.DefaultExt = estensione;
            dlg.Filter = estensione + " | *." + estensione;
            if (dlg.ShowDialog() == true)
            {
              string newfile = dlg.FileName;
              FileInfo newfi = new FileInfo(newfile);
              if (newfi.Exists)
              {
                newfi.Delete();
              }
              fi.CopyTo(newfile);
            }
          }
        }
        catch (Exception ex)
        {
          string log = ex.Message;
        }
      }
    }

   
    private void CopyToDocumentiAssociati(object sender, RoutedEventArgs e)
    {

      if (Tipologia != TipoVisualizzazione.Modelli)
      {
        return;
      }
      DataRowView node = null;
      if (dtgMain.SelectedIndex != -1)
      {
        node = (DataRowView)(dtgMain.SelectedItem);
        if (node == null)
        {
          MessageBox.Show("Selezionare un documento");
          return;
        }
      }
      if (node != null)
      {
        try
        {
          XmlDataProviderManager _y = new XmlDataProviderManager(App.AppDocumentiDataFile, true);
          XmlNode root = _y.Document.SelectSingleNode("//DOCUMENTI");
          int newID = Convert.ToInt32(root.Attributes["LastID"].Value) + 1;
          string newName = newID.ToString() + "." + node["File"].ToString().Split('.').Last();
          string xml = "<DOCUMENTO ID=\"" + newID.ToString() + "\" Cliente=\"" + Cliente.Replace("&", "&amp;").Replace("\"", "'") + "\" Sessione=\"" + Sessione + "\" Tree=\"" + Tree + "\" Nodo=\"" + Nodo + "\" Tipo=\"" + (Convert.ToInt32(TipoDocumento.Corrente)).ToString() + "\" Titolo=\"" + node["Titolo"].ToString().Replace("&", "&amp;").Replace("\"", "'") + "\" Descrizione=\"" + "" + "\" File=\"" + newName.Replace("&", "&amp;").Replace("\"", "'") + "\" Visualizza=\"True\" />";
          string directory = App.AppModelliFolder;
          string file = directory + "\\" + node["File"].ToString();
          FileInfo fi = new FileInfo(file);
          if (fi.Exists)
          {
            string newfile = App.AppDocumentiFolder + "\\" + newName;
            fi.CopyTo(newfile);
          }
          XmlDocument doctmp = new XmlDocument();
          doctmp.LoadXml(xml);
          XmlNode tmpNode = doctmp.SelectSingleNode("/DOCUMENTO");
          XmlNode nodenew = _y.Document.ImportNode(tmpNode, true);
          StaticUtilities.MarkNodeAsModified(nodenew,App.OBJ_MOD);
          root.AppendChild(nodenew);
          root.Attributes["LastID"].Value = newID.ToString();
          StaticUtilities.MarkNodeAsModified(root, App.OBJ_MOD);
          _y.isModified = true;
          _y.Save();
          MessageBox.Show("Modello copiato con successo");
        }
        catch (Exception ex)
        {
          string log = ex.Message;
        }
      }
    }

    static public string GetClienteString(string cliente)
    {
      //try
      //{
      MasterFile mf = MasterFile.Create();
      Hashtable ht = mf.GetAnagrafica(System.Convert.ToInt32(cliente));
      return ht["RagioneSociale"].ToString();
      //}
      //catch (Exception ex)
      //{
      //    string lof = ex.Message;
      //}
#pragma warning disable CS0162 // È stato rilevato codice non raggiungibile
      return "";
#pragma warning restore CS0162 // È stato rilevato codice non raggiungibile
    }

    static public string GetSessioneString(string albero, string sessione)
    {
      string ID = sessione;
      MasterFile mf = MasterFile.Create();
      Hashtable ht;
      string returnvalue = "";
      //try
      //{
      switch ((App.TipoFile)(System.Convert.ToInt32(albero)))
      {
        case App.TipoFile.Revisione:
          ht = mf.GetRevisione(ID);
          if (ht["Data"] != null && ht["Data"].ToString().Length == 10)
          {
            returnvalue = ht["Data"].ToString().Substring(6, 4);
          }
          break;
        case App.TipoFile.Verifica:
          ht = mf.GetVerifica(ID);
          if (ht["Data"] != null)
          {
            returnvalue = ht["Data"].ToString();
          }
          break;
        case App.TipoFile.Incarico:
            case App.TipoFile.IncaricoCS:
            case App.TipoFile.IncaricoSU:
            case App.TipoFile.IncaricoREV:
          ht = mf.GetIncarico(ID);
          if (ht["DataNomina"] != null && ht["DataNomina"].ToString().Length == 10)
          {
            returnvalue = ht["DataNomina"].ToString().Substring(6, 4);
          }
          break;
        case App.TipoFile.ISQC:
          ht = mf.GetISQC(ID);
          if (ht["DataNomina"] != null && ht["DataNomina"].ToString().Length == 10)
          {
            returnvalue = ht["DataNomina"].ToString().Substring(6, 4);
          }
          break;
        case App.TipoFile.Bilancio:
          ht = mf.GetBilancio(ID);
          if (ht["Data"] != null && ht["Data"].ToString().Length == 10)
          {
            returnvalue = ht["Data"].ToString().Substring(6, 4);
          }
          break;
        case App.TipoFile.Conclusione:
          ht = mf.GetConclusione(ID);
          if (ht["Data"] != null && ht["Data"].ToString().Length == 10)
          {
            returnvalue = ht["Data"].ToString().Substring(6, 4);
          }
          break;
        case App.TipoFile.Vigilanza:
          ht = mf.GetVigilanza(ID);
          if (ht.Count == 0)
          {
            ht = mf.GetVerifica(ID);
          }
          if (ht["Data"] != null)
          {
            returnvalue = ht["Data"].ToString();
          }
          break;
        case App.TipoFile.PianificazioniVerifica:
          ht = mf.GetPianificazioniVerifica(ID);
          if (ht["DataInizio"] != null)
          {
            returnvalue = ht["DataInizio"].ToString();
          }
          break;
        case App.TipoFile.PianificazioniVigilanza:
          ht = mf.GetPianificazioniVigilanza(ID);
          if (ht["DataInizio"] != null)
          {
            returnvalue = ht["DataInizio"].ToString();
          }
          break;
        default:
          break;
      }
      //}
      //catch (Exception ex)
      //{
      //    string lof = ex.Message;
      //}
      return returnvalue;
    }

    static public string GetNodeString(string albero, string sessione, string nodo)
    {
      MasterFile mf = MasterFile.Create();
      Hashtable ht;
      string file = "";
      string returnvalue = "";
      //try
      //{
      switch ((App.TipoFile)(System.Convert.ToInt32(albero)))
      {
        case App.TipoFile.Revisione:
          ht = mf.GetRevisione(sessione);
          if (ht.Contains("File"))
          {
            file = ht["File"].ToString();
          }
          else
          {
            file = App.AppTemplateTreeRevisione;
          }
          break;
        case App.TipoFile.Verifica:
          ht = mf.GetVerifica(sessione);
          if (ht.Contains("File"))
          {
            file = ht["File"].ToString();
          }
          else
          {
            file = App.AppTemplateTreeVerifica;
          }
          break;
        case App.TipoFile.Vigilanza:
          ht = mf.GetVigilanza(sessione);
          if (ht.Count == 0)
          {
            ht = mf.GetVerifica(sessione);
            ht = mf.GetVigilanzaAssociataFromVerifica(ht["ID"].ToString());
          }
          if (ht.Contains("File"))
          {
            file = ht["File"].ToString();
          }
          else
          {
            file = App.AppTemplateTreeVigilanza;
          }
          XmlDataProviderManager _yTMP = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + file);
          XmlNode nodeTMP = _yTMP.Document.SelectSingleNode("//Tree//Node[@ID='" + nodo + "']");
          if (nodeTMP == null)
          {
            file = App.AppTemplateTreeVigilanza;
          }
          break;
        case App.TipoFile.PianificazioniVerifica:
          ht = mf.GetPianificazioniVerifica(sessione);
          if (ht.Contains("File"))
          {
            file = ht["File"].ToString();
          }
          else
          {
            file = App.AppTemplateTreeVerifica;
          }
          break;
        case App.TipoFile.PianificazioniVigilanza:
          ht = mf.GetPianificazioniVigilanza(sessione);
          if (ht.Contains("File"))
          {
            file = ht["File"].ToString();
          }
          else
          {
            file = App.AppTemplateTreeVigilanza;
          }
          break;
        case App.TipoFile.Incarico:
                    case App.TipoFile.IncaricoCS:
            case App.TipoFile.IncaricoSU:
            case App.TipoFile.IncaricoREV:
          ht = mf.GetIncarico(sessione);
          if (ht.Contains("File"))
          {
            file = ht["File"].ToString();
          }
          else
          {
            file = App.AppTemplateTreeIncarico;
          }
          break;
        case App.TipoFile.ISQC:
          ht = mf.GetISQC(sessione);
          if (ht.Contains("File"))
          {
            file = ht["File"].ToString();
          }
          else
          {
            file = App.AppTemplateTreeISQC;
          }
          break;
        case App.TipoFile.Bilancio:
          ht = mf.GetBilancio(sessione);
          if (ht.Contains("File"))
          {
            file = ht["File"].ToString();
          }
          else
          {
            file = App.AppTemplateTreeBilancio;
          }
          break;
        case App.TipoFile.Conclusione:
          ht = mf.GetConclusione(sessione);
          if (ht.Contains("File"))
          {
            file = ht["File"].ToString();
          }
          else
          {
            file = App.AppTemplateTreeConclusione;
          }
          break;
        default:
          break;
      }
      XmlDataProviderManager _y = null;
      if (file.Contains('\\'))
      {
        _y = new XmlDataProviderManager(file);
      }
      else
      {
        _y = new XmlDataProviderManager(App.AppDataDataFolder + "\\" + file);
      }
      XmlNode node = _y.Document.SelectSingleNode("//Tree//Node[@ID='" + nodo + "']");
      if (node != null)
      {
        returnvalue = node.Attributes["Codice"].Value + " " + node.Attributes["Titolo"].Value;
      }
      //}
      //catch (Exception ex)
      //{
      //    string lof = ex.Message;
      //}
      return returnvalue;
    }

    private void buttonNuovoCerca_Click(object sender, RoutedEventArgs e)
    {
      searchTextBox.Text = "";
      buttonCerca_Click(sender, e);
    }

  
   
    private void buttonCerca_Click(object sender, RoutedEventArgs e)
    {

      bool isModified=false;
      string SearchFor = searchTextBox.Text.ToUpper();
      dtgMain.SelectedIndex = -1;
      string xpath = "";
      switch (Tipologia)
      {
        case TipoVisualizzazione.Modelli:
        xpath = "//MODELLI//MODELLO"; // [@Tree='" + Tree + "']
         foreach (XmlNode item in _x.Document.SelectNodes(xpath))
          {
            if (item.Attributes["Titolo"].Value.ToUpper().Contains(SearchFor)
              || item.Attributes["Descrizione"].Value.ToUpper().Contains(SearchFor))
            {
              if (item.Attributes["Visualizza"].Value != "True")
              {
                item.Attributes["Visualizza"].Value = "True";
                StaticUtilities.MarkNodeAsModified(item, App.OBJ_MOD);isModified = true;
              }
            }
            else
            {
              if (item.Attributes["Visualizza"].Value != "False")
              {
                item.Attributes["Visualizza"].Value = "False";
                StaticUtilities.MarkNodeAsModified(item, App.OBJ_MOD); isModified = true;
              }
            }
          }
          _x.isModified = isModified; _x.Save();
          break;
        case TipoVisualizzazione.Documenti:
          foreach (DataRow item in dati.Rows)
            {

                if (item["Titolo"].ToString().ToUpper().Contains(SearchFor)
                    || item["Descrizione"].ToString().ToUpper().Contains(SearchFor))
                {
                    if (item["Visualizza"].ToString() != "True")
                    {
                        item["Visualizza"] = "True";

                    }
                }
                else
                {
                    if (item["Visualizza"].ToString() != "False")
                    {
                        item["Visualizza"] = "False";

                    }
                }
            }
          break;
        default:
          break;
      }
     
    }

    private bool CheckValoriCompleti(DataRowView node)
    {
      if (Tipologia == TipoVisualizzazione.Modelli)
      {
        return true;
      }
      if (!bypass)
      {
        if (node["Titolo"].ToString() == "")
        {
          MessageBox.Show("Titolo e Tipo obbligatori. Premere SALVA dopo l'inserimento.");
          return false;
        }
        else
        {
          if (node["Titolo"].ToString() == "")
          {
            MessageBox.Show("Titolo e Tipo obbligatori. Premere SALVA dopo l'inserimento.");
            return false;
          }
        }
        
   
        if (node["Tipo"].ToString() == "")
          {
            MessageBox.Show("Titolo e Tipo obbligatori. Premere SALVA dopo l'inserimento.");
            return false;
          }
      
      }
      return true;
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
      //if (dtgMain.SelectedIndex != -1 && CheckValoriCompleti(((XmlNode)(dtgMain.SelectedItem))) == true)
      //{
      //    base.Close();
      //}
      //else
      //{
      //    return;
      //}
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
      //SelectFirst();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (ReadOnly)
      {
        return;
      }

      if (dtgMain.SelectedIndex == -1 || (dtgMain.SelectedIndex != -1 && CheckValoriCompleti(((DataRowView)(dtgMain.SelectedItem))) == true))
      {
        return;
      }
      else
      {
        e.Cancel = true;
        return;
      }
    }

    private void dtgMain_Loaded(object sender, RoutedEventArgs e)
    {
      //pw.Close();
    }

    private void searchTextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
      {
        buttonCerca_Click(sender, new RoutedEventArgs());
      }
    }

    public System.Data.DataTable ConvertXmlNodeListToDataTable(XmlNodeList xnl)
    {
      System.Data.DataTable dt = new System.Data.DataTable();
      int TempColumn = 0;
      foreach (XmlAttribute node in xnl.Item(0).Attributes)
      {
        TempColumn++;
        DataColumn dc = new DataColumn(node.Name, System.Type.GetType("System.String"));
        if (dt.Columns.Contains(node.Name))
        {
          dt.Columns.Add(dc.ColumnName = dc.ColumnName + TempColumn.ToString());
        }
        else
        {
          dt.Columns.Add(dc);
        }
      }
      int ColumnsCount = dt.Columns.Count;
      for (int i = 0; i < xnl.Count; i++)
      {
        DataRow dr = dt.NewRow();
        for (int j = 0; j < ColumnsCount; j++)
        {
          dr[j] = xnl.Item(i).Attributes[j].Value;
        }
        dt.Rows.Add(dr);
      }
      return dt;
    }

    private void buttonCreaPDF_Click(object sender, RoutedEventArgs e)
    {
    
      string rtf_text = "";
      rtf_text += "{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1040\\deflangfe1040\\deftab709";
      rtf_text += "{\\fonttbl{\\f0 Cambria}}";
      rtf_text += "{\\colortbl;\\red0\\green255\\blue255;\\red204\\green204\\blue204;\\red255\\green255\\blue255;\\red230\\green230\\blue230;}";
      rtf_text += "\\viewkind4\\uc1";
      int index = 0;
      string lastTree = "";
      string lastSessione = "";
      string inizioriga = "\\trowd\\trpaddl50\\trpaddt15\\trpaddr50\\trpaddb15\\trpaddfl3\\trpaddft3\\trpaddfr3\\trpaddfb3 ";
      string colore2 = "\\clcbpat2";
      string colore3 = "\\clcbpat3";
      string colore4 = "\\clcbpat4";
      string bordi = "\\clbrdrl\\brdrw10\\brdrs\\clbrdrt\\brdrw10\\brdrs\\clbrdrr\\brdrw10\\brdrs\\clbrdrb\\brdrw10\\brdrs"; //\\clpadt100
      string cell1 = "\\clvertalc\\cellx4770";
      string cell2 = "\\clvertalc\\cellx9610";
      string inizioriga2 = "\\pard\\intbl\\tx2291";
      string fineriga = "\\row ";
    //  System.Data.DataTable dt = ConvertXmlNodeListToDataTable(_x.Document.SelectNodes(xpathprostampe));
    //  dt.DefaultView.Sort = "TreeExtended ASC, SessioneExtended ASC, NodoExtended ASC";
      for (int i = 0; i < dati.DefaultView.Count; i++)
      {
        if (index == 0)
        {
          rtf_text += "\\pard \\keep \\fs30 Indice Documenti cliente: " + dati.DefaultView[i]["ClienteExtended"].ToString().ToUpper() + " \\fs20 \\line \\line \\par ";
          index++;
        }
        if (lastTree != dati.DefaultView[i]["TreeExtended"].ToString())
        {
          lastSessione = "x";
        }
        if (lastSessione != dati.DefaultView[i]["SessioneExtended"].ToString() && lastSessione != "")
        {
          rtf_text += "\\pard\\bgdkdcross\\cfpat1\\shading59110\\tx2291\\par \\line ";
        }
        if (lastTree != dati.DefaultView[i]["TreeExtended"].ToString())
        {
          lastTree = dati.DefaultView[i]["TreeExtended"].ToString();
          rtf_text += "\\pard\\keep Area: " + dati.DefaultView[i]["TreeExtended"].ToString().ToUpper() + " \\par ";
        }
        if (lastSessione != dati.DefaultView[i]["SessioneExtended"].ToString())
        {
          lastSessione = dati.DefaultView[i]["SessioneExtended"].ToString();
          rtf_text += "\\pard\\keep Sessione: " + dati.DefaultView[i]["SessioneExtended"].ToString().ToUpper() + " \\line \\par ";
          rtf_text += "\\pard\\keep";
          rtf_text += inizioriga + "\n" + colore2 + bordi + cell1 + colore2 + bordi + cell2 + inizioriga2;
          rtf_text += "\\b Carta di Lavoro\\b0\\cell";
          rtf_text += "\\b Titolo\\b0\\cell";
          rtf_text += fineriga;
        }
        rtf_text += inizioriga + "\n" + ((index % 1 == 0) ? colore3 : colore4) + bordi + cell1 + ((index % 1 == 0) ? colore3 : colore4) + bordi + cell2 + inizioriga2;
        rtf_text += " " + dati.DefaultView[i]["NodoExtended"].ToString() + " \\cell";
        rtf_text += " " + dati.DefaultView[i]["Titolo"].ToString() + " \\cell";
        rtf_text += fineriga;
      }
      if (lastSessione != "")
      {
        rtf_text += "\\pard\\bgdkdcross\\cfpat1\\shading59110\\tx2291\\par";
      }
      rtf_text += "}";
      rtf_text = Convert2RTF(rtf_text);
      string filename = App.AppTempFolder + Guid.NewGuid().ToString();
      TextWriter tw = new StreamWriter(filename + ".rtf");
      tw.Write(rtf_text);
      tw.Close();
      System.Diagnostics.Process process = new System.Diagnostics.Process();
      process.Refresh();
      process.StartInfo.FileName = filename + ".rtf";
      process.StartInfo.ErrorDialog = false;
      process.StartInfo.Verb = "open";
      process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
      process.Start();
      //process.WaitForInputIdle(3000);
      IntPtr hWnd = process.MainWindowHandle;
      SetFocus(hWnd);
      SetForegroundWindow(hWnd);
      
    }

    public string Convert2RTFChar(string carattere)
    {
      string newChar = "";
      switch (carattere)
      {
        //case "!":
        //    newChar = "\\'21";
        //    break;
        case "\"":
          newChar = "\\'22";
          break;
        //case "#":
        //    newChar = "\\'23";
        //    break;
        case "$":
          newChar = "\\'24";
          break;
        case "%":
          newChar = "\\'25";
          break;
        case "&":
          newChar = "\\'26";
          break;
        case "'":
          newChar = "\\'27";
          break;
        //case "(":
        //    newChar = "\\'28";
        //    break;
        //case ")":
        //    newChar = "\\'29";
        //    break;
        //case "*":
        //    newChar = "\\'2a";
        //    break;
        //case "+":
        //    newChar = "\\'2b";
        //    break;
        //case ",":
        //    newChar = "\\'2c";
        //    break;
        //case "-":
        //    newChar = "\\'2d";
        //    break;
        //case ".":
        //    newChar = "\\'2e";
        //    break;
        //case "/":
        //    newChar = "\\'2f";
        //    break;
        //case ":":
        //    newChar = "\\'3a";
        //    break;
        //case ";":
        //    newChar = "\\'3b";
        //    break;
        //case "<":
        //    newChar = "\\'3c";
        //    break;
        //case "=":
        //    newChar = "\\'3d";
        //    break;
        //case ">":
        //    newChar = "\\'3e";
        //    break;
        //case "?":
        //    newChar = "\\'3f";
        //    break;
        //case "@":
        //    newChar = "\\'40";
        //    break;
        //case "[":
        //    newChar = "\\'5b";
        //    break;
        //case "\\":
        //    newChar = "\\'5c";
        //    break;
        //case "]":
        //    newChar = "\\'5d";
        //    break;
        //case "^":
        //    newChar = "\\'5e";
        //    break;
        //case "_":
        //    newChar = "\\'5f";
        //    break;
        //case "`":
        //    newChar = "\\'60";
        //    break;
        //case "{":
        //    newChar = "\\'7b";
        //    break;
        //case "|":
        //    newChar = "\\'7c";
        //    break;
        //case "}":
        //    newChar = "\\'7d";
        //    break;
        //case "~":
        //    newChar = "\\'7e";
        //    break;
        case "€":
          newChar = "\\'80";
          break;
        //case "?":
        //    newChar = "\\'82";
        //    break;
        //case "ƒ":
        //    newChar = "\\'83";
        //    break;
        //case ""
        //    newChar = "\\'84";
        //    break;
        case "…":
          newChar = "\\'85";
          break;
        //case "†":
        //    newChar = "\\'86";
        //case "‡":
        //    newChar = "\\'87";
        //    break;
        case "∘":
          newChar = "\\'88";
          break;
        //case "‰":
        //    newChar = "\\'89";
        //    break;
        //case "Š":
        //    newChar = "\\'8a";
        //    break;
        //case "‹":
        //    newChar = "\\'8b";
        //    break;
        //case "Œ":
        //    newChar = "\\'8c";
        //    break;
        //case "Ž":
        //    newChar = "\\'8e";
        //    break;
        //case "‘":
        //    newChar = "\\'91";
        //    break;
        case "’":
          newChar = "\\'92";
          break;
        case "“":
          newChar = "\\'93";
          break;
        case "”":
          newChar = "\\'94";
          break;
        //case "•":
        //    newChar = "\\'95";
        //    break;
        //case "–":
        //    newChar = "\\'96";
        //    break;
        //case "—":
        //    newChar = "\\'97";
        //    break;
        //case "~":
        //    newChar = "\\'98";
        //    break;
        //case "™":
        //    newChar = "\\'99";
        //    break;
        //case "š":
        //    newChar = "\\'9a";
        //    break;
        //case "›":
        //    newChar = "\\'9b";
        //    break;
        //case "œ":
        //    newChar = "\\'9c";
        //    break;
        //case "ž":
        //    newChar = "\\'9e";
        //    break;
        //case "Ÿ":
        //    newChar = "\\'9f";
        //    break;
        //case "¡":
        //    newChar = "\\'a1";
        //    break;
        //case "¢":
        //    newChar = "\\'a2";
        //    break;
        //case "£":
        //    newChar = "\\'a3";
        //    break;
        //case "¤":
        //    newChar = "\\'a4";
        //    break;
        //case "¥":
        //    newChar = "\\'a5";
        //    break;
        //case "¦":
        //    newChar = "\\'a6";
        //    break;
        //case "§":
        //    newChar = "\\'a7";
        //    break;
        //case "¨":
        //    newChar = "\\'a8";
        //    break;
        case "©":
          newChar = "\\'a9";
          break;
        //case "ª":
        //    newChar = "\\'aa";
        //    break;
        //case "«":
        //    newChar = "\\'ab";
        //    break;
        //case "¬":
        //    newChar = "\\'ac";
        //    break;
        //case "®":
        //    newChar = "\\'ae";
        //    break;
        //case "¯":
        //    newChar = "\\'af";
        //    break;
        case "°":
          newChar = "\\'b0";
          break;
        case "±":
          newChar = "\\'b1";
          break;
        case "²":
          newChar = "\\'b2";
          break;
        case "³":
          newChar = "\\'b3";
          break;
        //case "´":
        //    newChar = "\\'b4";
        //    break;
        case "µ":
          newChar = "\\'b5";
          break;
        //case "¶":
        //    newChar = "\\'b6";
        //    break;
        //case "•":
        //  newChar = "\\'b7";
        //break;
        //case "¸":
        //    newChar = "\\'b8";
        //    break;
        //case "¹":
        //    newChar = "\\'b9";
        //    break;
        //case "º":
        //    newChar = "\\'ba";
        //    break;
        //case "»":
        //    newChar = "\\'bb";
        //    break;
        //case "¼":
        //    newChar = "\\'bc";
        //    break;
        //case "½":
        //    newChar = "\\'bd";
        //    break;
        //case "¾":
        //    newChar = "\\'be";
        //    break;
        //case "¿":
        //    newChar = "\\'bf";
        //    break;
        case "À":
          newChar = "\\'c0";
          break;
        case "Á":
          newChar = "\\'c1";
          break;
        case "Â":
          newChar = "\\'c2";
          break;
        case "Ã":
          newChar = "\\'c3";
          break;
        case "Ä":
          newChar = "\\'c4";
          break;
        case "Å":
          newChar = "\\'c5";
          break;
        case "Æ":
          newChar = "\\'c6";
          break;
        case "Ç":
          newChar = "\\'c7";
          break;
        case "È":
          newChar = "\\'c8";
          break;
        case "É":
          newChar = "\\'c9";
          break;
        case "Ê":
          newChar = "\\'ca";
          break;
        case "Ë":
          newChar = "\\'cb";
          break;
        case "Ì":
          newChar = "\\'cc";
          break;
        case "Í":
          newChar = "\\'cd";
          break;
        case "Î":
          newChar = "\\'ce";
          break;
        case "Ï":
          newChar = "\\'cf";
          break;
        case "Ð":
          newChar = "\\'d0";
          break;
        case "Ñ":
          newChar = "\\'d1";
          break;
        case "Ò":
          newChar = "\\'d2";
          break;
        case "Ó":
          newChar = "\\'d3";
          break;
        case "Ô":
          newChar = "\\'d4";
          break;
        case "Õ":
          newChar = "\\'d5";
          break;
        case "Ö":
          newChar = "\\'d6";
          break;
        //case "×":
        //    newChar = "\\'d7";
        //    break;
        case "Ø":
          newChar = "\\'d8";
          break;
        case "Ù":
          newChar = "\\'d9";
          break;
        case "Ú":
          newChar = "\\'da";
          break;
        case "Û":
          newChar = "\\'db";
          break;
        case "Ü":
          newChar = "\\'dc";
          break;
        case "Ý":
          newChar = "\\'dd";
          break;
        case "Þ":
          newChar = "\\'de";
          break;
        case "ß":
          newChar = "\\'df";
          break;
        case "à":
          newChar = "\\'e0";
          break;
        case "á":
          newChar = "\\'e1";
          break;
        case "â":
          newChar = "\\'e2";
          break;
        case "ã":
          newChar = "\\'e3";
          break;
        case "ä":
          newChar = "\\'e4";
          break;
        case "å":
          newChar = "\\'e5";
          break;
        case "æ":
          newChar = "\\'e6";
          break;
        case "ç":
          newChar = "\\'e7";
          break;
        case "è":
          newChar = "\\'e8";
          break;
        case "é":
          newChar = "\\'e9";
          break;
        case "ê":
          newChar = "\\'ea";
          break;
        case "ë":
          newChar = "\\'eb";
          break;
        case "ì":
          newChar = "\\'ec";
          break;
        case "í":
          newChar = "\\'ed";
          break;
        case "î":
          newChar = "\\'ee";
          break;
        case "ï":
          newChar = "\\'ef";
          break;
        case "ð":
          newChar = "\\'f0";
          break;
        case "ñ":
          newChar = "\\'f1";
          break;
        case "ò":
          newChar = "\\'f2";
          break;
        case "ó":
          newChar = "\\'f3";
          break;
        case "ô":
          newChar = "\\'f4";
          break;
        case "õ":
          newChar = "\\'f5";
          break;
        case "ö":
          newChar = "\\'f6";
          break;
        case "÷":
          newChar = "\\'f7";
          break;
        case "ø":
          newChar = "\\'f8";
          break;
        case "ù":
          newChar = "\\'f9";
          break;
        case "ú":
          newChar = "\\'fa";
          break;
        case "û":
          newChar = "\\'fb";
          break;
        case "ü":
          newChar = "\\'fc";
          break;
        case "ý":
          newChar = "\\'fd";
          break;
        case "þ":
          newChar = "\\'fe";
          break;
        case "ÿ":
          newChar = "\\'ff";
          break;
      }
      return newChar;
    }

    public string Convert2RTFString(string buff, string replaceChar)
    {
      return buff.Replace(replaceChar, Convert2RTFChar(replaceChar));
    }

    private string Convert2RTF(string buff)
    {
      buff = buff.Replace("\\'", "\\#");
      buff = Convert2RTFString(buff, "'"); //va messo per primo o causa problemi
      buff = buff.Replace("\\#", "\\'");
      //for (char c = '!'; c <= 'ÿ'; c++)
      //{
      //    buff = Convert2RTFString(buff, c.ToString());
      //}
      buff = Convert2RTFString(buff, "%");
      buff = Convert2RTFString(buff, "ì");
      buff = Convert2RTFString(buff, "è");
      buff = Convert2RTFString(buff, "é");
      buff = Convert2RTFString(buff, "ò");
      buff = Convert2RTFString(buff, "à");
      buff = Convert2RTFString(buff, "ù");
      buff = Convert2RTFString(buff, "°");
      buff = Convert2RTFString(buff, "€");
      buff = Convert2RTFString(buff, "\"");
      buff = Convert2RTFString(buff, "’");
      buff = Convert2RTFString(buff, "”");
      buff = Convert2RTFString(buff, "“");
      return buff;
    }
  } //-------------------------------------------------------- class wDocumenti
} //--------------------------------------------- namespace RevisoftApplication

namespace ConvNS
{
  [ValueConversion(typeof(string), typeof(string))]
  public class ClienteConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string returnvalue = wDocumenti.GetClienteString((string)value);
      return returnvalue;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class TreeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string tipo = (string)value;
      switch (((App.TipoFile)(System.Convert.ToInt32(tipo))))
      {
        case App.TipoFile.Incarico:
                    case App.TipoFile.IncaricoCS:
            case App.TipoFile.IncaricoSU:
            case App.TipoFile.IncaricoREV:
          tipo = "1";
          break;
        case App.TipoFile.ISQC:
          tipo = "ISQC";
          break;
        case App.TipoFile.Revisione:
          tipo = "2";
          break;
        case App.TipoFile.Bilancio:
          tipo = "3";
          break;
        case App.TipoFile.Conclusione:
          tipo = "9";
          break;
        case App.TipoFile.Verifica:
          tipo = "4";
          break;
        case App.TipoFile.Vigilanza:
          tipo = "5";
          break;
        case App.TipoFile.PianificazioniVerifica:
          tipo = "P4";
          break;
        case App.TipoFile.PianificazioniVigilanza:
          tipo = "P5";
          break;
        default:
          break;
      }
      return tipo;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class TipoDocumentoConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string tipo = (string)value;
      if (tipo.Trim() == "")
      {
        return "";
      }
      else
      {
        int valore = 0;
        int.TryParse(tipo, out valore);
        if (valore == 0 || valore == 1)
        {
          return ((TipoDocumento)(System.Convert.ToInt32(tipo))).ToString();
        }
        else
        {
          return "";
        }
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class ImageEstensioneConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string tipo = (string)value;
      string estensione = "";
      string image = ".\\Images\\icone\\Stato\\nothing.png";
      if (tipo.Split('.').Count() > 0)
      {
        estensione = tipo.Split('.').Last();
      }
      switch (estensione)
      {
        case "doc":
        case "docx":
          image = ".\\Images\\icone\\Documenti\\word.png";
          break;
        case "pdf":
          image = ".\\Images\\icone\\Documenti\\pdf.png";
          break;
        case "xls":
        case "xlsx":
          image = ".\\Images\\icone\\Documenti\\excel.png";
          break;
        case "zip":
          image = ".\\Images\\icone\\Documenti\\zip.png";
          break;
        default:
          image = ".\\Images\\icone\\Documenti\\nothing.png";
          break;
      }
      return image;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }

  public class SessioneConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values[0].GetType().Name == "String" && values[1].GetType().Name == "String")
      {
        string returnvalue = wDocumenti.GetSessioneString((string)(values[0]), (string)(values[1]));
        return returnvalue;
      }
      return "";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }

  public class NodoConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values[0].GetType().Name == "String" && values[1].GetType().Name == "String" && values[2].GetType().Name == "String")
      {
        string returnvalue = wDocumenti.GetNodeString((string)(values[0]), (string)(values[1]), (string)(values[2]));
        return returnvalue;
      }
      return "";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
} //---------------------------------------------------------- namespace ConvNS
