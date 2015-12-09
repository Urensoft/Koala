﻿using System;
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
        public Koala.Compiler compiler = new Koala.Compiler();
        TextWriter _writer;

        public Form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                compiler.execute(textBoxInput.Lines);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            _writer = new TextBoxStreamWriter(textBoxOutput);
            // Redirect the out Console stream 
            Console.SetOut(_writer);
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
