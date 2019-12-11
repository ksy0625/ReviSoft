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

namespace RevisoftApplication
{

    public partial class WindowConfermaTicket : Window
    {
        public WindowConfermaTicket(string numticket)
        {
            InitializeComponent();
            label_numeroticket.Content = "E' stato aperto il ticket numero: " + numticket;


        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }
    }
}
