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
using System.ComponentModel;
using System.Data;

namespace UserControls
{
  public enum AltoMedioBasso
  {
    Sconosciuto,
    Alto,
    Medio,
    Basso
  };

  public partial class ucAltoMedioBasso : UserControl
  {
    public int id;
    private DataTable dati = null;


    private string _ID = "-1";

    private bool _isLoading = true;
    private bool _ReadOnly = false;

    public WindowWorkArea Owner;

    public ucAltoMedioBasso()
    {
      InitializeComponent();
    }

    public bool ReadOnly
    {
      set
      {
        if (value)
        {
          _ReadOnly = value;
          //rdbAlto.IsHitTestVisible = value;
          //rdbMedio.IsHitTestVisible = value;
          //rdbBasso.IsHitTestVisible = value;
        }
      }
    }

    public void Load(string ID, string IDCliente, string IDSessione)
    {
      _isLoading = true;

      id = int.Parse(ID.ToString());
      cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
      cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());

      _ID = ID;

      dati = cBusinessObjects.GetData(id, typeof(clsAltoMedioBasso));

      rdbAlto.IsChecked = false;
      rdbMedio.IsChecked = false;
      rdbBasso.IsChecked = false;
      foreach (DataRow dtrow in dati.Rows)
      {
        if (dtrow["value"] != null)
        {
          //AltoMedioBasso value = ((AltoMedioBasso)(Convert.ToInt32(dtrow["value"].ToString())));
          AltoMedioBasso value;
          if (string.IsNullOrEmpty(dtrow["value"].ToString())) value = AltoMedioBasso.Sconosciuto;
          else value = ((AltoMedioBasso)(Convert.ToInt32(dtrow["value"].ToString())));

          switch (value)
          {
            case AltoMedioBasso.Alto:
              rdbAlto.IsChecked = true;
              break;
            case AltoMedioBasso.Medio:
              rdbMedio.IsChecked = true;
              break;
            case AltoMedioBasso.Basso:
              rdbBasso.IsChecked = true;
              break;
            case AltoMedioBasso.Sconosciuto:
            default:
              break;
          }
        }
      }

      _isLoading = false;
    }

