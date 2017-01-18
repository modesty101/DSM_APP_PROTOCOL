using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;
namespace DSM_APP_PROTOCOL_CLONE
{
    public partial class Main : Form
    {
        //Guid GUID = Guid.NewGuid();
        Socket DSM;
        public Main()
        {
            InitializeComponent();
        }
        public void initialize()
        {
            DSM = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            DSM.Connect(Dns.GetHostEntry("dsm2015.cafe24.com").AddressList[0], 10306);
            new Thread(new ThreadStart(() =>
            {
                while (!this.IsDisposed && DSM.Connected)
                {
                    try
                    {
                        byte[] responseData = new byte[1024];
                        using (MemoryStream ms = new MemoryStream())
                        {

                            int numofBytes = DSM.Receive(responseData);
                            StringBuilder responseString = new StringBuilder();

                            while (numofBytes != 0)
                            {
                                Console.WriteLine(numofBytes.ToString());
                                ms.Write(responseData, 0, numofBytes);
                                if (numofBytes < responseData.Length)
                                {
                                    break;
                                }
                                else
                                {
                                    numofBytes = DSM.Receive(responseData);
                                }
                            }
                            byte[] data = ms.ToArray();
                            ShowMsg(Encoding.UTF8.GetString(data));

                            //JObject JO = JObject.Parse(Encoding.UTF8.GetString(data).Trim());
                            //if(JO["Command"] != null)
                            //{
                            //    if(JO["Command"].Equals("876"))
                            //    {
                            //        isMatched = true;
                            //    }
                            //}
                        }

                        //Thread.Sleep(100);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

            })).Start();

        }

        public void ShowMsg(string msg)
        {
            Invoke(new Action(() =>
            {
                Message_Textbox.AppendText(msg + Environment.NewLine + Environment.NewLine);
                Message_Textbox.Select(Message_Textbox.Text.Length, 0);
                Message_Textbox.ScrollToCaret();
            }));
        }

        public bool GetSurveyStream(int Position)
        {
            JObject Data = new JObject();
            Data.Add("Command", 1127);
            Data.Add("Data", Position);
            byte[] buffer = Encoding.UTF8.GetBytes(Data.ToString());
            DSM.Send(buffer);
            return true;
        }
        private void Main_Load(object sender, EventArgs e)
        {
            initialize();
            if (DSM.Connected)
            {
                Starting();
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            DSM.Shutdown(SocketShutdown.Both);
            DSM.Close();
            DSM.Dispose();
        }

        public bool Starting()
        {
            JObject Data = new JObject();
            Data.Add("Command", 1229);
            Data.Add("UUID", "LUA");

            byte[] buffer = Encoding.UTF8.GetBytes(Data.ToString());
            DSM.Send(buffer);
            return true;
        }

        public bool Login(String ID, String PW)
        {
            JObject Data = new JObject();
            Data.Add("Command", 750);
            Data.Add("ID", ID);
            Data.Add("Password", PW);
            Data.Add("UUID", "LUA");

            byte[] buffer = Encoding.UTF8.GetBytes(Data.ToString());
            DSM.Send(buffer);
            return true;
        }
        //{"Command":1206,"Data":"123"}719
        public bool Password_Modify(String PW)
        {
            JObject Data = new JObject();
            Data.Add("Command", 1206);
            Data.Add("Data", PW);

            byte[] buffer = Encoding.UTF8.GetBytes(Data.ToString());
            DSM.Send(buffer);
            // 로그인 안하면 719는 오지 않고, 719 오면 성공
            return true;
        }

        /*
        private void Getsurvey_Click(object sender, EventArgs e)
        {
            GetSurveyStream((int)SurverItem_Index.Value);


        }

        private void SetSurvey_Click(object sender, EventArgs e)
        {
           
        }

        public bool SurveyOutputStream(string Name, string Date, string Permission, List<KeyValuePair<string, string[]>> L_SurveyItem)
        {
            
            return true;
        }

        private void Register_Students_Click(object sender, EventArgs e)
        {

        }
      

        private void Password_Modify_Button_Click(object sender, EventArgs e)
        {
            
        }
        */

        // C# decimal Check(MSDN)
        private void GET_MEAL(Decimal Year, Decimal Month, Decimal Day)
        {
            JObject Data = new JObject();
            Data.Add("Command", 319);
            Data.Add("Year", (int)Year);
            Data.Add("Month", (int)Month);
            Data.Add("Day", (int)Day);

            byte[] buffer = Encoding.UTF8.GetBytes(Data.ToString());
            DSM.Send(buffer);
            //return true;
        }

        public void Notice(Decimal Position)
        {
            JObject Data = new JObject();
            Data.Add("Command", 619);
            Data.Add("Position",(int)Position);

            byte[] buffer = Encoding.UTF8.GetBytes(Data.ToString());
            DSM.Send(buffer);
        }

        public void Midnight_Study(bool Check1, bool Check2, bool Check3, bool Check4)
        {
            JObject Data = new JObject();
            Data.Add("Command", 315);

            if (Check1 == true)
            {
                Data.Add("Select","코딩실1");
            }
            else if (Check2 == true)
            {
                Data.Add("Select", "코딩실2");
            }
            else if (Check3 == true)
            {
                Data.Add("Select", "코딩실3");
            }
            else if (Check4 == true)
            {
                Data.Add("Select", "코딩실4");
            }
            else
                return;

            byte[] buffer = Encoding.UTF8.GetBytes(Data.ToString());
            DSM.Send(buffer);
        }

        public void Midnight_Check()
        {
            JObject Data = new JObject();
            Data.Add("Command", 314);

            byte[] buffer = Encoding.UTF8.GetBytes(Data.ToString());
            DSM.Send(buffer);

        }

        // 로그인
        private void Login_Click(object sender, EventArgs e)
        {
            Login(textBox1.Text, textBox2.Text);
        }
        // 비밀번호 수정
        private void Password_Modify_Click(object sender, EventArgs e)
        {
            Password_Modify(textBox3.Text);
        }
        // 급식 정보
        private void GET_MEAL_Click(object sender, EventArgs e)
        {
            GET_MEAL(numericUpDown1.Value, numericUpDown2.Value, numericUpDown3.Value);
        }
        // 가정통신문
        private void Notice_Click(object sender, EventArgs e)
        {
            Notice(numericUpDown7.Value);
        }
        // 연장학습 신청
        private void Midnight_Study_Click(object sender, EventArgs e)
        {
            Midnight_Study(checkBox1.Checked, checkBox2.Checked, checkBox3.Checked, checkBox4.Checked);
        }
        // 연장학습 인원 확인
        private void Midnight_Check_Click(object sender, EventArgs e)
        {
            Midnight_Check();
        }

        // 디자인
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
