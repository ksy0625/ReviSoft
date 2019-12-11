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
    public partial class wInputBox : Window
    {
        public bool OKOnly = false;
        public bool diagres = false;

        public wInputBox(string Caption)
        {
            InitializeComponent();

            Title = Caption;
            CaptionTextBlock.Text = Caption;

            ResponseTextBox.Focus();
        }

        public wInputBox(string Caption, bool _OKOnly)
        {
            InitializeComponent();

            if(Caption == "Inserire Esecutore" || Caption == "Inserire Reviewer")
            {
                Title = "";
                ResponseTextBox.Width = ResponseTextBox.Width * 2.0;
            }
            else
            {
                Title = Caption;
            }
            
            CaptionTextBlock.Text = Caption;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (OKOnly)
            {
                this.Close();
                return;
            }

            if ( ResponseText == "" )
            {
                MessageBox.Show( "Attenzione: inserire un valore." );
                return;
            }
            diagres = true;
            this.Close();
        }

        private void ResponseTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (OKOnly)
            {                
                return;
            }
            /*
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if ( ResponseText == "" )
                {
                    MessageBox.Show( "Attenzione: inserire un valore." );
                    return;
                }

                this.Close();
            }
            */
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            diagres = false;
            this.Close();
        }

        private void buttonChiudi_Click(object sender, RoutedEventArgs e)
        {
            diagres = false;
            this.Close();
        }
    }
}
