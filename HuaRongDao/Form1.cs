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

/*
 * 华容道v0.02
 * 作者：卢建成
 * 时间：2018-6-21
 * 单位：北京交通大学
 * 邮箱：1036522761@qq.com
*/


/*
 * 游戏空间是5x4的网格,游戏空间总大小为648x520, 每个网格的大小为128*128
 * 游戏空间相对于窗体的位移是x=y=20
 */

namespace HuaRongDao
{
    enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        NULL
    }

    
    public partial class Form1 : Form
    {
        //振动相关
        bool VibrationFlag = false;//振动标志
        int VibrationNum = 0;//振动计数
        Point orignial;
        //体积碰撞数据结构
        int[,] GameSpace=new int[5,4] {
            {1,1, 1, 1},
            {1,1, 1, 1},
            {1,1, 1, 1},
            {1,1, 1, 1},
            {1,0, 0, 1},
        };
        List<MoveBlock> blockList = new List<MoveBlock>(10);
        List<Button> btnList = new List<Button>();
        MoveBlock caocao = new MoveBlock("曹操",2,2,1,0,1);
        MoveBlock zhangfei = new MoveBlock("张飞", 1, 2, 0, 0, 2);       
        MoveBlock machao= new MoveBlock("马超", 1, 2, 3, 0, 3);
        MoveBlock huangzhong= new MoveBlock("黄忠", 1, 2, 0, 2, 4);
        MoveBlock guanyu = new MoveBlock("关羽", 2, 1, 1, 2, 5);
        MoveBlock zhaoyun = new MoveBlock("赵云", 1, 2, 3, 2, 6);
        MoveBlock shibing1 = new MoveBlock("士兵1", 1, 1, 0, 4, 7);
        MoveBlock shibing2 = new MoveBlock("士兵2", 1, 1, 1, 3, 8);
        MoveBlock shibing3 = new MoveBlock("士兵3", 1, 1, 2, 3, 9);
        MoveBlock shibing4= new MoveBlock("士兵4", 1, 1, 3, 4, 10);
        //初始化位置参数
        int formWidth = 580;
        int formHight = 800;   //窗口参数
        int pWidth = 560; 
        int pHight =688;         //背景空间
        //滑块移动空间大小
        float gameArealogic_pos_x = 20;
        float gameArealogic_pos_y = 20;    
        int gameAreaHight = 648;
        int gameAreaWidth = 520;
        //定义鼠标移动的变量
        int mouseStartlogic_pos_x=0;
        int mouseStartlogic_pos_y=0;
        int mouseEndlogic_pos_x = 0;
        int mouseEndlogic_pos_y = 0;
        Control currentButton=null;//当前被选中的按钮
        public Form1()
        {
            InitializeComponent();
        }

        


        private void Form1_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            this.Size = new System.Drawing.Size(formWidth, formHight);//设置窗体大小
            pictureBox1.Size = new System.Drawing.Size(pWidth, pHight);//设置窗体大小
            //画框
            Graphics pic = Graphics.FromImage(pictureBox1.Image);
            Pen p = new Pen(Brushes.Red,4);
            pic.DrawRectangle(p, gameArealogic_pos_x, gameArealogic_pos_y, gameAreaWidth,gameAreaHight);  
            //画出口
            p = new Pen(Brushes.Green, 4);
            pic.DrawLine(p, 148, 668, 404, 668);
            pic.Dispose();
            Init_Game();
        }

        private void Init_Game()
        {
             caocao = new MoveBlock("曹操", 2, 2, 1, 0, 1);
             zhangfei = new MoveBlock("张飞", 1, 2, 0, 0, 2);
             machao = new MoveBlock("马超", 1, 2, 3, 0, 3);
             huangzhong = new MoveBlock("黄忠", 1, 2, 0, 2, 4);
             guanyu = new MoveBlock("关羽", 2, 1, 1, 2, 5);
             zhaoyun = new MoveBlock("赵云", 1, 2, 3, 2, 6);
            shibing1 = new MoveBlock("士兵1", 1, 1, 0, 4, 7);
            shibing2 = new MoveBlock("士兵2", 1, 1, 1, 3, 8);
            shibing3 = new MoveBlock("士兵3", 1, 1, 2, 3, 9);
             shibing4 = new MoveBlock("士兵4", 1, 1, 3, 4, 10);
             //将滑块与索引相对应
             blockList.Clear();
             blockList.Add(caocao);
             blockList.Add(zhangfei);
             blockList.Add(machao);
             blockList.Add(huangzhong);
             blockList.Add(guanyu);
             blockList.Add(zhaoyun);
             blockList.Add(shibing1);
             blockList.Add(shibing2);
             blockList.Add(shibing3);
             blockList.Add(shibing4);
             //将按钮与索引对应
             btnList.Clear();
             btnList.Add(button1);
             btnList.Add(button2);
             btnList.Add(button3);
             btnList.Add(button4);
             btnList.Add(button5);
             btnList.Add(button6);
             btnList.Add(button7);
             btnList.Add(button8);
             btnList.Add(button9);
             btnList.Add(button10);
             for (int i = 0; i < btnList.Count;i++ )
             {
                 btnList[i].Location = new Point(20 + 128 * blockList[i].logic_pos_x, 20 + 128 * blockList[i].logic_pos_y);
             }
                 GameSpace = new int[5, 4] {
                {1,1, 1, 1},
                {1,1, 1, 1},
                {1,1, 1, 1},
                {1,1, 1, 1},
                {1,0, 0, 1},
            };
        }

       /// <summary>
       ///  当鼠标抬起时，进行移动滑块动作
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void MouseUp(object sender, MouseEventArgs e)
        {
            //获取鼠标终止位置
            mouseEndlogic_pos_x = e.Location.X;
            mouseEndlogic_pos_y = e.Location.Y;
            Direction moveDir=GetDirection( mouseStartlogic_pos_x,mouseStartlogic_pos_y,mouseEndlogic_pos_x,mouseEndlogic_pos_y);
            //获取索引
            int BlockIndex = 0;
            for (int i = 0; i < btnList.Count; i++)
            {
                if (currentButton.Equals(btnList[i]))
                {
                    BlockIndex = i;
                    break;
                }
            }
            //移动前先检查currentButton在该方向上是否存在移动空间
            MoveBlock mb;
            switch(moveDir)
            {
                case Direction.UP:
                          for (int i = 0; i < blockList[BlockIndex].width; i++)
                          {
                               if (blockList[BlockIndex].logic_pos_y - 1 < 0 || GameSpace[blockList[BlockIndex].logic_pos_y - 1,blockList[BlockIndex].logic_pos_x + i] == 1)//判断是否可以移动
                               {
                                   VibrationFlag = true;//按钮振动
                                   return;
                               }                              
                           }   
                        //旧滑块空间清零 
                          ClearGameSpace(blockList[BlockIndex].logic_pos_x, blockList[BlockIndex].logic_pos_y, blockList[BlockIndex].width, blockList[BlockIndex].hight);
                        //更新滑块信息
                         mb=blockList[BlockIndex];
                         mb.logic_pos_y -= 1;
                         blockList[BlockIndex] = mb;
                         currentButton.Location = new System.Drawing.Point(20 + 128 * mb.logic_pos_x, 20 + 128 * mb.logic_pos_y);
                        //新滑块空间赋值
                         NewGameSpace(mb.logic_pos_x, mb.logic_pos_y, mb.width, mb.hight);
                         break;
                case Direction.DOWN:
                         for (int i = 0; i < blockList[BlockIndex].width; i++)
                         {
                             if (blockList[BlockIndex].logic_pos_y + blockList[BlockIndex].hight > 4 || GameSpace[blockList[BlockIndex].logic_pos_y + blockList[BlockIndex].hight, blockList[BlockIndex].logic_pos_x + i] == 1)//判断是否可以移动
                             {
                                 VibrationFlag = true;//按钮振动
                                 return;
                             }   
                         }
                         //旧滑块空间清零 
                         ClearGameSpace(blockList[BlockIndex].logic_pos_x, blockList[BlockIndex].logic_pos_y, blockList[BlockIndex].width, blockList[BlockIndex].hight);
                         //更新滑块信息
                         mb=blockList[BlockIndex];
                         mb.logic_pos_y += 1;
                         blockList[BlockIndex] = mb;
                         currentButton.Location = new System.Drawing.Point(20 + 128 * mb.logic_pos_x, 20 + 128 * mb.logic_pos_y);
                         //新滑块空间赋值
                         NewGameSpace(mb.logic_pos_x, mb.logic_pos_y, mb.width, mb.hight);
                         break;
                case Direction.LEFT:
                         for (int i = 0; i < blockList[BlockIndex].hight; i++)
                         {
                             if (blockList[BlockIndex].logic_pos_x-1 <0 || GameSpace[blockList[BlockIndex].logic_pos_y + i,blockList[BlockIndex].logic_pos_x-1] == 1)//判断是否可以移动
                             {
                                 VibrationFlag = true;//按钮振动
                                 return;
                             }   
                         }
                         //旧滑块空间清零 
                         ClearGameSpace(blockList[BlockIndex].logic_pos_x, blockList[BlockIndex].logic_pos_y, blockList[BlockIndex].width, blockList[BlockIndex].hight);
                         //更新滑块信息
                          mb=blockList[BlockIndex];
                         mb.logic_pos_x -= 1;
                         blockList[BlockIndex] = mb;
                         currentButton.Location = new System.Drawing.Point(20 + 128 * mb.logic_pos_x, 20 + 128 * mb.logic_pos_y);
                         //新滑块空间赋值
                         NewGameSpace(mb.logic_pos_x, mb.logic_pos_y, mb.width, mb.hight);
                         break;
                case Direction.RIGHT:
                         for (int i = 0; i < blockList[BlockIndex].hight; i++)
                         {
                             if (blockList[BlockIndex].logic_pos_x +blockList[BlockIndex].width >3 || GameSpace[blockList[BlockIndex].logic_pos_y + i,blockList[BlockIndex].logic_pos_x+blockList[BlockIndex].width] == 1)//判断是否可以移动
                             {
                                 VibrationFlag = true;//按钮振动
                                 return;
                             }   
                         }
                         //旧滑块空间清零 
                         ClearGameSpace(blockList[BlockIndex].logic_pos_x, blockList[BlockIndex].logic_pos_y, blockList[BlockIndex].width, blockList[BlockIndex].hight);
                         //更新滑块信息
                        mb=blockList[BlockIndex];
                         mb.logic_pos_x += 1;
                         blockList[BlockIndex] = mb;
                         currentButton.Location = new System.Drawing.Point(20 + 128*mb.logic_pos_x, 20+128*mb.logic_pos_y);
                         //新滑块空间赋值
                         NewGameSpace(mb.logic_pos_x, mb.logic_pos_y, mb.width, mb.hight);
                        break;                     
            }
            ShowGameSpace(GameSpace);
        }


      /// <summary>
      /// 输出调试结果到文本中
      /// </summary>
      /// <param name="array"></param>
      void ShowGameSpace(int [,] array)
      {
          StreamWriter sw = File.AppendText("debug.txt");
          int hang=array.GetLength(0);
          int lie=array.GetLength(1);
          for(int i=0; i<hang;i++)
          {
              for (int j = 0; j < lie; j++)
              {
                  sw.Write(array[i, j]);
              }
              sw.Write("\r\n");
          }
          sw.WriteLine("******************");
          sw.Close();
      }

        Direction GetDirection(int startlogic_pos_x, int startlogic_pos_y, int Endlogic_pos_x, int Endlogic_pos_y)
        {
            Direction dir = Direction.NULL;
            int dx = Endlogic_pos_x - startlogic_pos_x;
            int dy = Endlogic_pos_y- startlogic_pos_y;
            if (Math.Abs(dx) < 10 && Math.Abs(dy) < 10)
                return dir;//防止错误触发
            if(dx>0)
            {
                if (Math.Abs(dy) < dx)
                    dir = Direction.RIGHT;
                else if (dy > 0)
                    dir = Direction.DOWN;
                else
                    dir = Direction.UP;
            }
            else
            {
                if (Math.Abs(dy) < Math.Abs(dx))
                    dir = Direction.LEFT;
                else if (dy > 0)
                    dir = Direction.DOWN;
                else
                    dir = Direction.UP;
            }
            return dir;
        }

        /// <summary>
        /// 清除滑块空间
        /// </summary>
        /// <param name="baseX"></param>
        /// <param name="baseY"></param>
        /// <param name="width"></param>
        /// <param name="hight"></param>
        void ClearGameSpace(int baseX,int baseY,int width,int hight)
        {
            for (int i =0; i < width; i++)
            {
                for (int j = 0; j < hight; j++)
                    GameSpace[baseY+j, baseX+i] = 0;
            }
          
        }

        void NewGameSpace(int baseX,int baseY,int width,int hight)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < hight; j++)
                    GameSpace[baseY + j, baseX + i] = 1;
            }
        }


        private void MouseDown(object sender, MouseEventArgs e)
        {
            currentButton =(Control)sender;
            orignial.X = currentButton.Location.X;
            orignial.Y = currentButton.Location.Y;
            //获取鼠标起始位置         
            mouseStartlogic_pos_x = e.Location.X;
            mouseStartlogic_pos_y = e.Location.Y;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Init_Game();        
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
             if(VibrationFlag==true)
             {
                 if(VibrationNum%2==0)
                         currentButton.Location = new Point(orignial.X + 10, orignial.Y + 10);
               else
                          currentButton.Location = new Point(orignial.X - 10, orignial.Y - 10);
             }
            if(VibrationNum==0xFF)
            {
                VibrationNum = 0;
                currentButton.Location = orignial;//复原位置
                VibrationFlag = false;
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        /// <summary>
        ///  保存游戏进度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            StringBuilder strb=new StringBuilder();
            strb.Append("******\n\r");
            for(int i=0;i<blockList.Count;i++)
            {
                strb.Append("# ");
                strb.Append(blockList[i].logic_pos_x.ToString());
                strb.Append(" ");
                strb.Append(blockList[i].logic_pos_y.ToString());
                strb.Append("\n\r");
            }
            strb.Append("******\n\r");
           File.WriteAllText(@"history.txt",strb.ToString());
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 加载进度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            saveGame();
        }

        /// <summary>
        /// 保存游戏进度
        /// </summary>
        public void saveGame()
        {
            string[] strs = File.ReadAllLines(@"history.txt");
            int count = 0;
            int x, y;
            for (int i = 0; i < strs.Length; i++)
            {
                string[] subStr = strs[i].Split(' ');
                if (subStr[0] == "#")
                {
                    int.TryParse(subStr[1], out x);
                    blockList[count].logic_pos_x = x;
                    int.TryParse(subStr[2], out y);
                    blockList[count].logic_pos_y = y;
                    count++;
                }
            }
            //改变滑块位置
            for (int i = 0; i < btnList.Count; i++)
            {
                btnList[i].Location = new Point(20 + 128 * blockList[i].logic_pos_x, 20 + 128 * blockList[i].logic_pos_y);
            }
            //填充游戏空间
            GameSpace = new int[5, 4];
            for (int i = 0; i < blockList.Count; i++)
            {
                int h = blockList[i].hight;
                int w = blockList[i].width;
                x = blockList[i].logic_pos_x;
                y = blockList[i].logic_pos_y;
                for (int k = 0; k < h; k++)
                {
                    for (int j = 0; j < w; j++)
                        GameSpace[y + k, x + j] = 1;
                }

            }
            ShowGameSpace(GameSpace);
        }
    
    }
}
