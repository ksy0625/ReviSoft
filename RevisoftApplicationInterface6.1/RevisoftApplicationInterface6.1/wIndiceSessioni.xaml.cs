//----------------------------------------------------------------------------+
//                          wIndiceSessioni.xaml.cs                           |
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
using System.Windows.Shapes;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Data.SqlClient;
using System.Data;

namespace RevisoftApplication
{
//============================================================================+
//                            class IndiceSessioni                            |
//============================================================================+
  public partial class IndiceSessioni : Window
  {
    private Hashtable ht = new Hashtable();
    public bool daPianificazione = false;
    public string Cliente = "-1";
    public string Nodo = "-1";
    public XmlNode node;
    public string Sessione = "-1";
    public string Tree = "-1";
    public XmlDataProviderManager _xh;
    public bool _isModified=false; // E.B. nuova variabile

    //----------------------------------------------------------------------------+
    //                               IndiceSessioni                               |
    //----------------------------------------------------------------------------+
    public IndiceSessioni()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
    }

    //----------------------------------------------------------------------------+
    //                             buttonChiudi_Click                             |
    //----------------------------------------------------------------------------+
    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    //----------------------------------------------------------------------------+
    //                              btnProcedi_Click                              |
    //----------------------------------------------------------------------------+
   
    private void btnProcedi_Click(object sender, RoutedEventArgs e)
    {
      if (lstSessioni.SelectedIndex == -1)
      {
        MessageBox.Show("Selezionare una voce oppure uscire premendo Chiudi");
        return;
      }
      string messaggio = "Sicuri di voler copiare i dati? I dati " +
        "attualmente presenti verranno sovrascritti";
      if (daPianificazione)
      {
        messaggio += "\r\n\r\nRICORDA DI MODIFICARE LE DATE";
      }
      if (MessageBox.Show(
        messaggio, "Attenzione", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
        return;
      //--------------------------------------------------------------------------+
      //       L' utente ha selezionato una sessione da cui copiare i dati        |
      //                e ha confermato l' operazione. Si procede.                |
      //--------------------------------------------------------------------------+
      MasterFile mf = MasterFile.Create();
      //Hashtable valori;
      //XmlNode  NodoDaImportare, NodoDaSostituire, NodoImportato, root, newNode;
      //string file, IDNodeList, ext, nuovonomefile;
      //XmlDataProviderManager _d,_t,y;
      //XmlNodeList nodelisttmp;
      //int newID;
            int cid;
      if (node.Name == "Node")
      {

     
            cid = int.Parse( node.Attributes["ID"].Value);
            int tbnum = 0;
            int oldIDSessione = cBusinessObjects.idsessione;
            DataSet dsimport = new DataSet();
            cBusinessObjects.idsessione = int.Parse(ht[lstSessioni.SelectedIndex].ToString());
            List<string> tableslist = cBusinessObjects.FindTablesById(cid);
            foreach (string tb in tableslist)
                {
                  cBusinessObjects.logger.Info("btnProcedi_Click: elenco tables " + tb);
                }

            foreach (string tb in tableslist)
            {
                string nomeclasse = "RevisoftApplication." + tb + ", RevisoftApplication";
                DataTable dati = cBusinessObjects.GetData(cid, Type.GetType(cBusinessObjects.getfullnomeclass(tb)));
                dsimport.Tables.Add(dati);
                dsimport.Tables[dsimport.Tables.Count - 1].TableName = tb + "|" + tbnum.ToString();
                tbnum++;
            }

            cBusinessObjects.idsessione = oldIDSessione;
            foreach (DataTable dt in dsimport.Tables)
            {
                if (dt.TableName == "DatiClienteSessioneAttivita")
                    continue;
           
                foreach (DataRow dtrow in dt.Rows)
                {
                    dtrow["ID_SESSIONE"] = cBusinessObjects.idsessione;  
                }
                string[] tokens = dt.TableName.Split('|');
                string nomeclasse = "RevisoftApplication." + tokens[0] + ", RevisoftApplication";
                cBusinessObjects.SaveData(cid, dt, Type.GetType(cBusinessObjects.getfullnomeclass(tokens[0])));
            }

       }

      foreach (XmlNode item in node.ChildNodes)
        {
            if (item.Name != "Node")
            {
                continue;
            }
        
    
             cid = int.Parse( item.Attributes["ID"].Value);
            int tbnum = 0;
            int oldIDSessione = cBusinessObjects.idsessione;
            DataSet dsimport = new DataSet();
            cBusinessObjects.idsessione = int.Parse(ht[lstSessioni.SelectedIndex].ToString());
             List<string> tableslist = cBusinessObjects.FindTablesById(cid);
            foreach (string tb in tableslist)
                {
                  cBusinessObjects.logger.Info("btnProcedi_Click: elenco tables " + tb);
                }

            foreach (string tb in tableslist)
            {
                string nomeclasse = "RevisoftApplication." + tb + ", RevisoftApplication";
                DataTable dati = cBusinessObjects.GetData(cid, Type.GetType(cBusinessObjects.getfullnomeclass(tb)));
                dsimport.Tables.Add(dati);
                dsimport.Tables[dsimport.Tables.Count - 1].TableName = tb + "|" + tbnum.ToString();
                tbnum++;
            }

            cBusinessObjects.idsessione = oldIDSessione;
            foreach (DataTable dt in dsimport.Tables)
            {
                if (dt.TableName == "DatiClienteSessioneAttivita")
                    continue;
           
                foreach (DataRow dtrow in dt.Rows)
                {
                    dtrow["ID_SESSIONE"] = cBusinessObjects.idsessione;  
                }
                string[] tokens = dt.TableName.Split('|');
                string nomeclasse = "RevisoftApplication." + tokens[0] + ", RevisoftApplication";
                cBusinessObjects.SaveData(cid, dt, Type.GetType(cBusinessObjects.getfullnomeclass(tokens[0])));
            }

        }

        if (App.m_xmlCache.Contains("RevisoftApp.rdocf")) App.m_xmlCache.Remove("RevisoftApp.rdocf");

        //if (tobesaved) _d.Save();
        base.Close();
        
        

    }

    //----------------------------------------------------------------------------+
    //                                    Load                                    |
    //----------------------------------------------------------------------------+
    public void Load()
    {
      MasterFile mf = MasterFile.Create();
      int index = 0;
      ArrayList al = new ArrayList();
      List<KeyValuePair<string, string>> myList =
        new List<KeyValuePair<string, string>>();

      switch ((App.TipoFile)(System.Convert.ToInt32(Tree)))
      {
        case App.TipoFile.Revisione:
          al = mf.GetRevisioni(Cliente);
          foreach (Hashtable item in al)
          {
            if (item["ID"].ToString() != Sessione)
            {
              myList.Add(new KeyValuePair<string, string>(
                item["ID"].ToString(), item["Data"].ToString().Replace("01/01/", "")));
            }
          }
          break;
        case App.TipoFile.PianificazioniVerifica:
          al = mf.GetPianificazioniVerifiche(Cliente);
          foreach (Hashtable item in al)
          {
            if (item["ID"].ToString() != Sessione)
            {
              myList.Add(new KeyValuePair<string, string>(
                item["ID"].ToString(), item["DataInizio"].ToString() + " - " +
                item["DataFine"].ToString()));
            }
          }
          break;
        case App.TipoFile.PianificazioniVigilanza:
          al = mf.GetPianificazioniVigilanze(Cliente);
          foreach (Hashtable item in al)
          {
            if (item["ID"].ToString() != Sessione)
            {
              myList.Add(new KeyValuePair<string, string>(
                item["ID"].ToString(), item["DataInizio"].ToString() + " - " +
                item["DataFine"].ToString()));
            }
          }
          break;
        case App.TipoFile.Verifica:
          al = mf.GetVerifiche(Cliente);
          foreach (Hashtable item in al)
          {
            if (item["ID"].ToString() != Sessione)
            {
              myList.Add(new KeyValuePair<string, string>(
                item["ID"].ToString(), item["Data"].ToString()));
            }
          }
          break;
        case App.TipoFile.Vigilanza:
          al = mf.GetVigilanze(Cliente);
          foreach (Hashtable item in al)
          {
            if (item["ID"].ToString() != Sessione)
            {
              myList.Add(new KeyValuePair<string, string>(
                item["ID"].ToString(), item["Data"].ToString()));
            }
          }
          break;
        case App.TipoFile.Incarico:
         al = mf.GetIncarichi(Cliente);
          foreach (Hashtable item in al)
          {
            if (item["ID"].ToString() != Sessione)
            {
              myList.Add(new KeyValuePair<string, string>(
                item["ID"].ToString(), item["DataNomina"].ToString()));
            }
          }
          break;
        case App.TipoFile.IncaricoCS:
         al = mf.GetIncarichi(Cliente,"CS");
          foreach (Hashtable item in al)
          {
            if (item["ID"].ToString() != Sessione)
            {
              myList.Add(new KeyValuePair<string, string>(
                item["ID"].ToString(), item["DataNomina"].ToString()));
            }
          }
          break;
        case App.TipoFile.IncaricoSU:
         al = mf.GetIncarichi(Cliente,"SU");
          foreach (Hashtable item in al)
          {
            if (item["ID"].ToString() != Sessione)
            {
              myList.Add(new KeyValuePair<string, string>(
                item["ID"].ToString(), item["DataNomina"].ToString()));
            }
          }
          break;
        case App.TipoFile.IncaricoREV:
          al = mf.GetIncarichi(Cliente,"REV");
          foreach (Hashtable item in al)
          {
            if (item["ID"].ToString() != Sessione)
            {
              myList.Add(new KeyValuePair<string, string>(
                item["ID"].ToString(), item["DataNomina"].ToString()));
            }
          }
          break;
        case App.TipoFile.ISQC:
          al = mf.GetISQCs(Cliente);
          foreach (Hashtable item in al)
          {
            if (item["ID"].ToString() != Sessione)
            {
              myList.Add(new KeyValuePair<string, string>(
                item["ID"].ToString(), item["DataNomina"].ToString()));
            }
          }
          break;
        case App.TipoFile.Bilancio:
          al = mf.GetBilanci(Cliente);
          foreach (Hashtable item in al)
          {
            if (item["ID"].ToString() != Sessione)
            {
              myList.Add(new KeyValuePair<string, string>(
                item["ID"].ToString(), item["Data"].ToString().Replace("01/01/", "")));
            }
          }
          break;
        case App.TipoFile.Conclusione:
          al = mf.GetConclusioni(Cliente);
          foreach (Hashtable item in al)
          {
            if (item["ID"].ToString() != Sessione)
            {
              myList.Add(new KeyValuePair<string, string>(
                item["ID"].ToString(), item["Data"].ToString().Replace("01/01/", "")));
            }
          }
          break;
        case App.TipoFile.Licenza:
        case App.TipoFile.Master:
        case App.TipoFile.Info:
        case App.TipoFile.Messagi:
        case App.TipoFile.ImportExport:
        case App.TipoFile.ImportTemplate:
        case App.TipoFile.BackUp:
        case App.TipoFile.Formulario:
        case App.TipoFile.ModellPredefiniti:
        case App.TipoFile.DocumentiAssociati:
        default:
          base.Close();
          break;
      } // switch

      myList.Sort(
        delegate(KeyValuePair<string, string> firstPair,
          KeyValuePair<string, string> nextPair)
        {
          return Convert.ToDateTime(((firstPair.Value.Contains(' ')) ?
            firstPair.Value.Split(' ')[0] : ((firstPair.Value.Length == 4) ?
              "01/01/" + firstPair.Value : firstPair.Value))).CompareTo(
                Convert.ToDateTime(((firstPair.Value.Contains(' ')) ?
                  firstPair.Value.Split(' ')[0] :((nextPair.Value.Length == 4) ?
                    "01/01/" + nextPair.Value : nextPair.Value))));
        });

      foreach (KeyValuePair<string, string> item in myList)
      {
        ht.Add(index++, item.Key);
        lstSessioni.Items.Add(item.Value);
      }
    }
  } // class IndiceSessioni
} // namespace RevisoftApplication
