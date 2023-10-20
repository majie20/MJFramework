﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WXB
{
    public abstract class RectNode : NodeBase
	{
        public float width = 0;
        public float height = 0;

        public override float getHeight()
        {
            return height;
        }

        public override float getWidth()
        {
            return width;
        }

        protected override void ReleaseSelf()
        {
            width = 0f;
            height = 0f;
        }

        protected abstract void OnRectRender(RenderCache cache, Line line, Rect rect);

        public override void render(float maxWidth, RenderCache cache, ref float x, ref uint yline, List<Line> lines, float offsetX, float offsetY)
        {
			float width  = getWidth();
			float height = getHeight();

            if (x + width > maxWidth && cache.Owner.HorizontalOverflow == HorizontalWrapMode.Wrap)
            {
				x = NextLineX;
				yline++;
			}

			float alignedX = AlignedFormatting(owner, formatting, maxWidth, lines[(int)(yline)].x, 0);

			float y_offset = offsetY;
			for (int i = 0; i < yline; ++i)
				y_offset += lines[i].y;

			//y_offset += lines[(int)(yline)].y;
            Rect areaRect = new Rect(x + offsetX + alignedX, y_offset, width, height);

            float newfx = 0f;
            while (!owner.around.isContain(areaRect, out newfx))
            {
                areaRect.x = newfx;
                x = newfx - alignedX - offsetX;
                if (x + width > maxWidth)
                {
                    x = NextLineX;
                    yline++;
                    y_offset += lines[(int)yline].y;
                    areaRect = new Rect(x + offsetX + alignedX, y_offset, width, height);
                }
            }

            OnRectRender(cache, lines[(int)yline], areaRect);

            x += width + owner.GetWordSpacing(d_font, false);

			if (d_bNewLine)
			{
				x = 0;
				yline++;
			}
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
                if (((currentpos.x + totalwidth) > maxWidth) && owner.HorizontalOverflow == HorizontalWrapMode.Wrap)
                {
                    currentpos = TempList[i].Next(this, currentpos, lines, maxWidth, NextLineX, height, around, totalwidth, ref isContain);
                    ++i;
                }
                else if (around != null && !around.isContain(currentpos.x, currentpos.y, totalwidth, height, out newx))
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
