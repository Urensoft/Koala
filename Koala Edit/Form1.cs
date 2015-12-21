using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Koala;

namespace Koala_Edit
{

 

    public partial class Form1 : Form
    {
        public string[] code;
        public Koala.Compiler compiler = new Koala.Compiler();
        TextWriter _writer;

        public Form1()
        {
            InitializeComponent();
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {

            foreach(string s in Koala.DataTypes.Statements.statements)
            {
                statementsList.Items.Add(s);
            }
            _writer = new TextBoxStreamWriter(textBoxOutput);
            // Redirect the out Console stream 
            Console.SetOut(_writer);
        }


        private void runCode(object sender, DoWorkEventArgs e)
        {
            try
            {
                compiler.execute(code);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on line " + Error.currentLineNumber + ": " + ex.Message);
            }
        }

        private void startBackgroundWorker(object sender, EventArgs e)
        {
            code = textBoxInput.Lines;
            backgroundWorker1.RunWorkerAsync();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.progressBar1.Value = compiler.klogic.taskProgress;
            this.statusLabel.Text   = compiler.currentTask;
        }

        private void textBoxInput_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if(files.Count() > 1)
            {

            }
            else
            {
                var selectionIndex          = textBoxInput.SelectionStart;
                textBoxInput.Text           = textBoxInput.Text.Insert(selectionIndex, "\""+files[0]+"\"");
                textBoxInput.SelectionStart = selectionIndex + files[0].Length+2;
            }
        }

        private void textBoxInput_DragEnter(object sender, DragEventArgs e)
        {
                e.Effect = DragDropEffects.Copy;
            
        }

        public string selectedStatement;
        private void addStatementClicked(object sender, EventArgs e)
        {

        }

        private void statementsList_SelectedValueChanged(object sender, EventArgs e)
        {
           // selectedStatement = sender["Text"];
        }

        private void donateToUrensoftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DonationForm df = new DonationForm();
            df.Show();

        }
    }
    public class TextBoxStreamWriter : TextWriter
    {
        TextBox _output = null;

        public TextBoxStreamWriter(TextBox output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            _output.BeginInvoke(new Action(() =>
            {
                _output.AppendText(value.ToString());
            })
            ); // When character data is written, append it to the text box. 
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
