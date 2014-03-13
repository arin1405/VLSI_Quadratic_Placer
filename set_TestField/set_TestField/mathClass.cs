using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace set_TestField
{
    class mathClass
    {
        public int oneToNumSum(int num)
        {
            //if num is given as input, it returns (1+2+3+...+num)
            int a = num;
            if (num == 0)
                return 0;
            else
            {
                for (int i = 1; i < a; i++)
                    num = num + i;
                return num;
            }
        }
    }
}
