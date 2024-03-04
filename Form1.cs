using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace NCEdit
{
    public partial class Form1 : Form
    {
        private ArrayList code_list = new ArrayList();
        private bool b_formating = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            
            label_linecount.Text = richTextBox1.Lines.Length.ToString();
            int i_index = richTextBox1.GetFirstCharIndexOfCurrentLine();
            if (!b_formating)
                set_dataformate(richTextBox1.GetLineFromCharIndex(i_index));
            set_line_color(richTextBox1.GetLineFromCharIndex(i_index));
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            int i_index = richTextBox1.GetFirstCharIndexOfCurrentLine();
            label_selectlineno.Text = (richTextBox1.GetLineFromCharIndex(i_index) + 1).ToString();

            
        }

        private void button_open_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.Text = openFileDialog1.FileName;
                System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog1.FileName);
                string[] lines = sr.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                //string[] lines = File.ReadAllLines(openFileDialog1.FileName);
                toolStripProgressBar1.Value = 0;
                toolStripProgressBar1.Maximum = lines.Length;
                flowLayoutPanel1.Enabled = false;
                richTextBox1.Enabled = false;
                try
                {
                    for (var i = 0; i < lines.Length; i += 1)
                    {
                        toolStripProgressBar1.Value = i;
                        string line = lines[i];
                        if (richTextBox1.Lines.Length == 0)
                            richTextBox1.AppendText(line);
                        else
                            richTextBox1.AppendText(Environment.NewLine + line);
                        // Process line
                    }
                    //richTextBox1.Text = sr.ReadToEnd();
                    sr.Close();
                }
                finally
                {
                    richTextBox1.Enabled = true;
                    flowLayoutPanel1.Enabled = true;
                }
                //label_linecount.Text = richTextBox1.Lines.Length.ToString();
            }
        }
        private void set_dataformate(int i_index)
        {
            b_formating = true;
            string[] stra_d;
            string n_temp = "";
            string g_temp = "";
            string d_temp = "";
            string s_temp = "";
            string e_temp = "";
            string old_data = "";
            string line_data = "";
            if (richTextBox1.Lines.Length>0)
                line_data = richTextBox1.Lines[i_index].ToUpper().Trim();
            if (line_data.Length == 0)
            {
                b_formating = false;
                return;
            }
            int start = richTextBox1.GetFirstCharIndexFromLine(i_index);
            int i_selectstart = richTextBox1.SelectionStart;
            
            code_list = get_arr(line_data, start);
            ArrayList arl_temp = new ArrayList();
            if (i_index == 0)
            {
                b_formating = false;
                return;
            }
            if (richTextBox1.Lines[i_index].ToString().Length == 0)
            {
                b_formating = false;
                return;
            }
            old_data = richTextBox1.Lines[i_index].ToString();
            for (int i = 0; i < code_list.Count; i++)
            {
                stra_d = code_list[i].ToString().Split(';');
                if (stra_d.Length>1)
                {
                    
                    if (stra_d[0].ToUpper() == "N")
                    {
                        //n_temp = stra_d[1].Replace('　',' ').Trim();
                        n_temp = stra_d[1].Trim();
                    }
                    
                    if (stra_d[0].ToUpper() == "G")
                    {
                        //g_temp = stra_d[1].Replace('　', ' ').Trim();
                        g_temp = stra_d[1].Trim();
                    }
                    
                    if ("NG".IndexOf(stra_d[0].ToUpper()) <0)
                    {
                        //arl_temp.Add(stra_d[1].Replace('　', ' ').Trim());
                        arl_temp.Add(stra_d[1].Trim());
                    }
                    if (n_temp.Length > 0)
                        s_temp = n_temp;
                    if (g_temp.Length>0)
                    {
                        //s_temp = n_temp.PadRight(4, '　') + g_temp;
                        s_temp = n_temp.PadRight(4, ' ') + g_temp;
                    }
                    if (arl_temp.Count>0)
                    {
                        //s_temp = n_temp.PadRight(4, '　') + g_temp.PadRight(4, '　');
                        s_temp = n_temp.PadRight(4, ' ') + g_temp.PadRight(4, ' ');
                    }
                }
            }
            //s_temp = n_temp.PadRight(4, ' ') + g_temp.PadRight(4, ' ');
            for (int i=0;i< arl_temp.Count;i++)
            {
                if (arl_temp[i].ToString().Length==1)
                    d_temp = d_temp + arl_temp[i];
                else if (i+1!= arl_temp.Count)
                    //d_temp = d_temp + arl_temp[i] + "　";
                    d_temp = d_temp + arl_temp[i] + " ";
                else
                    d_temp = d_temp + arl_temp[i];
            }
            s_temp = s_temp + d_temp;
            if (s_temp == old_data)
            {
                b_formating = false;
                return;
            }
            richTextBox1.SelectionStart =start;
            richTextBox1.SelectionLength = old_data.Length;
            richTextBox1.SelectedText = s_temp;//塞回文本..
            
            richTextBox1.Select(start+(s_temp.Length - old_data.Length) + (i_selectstart - start) +1, 0);
            b_formating = false;
            //richTextBox1.Select(i_index1+ s_temp.Length, 0);
        }
        private void set_line_color(int i_index)
        {
            /*

  P：程式號指令，設定子程式號（如子程式呼叫：M98P1000）

L：重複，設定子程式或固定迴圈重複次數（如：M98 P1000 L2，省略L代表L1）

P / W / R / Q：引數，固定迴圈使用的引數（如：攻牙G98 /（G99）G84 X_ Y_ R_ Z_ P_ F_）
                */
            if (richTextBox1.Lines.Length == 0)
            {
                tabPage1.Text = "";
                return;
            }
            if (i_index > richTextBox1.Lines.Length)
                return;

            //string line_data = richTextBox1.Lines[i_index].ToUpper().Replace('　', ' ').Trim();
            string line_data = richTextBox1.Lines[i_index].ToUpper();
            if (line_data.Length==0)
                return;
            int start = richTextBox1.GetFirstCharIndexFromLine(i_index);
            int i_c_start = richTextBox1.SelectionStart;
            int length = line_data.Length;
            if (i_index == 0 )
            {
                tabPage1.Text = line_data;
            }
            string s_temp = line_data;
            int i_s = 0;
            int i_n = 0;

            string[] stra_d;
            char character;
            code_list = get_arr(line_data, start);
            for (int i = 0; i < code_list.Count; i++)
            {
                stra_d = code_list[i].ToString().Split(';');
                
                if (stra_d[0]!="")
                    character = char.Parse(stra_d[0]);
                else
                    character = char.Parse("1");
                switch (character)
                {
                    case 'O':
                        //O：程式號，設定程式號
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32( stra_d[2]) , Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.Black;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                    case 'N':
                        //N：程式段號，設定程式順序號
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32(stra_d[2]), Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.Red;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                    case 'G':
                        //G：準備功能
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32(stra_d[2]), Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.BlueViolet;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                    case 'X':
                        //X / Y / Z ：尺寸字元，軸移動指令
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32(stra_d[2]), Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.Coral;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                    case 'Y':
                        //X / Y / Z ：尺寸字元，軸移動指令
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32(stra_d[2]), Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.MediumVioletRed;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                    case 'Z':
                        //X / Y / Z ：尺寸字元，軸移動指令
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32(stra_d[2]), Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.MidnightBlue;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                    case 'A':
                        //A / B / C / U / V / W：附加軸移動指令
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32(stra_d[2]), Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.Navy;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                    case 'R':
                        //R：圓弧半徑
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32(stra_d[2]), Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.DarkCyan;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                    case 'I':
                        //I / J / K：圓弧中心座標（向量）
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32(stra_d[2]), Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.DarkKhaki;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                    case 'F':
                        //F：進給，設定進給量
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32(stra_d[2]), Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.PaleVioletRed;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                    case 'S':
                        //S：主軸轉速，設定主軸轉速
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32(stra_d[2]), Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.MediumBlue;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                    case 'T':
                        //T：刀具功能，設定刀具號
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32(stra_d[2]), Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.Maroon;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                    case 'M':
                        //M：輔助功能，開 / 關控制功能
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32(stra_d[2]), Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.Olive;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                    case 'H':
                    case 'D':
                        //H / D：刀具偏置號，設定刀具偏置號
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32(stra_d[2]), Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.Orange;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                    case 'P':
                    case 'Q':
                    case 'W':
                        //P / W / R / Q：引數，固定迴圈使用的引數（如：攻牙G98 /（G99）G84 X_ Y_ R_ Z_ P_ F_）
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32(stra_d[2]), Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.CadetBlue;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                    default:
                        //無法解析
                        //richTextBox1.SelectionFont = new Font("Tahoma", 12, FontStyle.Bold);
                        richTextBox1.Select(Convert.ToInt32(stra_d[2]), Convert.ToInt32(stra_d[3]));
                        richTextBox1.SelectionColor = Color.Black;
                        richTextBox1.Select(i_c_start, 0);
                        break;
                }
            }
            
   
        }

        private void button_new_Click(object sender, EventArgs e)
        {
            this.Text = "NewFile";
            richTextBox1.Text = "";
            tabPage1.Text = "";
        }

        private ArrayList get_arr(string s_data,int i_start)
        {
            string s_codes = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string s_temp = s_data;
            ArrayList arl_list = new ArrayList();
            int i_index = -1;
            int i_index1 = 0;
            string s_code = "";
            int i_s = 0;
            int i_temp = 0;
            int i_e = 0;
            int i_length = s_data.Length;
            if (i_length == 0)
                return arl_list;

            while (i_index1>=0)
            {
                s_code = "";
                i_length = s_temp.Length;
                if (i_length == 0)
                    break;
                i_index = s_codes.IndexOf(s_temp.Substring(0, 1));
                if (s_temp.Length==1)
                {
                    i_index1 = -1;
                }
                for (int j=0;j< i_length;j++)
                {

                    if (s_temp.Substring(j , 1).Length > 0)
                    {
                        if (i_index1 <= 0)
                        {
                            if (i_length > j + 1)
                                i_index1 = s_codes.IndexOf(s_temp.Substring(j + 1, 1));
                            
                        }
                        if (i_index1 > -1)
                        {
                            i_e = j+1 ;
                            i_index1 = 0;
                            break;
                        }
                        else
                        {
                            if (j > 0)
                            {
                                i_index1 = s_codes.IndexOf(s_temp.Substring(j, 1));
                            }
                            if (i_index1 > -1)
                            {
                                i_e = j ;
                                i_index1 = 0;
                                break;
                            }
                            else
                                i_e = j+1;
                        }

                    }
                    //else
                    //{
                    //    if (i_length > j + 1)
                    //    {
                    //        i_index1 = s_codes.IndexOf(s_temp.Substring(j + 1, 1));
                    //        if (i_index1 > -1)
                    //        {
                    //            if (i_index > 0)
                    //            {
                    //                i_e = j + 1;
                    //                break;
                    //            }
                    //            else
                    //            {
                    //                i_index = i_index1;
                    //                i_temp = j+1;
                    //                i_index1 = 0;
                    //                i_e = 1;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            i_e = j + 1;
                    //        }
                    //    }
                    //}

                    if (i_index1 == 0)
                        i_index1 = -1;
                }
                if (i_index > -1)
                {
                    s_code = s_temp.Substring(i_temp, 1);
                    arl_list.Add(s_code+";"+s_temp.Substring(0, i_e)+";"+ (i_start+i_s).ToString()+";"+(i_e).ToString());
                    s_temp = s_temp.Substring(i_e);
                    i_s = i_s + i_e;
                    if (i_index1>0)
                        i_index1 = 0;
                }
                else if(i_index1 > -1)
                {
                    arl_list.Add(s_code + ";" + s_temp.Substring(0, i_e) + ";" + (i_start + 0).ToString() + ";" + ( i_e).ToString());
                    s_temp = s_temp.Substring(i_e);
                    i_s = i_s + i_e;
                    if (i_index1 > 0)
                        i_index1 = 0;
                }
                else
                {
                    arl_list.Add(s_code + ";" + s_temp + ";" + (i_start + 0).ToString() + ";" + (i_length).ToString());
                    s_temp = "";
                }
               
            }

            return arl_list;
        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLower( e.KeyChar))
                e.KeyChar =char.ToUpper(e.KeyChar );
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
        }

        private void 離開ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text.Trim().Length == 0)
                return;
            saveFileDialog1.FileName = tabPage1.Text.Replace(":","");
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                toolStripProgressBar1.Value = 0;
                toolStripProgressBar1.Maximum = richTextBox1.Lines.Length;
                flowLayoutPanel1.Enabled = false;
                richTextBox1.Enabled = false;
                try
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(saveFileDialog1.FileName);
                    for (int i = 0; i < richTextBox1.Lines.Length; i++)
                    {
                        toolStripProgressBar1.Value = i;
                        sw.WriteLine(richTextBox1.Lines[i].Replace(" ", ""));
                    }
                    sw.Close();
                }
                finally
                {
                    flowLayoutPanel1.Enabled = true;
                    richTextBox1.Enabled = true;
                }
            }
        }

        private void ToolStripMenuItem_new_Click(object sender, EventArgs e)
        {
            button_new_Click(sender,null);
        }

        private void ToolStripMenuItem_open_Click(object sender, EventArgs e)
        {
            button_open_Click(sender, null);
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button_save_Click(sender, null);
        }
    }
}
