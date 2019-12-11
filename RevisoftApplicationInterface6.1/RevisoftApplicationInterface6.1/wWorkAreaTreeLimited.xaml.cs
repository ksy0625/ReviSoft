using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Xml;
using System.Collections;
using UserControls;
using System.Windows.Media.Imaging;

namespace RevisoftApplication
{
  public partial class wWorkAreaTreeLimited : Window
  {
    public string SelectedTreeSource = "";
    public string SelectedDataSource = "";
    public string SelectedSessioneSource = "";

    public bool ReadOnly = true;
    public bool ApertoInSolaLettura = true;

    public string _cliente = "";
    public string Esercizio = "";

    public string SessioneAlias = "";
    public string SessioneAliasAdditivo = "";
    public string SessioneFile = "";
    public string SessioneID = "";

    private string selectedAlias = "";
    private string selectedAliasCodificato = "";
    private App.TipoAttivita _TipoAttivita = App.TipoAttivita.Sconosciuto;

    public string IDTree = "-1";
    public string IDCliente = "-1";
    public string IDSessione = "-1";

    private bool firsttime = true;

    XmlDataProviderManager _x;
    public XmlDataProvider TreeXmlProvider;

    Hashtable YearColor = new Hashtable();
    Hashtable htStati = new Hashtable();
    Hashtable htSessioni = new Hashtable();
    Hashtable htSessioniAlias = new Hashtable();
    Hashtable htSessioniID = new Hashtable();

    public string Cliente
    {
      get
      {
        return _cliente;
      }
      set
      {
        _cliente = value;
        txtTitoloRagioneSociale.Text = _cliente;
      }
    }

    public App.TipoAttivita TipoAttivita
    {
      get
      {
        return _TipoAttivita;
      }
      set
      {
        _TipoAttivita = value;
      }
    }

    public wWorkAreaTreeLimited()
    {
      InitializeComponent();
      txtTitoloAttivita.Foreground = App._arrBrushes[0];
      txtTitoloRagioneSociale.Foreground = App._arrBrushes[9];
      ButtonBar.Background = App._arrBrushes[12];
      SolidColorBrush tmpBrush = (SolidColorBrush)Resources["buttonHover"];
      tmpBrush.Color = ((SolidColorBrush)App._arrBrushes[13]).Color;

      MasterFile mf = MasterFile.Create();

      //string date = mf.GetData();

      //try
      //{
      //    if (Convert.ToDateTime(date) < DateTime.Now)
      //    {
      //        MessageBox.Show("Licenza scaduta");
      //        this.Close();
      //        return;
      //    }
      //}
      //catch (Exception ex)
      //{
      //    string log = ex.Message;
      //    this.Close();
      //    return;
      //}


      TreeXmlProvider = this.FindResource("xdpTree") as XmlDataProvider;

      //Colonna selezionata
      YearColor.Add(-1, "82BDE4");
      //Colori colonne di sezione
      YearColor.Add(2000, "F1F1F1");
      YearColor.Add(2001, "D3D3D3");
      YearColor.Add(2002, "F1F1F1");
      YearColor.Add(2003, "D3D3D3");
      YearColor.Add(2004, "F1F1F1");
      YearColor.Add(2005, "D3D3D3");
      YearColor.Add(2006, "F1F1F1");
      YearColor.Add(2007, "D3D3D3");
      YearColor.Add(2008, "F1F1F1");
      YearColor.Add(2009, "D3D3D3");
      YearColor.Add(2010, "F1F1F1");
      YearColor.Add(2011, "D3D3D3");
      YearColor.Add(2012, "F1F1F1");
      YearColor.Add(2013, "D3D3D3");
      YearColor.Add(2014, "F1F1F1");
      YearColor.Add(2015, "D3D3D3");
      YearColor.Add(2016, "F1F1F1");
      YearColor.Add(2017, "D3D3D3");
      YearColor.Add(2018, "F1F1F1");
      YearColor.Add(2019, "D3D3D3");
      YearColor.Add(2020, "F1F1F1");
    }

    #region TreeDataSource

