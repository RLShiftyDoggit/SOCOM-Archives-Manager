﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOCOM_Archives
{
    public partial class Form1 : Form
    {
        private CA_Archive MyArchive = new CA_Archive();
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dlgRet;

            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "*.ZDB; *.ZAR|*.zdb;*.zar";
            dlgRet = openFileDialog1.ShowDialog();
            if (dlgRet.ToString() == "Cancel") { return; }

            int filesLoaded = MyArchive.OpenArchive(openFileDialog1.FileName);
            if (filesLoaded <= 0)
            {
                MessageBox.Show("Not a valid archive");
                return;
            }
            txtArchive.Text = openFileDialog1.FileName;

            string vBlF = Convert.ToChar(0x0a).ToString();
            string vBCrLf = Convert.ToChar(0x0d).ToString() + Convert.ToChar(0x0a).ToString();
            lstListings.Items.Clear();

            txtBrowsePath.Text = "z:/";
            string[] sp = MyArchive.GetDIRListing(txtBrowsePath.Text).Split(Convert.ToChar(0x0a));

            lblFileCount.Text = "File Count: " + MyArchive.GetFileCount().ToString();
            for (int i = 0; i < sp.Length-1; i++)
            {
                lstListings.Items.Add(sp[i]);
            }
            switch (MyArchive.GetArchiveType())
            {
                case 0:
                    optTypeC8.Checked = false;
                    optTypeFC.Checked = false;
                    break;
                case 1:
                    optTypeC8.Checked = true;
                    optTypeFC.Checked = false;
                    break;
                case 2:
                    optTypeC8.Checked = false;
                    optTypeFC.Checked = true;
                    break;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstListings.SelectedIndex < 0) { return; }

            if (lstListings.Items[lstListings.SelectedIndex].ToString().Substring(0,6) == "[FILE]")
            {
                string[] spc = lstListings.Items[lstListings.SelectedIndex].ToString().Split(' ');
                int i = Convert.ToInt32(spc[1], 16);
                txtDetails.Text = MyArchive.GetFileDetails(i);
            }
        }
        private void lstListings_DoubleClick(object sender, EventArgs e)
        {
            if (lstListings.SelectedIndex < 0) { return; }
            if (lstListings.Items[lstListings.SelectedIndex].ToString().Substring(0, 5) == "[DIR]")
            {
                string[] spc = lstListings.Items[lstListings.SelectedIndex].ToString().Split(' ');
                if (spc[1] == ".")
                {
                    //txtBrowsePath.Text
                }
                else if (spc[1] == "..")
                {
                    string tmp = txtBrowsePath.Text + "/";
                    tmp = tmp.Replace("//", "/");
                    spc = tmp.Split('/');
                    tmp = spc[0] + "/";
                    for (int i = 1; i < spc.Length - 2; i++)
                    {
                        tmp += spc[i] + "/";
                    }
                    txtBrowsePath.Text = tmp;
                }
                else
                {
                    txtBrowsePath.Text += spc[1] + "/";
                }

                lstListings.Items.Clear();
                spc = MyArchive.GetDIRListing(txtBrowsePath.Text).Split(Convert.ToChar(0x0a));
                for (int i = 0; i < spc.Length - 1; i++)
                {
                    lstListings.Items.Add(spc[i]);
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (lstListings.SelectedIndex < 0) { return; }

            string[] spc = lstListings.Items[lstListings.SelectedIndex].ToString().Split(' ');
            int fIndex = Convert.ToInt32(spc[1], 16);

            if (spc[0] != "[FILE]") { return; }

            DialogResult dlgRet;

            saveFileDialog1.FileName = spc[spc.Length-1];
            saveFileDialog1.Filter = "*.*|*.*";
            dlgRet = saveFileDialog1.ShowDialog();
            if (dlgRet.ToString() == "Cancel") { return; }
            
            if (MyArchive.ExtractFile(saveFileDialog1.FileName, fIndex))
            {
                MessageBox.Show("Extracted Successfully!");
            }
            else
            {
                MessageBox.Show("Extract Failed!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (txtBrowsePath.Text.Length > 1)
            {
                frmNewFolder nfolder = new frmNewFolder();
                DialogResult dlgRet = nfolder.ShowDialog();

                MyArchive.AddFolder(txtBrowsePath.Text + nfolder.FolderName);
                nfolder.Dispose();

                lstListings.Items.Clear();
                string[] spc;
                spc = MyArchive.GetDIRListing(txtBrowsePath.Text).Split(Convert.ToChar(0x0a));
                for (int i = 0; i < spc.Length - 1; i++)
                {
                    lstListings.Items.Add(spc[i]);
                }
                lblFileCount.Text = "File Count: " + MyArchive.GetFileCount().ToString();
            }
        }

        private void optTypeFC_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] startupArgs = Environment.GetCommandLineArgs();
            
            if (startupArgs.Length > 1)
            {
                int filesLoaded = MyArchive.OpenArchive(startupArgs[1]);
                if (filesLoaded <= 0)
                {
                    MessageBox.Show("Not a valid archive");
                    return;
                }
                txtArchive.Text = startupArgs[1];

                string vBlF = Convert.ToChar(0x0a).ToString();
                string vBCrLf = Convert.ToChar(0x0d).ToString() + Convert.ToChar(0x0a).ToString();
                lstListings.Items.Clear();

                txtBrowsePath.Text = "z:/";
                string[] sp = MyArchive.GetDIRListing(txtBrowsePath.Text).Split(Convert.ToChar(0x0a));

                lblFileCount.Text = "File Count: " + MyArchive.GetFileCount().ToString();
                for (int i = 0; i < sp.Length - 1; i++)
                {
                    lstListings.Items.Add(sp[i]);
                }
                switch (MyArchive.GetArchiveType())
                {
                    case 0:
                        optTypeC8.Checked = false;
                        optTypeFC.Checked = false;
                        break;
                    case 1:
                        optTypeC8.Checked = true;
                        optTypeFC.Checked = false;
                        break;
                    case 2:
                        optTypeC8.Checked = false;
                        optTypeFC.Checked = true;
                        break;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (lstListings.SelectedIndex < 0) { return; }

            string lstName = lstListings.Items[lstListings.SelectedIndex].ToString();
            if (lstName.Substring(0,6) != "[FILE]") { return; }
            string fname = lstName.Substring(12, lstName.Length - 12);
            
            MyArchive.DeleteFile(txtBrowsePath.Text + fname, fname);

            lstListings.Items.Clear();
            string[] spc;
            spc = MyArchive.GetDIRListing(txtBrowsePath.Text).Split(Convert.ToChar(0x0a));
            for (int i = 0; i < spc.Length - 1; i++)
            {
                lstListings.Items.Add(spc[i]);
            }
            lblFileCount.Text = "File Count: " + MyArchive.GetFileCount().ToString();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult;

            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "All Files *.*|*.*";

            dlgResult = openFileDialog1.ShowDialog();
            if (dlgResult.ToString() == "Cancel") { return; }

            string[] nameSplit = openFileDialog1.FileName.Split('\\');
            string fName = nameSplit[nameSplit.Length - 1];

            MyArchive.AddFile(openFileDialog1.FileName, txtBrowsePath.Text + fName, 0);

            lstListings.Items.Clear();
            string[] spc;
            spc = MyArchive.GetDIRListing(txtBrowsePath.Text).Split(Convert.ToChar(0x0a));
            for (int i = 0; i < spc.Length - 1; i++)
            {
                lstListings.Items.Add(spc[i]);
            }
            lblFileCount.Text = "File Count: " + MyArchive.GetFileCount().ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult;

            saveFileDialog1.FileName = "MyArchive";
            saveFileDialog1.Filter = ".ZDB;.ZAR|*.zdb;*.zar";
            dlgResult = saveFileDialog1.ShowDialog();
            if (dlgResult.ToString() == "Cancel") { return; }

            MyArchive.SaveArchive(saveFileDialog1.FileName);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            MyArchive.FixAllChecksums();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented yet");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (optTypeC8.Checked)
                MyArchive.NewArchive(1);
            else if (optTypeFC.Checked)
                MyArchive.NewArchive(2);

            txtBrowsePath.Text = "z:/";
            txtArchive.Text = "<New Archive>";

            lstListings.Items.Clear();
            string[] spc;
            spc = MyArchive.GetDIRListing(txtBrowsePath.Text).Split(Convert.ToChar(0x0a));
            for (int i = 0; i < spc.Length - 1; i++)
            {
                lstListings.Items.Add(spc[i]);
            }
            lblFileCount.Text = "File Count: " + MyArchive.GetFileCount().ToString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented yet");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Not implemented yet");
        }
    }
}
