using System;
using System.IO;
using System.Windows.Forms;
using The4Dimension;

namespace BYML_Editor
{

    public partial class Editor : Form
    {
        static DirectoryInfo tempPath = new DirectoryInfo($"{Path.GetTempPath()}/BYML-Editor");
        static FileInfo yamlPath = new FileInfo($"{Path.GetTempPath()}/BYML-Editor/temp.yaml");
        private bool IsXML;

        public Editor()
        {
            InitializeComponent();
        }

        private void OpenToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ConvertBYML(false);
        }

        private void OpenXMLDisplayToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ConvertBYML(true);
        }

        private void CreateXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Text = "";
            textBox.ReadOnly = false;
            IsXML = true;
        }

        private void CreateToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            textBox.Text = "";
            textBox.ReadOnly = true;
            IsXML = false;
        }

        private void ClearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            textBox.ReadOnly = true;
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //todo: big endian save
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                FileInfo savePath = new FileInfo(saveFileDialog.FileName);
                if (IsXML == true)
                {
                    if (savePath.Exists) savePath.Delete();
                    File.WriteAllBytes(savePath.FullName ,BymlConverter.GetByml(textBox.Text));
                }
                else
                { 
                if (yamlPath.Exists) yamlPath.Delete();
                File.WriteAllText(yamlPath.FullName, textBox.Text);
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C yml_to_byml.exe \"{yamlPath.FullName}\" \"{savePath.FullName}\""
                };
                process.StartInfo = startInfo;
                //todo: catch errors somehow
                process.Start();
                process.WaitForExit();
                }
                saveFileDialog.FileName = "";
            }
        }

        private void ConvertBYML(bool wantXML)
        {
            Directory.CreateDirectory(tempPath.FullName);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo selected = new FileInfo(openFileDialog.FileName);

                if (wantXML == false)
                {
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    if (yamlPath.Exists) yamlPath.Delete();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                        FileName = "cmd.exe",
                        Arguments = $"/C byml_to_yml.exe \"{selected.FullName}\" \"{yamlPath.FullName}\""
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();

                    textBox.Text = File.ReadAllText(yamlPath.FullName);
                    IsXML = false;
                }
                else
                {
                    textBox.Text = BymlConverter.GetXml(selected.FullName);
                    IsXML = true;
                }
                openFileDialog.FileName = "";
                textBox.ReadOnly = false;
            } 
        }
    }
}
