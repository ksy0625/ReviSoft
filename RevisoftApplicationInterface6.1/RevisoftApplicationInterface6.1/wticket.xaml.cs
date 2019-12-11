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
using System.Net;
using System.Collections.Specialized;


namespace RevisoftApplication
{
  public partial class wticket : Window
  {
    public ArrayList ContestiRiferimento = new ArrayList();

    public wticket()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      tiporichiesta.Items.Add("Anomalia");
      tiporichiesta.Items.Add("Richiesta informazioni");
      tiporichiesta.Items.Add("Licenza");
    }


    private bool IsValidEmail(string email)
    {
      try
      {
        var addr = new System.Net.Mail.MailAddress(email);
        return addr.Address == email;
      }
      catch
      {
        return false;
      }
    }


    private void ButtonApri_Click(object sender, RoutedEventArgs e)
    {
      if (email.Text == "")
      {
        MessageBox.Show("Indicare un indirizzo email");
        return;
      }
      if (!IsValidEmail(email.Text))
      {
        MessageBox.Show("Indirizzo email non corretto");
        return;
      }
      if (oggetto.Text == "")
      {
        MessageBox.Show("Indicare un oggetto");
        return;
      }
      if (tiporichiesta.Text == "")
      {
        MessageBox.Show("Indicare una tipologia di richiesta");
        return;
      }
      if (txtmessaggio.Text == "")
      {
        MessageBox.Show("Indicare un messaggio");
        return;
      }

      try
      {
        using (WebClient wc = new WebClient())
        {
          string txtm = txtmessaggio.Text;


          NameValueCollection parameters = new NameValueCollection();
          parameters.Add("email", email.Text);
          parameters.Add("oggetto", oggetto.Text);
          parameters.Add("messaggio", txtm); //.Replace(System.Environment.NewLine, " "));
          parameters.Add("tiporichiesta", tiporichiesta.Text);
          parameters.Add("gravita", "Normale");
          parameters.Add("utente", Environment.UserName);


          //    wc.QueryString = parameters;
          //    var responseBytes = wc.UploadFile(cBusinessObjects.url_ticket, @"C:\PROGETTI4\REVISOFT\modifiche.txt");
          // string response = Encoding.ASCII.GetString(responseBytes);

          byte[] responsebytes = wc.UploadValues(cBusinessObjects.url_ticket, "POST", parameters);
          string response = Encoding.UTF8.GetString(responsebytes);


          WindowConfermaTicket ct = new WindowConfermaTicket(response);
          ct.ShowDialog();

        }
      }
      catch (Exception)
      {
        MessageBox.Show("Impossibile inviare il messaggio. Riprovare pià tardi");

      }

      base.Close();
    }

    private void ButtonChiudi_Click_1(object sender, RoutedEventArgs e)
    {
      base.Close();
    }
  }
}
