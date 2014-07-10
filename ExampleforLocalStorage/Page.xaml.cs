using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.Windows.Browser;
using System.IO;

namespace ExampleforLocalStorage
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();

            BindDirectories();
        }

        private void CreateFile_Click(object sender, RoutedEventArgs e)
        {
            if(this.DirectoryList.SelectedItem == null && this.DirectoryName.Text == "")
            {
                HtmlPage.Window.Alert("Please select a directory or enter directory name");
                return;
            }

            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                String filePath;
                if (this.DirectoryList.SelectedItem == null)
                {
                    filePath = System.IO.Path.Combine(this.DirectoryName.Text, this.FileName.Text + ".txt");
                }
                else
                {
                    filePath = System.IO.Path.Combine(this.DirectoryList.SelectedItem.ToString(), this.FileName.Text + ".txt");
                }

                IsolatedStorageFileStream fileStream = iso.CreateFile(filePath);
                using (StreamWriter sw = new StreamWriter(fileStream))
                {
                    sw.WriteLine(this.TextFile.Text);
                }
                fileStream.Close();
                HtmlPage.Window.Alert("O.K");
            }

        }

        private void CreateDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                string directoryName = this.DirectoryName.Text;

                if (this.DirectoryList.SelectedItem != null)
                {
                    directoryName = System.IO.Path.Combine(this.DirectoryList.SelectedItem.ToString(), directoryName);
                }

                if (!iso.DirectoryExists(directoryName))
                {
                    iso.CreateDirectory(directoryName);
                    HtmlPage.Window.Alert("O.K");
                }
            }
        }

        private void ReadFile_Click(object sender, RoutedEventArgs e)
        {
            if (this.DirectoryList.SelectedItem == null && this.FileList.SelectedIndex == null)
            {
                HtmlPage.Window.Alert("Please select and file firstly");
            }

            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                String filePath = null;
                if (this.DirectoryList.SelectedItem != null && this.FileList.SelectedItem != null)
                {
                     filePath = System.IO.Path.Combine(this.DirectoryList.SelectedItem.ToString(), this.FileList.SelectedItem.ToString());
                }
                else
                {
                    filePath = System.IO.Path.Combine(this.DirectoryName.Text, this.FileName.Text); 
                }

                if (iso.FileExists(filePath))
                {
                    StreamReader reader = new StreamReader(iso.OpenFile(filePath, FileMode.Open, FileAccess.Read));
                    this.TextFile.Text = reader.ReadToEnd();

                    this.DirectoryName.Text = this.DirectoryList.SelectedItem.ToString();
                    this.FileName.Text = this.FileList.SelectedItem.ToString();
                }
            }

        }

        private void DeleteFile_Click(object sender, RoutedEventArgs e)
        {
            if (this.DirectoryList.SelectedItem != null && this.FileList.SelectedItem != null)
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    string filePath = System.IO.Path.Combine(this.DirectoryList.SelectedItem.ToString(),
                        this.FileList.SelectedItem.ToString());
                    iso.DeleteFile(filePath);
                    HtmlPage.Window.Alert("O.K");
                }
            }

        }

        private void DeleteDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (this.DirectoryList.SelectedItem != null)
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    string directoryPath = this.DirectoryList.SelectedItem.ToString();
                    iso.DeleteDirectory(directoryPath);
                    HtmlPage.Window.Alert("O.K");
                }
            }
        }

        private void DirectoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.BindFiles();
        }

        void BindDirectories()
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                String[] directories = iso.GetDirectoryNames("*");
                this.DirectoryList.ItemsSource = directories;
            }
        }

        void BindFiles()
        {
            if (this.DirectoryList.SelectedItem != null)
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    String[] files = iso.GetFileNames(this.DirectoryList.SelectedItem.ToString() + "/");
                    this.FileList.ItemsSource = files;
                }
            }
        }

        private void AddQuota_Click(object sender, RoutedEventArgs e)
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                long newQuetaSize = 5242880;
                long curAvail = iso.AvailableFreeSpace;

                if (curAvail < newQuetaSize)
                {
                    iso.IncreaseQuotaTo(newQuetaSize);
                }
            }
        }
    }
}