    private void SaveTreeSource()
    {
      if (TreeXmlProvider.Document != null)
      {
        RevisoftApplication.XmlManager x = new XmlManager();
        x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
        x.SaveEncodedFile(SelectedTreeSource, TreeXmlProvider.Document.OuterXml);
      }

      ReloadStatoNodiPadre();
    }

    public void LoadTreeSource()
    {

      //Titolo attivita
      Utilities u = new Utilities();
      txtTitoloAttivita.Text = u.TitoloAttivita(_TipoAttivita);

      //carico dati
      RevisoftApplication.XmlManager x = new XmlManager();
      x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
      TreeXmlProvider.Document = x.LoadEncodedFile(SelectedTreeSource);

      if (!u.CheckXmlDocument(TreeXmlProvider.Document, ((App.TipoFile)(Convert.ToInt32(IDTree))), "Tree"))
      {
        this.Close();
        return;
      }

      if (firsttime)
      {
        firsttime = false;

        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("//Node"))
        {
          if (item.ParentNode.Name == "Tree")
          {
            item.Attributes["Expanded"].Value = "True";
          }
          else
          {
            item.Attributes["Expanded"].Value = "False";
          }

          item.Attributes["Selected"].Value = "False";

          if (item.Attributes["HighLighted"] == null)
          {
            XmlAttribute attr = item.OwnerDocument.CreateAttribute("HighLighted");
            attr.Value = "Black";
            item.Attributes.Append(attr);
          }

          item.Attributes["HighLighted"].Value = "Black";
        }
      }

      TreeXmlProvider.Refresh();
      LoadDataSource();


    }

    #endregion

    #region DataDataSource

    private string ConvertDataToEsercizio(string anno)
    {
      string returnvalue = "";

      MasterFile mf = MasterFile.Create();
      Hashtable clientetmp = mf.GetAnagrafica(Convert.ToInt32(IDCliente));

      switch ((App.TipoAnagraficaEsercizio)(Convert.ToInt32(clientetmp["Esercizio"].ToString())))
      {
        case App.TipoAnagraficaEsercizio.ACavallo:
          returnvalue = anno + "/" + ((Convert.ToInt32(anno) % 100) + 1).ToString();
          break;
        case App.TipoAnagraficaEsercizio.AnnoSolare:
        case App.TipoAnagraficaEsercizio.Sconosciuto:
        default:
          returnvalue = anno;
          break;
      }

      return returnvalue;
    }

    private void LoadDataSource()
    {
      _x = new XmlDataProviderManager(SelectedDataSource);

      Utilities u = new Utilities();
      if (!u.CheckXmlDocument(_x.Document, ((App.TipoFile)(Convert.ToInt32(IDTree))), "Data"))
      {
        this.Close();
        return;
      }

      ReloadNodi();
    }

    Hashtable ht = new Hashtable();

