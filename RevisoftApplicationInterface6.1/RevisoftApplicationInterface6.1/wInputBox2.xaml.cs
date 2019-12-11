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
    public partial class wInputBox2 : Window
    {
        public bool OKOnly = false;

        public wInputBox2(string Value, string Value2, bool _OKOnly)
        {
            InitializeComponent();

            Title = "";
            ResponseTextBox.Width = ResponseTextBox.Width * 2.0;
            ResponseTextBox2.Width = ResponseTextBox2.Width * 2.0;

            ResponseTextBox.Text = Value;
            ResponseTextBox2.Text = Value2;

            OKOnly = _OKOnly;

            if (OKOnly)
            {
                ResponseTextBox.IsReadOnly = true;
                ResponseTextBox.KeyUp -= ResponseTextBox_KeyUp;
            }

            ResponseTextBox.Focus();
        }

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        public string ResponseText2
        {
            get { return ResponseTextBox2.Text; }
            set { ResponseTextBox2.Text = value; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (OKOnly)
            {
                this.Close();
                return;
            }

            //if ( ResponseText == "" && ResponseText2 == "")
            //{
            //    MessageBox.Show( "Attenzione: inserire un valore." );
            //    return;
            //}

            this.Close();
        }

        private void ResponseTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (OKOnly)
            {                
                return;
            }

            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                //if ( ResponseText == "" && ResponseText2 == "")
                //{
                //    MessageBox.Show( "Attenzione: inserire un valore." );
                //    return;
                //}

                this.Close();
            }
        }

        private void buttonChiudi_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
