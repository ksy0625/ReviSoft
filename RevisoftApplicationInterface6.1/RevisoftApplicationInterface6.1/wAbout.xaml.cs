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

    public partial class WindowAbout : Window
    {
        public WindowAbout()
        {
            InitializeComponent();

            //interfaccia
            label6.Content += App.AppVersioneAbout;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }
    }
}