    public int Save(App.TipoTreeNodeStato StatoSalvataggio)
    {
      if (StatoSalvataggio == App.TipoTreeNodeStato.Completato && _ReadOnly == false && rdbAlto.IsChecked == false && rdbMedio.IsChecked == false && rdbBasso.IsChecked == false)
      {
          MessageBox.Show("Attenzione, non è stato attribuito alcun RISCHIO (A - M - B).", "Attenzione");
      
          Owner.ForzaturaADaCompletare = true;
      
      }
      /* MM				
          if (_ID == "201" || _ID == "205" || _ID == "217" || _ID == "218" || _ID == "219" || _ID == "220" || _ID == "256")
          {
            Hashtable NodiAlias = new Hashtable();
            NodiAlias.Add("201", "201");
            NodiAlias.Add("205", "204");
            NodiAlias.Add("217", "213");
            NodiAlias.Add("218", "214");
            NodiAlias.Add("219", "215");
            NodiAlias.Add("220", "216");
                    NodiAlias.Add("256", "256" );

            XmlNode NodoDaImportare = _x.Document.SelectSingleNode("/Dati//Dato[@ID='22']");
            XmlNode NodoDaImportare2 = _x.Document.SelectSingleNode("/Dati//Dato[@ID='202']");

            XmlNode NodoDaSostituire = _true_x.Document.SelectSingleNode("/Dati//Dato[@ID='22']");
            XmlNode NodoImportato = _x.Document.ImportNode(NodoDaSostituire, true);

            NodoDaImportare.ParentNode.AppendChild(NodoImportato);
            NodoDaImportare.ParentNode.RemoveChild(NodoDaImportare);

            XmlNode NodoDaSostituire2 = _true_x.Document.SelectSingleNode("/Dati//Dato[@ID='202']");
            XmlNode NodoImportato2 = _x.Document.ImportNode(NodoDaSostituire2, true);

            NodoDaImportare2.ParentNode.AppendChild(NodoImportato2);
            NodoDaImportare2.ParentNode.RemoveChild(NodoDaImportare2);

            NodoDaImportare = _x.Document.SelectSingleNode("/Dati//Dato[@ID='22']");
            NodoDaImportare2 = _x.Document.SelectSingleNode("/Dati//Dato[@ID='202']");

            XmlNode node = _x.Document.SelectSingleNode("/Dati/Dato[@ID='" + _ID + "']");

            if (node != null)
            {
              AltoMedioBasso valore = AltoMedioBasso.Sconosciuto;

              if (node.Attributes["value"] != null)
              {
                valore = ((AltoMedioBasso)(Convert.ToInt32(node.Attributes["value"].Value)));
              }

              switch (_ID)
              {
                case "201":
                            case "256":
                  if (NodoDaImportare.Attributes["txt1"] == null)
                  {
                    XmlAttribute attr = _x.Document.CreateAttribute("txt1");
                    NodoDaImportare.Attributes.Append(attr);
                  }
                  NodoDaImportare.Attributes["txt1"].Value = valore.ToString();
                  break;
                case "205":
                  if (NodoDaImportare.Attributes["txt2"] == null)
                  {
                    XmlAttribute attr = _x.Document.CreateAttribute("txt2");
                    NodoDaImportare.Attributes.Append(attr);
                  }
                  NodoDaImportare.Attributes["txt2"].Value = valore.ToString();
                  break;
                case "217":
                  if (NodoDaImportare.Attributes["txt3"] == null)
                  {
                    XmlAttribute attr = _x.Document.CreateAttribute("txt3");
                    NodoDaImportare.Attributes.Append(attr);
                  }
                  NodoDaImportare.Attributes["txt3"].Value = valore.ToString();
                  break;
                case "218":
                  if (NodoDaImportare.Attributes["txt4"] == null)
                  {
                    XmlAttribute attr = _x.Document.CreateAttribute("txt4");
                    NodoDaImportare.Attributes.Append(attr);
                  }
                  NodoDaImportare.Attributes["txt4"].Value = valore.ToString();
                  break;
                case "219":
                  if (NodoDaImportare.Attributes["txt5"] == null)
                  {
                    XmlAttribute attr = _x.Document.CreateAttribute("txt5");
                    NodoDaImportare.Attributes.Append(attr);
                  }
                  NodoDaImportare.Attributes["txt5"].Value = valore.ToString();
                  break;
                case "220":
                  if (NodoDaImportare.Attributes["txt6"] == null)
                  {
                    XmlAttribute attr = _x.Document.CreateAttribute("txt6");
                    NodoDaImportare.Attributes.Append(attr);
                  }
                  NodoDaImportare.Attributes["txt6"].Value = valore.ToString();
                  break;
                default:
                  break;
              }

              XmlNode nodeNodo = NodoDaImportare2.SelectSingleNode("Valore[@ID='" + _ID + "']");

              if (nodeNodo == null)
              {
                string xml = "<Valore ID='" + _ID + "'/>";

                XmlDocument doctmp = new XmlDocument();
                doctmp.LoadXml(xml);

                XmlNode tmpNode_int = doctmp.SelectSingleNode("Valore");
                XmlNode node_imp = _x.Document.ImportNode(tmpNode_int, true);

                NodoDaImportare2.AppendChild(node_imp);
                nodeNodo = NodoDaImportare2.SelectSingleNode("Valore[@ID='" + _ID + "']");
              }


              if (nodeNodo.Attributes["name"] == null)
              {
                XmlAttribute attr = _x.Document.CreateAttribute("name");
                nodeNodo.Attributes.Append(attr);
              }

              RevisoftApplication.XmlManager xt = new XmlManager();
              xt.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
              XmlDataProvider TreeXmlProvider = new XmlDataProvider();
              TreeXmlProvider.Document = xt.LoadEncodedFile(App.AppTemplateTreeRevisione);
              XmlNode tnode = TreeXmlProvider.Document.SelectSingleNode("/Tree//Node[@ID=" + NodiAlias[_ID].ToString() + "]");

              nodeNodo.Attributes["name"].Value = tnode.Attributes["Codice"].Value.Replace(".B", "") + " " + tnode.Attributes["Titolo"].Value;

              if (nodeNodo.Attributes["value"] == null)
              {
                XmlAttribute attr = _x.Document.CreateAttribute("value");
                nodeNodo.Attributes.Append(attr);
              }

              valore = AltoMedioBasso.Sconosciuto;

              if (node.Attributes["value"] != null)
              {
                valore = ((AltoMedioBasso)(Convert.ToInt32(node.Attributes["value"].Value)));
              }

              nodeNodo.Attributes["value"].Value = (Convert.ToInt32(valore)).ToString();
            }

            string risultato = AltoMedioBasso.Sconosciuto.ToString();

            if (NodoDaImportare.Attributes["txt1"] != null && NodoDaImportare.Attributes["txt1"].Value != AltoMedioBasso.Sconosciuto.ToString() && NodoDaImportare.Attributes["txt2"] != null &&  NodoDaImportare.Attributes["txt2"].Value != AltoMedioBasso.Sconosciuto.ToString())
            {
              if (NodoDaImportare.Attributes["txt1"].Value == AltoMedioBasso.Alto.ToString())
              {
                if (NodoDaImportare.Attributes["txt2"].Value == AltoMedioBasso.Alto.ToString())
                {
                  risultato = "Molto Basso";
                }
                else if (NodoDaImportare.Attributes["txt2"].Value == AltoMedioBasso.Medio.ToString())
                {
                  risultato = "Basso";
                }
                else if (NodoDaImportare.Attributes["txt2"].Value == AltoMedioBasso.Basso.ToString())
                {
                  risultato = "Medio";
                }
              }
              else if (NodoDaImportare.Attributes["txt1"].Value == AltoMedioBasso.Medio.ToString())
              {
                if (NodoDaImportare.Attributes["txt2"].Value == AltoMedioBasso.Alto.ToString())
                {
                                risultato = "Basso";
                }
                else if (NodoDaImportare.Attributes["txt2"].Value == AltoMedioBasso.Medio.ToString())
                {
                  risultato = "Medio";
                }
                else if (NodoDaImportare.Attributes["txt2"].Value == AltoMedioBasso.Basso.ToString())
                {
                  risultato = "Alto";
                }
              }
              else if (NodoDaImportare.Attributes["txt1"].Value == AltoMedioBasso.Basso.ToString())
              {
                if (NodoDaImportare.Attributes["txt2"].Value == AltoMedioBasso.Alto.ToString())
                {
                  risultato = "Medio";
                }
                else if (NodoDaImportare.Attributes["txt2"].Value == AltoMedioBasso.Medio.ToString())
                {
                                risultato = "Alto";
                }
                else if (NodoDaImportare.Attributes["txt2"].Value == AltoMedioBasso.Basso.ToString())
                {
                                risultato = "Molto Alto";
                }
              }
            }

            if (NodoDaImportare.Attributes["txt2c"] == null)
            {
              XmlAttribute attr = _x.Document.CreateAttribute("txt2c");
              NodoDaImportare.Attributes.Append(attr);
            }
            NodoDaImportare.Attributes["txt2c"].Value = risultato;

            risultato = AltoMedioBasso.Sconosciuto.ToString();

            if (NodoDaImportare.Attributes["txt1"] != null && NodoDaImportare.Attributes["txt1"].Value != AltoMedioBasso.Sconosciuto.ToString() && NodoDaImportare.Attributes["txt3"] != null &&  NodoDaImportare.Attributes["txt3"].Value != AltoMedioBasso.Sconosciuto.ToString())
            {
              if (NodoDaImportare.Attributes["txt1"].Value == AltoMedioBasso.Alto.ToString())
              {
                if (NodoDaImportare.Attributes["txt3"].Value == AltoMedioBasso.Alto.ToString())
                {
                  risultato = "Molto Basso";
                }
                else if (NodoDaImportare.Attributes["txt3"].Value == AltoMedioBasso.Medio.ToString())
                {
                                risultato = "Basso";
                }
                else if (NodoDaImportare.Attributes["txt3"].Value == AltoMedioBasso.Basso.ToString())
                {
                  risultato = "Medio";
                }
              }
              else if (NodoDaImportare.Attributes["txt1"].Value == AltoMedioBasso.Medio.ToString())
              {
                if (NodoDaImportare.Attributes["txt3"].Value == AltoMedioBasso.Alto.ToString())
                {
                                risultato = "Basso";
                }
                else if (NodoDaImportare.Attributes["txt3"].Value == AltoMedioBasso.Medio.ToString())
                {
                  risultato = "Medio";
                }
                else if (NodoDaImportare.Attributes["txt3"].Value == AltoMedioBasso.Basso.ToString())
                {
                  risultato = "Alto";
                }
              }
              else if (NodoDaImportare.Attributes["txt1"].Value == AltoMedioBasso.Basso.ToString())
              {
                if (NodoDaImportare.Attributes["txt3"].Value == AltoMedioBasso.Alto.ToString())
                {
                  risultato = "Medio";
                }
                else if (NodoDaImportare.Attributes["txt3"].Value == AltoMedioBasso.Medio.ToString())
                {
                  risultato = "Alto";
                }
                else if (NodoDaImportare.Attributes["txt3"].Value == AltoMedioBasso.Basso.ToString())
                {
                                risultato = "Molto Alto";
                }
              }
            }

            if (NodoDaImportare.Attributes["txt3c"] == null)
            {
              XmlAttribute attr = _x.Document.CreateAttribute("txt3c");
              NodoDaImportare.Attributes.Append(attr);
            }
            NodoDaImportare.Attributes["txt3c"].Value = risultato;

            risultato = AltoMedioBasso.Sconosciuto.ToString();

            if (NodoDaImportare.Attributes["txt1"] != null && NodoDaImportare.Attributes["txt1"].Value != AltoMedioBasso.Sconosciuto.ToString() && NodoDaImportare.Attributes["txt4"] != null &&  NodoDaImportare.Attributes["txt4"].Value != AltoMedioBasso.Sconosciuto.ToString())
            {
              if (NodoDaImportare.Attributes["txt1"].Value == AltoMedioBasso.Alto.ToString())
              {
                if (NodoDaImportare.Attributes["txt4"].Value == AltoMedioBasso.Alto.ToString())
                {
                  risultato = "Molto Basso";
                }
                else if (NodoDaImportare.Attributes["txt4"].Value == AltoMedioBasso.Medio.ToString())
                {
                                risultato = "Basso";
                }
                else if (NodoDaImportare.Attributes["txt4"].Value == AltoMedioBasso.Basso.ToString())
                {
                  risultato = "Medio";
                }
              }
              else if (NodoDaImportare.Attributes["txt1"].Value == AltoMedioBasso.Medio.ToString())
              {
                if (NodoDaImportare.Attributes["txt4"].Value == AltoMedioBasso.Alto.ToString())
                {
                                risultato = "Basso";
                }
                else if (NodoDaImportare.Attributes["txt4"].Value == AltoMedioBasso.Medio.ToString())
                {
                  risultato = "Medio";
                }
                else if (NodoDaImportare.Attributes["txt4"].Value == AltoMedioBasso.Basso.ToString())
                {
                  risultato = "Alto";
                }
              }
              else if (NodoDaImportare.Attributes["txt1"].Value == AltoMedioBasso.Basso.ToString())
              {
                if (NodoDaImportare.Attributes["txt4"].Value == AltoMedioBasso.Alto.ToString())
                {
                  risultato = "Medio";
                }
                else if (NodoDaImportare.Attributes["txt4"].Value == AltoMedioBasso.Medio.ToString())
                {
                                risultato = "Alto";
                }
                else if (NodoDaImportare.Attributes["txt4"].Value == AltoMedioBasso.Basso.ToString())
                {
                                risultato = "Molto Alto";
                }
              }
            }

            if (NodoDaImportare.Attributes["txt4c"] == null)
            {
              XmlAttribute attr = _x.Document.CreateAttribute("txt4c");
              NodoDaImportare.Attributes.Append(attr);
            }
            NodoDaImportare.Attributes["txt4c"].Value = risultato;

            risultato = AltoMedioBasso.Sconosciuto.ToString();

            if (NodoDaImportare.Attributes["txt1"] != null && NodoDaImportare.Attributes["txt1"].Value != AltoMedioBasso.Sconosciuto.ToString() && NodoDaImportare.Attributes["txt5"] != null &&  NodoDaImportare.Attributes["txt5"].Value != AltoMedioBasso.Sconosciuto.ToString())
            {
              if (NodoDaImportare.Attributes["txt1"].Value == AltoMedioBasso.Alto.ToString())
              {
                if (NodoDaImportare.Attributes["txt5"].Value == AltoMedioBasso.Alto.ToString())
                {
                  risultato = "Molto Basso";
                }
                else if (NodoDaImportare.Attributes["txt5"].Value == AltoMedioBasso.Medio.ToString())
                {
                                risultato = "Basso";
                }
                else if (NodoDaImportare.Attributes["txt5"].Value == AltoMedioBasso.Basso.ToString())
                {
                  risultato = "Medio";
                }
              }
              else if (NodoDaImportare.Attributes["txt1"].Value == AltoMedioBasso.Medio.ToString())
              {
                if (NodoDaImportare.Attributes["txt5"].Value == AltoMedioBasso.Alto.ToString())
                {
                                risultato = "Basso";
                }
                else if (NodoDaImportare.Attributes["txt5"].Value == AltoMedioBasso.Medio.ToString())
                {
                  risultato = "Medio";
                }
                else if (NodoDaImportare.Attributes["txt5"].Value == AltoMedioBasso.Basso.ToString())
                {
                  risultato = "Alto";
                }
              }
              else if (NodoDaImportare.Attributes["txt1"].Value == AltoMedioBasso.Basso.ToString())
              {
                if (NodoDaImportare.Attributes["txt5"].Value == AltoMedioBasso.Alto.ToString())
                {
                  risultato = "Medio";
                }
                else if (NodoDaImportare.Attributes["txt5"].Value == AltoMedioBasso.Medio.ToString())
                {
                                risultato = "Alto";
                }
                else if (NodoDaImportare.Attributes["txt5"].Value == AltoMedioBasso.Basso.ToString())
                {
                                risultato = "Molto Alto";
                }
              }
            }

            if (NodoDaImportare.Attributes["txt5c"] == null)
            {
              XmlAttribute attr = _x.Document.CreateAttribute("txt5c");
              NodoDaImportare.Attributes.Append(attr);
            }
            NodoDaImportare.Attributes["txt5c"].Value = risultato;

            risultato = AltoMedioBasso.Sconosciuto.ToString();

            if (NodoDaImportare.Attributes["txt1"] != null && NodoDaImportare.Attributes["txt1"].Value != AltoMedioBasso.Sconosciuto.ToString() && NodoDaImportare.Attributes["txt6"] != null &&  NodoDaImportare.Attributes["txt6"].Value != AltoMedioBasso.Sconosciuto.ToString())
            {
              if (NodoDaImportare.Attributes["txt1"].Value == AltoMedioBasso.Alto.ToString())
              {
                if (NodoDaImportare.Attributes["txt6"].Value == AltoMedioBasso.Alto.ToString())
                {
                  risultato = "Molto Basso";
                }
                else if (NodoDaImportare.Attributes["txt6"].Value == AltoMedioBasso.Medio.ToString())
                {
                                risultato = "Basso";
                }
                else if (NodoDaImportare.Attributes["txt6"].Value == AltoMedioBasso.Basso.ToString())
                {
                  risultato = "Medio";
                }
              }
              else if (NodoDaImportare.Attributes["txt1"].Value == AltoMedioBasso.Medio.ToString())
              {
                if (NodoDaImportare.Attributes["txt6"].Value == AltoMedioBasso.Alto.ToString())
                {
                                risultato = "Basso";
                }
                else if (NodoDaImportare.Attributes["txt6"].Value == AltoMedioBasso.Medio.ToString())
                {
                  risultato = "Medio";
                }
                else if (NodoDaImportare.Attributes["txt6"].Value == AltoMedioBasso.Basso.ToString())
                {
                  risultato = "Alto";
                }
              }
              else if (NodoDaImportare.Attributes["txt1"].Value == AltoMedioBasso.Basso.ToString())
              {
                if (NodoDaImportare.Attributes["txt6"].Value == AltoMedioBasso.Alto.ToString())
                {
                  risultato = "Medio";
                }
                else if (NodoDaImportare.Attributes["txt6"].Value == AltoMedioBasso.Medio.ToString())
                {
                                risultato = "Alto";
                }
                else if (NodoDaImportare.Attributes["txt6"].Value == AltoMedioBasso.Basso.ToString())
                {
                                risultato = "Molto Alto";
                }
              }
            }

            if (NodoDaImportare.Attributes["txt6c"] == null)
            {
              XmlAttribute attr = _x.Document.CreateAttribute("txt6c");
              NodoDaImportare.Attributes.Append(attr);
            }
            NodoDaImportare.Attributes["txt6c"].Value = risultato;

            NodoDaSostituire = _true_x.Document.SelectSingleNode("/Dati//Dato[@ID='22']");
            NodoImportato = _true_x.Document.ImportNode(NodoDaImportare, true);

            NodoDaSostituire.ParentNode.AppendChild(NodoImportato);
            NodoDaSostituire.ParentNode.RemoveChild(NodoDaSostituire);

            NodoDaSostituire2 = _true_x.Document.SelectSingleNode("/Dati//Dato[@ID='202']");
            NodoImportato2 = _true_x.Document.ImportNode(NodoDaImportare2, true);

            NodoDaSostituire2.ParentNode.AppendChild(NodoImportato2);
            NodoDaSostituire2.ParentNode.RemoveChild(NodoDaSostituire2);

            _true_x.Save();				
          }

          _x.Save();

          return _x;
                */
      return cBusinessObjects.SaveData(id, dati, typeof(clsAltoMedioBasso));
    }

    private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      double newsize = e.NewSize.Width - 30.0;

      try
      {
        grdMainContainer.Width = Convert.ToDouble(newsize);
      }
      catch (Exception ex)
      {
        string log = ex.Message;
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

    private void RadioButton_Checked(object sender, RoutedEventArgs e)
    {
      if (_isLoading)
      {
        return;
      }


      AltoMedioBasso value = AltoMedioBasso.Sconosciuto;

      if (rdbAlto.IsChecked == true)
      {
        value = AltoMedioBasso.Alto;
      }

      if (rdbMedio.IsChecked == true)
      {
        value = AltoMedioBasso.Medio;
      }

      if (rdbBasso.IsChecked == true)
      {
        value = AltoMedioBasso.Basso;
      }
      if (dati.Rows.Count == 0)
        dati.Rows.Add(id, cBusinessObjects.idcliente, cBusinessObjects.idsessione);
      foreach (DataRow dtrow in dati.Rows)
      {
        dtrow["value"] = (Convert.ToInt32(value)).ToString();
      }

    }
  }
}
