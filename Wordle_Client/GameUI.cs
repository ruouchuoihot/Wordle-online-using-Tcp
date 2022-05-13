using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Wordle
{
    public partial class GameUI : Form
    {
        IPEndPoint IP;
        Socket client;

        string name;
        string answerCode = null;
        string result = null;
        int points = 0;
        int CountDown = 60;
        bool gameProcess = false;

        List<TextBox[]> rows = new List<TextBox[]>();
        TextBox[] row1 = new TextBox[5];
        TextBox[] row2 = new TextBox[5];
        TextBox[] row3 = new TextBox[5];
        TextBox[] row4 = new TextBox[5];
        TextBox[] row5 = new TextBox[5];
        TextBox[] row6 = new TextBox[5];
        int CorrectWords = 0;
        int rowIndex = 0, letterIndex = 0, wordIndex = 0;
        
        public GameUI()
        {
            InitializeComponent();
            //CheckForIllegalCrossThreadCalls = false;
        }

        private void TextEvent(object sender, EventArgs e)
        {
            //Nothing
        }

        private void TimerClient_Tick(object sender, EventArgs e)
        {
            if (gameProcess)
            {
                if (CountDown > 0)
                {
                    CountDown--;
                    lblTimer.Text = CountDown.ToString();
                    lblPoints.Text = points.ToString();
                }
                else if (CountDown == 0)
                {
                    ResetAll();
                    GameOver();
                }
                else { }
            }
        }
        private void GameOver()
        {
            gameProcess = false;

            Send("<EndGame>-" + name + "-" + points + "-" + CountDown);

            lblTimer.Text = "0";
            lblPoints.Text = "0";
            
            answerCode = null;
            wordIndex = 0;
            points = 0;
            CountDown = 60;
            while (true)
            {
                if (result != null)
                {
                    break;
                }
            }
            MessageBox.Show(result);
            result = null;

            btnReady.Enabled = true;
            btnGameUI.Enabled = true;
            groupJoin.Visible = true;
            groupJoin.Enabled = true;

        }
        private void ResetAll()
        {
            CorrectWords = 0; 
            rowIndex = 0;
            letterIndex = 0;
            //Word 1.
            word1_letter1.Clear();
            word1_letter2.Clear();
            word1_letter3.Clear();
            word1_letter4.Clear();
            word1_letter5.Clear();

            word1_letter1.BackColor = Color.Black;
            word1_letter2.BackColor = Color.Black;
            word1_letter3.BackColor = Color.Black;
            word1_letter4.BackColor = Color.Black;
            word1_letter5.BackColor = Color.Black;
            //Word 2.
            word2_letter1.Clear();
            word2_letter2.Clear();
            word2_letter3.Clear();
            word2_letter4.Clear();
            word2_letter5.Clear();

            word2_letter1.BackColor = Color.Black;
            word2_letter2.BackColor = Color.Black;
            word2_letter3.BackColor = Color.Black;
            word2_letter4.BackColor = Color.Black;
            word2_letter5.BackColor = Color.Black;
            //Word 3.
            word3_letter1.Clear();
            word3_letter2.Clear();
            word3_letter3.Clear();
            word3_letter4.Clear();
            word3_letter5.Clear();

            word3_letter1.BackColor = Color.Black;
            word3_letter2.BackColor = Color.Black;
            word3_letter3.BackColor = Color.Black;
            word3_letter4.BackColor = Color.Black;
            word3_letter5.BackColor = Color.Black;
            //Word 4.
            word4_letter1.Clear();
            word4_letter2.Clear();
            word4_letter3.Clear();
            word4_letter4.Clear();
            word4_letter5.Clear();

            word4_letter1.BackColor = Color.Black;
            word4_letter2.BackColor = Color.Black;
            word4_letter3.BackColor = Color.Black;
            word4_letter4.BackColor = Color.Black;
            word4_letter5.BackColor = Color.Black;
            //Word 5.
            word5_letter1.Clear();
            word5_letter2.Clear();
            word5_letter3.Clear();
            word5_letter4.Clear();
            word5_letter5.Clear();

            word5_letter1.BackColor = Color.Black;
            word5_letter2.BackColor = Color.Black;
            word5_letter3.BackColor = Color.Black;
            word5_letter4.BackColor = Color.Black;
            word5_letter5.BackColor = Color.Black;
            //Word 6.
            word6_letter1.Clear();
            word6_letter2.Clear();
            word6_letter3.Clear();
            word6_letter4.Clear();
            word6_letter5.Clear();

            word6_letter1.BackColor = Color.Black;
            word6_letter2.BackColor = Color.Black;
            word6_letter3.BackColor = Color.Black;
            word6_letter4.BackColor = Color.Black;
            word6_letter5.BackColor = Color.Black;

            if (wordIndex > 4)
            {
                GameOver();
            }  
        }
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (letterIndex > 4)
            {
                letterIndex = 4;
            }
            else if (letterIndex < 0)
            {
                letterIndex = 0;
            }
            if ((e.KeyValue >= 65 && e.KeyValue <= 90))
            {
                if (letterIndex + 1 == 5 && rows[rowIndex][letterIndex].Text != "") ;
                else
                {
                    rows[rowIndex][letterIndex].Text = e.KeyCode.ToString();
                    letterIndex++;
                }
            }
            else if (e.KeyCode == Keys.Enter && letterIndex == 4 && rows[rowIndex][4].Text != "")
            {
                string answer = "";
                for (int i = 0; i < 5; i++)
                {
                    char answerC = Convert.ToChar(rows[rowIndex][i].Text[0]);
                    answer += answerC;
                }
                Send("<Check>-" + name + "-" + wordIndex.ToString() + "-" + answer);
                while (true)
                {
                    if (answerCode != null)
                    {
                        break;
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    if (answerCode[i] == '2')
                    {
                        rows[rowIndex][i].BackColor = ColorTranslator.FromHtml("#019A01");
                        rows[rowIndex][i].ForeColor = Color.White;
                        CorrectWords++;
                    }
                    else if (answerCode[i] == '1')
                    {
                        rows[rowIndex][i].BackColor = ColorTranslator.FromHtml("#FFC425");
                        rows[rowIndex][i].ForeColor = Color.White;
                    }
                    else if (answerCode[i] == '0')
                    {
                        rows[rowIndex][i].BackColor = ColorTranslator.FromHtml("#444444");
                        rows[rowIndex][i].ForeColor = Color.White;
                    }
                }
                answerCode = null;
                if (CorrectWords == 5)
                {
                    wordIndex++;
                    if (wordIndex < 5)
                    {
                        CountDown += 5;                      
                    }
                    points++;
                    ResetAll();
                }
                else if (CorrectWords != 5 && rowIndex == 5)
                {
                    wordIndex++;
                    if (wordIndex < 5)
                        CountDown -= 10;
                    ResetAll();
                }
                else
                {
                    rowIndex++;
                    letterIndex = 0;
                    CorrectWords = 0;
                }
            }
            else if (e.KeyCode == Keys.Back)
            {
                if (letterIndex <= 4 && letterIndex >= 1)
                {
                    if (rows[rowIndex][4].Text != "")
                    {
                        rows[rowIndex][4].Text = "";
                    }
                    else if (letterIndex - 1 < 0) ;
                    else
                    {
                        rows[rowIndex][letterIndex - 1].Text = "";
                        letterIndex--;
                    }
                }
            }
        }

        private void GameUI_FormClosed(object sender, FormClosedEventArgs e)
        {
            TimerClient.Stop();
            if (client != null && client.Connected)
            {
                client.Close();
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text == null || textBoxName.Text == "")
            {
                MessageBox.Show("Please type your name!", "Warning", MessageBoxButtons.OK);
                textBoxName.Focus();
                return;
            }
            IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            name = textBoxName.Text;
            try
            {
                client.Connect(IP);
            }
            catch
            {
                MessageBox.Show("Fail to connect to server!", "Warning", MessageBoxButtons.OK);
                return;
            }
            //tạo luồng lắng nghe server khi vừa kết nối tới
            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
            btnConnect.Enabled = false;
            btnReady.Enabled = true;
            btnGameUI.Enabled = true;
            textBoxName.Enabled = false;
        }

        private void Receive()
        {
            try
            {
                while (true)
                {
                    //tạo mảng 1 byte để lưu dữ liệu
                    byte[] data = new byte[1024 * 5000];
                    // nhận data từ Socket và lưu vào buffer "data"
                    client.Receive(data);
                    //Gom mảnh data sang dạng string
                    string message = (string)Deserialize(data);

                    if (message != null)
                    {
                        string[] msg = message.Split('-');
                        if (message == "<StartGame>")
                        {
                            MessageBox.Show("Game start! Press JOIN to play.", "", MessageBoxButtons.OK);
                            gameProcess = true;
                        }
                        else if (msg[0] == "<CheckCode>")
                        {
                            answerCode = msg[1];
                        }
                        else if (msg[0] == "<Rank>")
                        {
                            result = msg[1];
                        }
                    } 
                }
            }
            catch
            {
                MessageBox.Show("You have disconnected!", "Warning", MessageBoxButtons.OK);
            }

        }
        private void Send(string msg)
        {
            client.Send(Serialize(msg));
        }
        private void btnGameUI_Click(object sender, EventArgs e)
        {
            if (!gameProcess)
            {
                MessageBox.Show("Waiting for other players...", "", MessageBoxButtons.OK);
                return;
            }
            groupJoin.Enabled = false;
            groupJoin.Visible = false;
            btnGameUI.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TimerClient.Start();

            row1[0] = word1_letter1;
            row1[1] = word1_letter2;
            row1[2] = word1_letter3;
            row1[3] = word1_letter4;
            row1[4] = word1_letter5;
            rows.Add(row1);

            row2[0] = word2_letter1;
            row2[1] = word2_letter2;
            row2[2] = word2_letter3;
            row2[3] = word2_letter4;
            row2[4] = word2_letter5;
            rows.Add(row2);

            row3[0] = word3_letter1;
            row3[1] = word3_letter2;
            row3[2] = word3_letter3;
            row3[3] = word3_letter4;
            row3[4] = word3_letter5;
            rows.Add(row3);

            row4[0] = word4_letter1;
            row4[1] = word4_letter2;
            row4[2] = word4_letter3;
            row4[3] = word4_letter4;
            row4[4] = word4_letter5;
            rows.Add(row4);

            row5[0] = word5_letter1;
            row5[1] = word5_letter2;
            row5[2] = word5_letter3;
            row5[3] = word5_letter4;
            row5[4] = word5_letter5;
            rows.Add(row5);

            row6[0] = word6_letter1;
            row6[1] = word6_letter2;
            row6[2] = word6_letter3;
            row6[3] = word6_letter4;
            row6[4] = word6_letter5;
            rows.Add(row6);

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    rows[i][j].Text = "";
                    rows[i][j].ForeColor = Color.White;
                    rows[i][j].BackColor = Color.Black;
                    rows[i][j].Enabled = false;
                }
            }
        }

        private void btnReady_Click(object sender, EventArgs e)
        {
            Send("<Ready>-" + name);
            btnReady.Enabled = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            labelName.Text= textBoxName.Text;
        }

        private object Deserialize(byte[] data)
        {
            //Khởi tạo stream để lưu trữ mảng byte
            MemoryStream stream = new MemoryStream(data);
            // Khởi tạo đổi tượng để chuyển đổi
            BinaryFormatter bf = new BinaryFormatter();
            //Chuyển đổi và return lại dữ liệu
            return bf.Deserialize(stream);
        }
        private byte[] Serialize(object obj)
        {
            //Khởi tạo stream để lưu các byte phân mảnh
            MemoryStream stream = new MemoryStream();
            //Khởi tạo đổi tượng để phân mảnh
            BinaryFormatter bf = new BinaryFormatter();
            //Phân mảnh 1 object và lưu nó lại dưới 1 mảng byte và lưu vào "stream"
            bf.Serialize(stream, obj);
            //Trả về kết quả là 1 mảng byte để chuẩn bị gửi đi
            return stream.ToArray();
        }
    }
}
