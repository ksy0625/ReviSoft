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
  public partial class wSigilloPassword : Window
  {
    public string Password = "";
    public bool PasswordOK = false;

    public wSigilloPassword()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      txtPassword.Focus();
    }

    private void buttonApplica_Click(object sender, RoutedEventArgs e)
    {

      if (txtPassword.Password.Trim() == "" || txtPasswordRipeti.Password.Trim() == "")
      {
        MessageBox.Show("Le password sono campi obbligatori.");
        return;
      }

      //controllo selezione clienti
      if (txtPassword.Password.Trim() != txtPasswordRipeti.Password.Trim())
      {
        MessageBox.Show("Le password devono essere uguali");
        return;
      }

      Password = txtPassword.Password;

      PasswordOK = true;
      base.Close();
    }

    private void buttonAnnulla_Click(object sender, RoutedEventArgs e)
    {
      PasswordOK = false;
      base.Close();
    }
  }
}
