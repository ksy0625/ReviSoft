using mshtml;
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
using System.Windows.Threading;

namespace RevisoftApplication
{
  public partial class wGuidaRevisoft : Window
  {

    public string testoHtml;

    public wGuidaRevisoft()
    {
      InitializeComponent();
      winBorder.BorderBrush = App._arrBrushes[0];
    }

    public void MostraGuida()
    {
      testoHtml = "<div style='font-family:Calibri'>" + testoHtml.Replace("–", "").Replace("à", "&agrave;").Replace("è", "&egrave;").Replace("é", "&agrave;").Replace("ù", "&ugrave;").Replace("ò", "&ograve;").Replace("ì", "&igrave;").Replace("’", "'").Replace("°", "&deg;").Replace("≤", "&le;").Replace("≥", "&GreaterEqual;").Replace("·", "&middot;").Replace("<?xml:namespace prefix = \"o\" ns = \"urn:schemas-microsoft-com:office:office\" />", "") + "</div>";
      webBrowserHelp.NavigateToString(testoHtml);
    }

    private void btnEsci_Click(object sender, EventArgs e)
    {
      base.Close();
    }

  }
}
