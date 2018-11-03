using System;
namespace SharpLights
{

    struct Spark
    {
       public Color color;
       public int fade_rate;
        public bool alive;
    }

    public class Twinkle
    {
        Palette palette;//palette for the twinkle colors
        Boolean sequential;//if the palette should be sequential over the strips
                           //or randomly spread
        Boolean static_sparks; //true if sparks stay where they are or randomly respawn in new spots.
        int percent;//percent of pixels to be lit 
        int frame_size; //how many pixels are we dealing with

        Spark[] sparks;

        Random rand;
        public Twinkle(Palette p_palette, Boolean p_seq, Boolean p_static_sparks,
                       int p_percent, 
                       int p_frame_size)
        {
            this.palette = p_palette;
            this.sequential = p_seq;
            this.static_sparks = p_static_sparks;
            this.percent = p_percent;
            this.frame_size = p_frame_size;

            this.rand = new Random((int)DateTime.Now.Ticks);

            sparks = new Spark[frame_size];
            for (int i = 0; i < sparks.Length;i++)
            {
                sparks[i] = new Spark();
                if (rand.Next(0, 100) < p_percent)//randomly decide it's lit
                {
                  //  if (sequential)
                        sparks[i].color = palette.getColor(i);//palette will autowrap for us
                                                              // else
                                                              //     sparks[i].color = palette.getRandomColor();
                    sparks[i].fade_rate = 1;// rand.Next(1, 3);
                    sparks[i].alive = true;
                }
               else
                {
                   sparks[i].color = new Color(CRGB.Black);
                    sparks[i].alive = false;
               }

            }
        }

   
        /// <summary>
        /// return a frame of colors based on the current state of sparks 
        /// </summary>
        /// <returns>The frame.</returns>
        public Color[] getFrame()
        {
            Color[] toreturn = new Color[frame_size];
            for (int i = 0; i < frame_size;i++)
                toreturn[i] = sparks[i].color;

            return toreturn;
        }

        public void iterate()
        {
            for (int i = 0; i < frame_size;i++)
            {
                //this spark has faded out
                if (sparkOut(sparks[i])&&sparks[i].alive)
                {
                    //there was a spark here before
                    //and we are only spawning where there 
                    //were sparks before
                   
                    //ok, this one is out and we are randomly respawning
                    //on any faded spark. (don't care about aliveness)
                   if (rand.Next(0, 100) < percent)
                        resetSparkColor(i);
                }
                else
                    fadeSpark(i);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spark_index">Spark index.</param>
        private void fadeSpark(int spark_index)
        {
            sparks[spark_index].color = 
                Color.GetFadedColor(sparks[spark_index].color,sparks[spark_index].fade_rate);
        //    if (sparkOut(sparks[spark_index]))
          //      sparks[spark_index].alive = false;
        }
        private void resetSparkColor(int spark_index)
        {
            if (sequential)
                sparks[spark_index].color = palette.getColor(spark_index);//palette will autowrap for us
            else
                sparks[spark_index].color = palette.getRandomColor();
            sparks[spark_index].alive = true;
        }

        private bool sparkOut(Spark spark){
            if (spark.color.red == 0 ||
               spark.color.green ==0 ||
               spark.color.blue ==0)
                return true;
            return false;
        }

    }
}
