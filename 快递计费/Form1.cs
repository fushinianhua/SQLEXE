using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 快递计费
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();        
            tabControl1.DrawItem += tabControl1_DrawItem; // 确保事件绑定
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabControl = (TabControl)sender;
            TabPage tabPage = tabControl.TabPages[e.Index];
            Rectangle tabRect = tabControl.GetTabRect(e.Index);

            // 1. 固定斜边宽度（不动态调整标签页大小）
            int slopeWidth = 15;

            // 2. 绘制梯形路径（仅左侧斜边）
            GraphicsPath path = new GraphicsPath();
            path.AddLine(tabRect.X + slopeWidth, tabRect.Y, tabRect.Right, tabRect.Y);      // 上边（水平）
            path.AddLine(tabRect.Right, tabRect.Y, tabRect.Right, tabRect.Bottom);         // 右边（垂直）
            path.AddLine(tabRect.Right, tabRect.Bottom, tabRect.X, tabRect.Bottom);        // 底边（水平）
            path.AddLine(tabRect.X, tabRect.Bottom, tabRect.X + slopeWidth, tabRect.Y);    // 左边（斜边）

            // 3. 填充背景色
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            using (SolidBrush brush = new SolidBrush(isSelected ? Color.LightSkyBlue : Color.White))
            {
                e.Graphics.FillPath(brush, path);
            }

            // 4. 绘制文字（从斜边结束处开始）
            Rectangle textRect = new Rectangle(
                tabRect.X + slopeWidth,  // 起点：斜边右侧
                tabRect.Y,
                tabRect.Width - slopeWidth, // 宽度：总宽度 - 斜边宽度
                tabRect.Height
            );

            TextRenderer.DrawText(
                e.Graphics,
                tabPage.Text,
                tabPage.Font,
                textRect,
                Color.Black,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter // 左对齐+垂直居中
            );

            // 5. 可选：绘制边框（清晰显示梯形边界）
            e.Graphics.DrawPath(Pens.Gray, path);
            path.Dispose();
        }

    }
}
