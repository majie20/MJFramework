﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WXB
{
	public class XSpaceNode : NodeBase
	{
        public override float getHeight()
		{
			return 0.01f;
		}

        public override float getWidth()
		{
			return d_offset;
		}

        public override void render(float maxWidth, RenderCache cache, ref float x, ref uint yline, List<Line> lines, float offsetX, float offsetY)
		{
            Vector2 pt = new Vector2(x + offsetX, offsetY);
			for (int i = 0; i < yline; ++i)
				pt.y += lines[i].y;

			// 因对齐，X轴偏移量
			float alignedX = AlignedFormatting(owner, formatting, maxWidth, lines[(int)(yline)].x, 0);

			if (x + d_offset + alignedX > maxWidth)
			{
				yline++;
				x = NextLineX;
			}
			else
			{
				x += d_offset;
			}

// 			d_beginLine = yline;
// 			d_endLine   = yline;

			if (d_bNewLine == true)
			{
				yline++;
				x = NextLineX;
			}
		}

		public float d_offset;

        protected override void ReleaseSelf()
        {
            d_offset = 0f;
        }

        public override void fill(ref Vector2 currentpos, List<Line> lines, float maxWidth, float pixelsPerUnit)
        {
            NextLineX = d_nextLineX * pixelsPerUnit;
            List<Element> TempList;
            UpdateWidthList(out TempList, pixelsPerUnit);
            float height = getHeight();
            float unitsPerPixel = 1f / pixelsPerUnit;
            int fontsize = (int)(d_fontSize * pixelsPerUnit);
            height = Mathf.Max(height, FontCache.GetLineHeight(d_font, fontsize, d_fontStyle) * unitsPerPixel);

            AlterX(ref currentpos.x, maxWidth);
            if (TempList.Count == 0)
                return;

            Around around = owner.around;
            bool isContain = false; // 当前行是否包含此元素
            for (int i = 0; i < TempList.Count;)
            {
                float totalwidth = TempList[i].totalwidth;
                float newx = 0f;
                if (around != null && !around.isContain(currentpos.x, currentpos.y, totalwidth, height, out newx))
                {
                    // 放置不下了
                    currentpos.x = newx;
                }
                else
                {
                    currentpos.x += totalwidth;
                    isContain = true;
                    ++i;
                }
            }

            Line bl = lines.back();
            bl.x = currentpos.x;
            bl.y = Mathf.Max(height, bl.y);

            if (d_bNewLine)
            {
                lines.Add(new Line(Vector2.zero));
                currentpos.y += height;
                currentpos.x = NextLineX;
            }
        }
    };
}