    private void ReloadNodi()
    {

      Hashtable htID = new Hashtable();
      Hashtable htAliasAdditivo = new Hashtable();
      ArrayList alCheckCompletezzaNodi = new ArrayList();

      List<DateTime> dates = new List<DateTime>();
      List<string> strings = new List<string>();
      bool alldates = true;

      htSessioni.Clear();
      htSessioniAlias.Clear();
      htSessioniID.Clear();

      RevisoftApplication.XmlManager x = new XmlManager();
      x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;

      for (int i = 0; i < SessioneFile.Split('|').Count(); i++)
      {
        ht.Add(SessioneAlias.Split('|')[i], SessioneFile.Split('|')[i]);
        htID.Add(SessioneAlias.Split('|')[i], SessioneID.Split('|')[i]);

        try
        {
          htAliasAdditivo.Add(SessioneAlias.Split('|')[i], SessioneAliasAdditivo.Split('|')[i]);
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wWorkAreaTreeLimited.ReloadNodi1 exception");
          string log = ex.Message;
          htAliasAdditivo.Add(SessioneAlias.Split('|')[i], "");
        }

        string aliastmp = SessioneAlias.Split('|')[i];

        strings.Add(aliastmp);

        if (aliastmp == "")
        {
          aliastmp = "31/12/" + DateTime.Now.Year.ToString();
        }

        DateTime data;
        try
        {
          data = Convert.ToDateTime(aliastmp);
          dates.Add(data);
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wWorkAreaTreeLimited.ReloadNodi2 exception");
          string log = ex.Message;
          alldates = false;
        }
      }

      if (alldates)
      {
        dates.Sort();
        dates.Reverse();
      }

      for (int i = 0; i < strings.Count; i++)
      {
        string alias;
        if (alldates)
        {
          alias = dates[i].ToShortDateString();
          if (alias == "31/12" + DateTime.Now.Year.ToString())
          {
            alias = "";
          }
        }
        else
        {
          alias = strings[i];
        }

        htSessioni.Add(i, ht[alias].ToString());
        htSessioniID.Add(i, htID[alias].ToString());

        XmlDocument tmpDoc = x.LoadEncodedFile(ht[alias].ToString());

        foreach (XmlNode node in tmpDoc.SelectNodes("/Dati//Dato"))
        {
          if (!alCheckCompletezzaNodi.Contains(node.Attributes["ID"].Value))
          {
            alCheckCompletezzaNodi.Add(node.Attributes["ID"].Value);
          }
        }
      }

      for (int i = 0; i < strings.Count; i++)
      {
        string alias;
        if (alldates)
        {
          alias = dates[i].ToShortDateString();
          if (alias == "31/12" + DateTime.Now.Year.ToString())
          {
            alias = "";
          }
        }
        else
        {
          alias = strings[i];
        }

        XmlDocument tmpDoc = x.LoadEncodedFile(ht[alias].ToString());

        XmlNode nodeTree = TreeXmlProvider.Document.SelectSingleNode("/Tree");
        if (nodeTree != null)
        {
          XmlNode nodeSessioni = nodeTree.SelectSingleNode("Sessioni");
          if (i == 0 && nodeSessioni != null)
          {
            nodeSessioni.ParentNode.RemoveChild(nodeSessioni);
            nodeSessioni = null;
          }

          if (nodeSessioni == null)
          {
            nodeSessioni = nodeTree.OwnerDocument.CreateNode(XmlNodeType.Element, "Sessioni", "");
            nodeTree.AppendChild(nodeSessioni);
            nodeSessioni = nodeTree.SelectSingleNode("Sessioni");
          }

          XmlNode nodeSessione = nodeTree.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + alias + "\"]");
          if (nodeSessione == null)
          {
            nodeSessione = nodeSessioni.OwnerDocument.CreateNode(XmlNodeType.Element, "Sessione", "");

            XmlAttribute attr = nodeSessioni.OwnerDocument.CreateAttribute("Alias");

            try
            {
              switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
              {
                case App.TipoFile.Bilancio:
                case App.TipoFile.Conclusione:
                case App.TipoFile.Revisione:
                  attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                  htSessioniAlias.Add(i, ConvertDataToEsercizio(alias.Split('/')[2]));
                  break;
                case App.TipoFile.Vigilanza:
                case App.TipoFile.Verifica:
                case App.TipoFile.Incarico:
                case App.TipoFile.IncaricoCS:
                case App.TipoFile.IncaricoSU:
                case App.TipoFile.IncaricoREV:
                  attr.Value = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "\r\n" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
                  htSessioniAlias.Add(i, alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : " - " + htAliasAdditivo[alias].ToString()));
                  break;
                case App.TipoFile.ISQC:
                  attr.Value = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
                  htSessioniAlias.Add(i, alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : " - " + htAliasAdditivo[alias].ToString()));
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
                  break;
              }
            }
            catch (Exception ex)
            {
              cBusinessObjects.logger.Error(ex, "wWorkAreaTreeLimited.ReloadNodi3 exception");
              string log = ex.Message;
              attr.Value = strings[i];
            }

            nodeSessione.Attributes.Append(attr);

            attr = nodeSessioni.OwnerDocument.CreateAttribute("Selected");
            if (SelectedDataSource == ht[alias].ToString())
            {
              switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
              {
                case App.TipoFile.Bilancio:
                case App.TipoFile.Conclusione:
                case App.TipoFile.Revisione:
                  selectedAliasCodificato = ConvertDataToEsercizio(alias.Split('/')[2]);
                  break;
                case App.TipoFile.Vigilanza:
                case App.TipoFile.Verifica:
                case App.TipoFile.Incarico:
                case App.TipoFile.IncaricoCS:
                case App.TipoFile.IncaricoSU:
                case App.TipoFile.IncaricoREV:
                  selectedAliasCodificato = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "\r\n" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
                  break;
                case App.TipoFile.ISQC:
                  selectedAliasCodificato = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
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
                  break;
              }

              selectedAlias = alias;
              attr.Value = "#AA" + YearColor[-1].ToString();
            }
            else
            {
              attr.Value = "White";
            }
            nodeSessione.Attributes.Append(attr);

            nodeSessioni.AppendChild(nodeSessione);
          }
        }

        foreach (string ID in alCheckCompletezzaNodi)
        {
          nodeTree = TreeXmlProvider.Document.SelectSingleNode("/Tree//Node[@ID=" + ID + "]");
          if (nodeTree != null)
          {
            XmlNode node = tmpDoc.SelectSingleNode("/Dati//Dato[@ID='" + ID + "']");

            if (node != null && !htStati.ContainsKey(node.Attributes["ID"].Value))
            {
              htStati.Add(node.Attributes["ID"].Value, node);
            }

            XmlNode nodeSessioni = nodeTree.SelectSingleNode("Sessioni");
            if (i == 0 && nodeSessioni != null)
            {
              nodeSessioni.ParentNode.RemoveChild(nodeSessioni);
              nodeSessioni = null;
            }

            if (nodeSessioni == null)
            {
              XmlNode newElemOut = nodeTree.OwnerDocument.CreateNode(XmlNodeType.Element, "Sessioni", "");
              nodeTree.AppendChild(newElemOut);
              nodeSessioni = nodeTree.SelectSingleNode("Sessioni");
            }

            XmlNode nodeSessione = nodeTree.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + alias + "\"]");
            if (nodeSessione == null)
            {
              nodeSessione = nodeSessioni.OwnerDocument.CreateNode(XmlNodeType.Element, "Sessione", "");

              XmlAttribute attr = nodeSessioni.OwnerDocument.CreateAttribute("Alias");

              try
              {
                switch ((App.TipoFile)(System.Convert.ToInt32(IDTree)))
                {
                  case App.TipoFile.Bilancio:
                  case App.TipoFile.Conclusione:
                  case App.TipoFile.Revisione:
                    attr.Value = ConvertDataToEsercizio(alias.Split('/')[2]);
                    break;
                  case App.TipoFile.Vigilanza:
                  case App.TipoFile.Verifica:
                  case App.TipoFile.Incarico:
                  case App.TipoFile.IncaricoCS:
                  case App.TipoFile.IncaricoSU:
                  case App.TipoFile.IncaricoREV:
                    attr.Value = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "\r\n" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
                    break;
                  case App.TipoFile.ISQC:
                    attr.Value = alias.Split('/')[0] + "/" + alias.Split('/')[1] + "/" + alias.Split('/')[2] + ((htAliasAdditivo[alias].ToString() == "") ? "" : "\r\n" + htAliasAdditivo[alias].ToString());
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
                    break;
                }
              }
              catch (Exception ex)
              {
                cBusinessObjects.logger.Error(ex, "wWorkAreaTreeLimited.ReloadNodi4 exception");
                string log = ex.Message;
                attr.Value = strings[i];
              }

              nodeSessione.Attributes.Append(attr);

              attr = nodeSessioni.OwnerDocument.CreateAttribute("Selected");
              if (SelectedDataSource == ht[alias].ToString())
              {
                attr.Value = "#AA" + YearColor[-1].ToString();
              }
              else
              {
                int anno = Convert.ToInt32(alias.Substring(alias.Length - 4, 4));

                if (i % 2 == 0)
                {
                  attr.Value = "#80" + YearColor[anno].ToString();
                }
                else
                {
                  attr.Value = "#AA" + YearColor[anno].ToString();
                }
              }
              nodeSessione.Attributes.Append(attr);

              attr = nodeSessioni.OwnerDocument.CreateAttribute("Stato");
              if (nodeTree != null && nodeTree.ParentNode != null && nodeTree.ParentNode.Name == "Tree")
              {
                if (SelectedDataSource == ht[alias].ToString() && nodeTree.Attributes["Osservazioni"] != null && nodeTree.Attributes["Osservazioni"].Value.Trim() != "")
                {
                  attr.Value = (Convert.ToInt32(App.TipoTreeNodeStato.NodoFazzoletto)).ToString();
                }
                else
                {
                  attr.Value = (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString();
                }
              }
              else
              {
                attr.Value = getStato(nodeTree, tmpDoc);

                if (nodeTree.Attributes["Report"].Value == "True")
                {
                  attr.Value = (Convert.ToInt32(App.TipoTreeNodeStato.SolaLettura)).ToString();
                }
              }

              nodeSessione.Attributes.Append(attr);

              nodeSessioni.AppendChild(nodeSessione);
            }
          }
        }
        _x.Save();
      }
    }

    private void ReloadStatoNodiPadre()
    {
      foreach (XmlNode nodeTree in TreeXmlProvider.Document.SelectSingleNode("/Tree//Node"))
      {
        if (nodeTree.ChildNodes.Count > 1 && nodeTree.Name == "Node")//nodeTree != null)
        {
          XmlNode nodeSessione = nodeTree.SelectSingleNode("Sessioni/Sessione[@Selected='#AA82BDE4']");

          if (nodeTree.ParentNode.Name == "Tree")
          {
            if (nodeTree.Attributes["Osservazioni"] != null && nodeTree.Attributes["Osservazioni"].Value.Trim() != "")
            {
              if (nodeSessione.Attributes["Stato"] != null)
              {
                nodeSessione.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.NodoFazzoletto)).ToString();
              }
            }
            else
            {
              if (nodeSessione.Attributes["Stato"] != null)
              {
                nodeSessione.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString();
              }
            }
          }
          else
          {
            if (nodeSessione.Attributes["Stato"] != null)
            {
              nodeSessione.Attributes["Stato"].Value = getStato(nodeTree, _x.Document);

              // ANDREA MessageBox.Show(nodeSessione.Attributes["Stato"].Value);
            }
          }
        }
      }

      _x.Save();
    }

    private string getStato(XmlNode nodeTree, XmlDocument tmpDoc)
    {
      string returnvalue = "";
      string statotmp = (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString();

      if (nodeTree.ChildNodes.Count > 1 && !(nodeTree.Attributes["Tipologia"].Value == "Nodo Multiplo") && !(nodeTree.Attributes["Report"].Value == "True"))
      {
        foreach (XmlNode nodesStati in nodeTree.ChildNodes)
        {
          if (nodesStati.Name == "Node")
          {
            if (returnvalue != (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString() /* ANDREA && returnvalue != (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString() */)
            {
              statotmp = getStato(nodesStati, tmpDoc);

              if (statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.DaCompletare)).ToString())
              {
                returnvalue = statotmp;
              }
              else
              {
                if (statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString())
                {
                  if (returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString() || returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.VociCompilate)).ToString())
                  {
                    returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.VociCompilate)).ToString();
                  }
                  else
                  {
                    returnvalue = statotmp;
                  }
                }
                else
                {
                  if ((statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString() || statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabileBucoTemplate)).ToString()) && ((returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabile)).ToString() || returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.NonApplicabileBucoTemplate)).ToString()) || returnvalue == ""))
                  {
                    returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString();
                  }

                  if (statotmp == (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString())
                  {
                    if (returnvalue == "")
                    {
                      returnvalue = statotmp;
                    }
                    else
                    {
                      if (returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.VociCompilate)).ToString() || returnvalue == (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString())
                      {
                        returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.VociCompilate)).ToString();
                      }
                    }
                  }
                }
              }
            }
            else
            {
              break;
            }
          }
        }
      }
      else
      {
        if (nodeTree.Attributes["Report"].Value == "True")
        {
          returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString();
        }
        else
        {
          XmlNode node = tmpDoc.SelectSingleNode("/Dati//Dato[@ID='" + nodeTree.Attributes["ID"].Value + "']");

          if (node != null)
          {
            if (node.Attributes["Stato"] != null)
            {
              returnvalue = node.Attributes["Stato"].Value;
            }
            else
            {
              XmlAttribute attr2 = node.OwnerDocument.CreateAttribute("Stato");
              attr2.Value = "-1";
              node.Attributes.Append(attr2);

              returnvalue = (Convert.ToInt32(App.TipoTreeNodeStato.Sconosciuto)).ToString();
            }
          }
          else
          {
            returnvalue = "-1";
          }
        }
      }

      return returnvalue;
    }

    #endregion

    private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      ;
    }

    private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      string SearchFor = ((TextBox)sender).Text.ToUpper();
      //int foundID = -1;
      bool found = false;

      if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
      {
        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
        {
          if (item.Attributes["Selected"] != null)
          {
            //if (item.Attributes["Selected"].Value == "True")
            //{
            //    foundID = Convert.ToInt32(item.Attributes["ID"].Value);
            //}

            item.Attributes["Selected"].Value = "False";
          }

          if (item.Attributes["HighLighted"] != null)
          {
            item.Attributes["HighLighted"].Value = "Black";
          }
        }

        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
        {
          if ( /*found == false && foundID != Convert.ToInt32(item.Attributes["ID"].Value) &&*/ (item.Attributes["Titolo"].Value.ToUpper().Contains(SearchFor) || item.Attributes["Codice"].Value.ToUpper().Contains(SearchFor)))
          {
            found = true;
            item.Attributes["HighLighted"].Value = "Red";

            if (item.ParentNode != null)
            {
              XmlNode parent = item.ParentNode;

              while (parent != null && parent.GetType().Name == "XmlElement" && parent.Name == "Node")
              {
                parent.Attributes["Expanded"].Value = "True";
                parent = parent.ParentNode;
              }
            }
          }
        }
      }

      if (found == false)
      {
        MessageBox.Show("Nessuna Carta di Lavoro presente per il testo ricercato");
      }
    }

    private void ItemsControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      ;
    }

    #region RICERCA_TESTO

    private void searchTextBox_KeyUp()
    {
      //if (e.Key == Key.Enter || e.Key == Key.Tab)
      //{
      string SearchFor = searchTextBox.Text.Trim().ToUpper();
      //int foundID = -1;
      bool found = false;

      if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
      {
        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
        {
          if (item.Attributes["Selected"] != null)
          {
            //if (item.Attributes["Selected"].Value == "True")
            //{
            //    foundID = Convert.ToInt32(item.Attributes["ID"].Value);
            //}

            item.Attributes["Selected"].Value = "False";
          }

          if (item.Attributes["Expanded"] != null)
          {
            if (item.ParentNode.Name == "Tree")
            {
              item.Attributes["Expanded"].Value = "True";
            }
            else
            {
              item.Attributes["Expanded"].Value = "False";
            }
          }

          if (item.Attributes["HighLighted"] != null)
          {
            item.Attributes["HighLighted"].Value = "Black";
          }
        }

        if (SearchFor == "")
        {
          return;
        }

        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
        {
          if (/*found == false && foundID != Convert.ToInt32(item.Attributes["ID"].Value) && */(item.Attributes["Titolo"].Value.ToUpper().Contains(SearchFor) || item.Attributes["Codice"].Value.ToUpper().Contains(SearchFor)))
          {
            found = true;
            item.Attributes["HighLighted"].Value = "Red";

            if (item.ParentNode != null)
            {
              XmlNode parent = item.ParentNode;

              while (parent != null && parent.GetType().Name == "XmlElement" && parent.Name == "Node")
              {
                parent.Attributes["Expanded"].Value = "True";
                parent = parent.ParentNode;
              }
            }
          }
        }
      }

      if (found == false)
      {
        MessageBox.Show("Nessuna Carta di Lavoro presente per il testo ricercato");
      }
      //}
    }

    private void buttonCerca_Click(object sender, RoutedEventArgs e)
    {
      searchTextBox_KeyUp();
    }

    private void buttonCercaAnnulla_Click(object sender, RoutedEventArgs e)
    {
      searchTextBox.Text = "";
      searchTextBox_KeyUp();
    }

    #endregion

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      ;
    }

    private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
    {
      ;
    }

    private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount != 2)
      {
        return;
      }

      XmlNode node;

      try
      {
        node = ((XmlNode)(tvMain.SelectedItem));
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wWorkAreaTreeLimited.OnItemMouseDoubleClick1 exception");
        string log = ex.Message;
        e.Handled = true;
        return;
      }

      if (node == null)
      {
        e.Handled = true;
        return;
      }

      if (node.ParentNode == null)
      {
        e.Handled = true;
        return;
      }

      if (node.ParentNode.Name == "Tree")
      {
        NodoFazzoletto o = new NodoFazzoletto();
        o.Owner = this;
        //MM   o.ApertoInSolaLettura = ApertoInSolaLettura;
        // o.ReadOnly = ReadOnly;
        o.ApertoInSolaLettura = false;
        o.ReadOnly = false;
        //  o.ApertoInSolaLettura = ApertoInSolaLettura;
        o.ReadOnly = ReadOnly;
        o.Nodo = node.Attributes["ID"].Value;
        o.Load(IDCliente);
        o.ShowDialog();
        ReloadNodi();
        SaveTreeSource();
        e.Handled = true;
        return;
      }

      try
      {
        WindowWorkArea wa = new WindowWorkArea(ref _x);

        //Nodi
        int index = -1;
        wa.NodeHome = -1;

        if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
        {
          foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
          {
            if (item.Attributes["Tipologia"].Value == "Nodo Multiplo" || item.ChildNodes.Count == 1)
            {
              index++;

              if (item.Attributes["ID"].Value == node.Attributes["ID"].Value)
              {
                wa.NodeHome = index;
              }

              wa.Nodes.Add(index, item);
            }
          }
        }

        if (wa.NodeHome == -1)
        {
          e.Handled = true;
          return;
        }

        wa.NodeNow = wa.NodeHome;

        wa.Owner = Window.GetWindow(this);

        //posizione e dimensioni finestra
        wa.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        switch (base.WindowState)
        {
          case System.Windows.WindowState.Normal:
            wa.Width = ActualWidth * 97 / 100;
            wa.Height = ActualHeight * 95 / 100;
            break;
          case System.Windows.WindowState.Maximized:
            wa.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 97 / 100;
            wa.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 95 / 100;
            break;
        }

        //Sessioni
        wa.Sessioni = htSessioni;
        wa.SessioniTitoli = htSessioniAlias;
        wa.SessioniID = htSessioniID;

        foreach (DictionaryEntry item in htSessioni)
        {
          if (item.Value.ToString() == _x.File)
          {
            wa.SessioneHome = Convert.ToInt32(item.Key.ToString());
            wa.SessioneNow = wa.SessioneHome;
            break;
          }
        }

        //Variabili
        wa.ReadOnly = ReadOnly;
        wa.ReadOnlyOLD = ReadOnly;
        wa.ApertoInSolaLettura = ApertoInSolaLettura;

        XmlNode nodeSessione = node.SelectSingleNode("Sessioni/Sessione[@Alias=\"" + selectedAliasCodificato + "\"]");
        if (nodeSessione != null)
        {
          wa.Stato = ((App.TipoTreeNodeStato)(Convert.ToInt32(nodeSessione.Attributes["Stato"].Value)));
          wa.OldStatoNodo = wa.Stato;
        }

        //passaggio dati
        wa.IDTree = IDTree;
        wa.IDSessione = IDSessione;
        wa.IDCliente = IDCliente;

        //apertura
        wa.Load();

        wa.ShowDialog();
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wWorkAreaTreeLimited.OnItemMouseDoubleClick2 exception");
        string log = ex.Message;
      }

      e.Handled = true;
    }

    private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (grdMainContainer.Visibility == System.Windows.Visibility.Collapsed)
      {
        grdMainContainer.Visibility = System.Windows.Visibility.Visible;
        //brdSearch.Visibility = System.Windows.Visibility.Visible; *** andrea
        var uriSource = new Uri("./Images/icone/navigate_up.png", UriKind.Relative);
        ((Image)sender).Source = new BitmapImage(uriSource);
      }
      else
      {
        grdMainContainer.Visibility = System.Windows.Visibility.Collapsed;
        //brdSearch.Visibility = System.Windows.Visibility.Collapsed; *** andrea
        var uriSource = new Uri("./Images/icone/navigate_down.png", UriKind.Relative);
        ((Image)sender).Source = new BitmapImage(uriSource);
      }
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    private void buttonApriFormulario_Click(object sender, RoutedEventArgs e)
    {
      Formulario formulario = new Formulario();
      formulario.Owner = this;
      formulario.LoadTreeSource();
      formulario.ShowDialog();
    }

    private void buttonApriDocumenti_Click(object sender, RoutedEventArgs e)
    {
      wDocumenti documenti = new wDocumenti();

      documenti.ReadOnly = ReadOnly;
      documenti.Titolo = "Indice Documenti per Cliente";
      documenti.Tipologia = TipoVisualizzazione.Documenti;
      documenti.Tree = IDTree;
      documenti.Cliente = IDCliente;
      documenti.Sessione = IDSessione;

      documenti.Owner = this;

      if (System.Windows.SystemParameters.PrimaryScreenWidth < 1100 || System.Windows.SystemParameters.PrimaryScreenHeight < 600)
      {
        documenti.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        documenti.Width = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        documenti.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        documenti.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
        documenti.MinHeight = System.Windows.SystemParameters.PrimaryScreenHeight * 90.0 / 100.0;
        documenti.MinWidth = System.Windows.SystemParameters.PrimaryScreenWidth * 90.0 / 100.0;
      }
      else
      {
        documenti.Width = 1100;
        documenti.Height = 600;
      }

      documenti.Load();
      documenti.ShowDialog();
    }

    private void btn_ScambioDati_Click(object sender, RoutedEventArgs e)
    {
      WindowWorkAreaTree_ScambioDati wWorkAreaSD = new WindowWorkAreaTree_ScambioDati();

      wWorkAreaSD.Owner = this;
      wWorkAreaSD.SelectedTreeSource = SelectedTreeSource;
      wWorkAreaSD.SelectedDataSource = SelectedDataSource;
      wWorkAreaSD.Cliente = _cliente;

      wWorkAreaSD.IDTree = IDTree;
      wWorkAreaSD.IDCliente = IDCliente;
      wWorkAreaSD.IDSessione = IDSessione;

      //andrea
      wWorkAreaSD.TitoloSessione = selectedAlias;
      wWorkAreaSD.Tipo = App.TipoScambioDati.Esporta;
      wWorkAreaSD.TipoAttivita = _TipoAttivita;

      //carico dati
      wWorkAreaSD.LoadTreeSource();
      wWorkAreaSD.ShowDialog();
    }

    private bool RecursiveCheck(XmlNode node)
    {
      bool returnvalue = false;

      if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
      {
        try
        {
          XmlNode NodoDato = _x.Document.SelectSingleNode("/Dati//Dato[@ID='" + node.Attributes["ID"].Value + "']");
          if (NodoDato.Attributes["Stato"].Value == (Convert.ToInt32(App.TipoTreeNodeStato.Completato)).ToString())
          {
            return true;
          }
        }
        catch (Exception ex)
        {
          cBusinessObjects.logger.Error(ex, "wWorkAreaTreeLimited.RecursiveCheck exception");
          string log = ex.Message;
        }
      }
      else
      {
        if (node.ParentNode.Name != "Tree")
        {
          foreach (XmlNode item in node.ChildNodes)
          {
            if (item.Name == "Node")
            {
              returnvalue = RecursiveCheck(item);
              if (returnvalue)
              {
                return true;
              }
            }
          }
        }
      }

      return returnvalue;
    }

    //private void RecursiveNode(XmlNode node, WordLib wl)
    private void RecursiveNode(XmlNode node, RTFLib wl, string nomefile)
    {
      if (node.ChildNodes.Count == 1 || node.Attributes["Tipologia"].Value == "Nodo Multiplo")
      {
        if (RecursiveCheck(node))
        {
          wl.Add(node, IDCliente, IDTree, IDSessione, nomefile);
        }
      }
      else
      {
        if (node.ParentNode.Name == "Tree" || RecursiveCheck(node))
        {
          if (node.ParentNode.Name != "Tree")
          {
            wl.AddTitle(node.Attributes["Codice"].Value + " " + node.Attributes["Titolo"].Value, true);
          }

          foreach (XmlNode item in node.ChildNodes)
          {
            if (item.Name == "Node")
            {
              RecursiveNode(item, wl, nomefile);
            }
          }
        }
      }
    }
  }
}