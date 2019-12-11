//----------------------------------------------------------------------------+
//                       wSigilloSbloccoBlocco.xaml.cs                        |
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
using System.Collections;
using System.ComponentModel;

namespace RevisoftApplication
{
  public partial class wSigilloSbloccoBlocco : Window
  {
    public string Titolo = "";
    public string IDCliente = "-1";
    public string AliasSessione = "";
    public XmlNode Nodo = null; // /Tree/Node/Sessioni/Sessione[@Selected='#AA82BDE4']
    public XmlNode NodoTree = null; // /Tree/Sessioni/Sessione[@Selected='#AA82BDE4']

    //----------------------------------------------------------------------------+
    //                           wSigilloSbloccoBlocco                            |
    //----------------------------------------------------------------------------+
    public wSigilloSbloccoBlocco()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      txtPassword.Focus();
    }

    //----------------------------------------------------------------------------+
    //                                    Load                                    |
    //----------------------------------------------------------------------------+
    public void Load()
    {
      labelTitolo.Content = Titolo;
    }

    //----------------------------------------------------------------------------+
    //                            buttonApplica_Click                             |
    //----------------------------------------------------------------------------+
    private void buttonApplica_Click_old(object sender, RoutedEventArgs e)
    {
      if (MessageBox.Show(
        "Si ricorda che il Sigillo viene applicato contemporaneamente ai " +
        "nodi 1) 2) 3) e 9) del cliente e dell'anno selezionato. La Rimozione " +
        "di questo sigillo equivale a rimuovere il Sigillo in toto. Procedere?",
        "ATTENZIONE", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
      {
        return;
      }
      GestioneLicenza gl = new GestioneLicenza();
      string intestatario = gl.Utente; // UtenteSigillo;
      if (Nodo.Attributes["Revisore"] != null
        && intestatario == Nodo.Attributes["Revisore"].Value)
      {
        if (Nodo.Attributes["Password"] != null
          && txtPassword.Password == Nodo.Attributes["Password"].Value)
        {
          MasterFile mf = MasterFile.Create();
          foreach (Hashtable item in mf.GetIncarichi(IDCliente))
          {
            if (ConvertDataToEsercizio(AliasSessione) == ConvertDataToEsercizio(item["DataNomina"].ToString()))
            {
              mf.RemoveSigilloIncarico(Convert.ToInt32(item["ID"].ToString()));
            }
          }
          foreach (Hashtable item in mf.GetISQCs(IDCliente))
          {
            if (ConvertDataToEsercizio(AliasSessione) == ConvertDataToEsercizio(item["DataNomina"].ToString()))
            {
              mf.RemoveSigilloISQC(Convert.ToInt32(item["ID"].ToString()));
            }
          }
          foreach (Hashtable item in mf.GetBilanci(IDCliente))
          {
            if (ConvertDataToEsercizio(AliasSessione) == ConvertDataToEsercizio(item["Data"].ToString()))
            {
              mf.RemoveSigilloBilancio(Convert.ToInt32(item["ID"].ToString()));
            }
          }
          foreach (Hashtable item in mf.GetConclusioni(IDCliente))
          {
            if (ConvertDataToEsercizio(AliasSessione) == ConvertDataToEsercizio(item["Data"].ToString()))
            {
              mf.RemoveSigilloConclusione(Convert.ToInt32(item["ID"].ToString()));
            }
          }
          foreach (Hashtable item in mf.GetRevisioni(IDCliente))
          {
            if (ConvertDataToEsercizio(AliasSessione) == ConvertDataToEsercizio(item["Data"].ToString()))
            {
              mf.RemoveSigilloRevisione(Convert.ToInt32(item["ID"].ToString()));
            }
          }
          Nodo.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.SigilloRotto)).ToString();
          ((WindowWorkAreaTree)Owner).ReadOnly = false;
        }
        else
        {
          MessageBox.Show("Attenzione! Password Errata");
        }
      }
      else
      {
        MessageBox.Show("Attenzione! Questo utente non è autorizzato a interagire con il Sigillo");
      }
      base.Close();
    }
    private void buttonApplica_Click(object sender, RoutedEventArgs e)
    {
#if (!DBG_TEST)
      buttonApplica_Click_old(sender, e);return;
#endif
      if (MessageBox.Show(
        "Si ricorda che il Sigillo viene applicato contemporaneamente ai " +
        "nodi 1) 2) 3) e 9) del cliente e dell'anno selezionato. La Rimozione " +
        "di questo sigillo equivale a rimuovere il Sigillo in toto. Procedere?",
        "ATTENZIONE", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
      {
        return;
      }
      GestioneLicenza gl = new GestioneLicenza();
      string intestatario = gl.Utente; // UtenteSigillo;
      if (Nodo.Attributes["Revisore"] != null
        && intestatario == Nodo.Attributes["Revisore"].Value)
      {
        if (Nodo.Attributes["Password"] != null
          && txtPassword.Password == Nodo.Attributes["Password"].Value)
        {
          MasterFile mf = MasterFile.Create();
          foreach (Hashtable item in mf.GetIncarichi(IDCliente))
          {
            if (ConvertDataToEsercizio(AliasSessione) == ConvertDataToEsercizio(item["DataNomina"].ToString()))
            {
              mf.RemoveSigilloIncarico(Convert.ToInt32(item["ID"].ToString()));
            }
          }
          foreach (Hashtable item in mf.GetISQCs(IDCliente))
          {
            if (ConvertDataToEsercizio(AliasSessione) == ConvertDataToEsercizio(item["DataNomina"].ToString()))
            {
              mf.RemoveSigilloISQC(Convert.ToInt32(item["ID"].ToString()));
            }
          }
          foreach (Hashtable item in mf.GetBilanci(IDCliente))
          {
            if (ConvertDataToEsercizio(AliasSessione) == ConvertDataToEsercizio(item["Data"].ToString()))
            {
              mf.RemoveSigilloBilancio(Convert.ToInt32(item["ID"].ToString()));
            }
          }
          foreach (Hashtable item in mf.GetConclusioni(IDCliente))
          {
            if (ConvertDataToEsercizio(AliasSessione) == ConvertDataToEsercizio(item["Data"].ToString()))
            {
              mf.RemoveSigilloConclusione(Convert.ToInt32(item["ID"].ToString()));
            }
          }
          foreach (Hashtable item in mf.GetRevisioni(IDCliente))
          {
            if (ConvertDataToEsercizio(AliasSessione) == ConvertDataToEsercizio(item["Data"].ToString()))
            {
              mf.RemoveSigilloRevisione(Convert.ToInt32(item["ID"].ToString()));
            }
          }
          Nodo.Attributes["Stato"].Value = (Convert.ToInt32(App.TipoTreeNodeStato.SigilloRotto)).ToString();
          StaticUtilities.MarkNodeAsModified(Nodo,App.OBJ_MOD);
          ((WindowWorkAreaTree)Owner).ReadOnly = false;
        }
        else
        {
          MessageBox.Show("Attenzione! Password Errata");
        }
      }
      else
      {
        MessageBox.Show("Attenzione! Questo utente non è autorizzato a interagire con il Sigillo");
      }
      base.Close();
    }

    //----------------------------------------------------------------------------+
    //                            buttonAnnulla_Click                             |
    //----------------------------------------------------------------------------+
    private void buttonAnnulla_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    //----------------------------------------------------------------------------+
    //                           ConvertDataToEsercizio                           |
    //----------------------------------------------------------------------------+
    private string ConvertDataToEsercizio(string data)
    {
      string returnvalue = "";
      MasterFile mf = MasterFile.Create();
      Hashtable clientetmp = mf.GetAnagrafica(Convert.ToInt32(IDCliente));
      switch (
        (App.TipoAnagraficaEsercizio)
          (Convert.ToInt32(clientetmp["Esercizio"].ToString())))
      {
        case App.TipoAnagraficaEsercizio.ACavallo:
          returnvalue = Convert.ToDateTime(data).Year.ToString() + " - " +
            (Convert.ToDateTime(data).Year + 1).ToString();
          break;
        case App.TipoAnagraficaEsercizio.AnnoSolare:
        case App.TipoAnagraficaEsercizio.Sconosciuto:
        default:
          returnvalue = Convert.ToDateTime(data).Year.ToString();
          break;
      }
      return returnvalue;
    }
  } // class wSigilloSbloccoBlocco
} // namespace RevisoftApplication