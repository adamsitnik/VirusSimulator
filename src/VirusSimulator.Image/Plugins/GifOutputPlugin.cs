﻿using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VirusSimulator.Image.Plugins
{
    public class GifOutputPlugin<TPixel> : IImageProcessorPlugIn<TPixel> where TPixel : struct, IPixel<TPixel>
    {
        private int w, h;
        private Image<TPixel> gif;
        //bool isFirst = false;
        public string OutputPath { get; private set; }
        private Color[] pattern;
        public GifOutputPlugin(int width, int height,Color[] colorPattern, string outputPath=null)
        {
            w = width;
            h = height;
            pattern = colorPattern;
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                OutputPath = "Output.gif";
            }
            else
            {
                OutputPath = outputPath;
            }
        }

        
            
        public void Close(Image<TPixel> image)
        {
            using (FileStream fs=new FileStream(OutputPath,FileMode.Create))
            {
                gif.SaveAsGif(fs);
            }
        }

        public void Init(Image<TPixel> image)
        {
            gif = new Image<TPixel>(w, h);
            //isFirst = true;
            gif.Mutate(op =>
            {
                for (int i = 0; i < pattern.Length; i++)
                {
                    op.DrawLines(pattern[i], 1, new PointF(0, i), new PointF(1, i));
                }
            });
        }

        public void OnDraw(Image<TPixel> image)
        {
            var f=image.Frames.CloneFrame(0);
            if (w!=f.Width || h!=f.Height)
            {
                f.Mutate(op =>
                {
                    op.Resize(w, h);
                });
            };
            gif.Frames.AddFrame(f.Frames.RootFrame);
            //if (isFirst)
            //{
            //    gif.Frames.RemoveFrame(0);//remove first empty frame
            //    isFirst = false;
            //}
        }
    }
}
