using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuaRongDao
{
    public class MoveBlock
    {
        string name;
        public int hight;
        public int width;
        //逻辑位置，即网格索引
        public int logic_pos_x;
        public int logic_pos_y;
        int btn; //对应的Button编号
        public MoveBlock(string Name, int Width, int Hight, int LPos_x, int LPos_y, int Btn)
        {
            name = Name;
            hight = Hight;
            width = Width;
            logic_pos_x = LPos_x;
            logic_pos_y = LPos_y;
            btn = Btn;
        }

    }
}
