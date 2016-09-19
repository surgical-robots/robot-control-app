using System;

namespace path_generation
{

    class autoSuturing
    {    
        static void Main(string[] args)
        {
            
            double t = 0;
            double t_incr = Math.PI / 10;
            /*while (true)
            {
                point p;
                trajectory obj = new trajectory(0, 0, 130);
                p = obj.end_effector(t);
                t = t + t_incr;
                Console.WriteLine("{0}\t{1}\t{2}", p.pos.x, p.pos.y, p.pos.z);
                if (t > Math.PI) break;
            }
            */
            /*output: 
             * p.pos.x
             * p.pos.y
             * p.pos.z
             * /
            /*Vector v1 = new Vector(1,2,3);
            Vector v2 = new Vector(2, 3,4);
            Vector normal = new Vector();
            normal=normal.cross(v1, v2);
            Console.WriteLine("normal : {0}, {1}, {2}", normal.x, normal.y, normal.z);*/
            Console.ReadKey();


        }
    }
}
