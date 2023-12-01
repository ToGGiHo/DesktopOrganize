using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DesktopOrganize
{
    public partial class Form1 : Form
    {
        private string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private string defaultFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\desktopFiles";
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_load;
            this.Icon = Properties.Resources.icon;
            this.MaximizeBox = false;
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            textBox1.Text = defaultFolderPath;
            folderBrowserDialog1.SelectedPath = defaultFolderPath;
            toolTip1.SetToolTip(pictureBox1, "Приложение для массового удаления и перемещения файлов с рабочего стола");
        }

        private void Form1_load (object sender, EventArgs e)
        {
            LoadFilesAndFolders();
        }

        private void LoadFilesAndFolders()
        {
            string[] files = Directory.GetFiles(desktopPath);
            string[] folders = Directory.GetDirectories(desktopPath);

            // Очистить предыдущие элементы списка
            checkedListBox1.Items.Clear();


            if (radioButton1.Checked)
            {
                // Добавить файлы в список
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (!fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                        checkedListBox1.Items.Add("📄" + fileInfo.Name);
                }

                // Добавить папки в список
                foreach (string folder in folders)
                {
                    DirectoryInfo folderInfo = new DirectoryInfo(folder);
                    if (!folderInfo.Attributes.HasFlag(FileAttributes.Hidden))
                        checkedListBox1.Items.Add("📁" + folderInfo.Name);
                }
            }
            if (radioButton2.Checked)
            {
                // Добавить файлы в список
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (!fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                        checkedListBox1.Items.Add("📄" + fileInfo.Name);
                }
            }
            if (radioButton3.Checked)
            {
                // Добавить папки в список
                foreach (string folder in folders)
                {
                    DirectoryInfo folderInfo = new DirectoryInfo(folder);
                    if (!folderInfo.Attributes.HasFlag(FileAttributes.Hidden))
                        checkedListBox1.Items.Add("📁" + folderInfo.Name);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string destinationPath = textBox1.Text + @"\" + DateTime.Today.ToString("yyyy-MM-dd");
            if (checkedListBox1.CheckedItems.Count == 0)
            {
                MessageBox.Show("Не выбрано ни одного файла или папки.",
                    "Оповещение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            if (destinationPath.StartsWith(desktopPath))
            {
                MessageBox.Show("Путь назначения не может содержать в себе путь к рабочему столу",
                    "Оповещение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            foreach (object item in checkedListBox1.CheckedItems)
            {
                string itemName = item.ToString().Substring(2);
                string itemPath = desktopPath + @"\" + itemName;

                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }

                if (File.Exists(itemPath))
                {
                    string destinationFilePath = Path.Combine(destinationPath, itemName);
                    if (File.Exists(destinationFilePath))
                    {
                        // Изменить имя файла, добавив уникальный суффикс
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(itemName);
                        string fileExtension = Path.GetExtension(itemName);
                        string newFileName = GetUniqueFileName(destinationPath, fileNameWithoutExtension, fileExtension);
                        destinationFilePath = Path.Combine(destinationPath, newFileName);
                    }

                    File.Move(itemPath, destinationFilePath);
                }else if (Directory.Exists(itemPath))
                {
                    string destinationFolderPath = Path.Combine(destinationPath, itemName);
                    if (Directory.Exists(destinationFolderPath))
                    {
                        string newDirectoryName = GetUniqueDirectoryName(destinationPath, itemName);
                        destinationFolderPath = Path.Combine(destinationPath, newDirectoryName);
                    }
                    Directory.Move(itemPath, destinationFolderPath);
                }


            }
            checkedListBox1.Items.Clear();
            LoadFilesAndFolders();
            MessageBox.Show($"Файлы перемещены в папку: \n{Path.GetFullPath(destinationPath)}",
                    "Оповещение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            
        }
        private string GetUniqueFileName(string directoryPath, string fileNameWithoutExtension, string fileExtension)
        {
            string uniqueFileName = fileNameWithoutExtension + fileExtension;
            int counter = 1;

            while (File.Exists(Path.Combine(directoryPath, uniqueFileName)))
            {
                uniqueFileName = $"{fileNameWithoutExtension} ({counter}){fileExtension}";
                counter++;
            }

            return uniqueFileName;
        }

        private string GetUniqueDirectoryName(string parentDirectoryPath, string directoryName)
        {
            string uniqueDirectoryName = directoryName;
            int counter = 1;

            string fullPath = Path.Combine(parentDirectoryPath, uniqueDirectoryName);

            while (Directory.Exists(fullPath))
            {
                uniqueDirectoryName = $"{directoryName} ({counter})";
                fullPath = Path.Combine(parentDirectoryPath, uniqueDirectoryName);
                counter++;
            }

            return uniqueDirectoryName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            LoadFilesAndFolders();
            var items = checkedListBox1.Items.Cast<object>().ToList();
            if (checkBox1.Checked)
                foreach (object item in items)
                {
                    checkedListBox1.SetItemChecked(checkedListBox1.Items.IndexOf(item), true);
                }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkedListBox1.CheckedItems.Count == 0)
            {
                MessageBox.Show("Не выбрано ни одного файла или папки.",
                    "Оповещение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            if (MessageBox.Show("Вы точно хотите удалить выбранные файлы?",
                    "Оповещение",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.No) return;

            foreach (object item in checkedListBox1.CheckedItems)
            {
                string itemName = item.ToString().Substring(2);
                string itemPath = desktopPath + @"\" + itemName;
                if (File.Exists(itemPath))
                {
                    File.Delete(itemPath);
                }
            }
            checkedListBox1.Items.Clear();
            LoadFilesAndFolders();
            MessageBox.Show($"Файлы были удалены",
                    "Оповещение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var items = checkedListBox1.Items.Cast<object>().ToList();
            if (checkBox1.Checked)
            {
                foreach (object item in items)
                {
                    checkedListBox1.SetItemChecked(checkedListBox1.Items.IndexOf(item), true);
                }
            }
            else
            {
                foreach (object item in items)
                {
                    checkedListBox1.SetItemChecked(checkedListBox1.Items.IndexOf(item), false);
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            LoadFilesAndFolders();
            var items = checkedListBox1.Items.Cast<object>().ToList();
            if (checkBox1.Checked)
                foreach (object item in items)
                {
                    checkedListBox1.SetItemChecked(checkedListBox1.Items.IndexOf(item), true);
                }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            LoadFilesAndFolders();
            var items = checkedListBox1.Items.Cast<object>().ToList();
            if (checkBox1.Checked)
                foreach (object item in items)
                {
                    checkedListBox1.SetItemChecked(checkedListBox1.Items.IndexOf(item), true);
                }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            LoadFilesAndFolders();
            var items = checkedListBox1.Items.Cast<object>().ToList();
            if (checkBox1.Checked)
                foreach (object item in items)
                {
                    checkedListBox1.SetItemChecked(checkedListBox1.Items.IndexOf(item), true);
                }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
            
        }


    }
}
