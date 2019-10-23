using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagLib;

namespace M3U_list_creater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public void SortBy(List<string> str, int col)
        {
            List<string> tmp = new List<string>();
            int i = 0;
            progressBar1.Value = 0;
            progressBar1.Maximum = str.Count;
            while (i < str.Count)
            {
                if (!System.IO.File.Exists(str[i]))
                {
                    str.RemoveAt(i);
                    i++;
                    continue;
                }
                bool valid = false;
                if (str[i].ToLower().Contains(".flac")) valid = true;
                if (str[i].ToLower().Contains(".mp3")) valid = true;
                if (str[i].ToLower().Contains(".wav")) valid = true;
                if (str[i].ToLower().Contains(".aac")) valid = true;
                if (str[i].ToLower().Contains(".ape")) valid = true;
                if (str[i].ToLower().Contains(".ogg")) valid = true;
                if (!valid)
                {
                    str.RemoveAt(i);
                    i++;
                    continue;
                }
                TagLib.File file = TagLib.File.Create(str[i]);
                if(file != null)
                {
                    switch(col)
                    {
                        case 0: tmp.Add(file.Tag.Title); break;
                        case 1: tmp.Add(file.Tag.FirstPerformer); break;
                        case 2: tmp.Add(file.Tag.Album + " " + file.Tag.Disc.ToString()); break;
                        case 3: tmp.Add(Convert.ToInt32(file.Properties.Duration.TotalSeconds).ToString()); break;
                        default: tmp.Add(file.Tag.Title); break;
                    }
                    tmp[tmp.Count - 1] = tmp[tmp.Count - 1] + "|" + str[i]; 
                }
                else
                {
                    str.RemoveAt(i);
                }
                i++;
                progressBar1.Value = progressBar1.Value + 1;
                progressBar1.Maximum = str.Count;
            }
            tmp.Sort();
            str.Clear();
            for(i = 0; i < tmp.Count;i++)
            {
                str.Add(tmp[i].Split('|')[1]);
            }
            tmp.Clear();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox1.Text) != true)
                return;
            List<string> tmp = new List<string>(Directory.GetFiles(textBox1.Text));
            List<string> fin = new List<string>();

            SortBy(tmp,comboBox1.SelectedIndex);
            progressBar1.Value = 0;
            progressBar1.Maximum = tmp.Count;
            fin.Add("#EXTM3U");
            for(int i = 0; i < tmp.Count;i++)
            {
                TagLib.File file = TagLib.File.Create(tmp[i]);
                fin.Add("#EXTINF:"+ Convert.ToInt32(file.Properties.Duration.TotalSeconds).ToString() + ","+file.Tag.Performers.ToString()+" - "+file.Tag.Title);
                fin.Add(Path.GetFileName(file.Name));
                progressBar1.Value = progressBar1.Value + 1;
            }
            richTextBox1.Clear();
            richTextBox1.Lines = fin.ToArray();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                if (Directory.Exists(folderBrowserDialog1.SelectedPath))
                    textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllLines(saveFileDialog1.FileName,richTextBox1.Lines);        
            }
        }
    }
}
